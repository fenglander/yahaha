

namespace Yahaha.WMS;

[SugarTable(null,"存货档案")]
public class Inventory : EntityBase
{

    [YhhColumn(ColumnDescription = "Completed Quantity", DecimalDigits = 7)]
    public float qty_operation_wip { get; set; }

    [YhhColumn(ColumnDescription = "Code")]
    public string? Code { get; set; }
    
    [YhhColumn(ColumnDescription = "单位")]
    public int? Unit { get; set; }

    [YhhColumn(ColumnDescription = "名称",Display = true)]
    public string? Name { get; set; }

    [YhhColumn(ColumnDescription = "受控")]
    public bool Audit { get; set; }
    [YhhColumn(ColumnDescription = "规格")]
    public string? Spce { get; set; }
}
