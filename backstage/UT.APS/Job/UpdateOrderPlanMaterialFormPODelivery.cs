using AngleSharp.Common;
using Furion.Schedule;
using Nest;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UT.APS.Base;
using UT.U9.Base;
using DbType = SqlSugar.DbType;

namespace UT.APS.Job;
/// <summary>
/// 查询逾期未执行保养计划并发送给指定邮箱
/// </summary>
[JobDetail("job_aps_PlanMaterial_po_delivery", Description = "更新物料配套表交期", GroupName = "default", Concurrent = false)]
[Hourly(TriggerId = "trigger_aps_PlanMaterial_po_delivery", Description = "更新物料配套表交期")]
public class UpdateOrderPlanMaterialFormPODelivery : IJob
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        SqlSugarClient ApsConn = ApsSql.GetDbConn();
        SqlSugarClient U9Conn = U9Sql.GetDbConn();
        string sql = @"SELECT MaterialFormID,PODocs,A.Code,A.OweQty * -1 AS OweQty FROM APS_OrderPlanMaterialForm A
        INNER JOIN APS_Order A2 ON A.OrderID=A2.OrderID
        WHERE A2.CompletionDate IS NULL AND A.OweQty < 0 AND len(PODocs)>0 ";
        var ApsDt = await ApsConn.Ado.GetDataTableAsync(sql);
        string POCodes = "";
        foreach (DataRow row in ApsDt.Rows)
        {
            POCodes = POCodes + row["PODocs"] + ',';
        }

        // 使用逗号分隔并去掉空白字符
        string[] AllLines = POCodes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        // 去除重复项
        HashSet<string> uniqueLines = new HashSet<string>(AllLines);
        AllLines = uniqueLines.ToArray();

        sql = @"select a.DocNo,b.ItemInfo_ItemCode,c.PlanArriveQtyTU,c.DeliveryDate
        from PM_PurchaseOrder a
        Join PM_POLine b on a.ID=b.PurchaseOrder
        Join PM_POShipLine c on b.ID=c.POLine
         where a.DocNo in(@DocNo) ";
        // 获取U9数据
        var U9Dt = await U9Conn.Ado.GetDataTableAsync(sql, new { DocNo = AllLines });

        var dtList = new List<Dictionary<string, object>>();
        foreach (DataRow row in ApsDt.Rows)
        {
            string[] lines = ((string)row["PODocs"]).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string Code = (string)row["Code"];
            // 筛选对应交期
            var filteredData = from r in U9Dt.AsEnumerable()
                               where lines.Contains(r.Field<string>("DocNo")) && Code == r.Field<string>("ItemInfo_ItemCode")
                               orderby r.Field<DateTime>("DeliveryDate")
                               select r;
            decimal OweQty = (decimal)row["OweQty"];
            List<DataRow> filteredRows = filteredData.ToList();
            DateTime minValue = DateTime.MinValue;
            if (filteredData.Any())
            {
                foreach(var data in filteredData)
                {
                    var PlanArriveQtyTU = (decimal)data["PlanArriveQtyTU"];
                    OweQty = OweQty - PlanArriveQtyTU;
                    if (OweQty <= 0)
                    {
                        minValue = (DateTime)data["DeliveryDate"];
                        break;
                    }
                }
            }

            var dt = new Dictionary<string, object>
            {
                { "Remark3", minValue.ToString("yyyy-MM-dd") },
                { "MaterialFormID", (long)row["MaterialFormID"] }
            };
            dtList.Add(dt);
        }

        var t66 = ApsConn.Updateable(dtList).AS("APS_OrderPlanMaterialForm").WhereColumns("MaterialFormID").ExecuteCommand();


    }
}