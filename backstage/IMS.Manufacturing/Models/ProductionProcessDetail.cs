namespace IMS.Manufacturing.Models;

[YhhTable("制程明细")]
public class ProductionProcessDetail : EntityBase
{
    [YhhColumn(ColumnDescription = "生产制程", RelationalType = RelationalType.ManyToOne)]
    public ProductionProcess Process { get; set; }

    [YhhColumn(ColumnDescription = "工序", RelationalType = RelationalType.ManyToOne)]
    public ProductionOperation Operation { get; set; }

    [YhhColumn(ColumnDescription = "顺序")]
    public long Sequence { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }
}