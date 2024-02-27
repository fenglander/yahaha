using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.VisualDev.Entity;
[YhhTable("用户列表设计方案")]
public class UserListDesignScheme : EntityBase
{
    [YhhColumn(ColumnDescription = "关联用户名")]
    public string? UserName { get; set; }

    [YhhColumn(ColumnDescription = "关联用户", RelationalType = RelationalType.ManyToOne)]
    public SysUser? RelUser { get; set; }

    [YhhColumn(ColumnDescription = "模型全称")]
    public string? ModelFull { get; set; }

    [YhhColumn(ColumnDescription = "模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel RelModel { get; set; }

    [YhhColumn(ColumnDescription = "关联列表", RelationalType = RelationalType.ManyToOne)]
    public ListDesign? ListDesign { get; set; }

    [YhhColumn(ColumnDescription = "设计数据", DataType = StaticConfig.CodeFirst_BigString)]
    public string? DesignData { get; set; }

    [YhhColumn(ColumnDescription = "默认")]
    public bool? Default { get; set; }
}
