namespace IMS.Manufacturing.Models;

[YhhTable("生产订单备料明细")]
public class ManufacturingOrderMaterial : EntityBase
{
    [YhhColumn(ColumnDescription = "生产订单", RelationalType = RelationalType.ManyToOne)]
    public ManufacturingOrder Order { get; set; }

    [YhhColumn(ColumnDescription = "材料", RelationalType = RelationalType.ManyToOne)]
    public Material Material { get; set; }

    [YhhColumn(ColumnDescription = "材料名称", RelationalType = RelationalType.Relate, Related = "Material.Name")]
    public string? ProductName { get; set; }

    [YhhColumn(ColumnDescription = "材料规格", RelationalType = RelationalType.Relate, Related = "Material.Spce")]
    public string? ProductSpce { get; set; }

    [YhhColumn(ColumnDescription = "材料描述", RelationalType = RelationalType.Relate, Related = "Material.Description")]
    public string? ProductDescription { get; set; }

    [YhhColumn(ColumnDescription = "材料分类", RelationalType = RelationalType.Relate, Related = "Material.Category")]
    public MaterialCategory? ProductCategory { get; set; }

    [YhhColumn(ColumnDescription = "计量单位", RelationalType = RelationalType.ManyToOne)]
    public Uom? Unit { get; set; }

    [YhhColumn(ColumnDescription = "替代料")]
    public bool? IsSubstitute { get; set; }

    [YhhColumn(ColumnDescription = "计划需求日期")]
    public DateTime? PlanReqDate { get; set; }

    [YhhColumn(ColumnDescription = "实际需求日期")]
    public DateTime? ActualReqDate { get; set; }

    [YhhColumn(ColumnDescription = "实际发料日期")]
    public DateTime? ActualIssueDate { get; set; }

    [YhhColumn(ColumnDescription = "计划用量")]
    public decimal? PlannedQty { get; set; }

    [YhhColumn(ColumnDescription = "已领用量")]
    public decimal? IssuedQty { get; set; }

    [YhhColumn(ColumnDescription = "领料未发数量")]
    public decimal? IssueNotDeliverQty { get; set; }
}