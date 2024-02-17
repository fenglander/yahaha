using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.VisualDev;


/// <summary>
/// 可视化开发功能实体
/// </summary>
[SugarTable(null, "列表设计")]
[SystemTable]
public class ListDesign : EntityBase
{
    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称")]
    [YhhColumn(Display = true)]
    public string FullName { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码")]
    public string? EnCode { get; set; }

    /// <summary>
    /// 状态(0-暂存（默认），1-发布)
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int State { get; set; } = 0;

    /// <summary>
    /// 类型
    /// 1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单
    /// </summary>
    [SugarColumn(ColumnDescription = "类型")]
    public int Type { get; set; }

    [YhhColumn(ColumnDescription = "模型ID")]
    public long? ModelId { get; set; }
    /// <summary>
    /// 模型
    /// </summary>
    [YhhColumn(ColumnDescription = "模型",RelationalType = RelationalType.ManyToOne)]
    public SysModel SysModel { get; set; }
    /// <summary>
    /// 类名称
    /// </summary>
    [SugarColumn(ColumnDescription = "类名称")]
    public string? ModelFullName { get; set; }
    /// <summary>
    /// 列表配置JSON
    /// </summary>
    [SugarColumn(ColumnDescription = "列表配置", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? ColumnData { get; set; }

    /// <summary>
    /// 描述或说明
    /// </summary>
    [SugarColumn(ColumnDescription = "描述或说明")]
    public string? Description { get; set; }

    /// <summary>
    /// App列表配置JSON.
    /// </summary>
    [SugarColumn(ColumnDescription = "App列表配置JSON", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? AppColumnData { get; set; }
}