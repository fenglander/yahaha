


namespace Yahaha.Core;
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class YhhTable : Attribute
{
    public string Description { get; set; }
    /// <summary>
    /// 帮助信息
    /// </summary>
    public string DefaultOrder { set;get; }
    public YhhTable(string description, string? defaultOrder = "Id")
    {
        this.Description = description;
        this.DefaultOrder = defaultOrder;
    }
}
