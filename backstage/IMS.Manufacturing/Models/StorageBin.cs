namespace IMS.Manufacturing.Models;

[YhhTable("库位档案")]
public class StorageBin : EntityCompany
{
    [YhhColumn(ColumnDescription = "编码")]
    public string? Code { get; set; }

    [YhhColumn(ColumnDescription = "名称")]
    public string? Name { get; set; }

    [YhhColumn(ColumnDescription = "仓库", RelationalType = RelationalType.ManyToOne)]
    public Warehouse Warehouse { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }

    [YhhColumn(ColumnDescription = "外部更新时间")]
    public DateTime? ExternalUpdateTime { get; set; }

    public override string ModelTitle => this.Name + "[" + this.Code + "]";
}