using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Entity;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.CgibinGetCurrentSelfMenuInfoResponse.Types.Menu.Types.Button.Types;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateMarketingMemberCardActivityRequest.Types.AwardSendPeriod.Types.AwardSendDayTime.Types;

namespace IMS.Manufacturing.Action;
public class ManufacturingOrderAction : ModelAction<ManufacturingOrder>
{
    private DataElement _de;
    private IServiceProvider _serviceProvider;
    public ManufacturingOrderAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _de = serviceProvider.GetRequiredService<DataElement>();
    }
    public List<ManufacturingOrder>? Rec { get; set; }

    public void SyncU9ManufacturingOrderType()
    {
        SqlSugarClient U9Conn = U9Sql.GetDbConn();
        List<SysCompany> Companys = _de.Search<SysCompany>().ToList();
        List<ManufacturingOrderType> ManufacturingOrderTypeExists = _de.Search<ManufacturingOrderType>().ToList();
        foreach (SysCompany company in Companys)
        {

            string U9_MODocType_Sql = @"
            SELECT T1.Id,T1.Code,T2.Name,T2.Description,T1.Effective_IsEffective 
            FROM MO_MODocType T1
            INNER JOIN MO_MODocType_Trl T2 ON T1.Id = T2.Id AND T2.SysMLFlag = 'zh-CN' AND Org = @org";
            List<dynamic> U9MODocType = U9Conn.Ado.SqlQuery<dynamic>(U9_MODocType_Sql, new {org = company.ExternalId});

            List<ManufacturingOrderType> ManufacturingOrderTypes = new List<ManufacturingOrderType>();
            U9MODocType.ForEach(item =>
            {
                ManufacturingOrderType manufacturingOrderType = ManufacturingOrderTypeExists.FirstOrDefault(t => t.ExternalId == item.Id.ToString()) ?? new ManufacturingOrderType();
                manufacturingOrderType.Code = item.Code;
                manufacturingOrderType.Name = item.Name;
                manufacturingOrderType.Description = item.Description;
                manufacturingOrderType.Active = item.Effective_IsEffective;
                manufacturingOrderType.Company = company;
                manufacturingOrderType.ExternalId = item.Id.ToString();
                ManufacturingOrderTypes.Add(manufacturingOrderType);
            });
            _de.BatchAddElseUpdate(ManufacturingOrderTypes);
        }

    }

    public void SyncU9ManufacturingOrder()
    {
        SyncU9ManufacturingOrderType();
        SqlSugarClient U9Conn = U9Sql.GetDbConn();
        List<SysCompany> Companys = _de.Search<SysCompany>().ToList();
        List<ProjectInfo> ProjectInfos = _de.Search<ProjectInfo>().ToList();
        List<Warehouse> Warehouses = _de.Search<Warehouse>().ToList();
        foreach (SysCompany company in Companys)
        {
            List<ManufacturingOrder> ManufacturingOrderExists = _de.Search<ManufacturingOrder>().Where("company = @company", new {company = company.Id}).ToList();
            List<ManufacturingOrderType> ManufacturingOrderTypeExists = _de.Search<ManufacturingOrderType>().ToList();
            var maxTime = ManufacturingOrderExists.Select(p => p.ExternalUpdateTime).Max();
            string U9_ManufacturingOrder_Sql = @"
            SELECT TOP 10000
	        MO.Org,--制造中心ID
	        MO.CreatedOn AS OrderDate,--下单日期
	        MO.ItemMaster,--产品
	        MO.ProductQty,--订单数量
	        MO.CompleteDate,--订单交期
	        MO.DocNo,--工单号
	        MO.Id,--主键ID
	        MO.DocState,--状态
	        MO.TotalCompleteQty,--已完工数量
	        MO.StartDate,--开工其
	        MO.createdby,--创建人
	        CASE WHEN MO.SrcDoc_SrcDoc_EntityType = 'UFIDA.U9.MO.MO.MO' THEN 	MO.SrcDoc_SrcDocNo ELSE '' END AS SourceOrderNo,
	        MO.MODocType,
	        MO.Cancel_Canceled,--终止
	        MO.descflexfield_privatedescseg7,
	        MO.SCVWh,--仓库
	        MO.Project,--项目名称
	        MO.DescFlexField_PubDescSeg25,--分组标记
	        MO.IsHoldRelease,--挂起
	        ISNULL( MO.ModifiedOn, MO.CreatedOn ) ModifiedOn 
            FROM mo_mo mo 
            WHERE
	        MO.CreatedOn >= '2019-01-01' 
	        AND MO.DocState IN ( 1, 2, 3, 4, 0 ) 
	        AND MO.MODocType NOT IN ( '1007501067449621', '1010612145696350' ) 
            AND MO.Org = @org ";
            string OrderbySql = " ORDER BY ISNULL(MO.ModifiedOn,MO.CreatedOn) ";
            List<dynamic> U9ManufacturingOrder = new List<dynamic>();
            if (maxTime == null)
            {
                U9ManufacturingOrder = U9Conn.Ado.SqlQuery<dynamic>(U9_ManufacturingOrder_Sql + OrderbySql, new { org = company.ExternalId });
            }
            else
            {
                U9ManufacturingOrder = U9Conn.Ado.SqlQuery<dynamic>(U9_ManufacturingOrder_Sql + "AND ISNULL(MO.ModifiedOn, MO.CreatedOn) >= @max " + OrderbySql, new { org = company.ExternalId, max = maxTime });
            }
            // 获取料品
            List<dynamic?> ItemMasterGroupBy = U9ManufacturingOrder.GroupBy(d => d.ItemMaster).Select(d => d.FirstOrDefault()?.ItemMaster).ToList();
            var IdString = string.Join(",", ItemMasterGroupBy.Select(d => $"{d}"));
            var conModels = new List<IConditionalModel>
            {
                new ConditionalModel { FieldName = "externalid", ConditionalType = ConditionalType.In, FieldValue = IdString }
            };
            string materialsStr = _de.Search<Material>().Where(conModels).ToSqlString();
            List<Material> materials = _de.Search<Material>().Where(conModels).ToList();

            List<ManufacturingOrder> ManufacturingOrders = new List<ManufacturingOrder>();
            U9ManufacturingOrder.ForEach(item =>
            {
                Material? Product = materials.FirstOrDefault(t => t.ExternalId == (item.ItemMaster != null ? item.ItemMaster.ToString() : ""));
                if(Product != null) {
                    ManufacturingOrder manufacturingOrder = ManufacturingOrderExists.FirstOrDefault(t => t.ExternalId == item.Id.ToString()) ?? new ManufacturingOrder();
                    manufacturingOrder.Product = Product;
                    manufacturingOrder.Code = item.DocNo;
                    manufacturingOrder.PlannedQty = item.ProductQty;
                    manufacturingOrder.ActualEndDate = item.CompleteDate;
                    manufacturingOrder.OrderDate = item.OrderDate;
                    manufacturingOrder.Status = (ManufacturingOrderStatus)item.DocState;
                    manufacturingOrder.OutputQty = item.TotalCompleteQty;
                    manufacturingOrder.ActualStartDate = item.StartDate;
                    manufacturingOrder.ManufacturingOrderType = ManufacturingOrderTypeExists.FirstOrDefault(t=>t.ExternalId == item.MODocType.ToString());
                    manufacturingOrder.IsCancel = item.Cancel_Canceled;
                    manufacturingOrder.IsHoldRelease = item.IsHoldRelease;
                    manufacturingOrder.GroupTag = item.DescFlexField_PubDescSeg25;
                    manufacturingOrder.Project = item.Project != null ? ProjectInfos.FirstOrDefault(t => t.ExternalId == item.Project.ToString()) : null;
                    manufacturingOrder.ReceivingWarehouse = item.SCVWh != null ? Warehouses.FirstOrDefault(t => t.ExternalId == item.SCVWh.ToString()) : null;
                    manufacturingOrder.Company = company;
                    manufacturingOrder.ExternalId = item.Id.ToString();
                    manufacturingOrder.ExternalUpdateTime = item.ModifiedOn;
                    ManufacturingOrders.Add(manufacturingOrder);
                }
            });
            _de.BatchAddElseUpdate(ManufacturingOrders);
        }

    }
}
