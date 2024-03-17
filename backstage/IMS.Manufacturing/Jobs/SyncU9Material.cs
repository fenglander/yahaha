using IMS.Manufacturing.Action;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Manufacturing.Jobs;

[JobDetail("SyncU9Material", Description = "从U9同步料品和料品分类", GroupName = "default", Concurrent = false)]
[Hourly(TriggerId = "sync_u9_Material", Description = "从U9同步料品和料品分类")]
public class SyncU9Material : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        MaterialAction x = new MaterialAction(context.ServiceProvider);
        x.SyncU9Material();
    }
}