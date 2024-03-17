using IMS.Manufacturing.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Manufacturing.Jobs;
[JobDetail("SyncU9ManufacturingOrder", Description = "从U9同步订单和订单分类", GroupName = "default", Concurrent = false)]
[Hourly(TriggerId = "sync_u9_ManufacturingOrder", Description = "从U9同步订单和订单分类")]
public class SyncU9ManufacturingOrder : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        ManufacturingOrderAction x = new ManufacturingOrderAction(context.ServiceProvider);
        x.SyncU9ManufacturingOrder();
    }
}