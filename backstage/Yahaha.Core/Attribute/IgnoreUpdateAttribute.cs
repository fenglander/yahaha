namespace Yahaha.Core;

/// <summary>
/// 忽略更新种子数据特性
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class IgnoreUpdateAttribute : Attribute
{
    /// <summary>
    /// 判断记录是否存在的字段
    /// </summary>
    public string[]? Keys { get; set; }
    /// <summary>
    /// 处理顺序
    /// </summary>
    public long Order { get; set; }
    public IgnoreUpdateAttribute(string key, long order = 0)
    {
        this.Keys = new string[] { key };
        this.Order = order;
    }

    public IgnoreUpdateAttribute(string[] keys, long order = 0)
    {
        this.Keys = keys;
        this.Order = order;
    }

    public IgnoreUpdateAttribute(long order)
    {
        this.Order = order;
    }

    public IgnoreUpdateAttribute()
    {
        this.Keys = null;
        this.Order = 0;
    }
}