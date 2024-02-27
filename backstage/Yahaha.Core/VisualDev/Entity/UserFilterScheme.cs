using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.VisualDev;

namespace Yahaha.Core.VisualDev.Entity;

[SugarTable(null, "用户查询方案")]
[SystemTable]
public class UserFilterScheme : EntityBase
{

    [YhhColumn(ColumnDescription = "名称")]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "关联用户")]
    public string UserName { get; set; }
    [YhhColumn(ColumnDescription = "关联用户", RelationalType = RelationalType.ManyToOne)]
    public SysUser RelUser { get; set; }

    [YhhColumn(ColumnDescription = "模型全称")]
    public string ModelFull { get; set; }

    [YhhColumn(ColumnDescription = "模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel RelModel { get; set; }

    [YhhColumn(ColumnDescription = "关联列表设计", RelationalType = RelationalType.ManyToOne)]
    public ListDesign ListDesign { get; set; }

    [YhhColumn(ColumnDescription = "查询字段",DataType = StaticConfig.CodeFirst_BigString)]
    public string DefaultFields { get; set; }

    [YhhColumn(ColumnDescription = "查询参数", DataType = StaticConfig.CodeFirst_BigString)]
    public string DefaultFilter { get; set; }

    [YhhColumn(ColumnDescription = "默认")]
    public bool? Default { get; set; }

}
