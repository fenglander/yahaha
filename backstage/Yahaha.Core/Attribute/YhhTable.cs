


namespace Yahaha.Core;
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class YhhTable : Attribute
{
    public string Description { get; set; }
    /// <summary>
    /// 排序
    /// </summary>
    public string DefaultOrder { set;get; }
    public YhhTable(string description, string? defaultOrder = "Id")
    {
        this.Description = description;
        this.DefaultOrder = defaultOrder;
    }
}
