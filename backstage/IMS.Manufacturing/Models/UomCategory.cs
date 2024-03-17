namespace IMS.Manufacturing.Models;

[YhhTable("单位分类")]
public class UomCategory : EntityBase
{
    [YhhColumn(ColumnDescription = "名称", Display = true)]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "编号")]
    public string Code { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }
}