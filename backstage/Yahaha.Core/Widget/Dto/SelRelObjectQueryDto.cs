using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Widget.Dto;
public class SelRelObjectQueryDto
{
    /// <summary>
    /// 关联模型
    /// </summary>
    public string RelModel { get; set; }
    /// <summary>
    /// 关系字段
    /// </summary>
    public string? RelFieldName { get; set; }
    /// <summary>
    /// 关系字段
    /// </summary>
    public string? RelFieldName2 { get; set; }
}
