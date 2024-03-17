

namespace IMS.Manufacturing.Action;


public class ProductionProcessAction : ModelAction<ProductionProcess>
{
    private DataElement _de;
    private IServiceProvider _serviceProvider;
    public ProductionProcessAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _de = serviceProvider.GetRequiredService<DataElement>();
    }

    public List<ProductionProcess>? Rec { get; set; }

    public void SyncAPSProcess()
    {

        SqlSugarClient APSConn = ApsSql.GetDbConn();

        var APS_ProcessGroup = APSConn.Ado.SqlQuery<dynamic>("SELECT ProcessGroupID,ProcessGroupName,Status FROM APS_ProcessGroup");

        string APS_ProcessGroupInfo_SQLStr = @"SELECT t1.ProcessGroupInfoID,t1.ProcessID,ProcessName,ProcessGroupID,ProcessPriority
        FROM APS_ProcessGroupInfo t1
        join APS_Process t2 on t1.ProcessID = t2.ProcessID
        order by ProcessGroupID,ProcessPriority";

        var APS_ProcessGroupInfos = APSConn.Ado.SqlQuery<dynamic>(APS_ProcessGroupInfo_SQLStr);

        var APS_Process = APSConn.Queryable<dynamic>().AS("APS_Process").ToList();

        List<ProductionOperation> productionOperations = new List<ProductionOperation>();

        var ProductionOperationExists = _de.Search<ProductionOperation>().ToList();
        // 处理工序档案
        foreach (var item in APS_Process)
        {
            ProductionOperation newItem = ProductionOperationExists.FirstOrDefault(t => t.ExternalId == item.ProcessID.ToString()) ?? new ProductionOperation();

            newItem.Code = item.ProcessID;
            newItem.Name = item.ProcessName;
            newItem.Active = item.Status == 1;
            newItem.ExternalId = item.ProcessID;
            productionOperations.Add(newItem);
        }

        productionOperations = _de.BatchAddElseUpdate(productionOperations);
        var ProcesssExists = _de.Search<ProductionProcess>().ToList();
        // 处理制程档案主从表
        List<ProductionProcess> productionProcesss = new List<ProductionProcess>();

        foreach (var item in APS_ProcessGroup)
        {
            var RowDetails = APS_ProcessGroupInfos.FindAll(t => t.ProcessGroupID == item.ProcessGroupID)
                .Select(t => new
                {
                    ExternalId = t.ProcessGroupInfoID,
                    Sequence = t.ProcessPriority,
                    Operation = productionOperations.FirstOrDefault(x => x.ExternalId == t.ProcessID)
                }).ToList();
            string JsonDetails = JsonConvert.SerializeObject(RowDetails);
            List<ProductionProcessDetail>? productionProcesses = JsonConvert.DeserializeObject<List<ProductionProcessDetail>>(JsonDetails);

            ProductionProcess newIten = ProcesssExists.FirstOrDefault(t => t.ExternalId == item.ProcessGroupID) ?? new ProductionProcess();
            newIten.Name = item.ProcessGroupName;
            newIten.Active = item.Status == 1;
            newIten.ExternalId = item.ProcessGroupID;
            newIten.Code = item.ProcessGroupID;
            newIten.Details = productionProcesses;
            productionProcesss.Add(newIten);
        }

        _de.BatchAddElseUpdate(productionProcesss);
    }


}
