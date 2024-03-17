namespace Yahaha.Core;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class YhhTable : Attribute
{
    public string Description { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public string? DefaultSort { set; get; }

    public YhhTable(string description, string? defaultOrder = null)
    {
        this.Description = description;
        this.DefaultSort = defaultOrder;
    }
}