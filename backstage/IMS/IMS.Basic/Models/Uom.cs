namespace IMS.Basic.Models;

public enum UomType
{ Reference, Bigger, Smaller };

[YhhTable("计量单位档案")]
public class Uom : EntityBase
{
    [YhhColumn(ColumnDescription = "名称", Display = true, NotNull = true)]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "分类",RelationalType = RelationalType.ManyToOne)]
    public UomCategory Category { get; set; }

    [YhhColumn(ColumnDescription = "精度")]
    public long? Rounding { get; set; }

    /// <summary>
    ///  <para>reference, Reference Unit of Measure for this category</para>
    ///  <para>bigger, Bigger than the reference Unit of Measure</para>
    ///  <para>smaller, Smaller than the reference Unit of Measure</para>
    /// </summary>
    [YhhColumn(ColumnDescription = "类型")]
    public UomType UomType { get; set; }

    [YhhColumn(ColumnDescription = "比率")]
    public long? Ratio { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalSysId { get; set; }
}