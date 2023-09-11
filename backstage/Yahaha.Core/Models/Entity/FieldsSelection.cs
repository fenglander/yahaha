

namespace Yahaha.Core.Models.Entity;
[SugarTable(null, "字段字典")]
[SystemTable]
public class SysFieldsSelection : EntityBase
{

    [SugarColumn(ColumnDescription = "字段")]
    [ColumnExt(Relation = "SysFields")]
    public long Field { get; set; }

    [SugarColumn(ColumnDescription = "值")]
    public string value { get; set; }

    [SugarColumn(ColumnDescription = "名称")]
    public string? Name { get; set; }

    [SugarColumn(ColumnDescription = "顺序")]
    public int Sequence { get; set; }

}
