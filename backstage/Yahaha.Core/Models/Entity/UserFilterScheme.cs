using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.VisualDev;

namespace Yahaha.Core.Models.Entity;

[SugarTable(null, "用户查询方案")]
[SystemTable]
public class UserFilterScheme : EntityBase
{

    [YhhColumn(ColumnDescription = "名称")]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "关联用户")]
    public string? UserName { get; set; }
    [YhhColumn(ColumnDescription = "关联用户")]
    public SysUser? User { get; set; }

    [YhhColumn(ColumnDescription = "模型全称")]
    public string? SysModelFull { get; set; }

    [YhhColumn(ColumnDescription = "模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel SysModel { get; set; }

    [YhhColumn(ColumnDescription = "关联列表设计", RelationalType = RelationalType.ManyToOne)]
    public ListDesign? ListDesign { get; set; }

    [YhhColumn(ColumnDescription = "查询字段")]
    public string? DefaultFields { get; set; }

    [YhhColumn(ColumnDescription = "查询参数")]
    public string? DefaultFilter { get; set; }

    [YhhColumn(ColumnDescription = "默认")]
    public bool? Default { get; set; }

}
