namespace IMS.Basic.Models;

[YhhTable("料号分类档案")]
public class MaterialCategory : EntityBase
{
    [YhhColumn(ColumnDescription = "编码")]
    public string? Code { get; set; }

    [YhhColumn(ColumnDescription = "名称", Display = true)]
    public string? Name { get; set; }

    [YhhColumn(ColumnDescription = "父级", RelationalType = RelationalType.ManyToOne)]
    public MaterialCategory? Parent { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalSysId { get; set; }
}