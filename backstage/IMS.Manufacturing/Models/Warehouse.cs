namespace IMS.Manufacturing.Models;

[YhhTable("仓库档案","Code")]
public class Warehouse : EntityCompany
{
    [YhhColumn(ColumnDescription = "名称")]
    public string Name { get; set; } = string.Empty;

    [YhhColumn(ColumnDescription = "编码")]
    public string? Code { get; set; }

    [YhhColumn(ColumnDescription = "启动库位")]
    public bool IsStorageBin { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }

    [YhhColumn(ColumnDescription = "相关库位", RelationalType = RelationalType.OneToMany, Related = nameof(StorageBin.Warehouse))]
    public List<StorageBin>? StorageBinList { get; set; }

    [YhhColumn(ColumnDescription = "外部更新时间")]
    public DateTime? ExternalUpdateTime { get; set; }


    public override string ModelTitle => this.Name + "[" + this.Code + "]";

}