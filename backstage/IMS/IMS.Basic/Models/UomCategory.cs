namespace IMS.Basic.Models;

[YhhTable("单位分类")]
public class UomCategory : EntityBase
{
    [YhhColumn(ColumnDescription = "名称", Display = true)]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalSysId { get; set; }
}