using IMS.Manufacturing.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Entity;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateMarketingMemberCardActivityRequest.Types.AwardSendPeriod.Types.AwardSendDayTime.Types;

namespace IMS.Manufacturing.Action;
public class MaterialAction : ModelAction<Material>
{
    private DataElement _de;
    private IServiceProvider _serviceProvider;
    public MaterialAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _de = serviceProvider.GetRequiredService<DataElement>();
    }

    public List<Material>? Rec { get;set; }

    public void SyncU9Material()
    {
        SqlSugarClient U9Conn = U9Sql.GetDbConn();
        List<SysCompany> Companys = _de.Search<SysCompany>().ToList();
        List<MaterialCategory> MaterialCategoryExists = _de.Search<MaterialCategory>().ToList();
        foreach (SysCompany company in Companys)
        {
            if(company.ExternalId == null) continue;
            // 分类
            string U9_Material_Category_Sql = @"
            SELECT Id,Code,Name,Remarks,Parent,IsValid
            FROM [dbo].[UTIS_BOM_ItemCategory] t1
            WHERE [ItemCategoryType] = N'1001108090700030' AND Org = @org  ORDER BY isnull(ModifiedOn,CreatedOn) ";
            List<dynamic> U9_Material_Category = U9Conn.Ado.SqlQuery<dynamic>(U9_Material_Category_Sql, new { org  = company.ExternalId});
            List<MaterialCategory> MaterialCategories = new List<MaterialCategory>();
            foreach (dynamic Category in U9_Material_Category)
            {
                MaterialCategory NewCategory = MaterialCategoryExists.FirstOrDefault(t => t.ExternalId == Category.Id.ToString()) ?? new MaterialCategory();
                NewCategory.Code = Category.Code;
                NewCategory.Name = Category.Name;
                NewCategory.Active = Category.IsValid;
                NewCategory.Remarks = Category.Remarks;
                NewCategory.ExternalId = Category.Id.ToString();
                if (Category.Parent != null)
                {
                    NewCategory.Parent = MaterialCategoryExists.FirstOrDefault(t => t.ExternalId == Category.Parent.ToString());
                }
                NewCategory.Company = company;
                MaterialCategories.Add(NewCategory);
            }
            MaterialCategories = _de.BatchAddElseUpdate(MaterialCategories);

            // 料号
            List<Material> MaterialExists = _de.Search<Material>().ToList();
            List<Uom> UomExists = _de.Search<Uom>().ToList();
            var maxTime = MaterialExists.Where(p=> !string.IsNullOrEmpty(p.ExternalId)).Select(p => p.ExternalUpdateTime).Max();
            string U9_Material_Sql = @"
            select top 10000 item.Id,item.Code,trl.NameCombineName,item.SPECS,trl.Description,item.Version,ext.Cate4,item.InventoryUOM,
            isnull(item.ModifiedOn,item.CreatedOn) ModifiedOn,
            item.DescFlexField_PrivateDescSeg27,item.Effective_IsEffective
            from CBO_ItemMaster item
            join UTIS_CBO_ItemMasterExtend ext on ext.itemmaster=item.id
            join CBO_ItemMaster_trl trl on item.ID = trl.ID
            AND Org = @org";
            List<dynamic> U9_Material = new List<dynamic>();
            string OrderbySql = " ORDER BY isnull(item.ModifiedOn,item.CreatedOn) asc ";
            List<dynamic> U9Project = new List<dynamic>();
            if (maxTime == null)
            {
                U9_Material = U9Conn.Ado.SqlQuery<dynamic>(U9_Material_Sql + OrderbySql, new { org = company.ExternalId });
            }
            else
            {
                U9_Material = U9Conn.Ado.SqlQuery<dynamic>(U9_Material_Sql + " WHERE ISNULL(item.ModifiedOn, item.CreatedOn) >= @max " + OrderbySql, new { max = maxTime, org = company.ExternalId });
            }
            List<Material> MaterialInfos = new List<Material>();
            foreach (dynamic item in U9_Material)
            {
                Material newItem = MaterialExists.FirstOrDefault(t => t.ExternalId == item.Id.ToString()) ?? new Material();
                newItem.Code = item.Code;
                newItem.Name = item.NameCombineName;
                newItem.Description = item.Description;
                newItem.Spce = item.SPECS;
                newItem.Category = item.Cate4 != null ? MaterialCategories.FirstOrDefault(t => t.ExternalId == item.Cate4.ToString()) : null;
                newItem.Unit = UomExists.FirstOrDefault(t => t.ExternalId == item.InventoryUOM.ToString());
                newItem.Active = item.Effective_IsEffective;
                newItem.UsageAndInventory = item.DescFlexField_PrivateDescSeg27;
                newItem.Company = company;
                newItem.ExternalId = item.Id.ToString();
                newItem.ExternalUpdateTime = item.ModifiedOn;
                MaterialInfos.Add(newItem);
            }
            _de.BatchAddElseUpdate(MaterialInfos);
        }

    }

}
