namespace Yahaha.WMS.Action;
[YhhAction]
public class ActionInventory
{
    public List<Inventory>? Rec;

    /// <summary>
    /// 审核
    /// </summary>
    [YhhFunction("审核1")]
    public int AuditRes()
    {
        int i = 0;
        if (this.Rec == null) { return i; }
        foreach (var item in this.Rec)
        {
            item.Code = "xxxx";
            i++;
        }
        return i;
    }


    [YhhTrigger("Code")]
    public void Trigger_SetName()
    {
        if (this.Rec == null) { return; }
        foreach (var item in Rec)
        {
            item.Name = item.Code;
        }
    }


    public void SetCode()
    {
        if (this.Rec == null) { return; }
        foreach (var item in Rec)
        {
            item.Code = DateTime.Now.ToString();
        }
    }
}
