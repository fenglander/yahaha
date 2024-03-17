namespace IMS.Manufacturing.Models;

[YhhTable("生产订单类型")]
public class ManufacturingOrderType: EntityCompany
{
    [YhhColumn(ColumnDescription = "编码")]
    public string? Code { get; set; }

    [YhhColumn(ColumnDescription = "名称")]
    public string? Name { get; set; }

    [YhhColumn(ColumnDescription = "描述", Length = 2000)]
    public string? Description { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }
}