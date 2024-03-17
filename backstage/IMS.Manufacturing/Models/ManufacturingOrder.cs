using SqlSugar;
using System.ComponentModel;

namespace IMS.Manufacturing.Models;

[YhhTable("生产订单", "Code")]
public class ManufacturingOrder : EntityCompany
{
    [YhhColumn(ColumnDescription = "编码", Display = true)]
    public string? Code { get; set; }

    [YhhColumn(ColumnDescription = "顶层MO", RelationalType = RelationalType.ManyToOne)]
    public string? TopLevelOrder { get; set; }

    [YhhColumn(ColumnDescription = "产品", RelationalType = RelationalType.ManyToOne)]
    public Material Product { get; set; }

    [YhhColumn(ColumnDescription = "产品名称", RelationalType = RelationalType.Relate, Related = "Product.Name")]
    public string? ProductName { get; set; }

    [YhhColumn(ColumnDescription = "产品规格", RelationalType = RelationalType.Relate, Related = "Product.Spce")]
    public string? ProductSpce { get; set; }

    [YhhColumn(ColumnDescription = "产品描述", RelationalType = RelationalType.Relate, Related = "Product.Description")]
    public string? ProductDescription { get; set; }

    [YhhColumn(ColumnDescription = "产品分类", RelationalType = RelationalType.Relate, Related = "Product.Category")]
    public MaterialCategory? ProductCategory { get; set; }

    [YhhColumn(ColumnDescription = "版本")]
    public string? ProductVersion { get; set; }

    [YhhColumn(ColumnDescription = "生产单位", RelationalType = RelationalType.ManyToOne)]
    public Uom? Unit { get; set; }

    [YhhColumn(ColumnDescription = "客户料号")]
    public string? OemProductSN { get; set; }

    [YhhColumn(ColumnDescription = "状态")]
    public ManufacturingOrderStatus Status { get; set; }

    [YhhColumn(ColumnDescription = "计划产量")]
    public decimal PlannedQty { get; set; }

    [YhhColumn(ColumnDescription = "投入量")]
    public decimal? InputQty { get; set; }

    [YhhColumn(ColumnDescription = "产出量")]
    public decimal? OutputQty { get; set; }

    [YhhColumn(ColumnDescription = "转移数量")]
    public decimal? TransferredQty { get; set; }

    [YhhColumn(ColumnDescription = "制造群", RelationalType = RelationalType.ManyToOne)]
    public SysOrg? ManufacturingGroup { get; set; }

    [YhhColumn(ColumnDescription = "工单种类")]
    public WorkOrderType? WorkOrderType { get; set; }

    [YhhColumn(ColumnDescription = "订单日期")]
    public DateTime? OrderDate { get; set; }

    [YhhColumn(ColumnDescription = "计划投产日期")]
    public DateTime PlannedStartDate { get; set; }

    [YhhColumn(ColumnDescription = "计划完成日期")]
    public DateTime PlannedEndDate { get; set; }

    [YhhColumn(ColumnDescription = "实际投产日期")]
    public DateTime? ActualStartDate { get; set; }

    [YhhColumn(ColumnDescription = "实际完成日期")]
    public DateTime? ActualEndDate { get; set; }

    [YhhColumn(ColumnDescription = "出货日期")]
    public DateTime? ShippingDate { get; set; }

    [YhhColumn(ColumnDescription = "物料投入日期")]
    public DateTime? MaterialInputDate { get; set; }

    [YhhColumn(ColumnDescription = "物料释放日期")]
    public DateTime? MaterialReleaseDate { get; set; }

    [YhhColumn(ColumnDescription = "制造类型")]
    public ManufacturingType? ManufacturingType { get; set; }

    [YhhColumn(ColumnDescription = "备料明细", RelationalType = RelationalType.OneToMany, Related = "Order")]
    public List<ManufacturingOrderMaterial>? MaterialDetails { get; set; }

    [YhhColumn(ColumnDescription = "入库量")]
    public decimal? ReceiptQty { get; set; }

    [YhhColumn(ColumnDescription = "ERP入库量")]
    public decimal? ERPReceiptQty { get; set; }

    [YhhColumn(ColumnDescription = "销售订单需求量")]
    public decimal? SODemandQty { get; set; }

    [YhhColumn(ColumnDescription = "现存量")]
    public decimal? CurrInvQty { get; set; }

    [YhhColumn(ColumnDescription = "计划排产量")]
    public decimal? ScheduledQty { get; set; }

    [YhhColumn(ColumnDescription = "安全库存量")]
    public decimal? SafetyStockQty { get; set; }

    [YhhColumn(ColumnDescription = "库存可供应天数")]
    public decimal? DaysofSupply { get; set; }

    [YhhColumn(ColumnDescription = "库存可供应月数")]
    public decimal? MonthsofSupply { get; set; }

    [YhhColumn(ColumnDescription = "计划员备注", DataType = StaticConfig.CodeFirst_BigString)]
    public string? PlannerNote { get; set; }

    [YhhColumn(ColumnDescription = "生产订单类型", RelationalType = RelationalType.ManyToOne)]
    public ManufacturingOrderType? ManufacturingOrderType { get; set; }

    [YhhColumn(ColumnDescription = "入库仓库", RelationalType = RelationalType.ManyToOne)]
    public Warehouse? ReceivingWarehouse { get; set; }

    [YhhColumn(ColumnDescription = "项目", RelationalType = RelationalType.ManyToOne)]
    public ProjectInfo? Project { get; set; }

    [YhhColumn(ColumnDescription = "项目编号", RelationalType = RelationalType.Relate, Related = "Project.Code")]
    public string? ProjectCode { get; set; }
    /// <summary>
    /// 分组标记
    /// </summary>
    [YhhColumn(ColumnDescription = "分组标记")]
    public string? GroupTag { get; set; }

    [YhhColumn(ColumnDescription = "版本")]
    public decimal? Version { get; set; }
    /// <summary>
    /// 终止
    /// </summary>
    [YhhColumn(ColumnDescription = "终止")]
    public bool? IsCancel { get; set; }
    /// <summary>
    /// 挂起
    /// </summary>
    [YhhColumn(ColumnDescription = "挂起")]
    public bool? IsHoldRelease { get; set; }

    [YhhColumn(ColumnDescription = "生产制程", RelationalType = RelationalType.ManyToOne)]
    public ProductionProcess? Process { get; set; }

    [YhhColumn(ColumnDescription = "用量与库存", RelationalType = RelationalType.Relate, Related = "Product.UsageAndInventory")]
    public string? UsageAndInventory { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }

    [YhhColumn(ColumnDescription = "外部更新时间")]
    public DateTime? ExternalUpdateTime { get; set; }

}

public enum ManufacturingOrderStatus
{
    [Description("草稿")]
    Draft,

    [Description("审核")]
    Approved,

    [Description("排产")]
    Scheduled,

    [Description("生产中")]
    InProduction,

    [Description("关闭")]
    Closed,

    [Description("完工")]
    Finished
}

public enum WorkOrderType
{
    [Description("主板SMT段工单")]
    SMT = 1,

    [Description("主板PCB段工单")]
    PCB,

    [Description("系统组装工单")]
    SYS,

    [Description("虚拟工单")]
    DUMMY,
}

public enum ManufacturingType
{
    [Description("CTO制造模式")]
    CTO = 1,

    [Description("BTO制造模式")]
    BTO,
}