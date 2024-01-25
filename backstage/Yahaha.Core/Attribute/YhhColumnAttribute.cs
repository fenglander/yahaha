

namespace Yahaha.Core;
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class YhhColumn : Attribute
{
    // <summary>
    // 帮助信息
    // </summary>
    public string Help { set; get; }
    // <summary>
    // 默认显示
    // </summary>
    public bool Display { set; get; }
    /// <summary>
    /// 字段的名称
    /// </summary>
    public string ColumnName { set; get; }
    /// <summary>
    /// ORM不处理该列
    /// </summary>
    public bool IsIgnore { set; get; }
    /// <summary>
    /// 注释
    /// </summary>
    public string ColumnDescription { set; get; }
    /// <summary>
    /// 长度
    /// </summary>
    public int Length { set; get; }
    /// <summary>
    /// 小数点长度
    /// </summary>
    public int DecimalDigits { set; get; }
    /// <summary>
    /// 是否可以为null默为false
    /// </summary>
    public bool NotNull { set; get; }
    /// <summary>
    /// ORM查询时json格式化
    /// </summary>
    public bool IsJson { set; get; }
    /// <summary>
    /// 默认值
    /// </summary>
    public string DefaultValue { set; get; }
    /// <summary>
    /// 关联类型
    /// </summary>
    public RelationalType RelationalType { get; set; }
    /// <summary>
    /// 关联对象
    /// </summary>
    public string Related { get; set; }
    /// <summary>
    /// null 删除关联记录后，当前值会设为null。
    /// restrict 删除关联记录时，如果与本记录存在关联，则不允许删除。
    /// cascade 删除关联记录后，当前记录也会被删除。
    /// </summary>
    public OnDelete OnDelete { get; set; }
    /// <summary>
    /// 强制必填
    /// </summary>
    public bool ForceRequired { get; set; } = false;
}

public enum RelationalType
{
    ManyToOne = 1,
    OneToMany,
    ManyToMany,
    Relate,
}

public enum OnDelete
{
    Null = 1,
    Cascade,
    Restrict,
}
