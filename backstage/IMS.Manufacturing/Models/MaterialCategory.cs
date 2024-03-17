using Yahaha.Core.Entity;

namespace IMS.Manufacturing.Models;

[YhhTable("料号分类档案", "Code")]
public class MaterialCategory : EntityCompany
{
    [YhhColumn(ColumnDescription = "编码")]
    public string? Code { get; set; }

    [YhhColumn(ColumnDescription = "名称", Display = true)]
    public string Name { get; set; } = string.Empty;

    [YhhColumn(ColumnDescription = "备注")]
    public string? Remarks { get; set; }

    [YhhColumn(ColumnDescription = "父级", RelationalType = RelationalType.ManyToOne)]
    public MaterialCategory? Parent { get; set; }

    [YhhColumn(ColumnDescription = "子级", RelationalType = RelationalType.OneToMany, Related = "Parent", IsOnlyIgnoreUpdate = true)]
    public List<MaterialCategory>? Childs { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }
}