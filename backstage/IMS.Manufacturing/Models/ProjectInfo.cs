using System.ComponentModel;

namespace IMS.Manufacturing.Models;

[YhhTable("项目档案")]
public class ProjectInfo : EntityCompany
{
    [YhhColumn(ColumnDescription = "编码")]
    public string Code { get; set; } = string.Empty;

    [YhhColumn(ColumnDescription = "名称", Display = true)]
    public string Name { get; set; } = string.Empty;

    [YhhColumn(ColumnDescription = "简称")]
    public string? ShortName { get; set; }

    [YhhColumn(ColumnDescription = "描述", Length = 2000)]
    public string? Description { get; set; }

    [YhhColumn(ColumnDescription = "状态")]
    public ProjectStateEnum State { get; set;}

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }

    /// <summary>
    /// 外部更新时间
    /// </summary>
    [YhhColumn(ColumnDescription = "外部更新时间")]
    public DateTime? ExternalUpdateTime { get; set; }
}

public enum ProjectStateEnum
{
    [Description("开立")]
    Opening,
    [Description("核准中")]
    Approving,
    [Description("核准")]
    Aproved,
    [Description("完成")]
    Accomplish,
    [Description("关闭")]
    Closed
}