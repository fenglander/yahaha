using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NewLife.Caching;
using Newtonsoft.Json;
using SqlSugar;
using UT.APS.Base;
using Yahaha.Core;

namespace IMS.Manufacturing.Models;

[YhhTable("生产制程")]
public class ProductionProcess : EntityBase
{

    [YhhColumn(ColumnDescription = "编码")]
    public string Code { get; set; } = string.Empty;

    [YhhColumn(ColumnDescription = "名称", Display = true)]
    public string Name { get; set; } = string.Empty;

    [YhhColumn(ColumnDescription = "简称")]
    public string? ShortName { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "工序明细", RelationalType = RelationalType.OneToMany, Related = "Process")]
    public List<ProductionProcessDetail>? Details { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }


}