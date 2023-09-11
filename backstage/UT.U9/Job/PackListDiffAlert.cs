// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。


namespace UT.U9.Job;

/// <summary>
/// 查询装箱单差异邮件提醒致指定邮箱
/// </summary>
[JobDetail("job_list_diff_alert", Description = "装箱单差异邮件提醒", GroupName = "default", Concurrent = false)]
[Workday(TriggerId = "maint_reminder", Description = "装箱单对比差异邮件提醒")]
public class PackListDiffAlert : IJob
{
    private readonly IMailKitService _MailKitService;

    public PackListDiffAlert(IMailKitService MailKitService)
    {
        _MailKitService = MailKitService;
    }


    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        
        SqlSugarClient U9Conn = U9Sql.GetDbConn();

        var dt = U9Conn.Ado.UseStoredProcedure().GetDataTable("UTIMS_PackListDiffComp", new { docno = ""});

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

        string title = "装箱单对比差异";
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
                toAddresses = new string[] {};
            }

        }
        catch
        {
            toAddresses = new string[] {};
        }
        string TableHtml = "<table>";

        if (dt != null && dt.Rows.Count > 0)
        {
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

    }
}