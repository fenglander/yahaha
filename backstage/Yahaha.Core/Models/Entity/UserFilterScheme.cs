using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Models.Entity;

[SugarTable(null, "用户筛选方案")]
[SystemTable]
public class UserFilterScheme : EntityBase
{

    [SugarColumn(ColumnDescription = "名称")]
    public string Name { get; set; }

    [SugarColumn(ColumnDescription = "表名")]
    public string? TableName { get; set; }

    [SugarColumn(ColumnDescription = "模型ID")]
    public long? ModelId { get; set; }

    [YhhColumn(ColumnDescription = "模型",RelationalType = RelationalType.ManyToOne)]
    public SysModels SysModel { get; set; } //不能赋值只能是null

    [SugarColumn(ColumnDescription = "默认查询字段")]
    public string? DefaultFields { get; set; }

    [SugarColumn(ColumnDescription = "默认查询参数")]
    public string? DefaultFilter { get; set; }

    [SugarColumn(ColumnDescription = "默认")]
    public bool? Default { get; set; }

}
