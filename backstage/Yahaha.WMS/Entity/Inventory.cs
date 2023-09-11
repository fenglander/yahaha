

namespace Yahaha.WMS;

[SugarTable(null,"存货档案")]
public class Inventory : EntityBase
{

    [SugarColumn(ColumnDescription = "Completed Quantity")]
    public float qty_operation_wip { get; set; }

    [SugarColumn(ColumnDescription = "Code")]
    public string? code { get; set; }
    
    [SugarColumn(ColumnDescription = "单位")]
    public int? Unit { get; set; }


}
