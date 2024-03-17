using IMS.Manufacturing.Action;
using IMS.Manufacturing.Models;
using Nest;
using Newtonsoft.Json;
using SqlSugar;
using UT.APS.Base;
using Yahaha.Plugin.CommonTool;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ComponentTCBBatchCreateContainerServiceVersionRequest.Types;

namespace IMS.Manufacturing.Jobs;

[JobDetail("SyncProductionProcess", Description = "从APS同步生产制程", GroupName = "default", Concurrent = false)]
[Workday(TriggerId = "sync_production_process", Description = "从APS同步生产制程")]
public class SyncProductionProcess : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        ProductionProcessAction x = new ProductionProcessAction(context.ServiceProvider);
        x.SyncAPSProcess();
    }
}