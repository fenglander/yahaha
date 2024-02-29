namespace IMS.Basic.Models
{
    [YhhTable("料品档案")]
    public class Material : EntityBase
    {
        [YhhColumn(ColumnDescription = "编码", Display = true)]
        public string? Code { get; set; }

        [YhhColumn(ColumnDescription = "名称")]
        public string? Name { get; set; }

        [YhhColumn(ColumnDescription = "规格")]
        public string? Spce { get; set; }

        [YhhColumn(ColumnDescription = "版本")]
        public string? Version { get; set; }

        [YhhColumn(ColumnDescription = "单位", RelationalType = RelationalType.ManyToOne)]
        public Uom? Unit { get; set; }

        [YhhColumn(ColumnDescription = "分类", RelationalType = RelationalType.ManyToOne)]
        public MaterialCategory? Category { get; set; }

        [YhhColumn(ColumnDescription = "描述")]
        public string? Description { get; set; }


        [YhhColumn(ColumnDescription = "外部标识")]
        public string? ExternalSysId { get; set; }
    }
}