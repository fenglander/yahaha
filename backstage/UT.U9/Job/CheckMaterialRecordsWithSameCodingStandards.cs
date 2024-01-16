

using Newtonsoft.Json.Linq;
using System.Data;
using UT.TPlus.Base;

namespace UT.U9.Job;
[JobDetail("job_check_same_coding_standards", Description = "检查同编码规格物料档案", GroupName = "default", Concurrent = false)]
[Weekly(TriggerId = "maint_reminder", Description = "检查同编码规格物料档案")]
public class CheckMaterialRecordsWithSameCodingStandards : IJob
{
    private readonly IMailKitService _MailKitService;

    public CheckMaterialRecordsWithSameCodingStandards(IMailKitService MailKitService)
    {
        _MailKitService = MailKitService;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {

        SqlSugarClient U9Conn = U9Sql.GetDbConn();

        var dt = U9Conn.Ado.GetDataTable(@"select t1.code as Code,t1.name as T名称,t1.specification as T规格型号,t4.name 仓库,round(BaseQuantity,2) 现存量,UP.Code u9料号
        from [TJ].[UFTData801045_000005].[dbo].[AA_Inventory] t1
        join 
        (
        SELECT name,specification FROM [TJ].[UFTData801045_000005].[dbo].[AA_Inventory]
        where disabled = 0
        group by name,specification
        HAVING count(*)> 1
        ) t2 on t1.name+t1.specification = t2.name+t2.specification
        left join [TJ].[UFTData801045_000005].[dbo].ST_CurrentStock t3 on t1.id = t3.idinventory ----and isnull(t3.BaseQuantity,0)>0
        left join [TJ].[UFTData801045_000005].[dbo].AA_Warehouse t4 on t3.idwarehouse = t4.id 
        left join [dbo].[CBO_ItemMaster] up on up.SPECS=t1.specification and up.Code not like '%A' AND UP.Effective_DisableDate>=GETDATE() AND UP.Effective_IsEffective=1 
        where t1.disabled = 0
        and not exists(select 1 from [dbo].[CBO_ItemMaster] P where P.Code=t1.code and p.SPECS=t1.specification)
        order by t1.name,t1.specification,t1.code"
        );

        if (dt == null && dt.Rows.Count == 0) {   return; }

        foreach (DataRow row in dt.Rows)
        {
            if (row["现存量"] != DBNull.Value)
            {
                decimal originalValue = Convert.ToDecimal(row["现存量"]);
                decimal adjustedValue = Math.Round(originalValue, 2);
                row["现存量"] = adjustedValue;
            }
        }

        string title = "相同规格，T+与U9料号不一致";
        string[] toAddresses;
        try
        {
            if (context.JobDetail.Properties != "{}" && context.JobDetail.Properties != null && context.JobDetail.Properties != "" && IsJsonStringDictionary(context.JobDetail.Properties))
            {
                string addstr = JObject.Parse(context.JobDetail.Properties)["to"].ToString();
                toAddresses = addstr.Split(',');
            }
            else
            {
                toAddresses = new string[] { };
            }

        }
        catch
        {
            toAddresses = new string[] { };
        }
        string TableHtml = "<table>";

            TableHtml += "<tr>";
            foreach (DataColumn column in dt.Columns)
            {
                TableHtml += "<th>" + column.ColumnName + "</th>";
            }
            TableHtml += "</tr>";

            // 生成数据行
            foreach (DataRow row in dt.Rows)
            {
                TableHtml += "<tr>";
                foreach (var item in row.ItemArray)
                {
                    TableHtml += "<td>" + item + "</td>";
                }
                TableHtml += "</tr>";
            }

            TableHtml += "</table>";
            string MailHtml = @"
        <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

        <html xmlns=""http://www.w3.org/1999/xhtml"">

　           <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>" + title + @"</title>
            <style>
                table {
                width: 100%;
                border-collapse: collapse;
                border: 1px solid #ddd;
                }
                th, td {
                padding: 8px;
                text-align: left;
                border-bottom: 1px solid #ddd;
                }
                th {
                background-color: #f2f2f2;
                }
            </style>
            </head>

            <body>
                " + TableHtml + @"
            </body>

        </html>        
        ";

            // 设置邮件收件人、主题和正文（HTML格式）
            await _MailKitService.SendEmailAsync(MailHtml, true, toAddresses, title);


    }

    static DataTable MergeDataTables(DataTable table1, DataTable table2, string columnName)
    {
        // 使用 LINQ 进行关联合并
        var query = from row1 in table1.AsEnumerable()
                    join row2 in table2.AsEnumerable()
                    on row1[columnName] equals row2[columnName] into gj
                    from subRow in gj.DefaultIfEmpty()
                    select row1.ItemArray.Concat(subRow?.ItemArray ?? Enumerable.Repeat<object>(null, table2.Columns.Count)).ToArray();

        // 创建合并后的 DataTable
        DataTable mergedTable = new DataTable();

        // 添加合并后的 DataTable 列
        foreach (DataColumn column in table1.Columns)
        {
            mergedTable.Columns.Add(column.ColumnName, column.DataType);
        }

        foreach (DataColumn column in table2.Columns)
        {
            // 确保列的唯一性，如果存在相同的列名，就在列名后加上下划线和索引
            string columnNameToUse = column.ColumnName;
            int index = 1;
            while (mergedTable.Columns.Contains(columnNameToUse))
            {
                columnNameToUse = $"{column.ColumnName}_{index}";
                index++;
            }
            mergedTable.Columns.Add(columnNameToUse, column.DataType);
        }

        // 添加数据到合并后的 DataTable
        foreach (var rowArray in query)
        {
            mergedTable.Rows.Add(rowArray);
        }

        return mergedTable;
    }

    static bool IsJsonStringDictionary(string jsonString)
    {
        try
        {
            JObject json = JObject.Parse(jsonString);
            return json != null && json.Count > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
