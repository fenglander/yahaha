namespace Yahaha.Core.Models.Entity;

[YhhTable("模型基础权限")]
public class SysAccess : EntityBase
{
    [YhhColumn(ColumnDescription = "名称")]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel SysMoel { get; set; }

    [YhhColumn(ColumnDescription = "角色", RelationalType = RelationalType.ManyToOne)]
    public SysRole SysRole { get; set; }

    [YhhColumn(ColumnDescription = "读取")]
    public bool Read { get; set; }

    [YhhColumn(ColumnDescription = "修改")]
    public bool Update { get; set; }

    [YhhColumn(ColumnDescription = "新增")]
    public bool Create { get; set; }

    [YhhColumn(ColumnDescription = "删除")]
    public bool Delete { get; set; }
}