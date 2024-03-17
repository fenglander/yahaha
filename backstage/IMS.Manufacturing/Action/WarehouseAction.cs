using Yahaha.Core.Entity;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.CgibinGetCurrentSelfMenuInfoResponse.Types.Menu.Types.Button.Types;

namespace IMS.Manufacturing.Action;

public class WarehouseAction : ModelAction<Warehouse>
{
    public List<Warehouse>? Rec { get; set; }
    private DataElement _de;
    private IServiceProvider _serviceProvider;

    public WarehouseAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _de = serviceProvider.GetRequiredService<DataElement>();
    }

    public void SyncU9Warehouse()
    {
        SqlSugarClient U9Conn = U9Sql.GetDbConn();
        List<SysCompany> Companys = _de.Search<SysCompany>().ToList();
        List<Warehouse> WarehouseExists = _de.Search<Warehouse>().ToList();
        List<StorageBin> StorageBinExists = _de.Search<StorageBin>().ToList();
        var maxTime = StorageBinExists.Select(p => p.ExternalUpdateTime).Max();
        foreach (SysCompany company in Companys)
        {
            string U9_Warehouse_Sql = @"SELECT T2.ID AS ExternalId,T2.Name,T1.Code,
            T1.IsBin AS IsStorageBin,T1.Effective_IsEffective AS Active,isnull(T1.ModifiedOn,T1.CreatedOn) ModifiedOn
            FROM [dbo].[CBO_Wh] T1
            INNER JOIN [dbo].[CBO_Wh_Trl] T2 ON T1.ID = T2.ID AND T2.SysMLFlag = 'zh-CN' WHERE Org = @org ";

            List<dynamic> U9Warehouse = U9Conn.Ado.SqlQuery<dynamic>(U9_Warehouse_Sql, new { org = company.ExternalId });

            string U9_Bin_Sql = @"SELECT T1.ID,T1.Code,T1.Effective_IsEffective,T1.Warehouse,T2.Name,isnull(T1.ModifiedOn,T1.CreatedOn) ModifiedOn
            FROM CBO_Bin T1
            INNER JOIN CBO_Bin_Trl T2 ON T1.ID = T2.ID AND T2.SysMLFlag = 'zh-CN' WHERE Org = @org ";

            List<dynamic> U9Bin = new List<dynamic>();
            if (maxTime == null)
            {
                U9Bin = U9Conn.Ado.SqlQuery<dynamic>(U9_Bin_Sql, new { org = company.ExternalId });
            }
            else
            {
                U9Bin = U9Conn.Ado.SqlQuery<dynamic>(U9_Bin_Sql + "AND ISNULL(T1.ModifiedOn, T1.CreatedOn) >= @max ", new { org = company.ExternalId, max = maxTime });
            }


            List<Warehouse> warehouses = new List<Warehouse>();
            List<StorageBin> StorageBins = new List<StorageBin>();

            foreach (var item in U9Warehouse)
            {
                Warehouse newItem = WarehouseExists.FirstOrDefault(t => t.ExternalId == item.ExternalId.ToString()) ?? new Warehouse();
                newItem.Active = item.Active;
                newItem.IsStorageBin = item.IsStorageBin;
                newItem.Code = item.Code;
                newItem.Name = item.Name;
                newItem.ExternalId = item.ExternalId.ToString();
                newItem.ExternalUpdateTime = item.ModifiedOn;
                newItem.Company = company;
                warehouses.Add(newItem);
            }
            WarehouseExists = _de.BatchAddElseUpdate(warehouses);

            U9Bin.ForEach(b =>
            {
                if (WarehouseExists.Any(x => x.ExternalId == b.Warehouse.ToString()))
                {
                    StorageBin storageBin = StorageBinExists.FirstOrDefault(x => x.ExternalId == b.ID.ToString()) ?? new StorageBin();
                    storageBin.Code = b.Code;
                    storageBin.Name = b.Name;
                    storageBin.Active = b.Effective_IsEffective;
                    storageBin.Warehouse = WarehouseExists.FirstOrDefault(x => x.ExternalId == b.Warehouse.ToString())!;
                    storageBin.ExternalId = b.ID.ToString();
                    storageBin.ExternalUpdateTime = b.ModifiedOn;
                    storageBin.Company = company;
                    StorageBins.Add(storageBin);
                }
            });
            _de.BatchAddElseUpdate(StorageBins);
        }

        
    }
}