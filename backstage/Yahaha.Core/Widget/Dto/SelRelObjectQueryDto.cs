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
    public string RelModelName { get; set; }
    /// <summary>
    /// 关键词
    /// </summary>
    public string? Keywords { get; set; }
    /// <summary>
    /// 记录数
    /// </summary>
    public int PageSize { get; set; } = 10;
}
