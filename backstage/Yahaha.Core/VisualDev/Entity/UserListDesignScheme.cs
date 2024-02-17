using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.VisualDev.Entity;
public class UserListDesignScheme : EntityBase
{
    [YhhColumn(ColumnDescription = "关联用户")]
    public string? UserName { get; set; }
    [YhhColumn(ColumnDescription = "关联用户")]
    public SysUser? User { get; set; }

    [YhhColumn(ColumnDescription = "模型全称")]
    public string? SysModelFull { get; set; }

    [YhhColumn(ColumnDescription = "模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel SysModel { get; set; }

    [YhhColumn(ColumnDescription = "关联列表", RelationalType = RelationalType.ManyToOne)]
    public ListDesign? ListDesign { get; set; }

    [YhhColumn(ColumnDescription = "设计数据")]
    public string? DesignData { get; set; }

    [YhhColumn(ColumnDescription = "默认")]
    public bool? Default { get; set; }
}
