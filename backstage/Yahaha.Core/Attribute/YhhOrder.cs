namespace Yahaha.Core;

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class YhhOrder : Attribute
{
    /// <summary>
    /// [排序]
    /// </summary>
    public int Order { set; get; }

    public YhhOrder(int order)
    {
        this.Order = order;
    }
}