using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Models.Entity;
[SystemTable]
[YhhTable("系统模型", "Name")]
public class SysModel:EntityBase
{
    
    [YhhColumn(ColumnDescription = "名称", Display =true)]
    public string? Description { get; set; }

    /// <summary>
    /// 模型
    /// </summary>
    [YhhColumn(ColumnDescription = "模型", Help = "模型名称")]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "表名")]
    public string? FullName { get; set; }

    [YhhColumn(ColumnDescription = "信息")]
    public string? Info { get; set; }

    [YhhColumn(ColumnDescription = "启用租户")]
    public bool IsTenant { get; set; }

    [YhhColumn(ColumnDescription = "字段", RelationalType = RelationalType.OneToMany, Related = "SysModel")]
    public List<SysField> Fields { get; set; }
}
