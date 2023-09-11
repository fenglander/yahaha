using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Models.Entity;
[SugarTable(null, "模型")]
[SystemTable]
public class SysModels:EntityBase
{
    [SugarColumn(ColumnDescription = "名称")]
    public string? Name { get; set; }

    [SugarColumn(ColumnDescription = "模型")]
    public string Model { get; set; }
    [SugarColumn(ColumnDescription = "表名")]
    public string? TableName { get; set; }

    [SugarColumn(ColumnDescription = "信息")]
    public string? Info { get; set; }

    [SugarColumn(ColumnDescription = "启用租户")]
    public bool IsTenant { get; set; }

    [SugarColumn(ColumnDescription = "字段")]
    [Navigate(NavigateType.OneToMany, nameof(SysFields.ModelId))]
    public List<SysFields> Fields { get; set; }//注意禁止手动赋值
}
