
namespace Yahaha.Core.Models;

// 自定义字段特性
[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class ColumnExt : Attribute
{

    /// <summary>
    /// 帮助
    /// </summary>
    public string Help { get; set; }
    /// <summary>
    /// 必填
    /// </summary>
    public bool? Required { get; set; }
    /// <summary>
    /// 只读
    /// </summary>
    public bool? Readonly { get; set; }
    /// <summary>
    /// default 删除关联记录后，当前记录会被设置成默认值。
    /// restrict 删除关联记录时，如果与本记录存在关联，则不允许删除。
    /// cascade 删除关联记录后，当前记录也会被删除。
    /// null 删除关联记录后，当前值会设为null。
    /// </summary>
    public string? OnDelete { get; set; }
    /// <summary>
    /// 关联表
    /// </summary>
    public string? Relation { get; set; }
    /// <summary>
    /// 管理表索引字段
    /// </summary>
    public string? RelationField { get; set; }
    /// <summary>
    /// 关联字段
    /// </summary>
    public string? Related { get; set; }
    /// <summary>
    /// 索引
    /// </summary>
    public bool Index { get; set; }


    // 添加自定义逻辑
    public void CustomMethod()
    {
        Console.WriteLine("Custom logic in CustomSugarColumn.");
    }
}


