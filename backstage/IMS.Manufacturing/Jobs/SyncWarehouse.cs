using IMS.Manufacturing.Action;
using IMS.Manufacturing.Models;
using Nest;
using Newtonsoft.Json;
using SqlSugar;
using UT.APS.Base;
using Yahaha.Plugin.CommonTool;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ComponentTCBBatchCreateContainerServiceVersionRequest.Types;

namespace IMS.Manufacturing.Jobs;

[JobDetail("SyncU9Warehouse", Description = "从U9同步仓库档案", GroupName = "default", Concurrent = false)]
[Workday(TriggerId = "sync_u9_warehouse", Description = "从U9同步仓库档案")]
public class SyncWarehouse : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        WarehouseAction x = new WarehouseAction(context.ServiceProvider);
        x.SyncU9Warehouse();
    }
}