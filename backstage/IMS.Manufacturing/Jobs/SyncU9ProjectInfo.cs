using IMS.Manufacturing.Action;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Manufacturing.Jobs;

[JobDetail("SyncU9ProjectInfo", Description = "从U9同步项目档案", GroupName = "default", Concurrent = false)]
[Hourly(TriggerId = "sync_u9_ProjectInfo", Description = "从U9同步项目档案")]
public class SyncU9ProjectInfo : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        ProjectInfoAction x = new ProjectInfoAction(context.ServiceProvider);
        x.SyncU9ProjectInfo();
    }
}