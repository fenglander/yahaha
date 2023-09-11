using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Service.Role.Dto;
/// <summary>
/// 下探数据传参
/// </summary>
public class DrillDownDataDto
{
    /// <summary>
    /// 模型名称
    /// </summary>
    public string model { set; get; }
    /// <summary>
    /// 条件值
    /// </summary>
    public long id { set; get; } = 0;
    /// <summary>
    /// 条件字段
    /// </summary>
    public string relField { set; get; } = "id";
    /// <summary>
    /// 当前下探层
    /// </summary>
    public int curLevel { set; get; } = 0;
    /// <summary>
    /// 最大层（否则会导致循环）
    /// </summary>
    public int maxLevel { set; get; } = 3;
    /// <summary>
    /// 元数据
    /// </summary>
    public List<dynamic> items { get; set; }

}