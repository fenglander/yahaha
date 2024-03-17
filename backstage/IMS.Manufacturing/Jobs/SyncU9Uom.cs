using IMS.Manufacturing.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Manufacturing.Jobs;
[JobDetail("SyncU9Uom", Description = "从U9同步单位", GroupName = "default", Concurrent = false)]
[Workday(TriggerId = "sync_u9_uom", Description = "从U9同步单位")]
public class SyncU9Uom : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        UomAction x = new UomAction(context.ServiceProvider);
        x.SyncU9Uom();
    }
}
