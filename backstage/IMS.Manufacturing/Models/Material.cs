namespace IMS.Manufacturing.Models
{
    [YhhTable("料品档案","Code, Name")]
    public class Material : EntityCompany
    {
        [YhhColumn(ColumnDescription = "编码", Display = true)]
        public string Code { get; set; } = string.Empty;

        [YhhColumn(ColumnDescription = "名称", Length = 300)]
        public string Name { get; set; } = string.Empty;

        [YhhColumn(ColumnDescription = "规格", Length = 300)]
        public string? Spce { get; set; }

        [YhhColumn(ColumnDescription = "版本")]
        public string? Version { get; set; }

        [YhhColumn(ColumnDescription = "单位", RelationalType = RelationalType.ManyToOne)]
        public Uom? Unit { get; set; }

        [YhhColumn(ColumnDescription = "分类", RelationalType = RelationalType.ManyToOne)]
        public MaterialCategory? Category { get; set; }

        [YhhColumn(ColumnDescription = "描述", Length = 2000)]
        public string? Description { get; set; }

        [YhhColumn(ColumnDescription = "用量与库存", Length = 2000)]
        public string? UsageAndInventory { get; set; }

        [YhhColumn(ColumnDescription = "外部标识")]
        public string? ExternalId { get; set; }


        [YhhColumn(ColumnDescription = "外部更新时间")]
        public DateTime? ExternalUpdateTime { get; set; }

        [YhhColumn(ColumnDescription = "活动")]
        public bool? Active { get; set; }
    }
}