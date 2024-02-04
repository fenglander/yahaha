namespace Yahaha.Core.Models.Entity;

[YhhTable("模型函数")]
public class SysAction : EntityBase
{
    [YhhColumn(ColumnDescription = "方法类名")]
    public string ActionClassName { get; set; }

    [YhhColumn(ColumnDescription = "方法名")]
    public string ActionName { get; set; }

    [YhhColumn(ColumnDescription = "程序集名")]
    public string ActionModuleName { get; set; }

    [YhhColumn(ColumnDescription = "绑定模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel BindingModel { get; set; }

    [YhhColumn(ColumnDescription = "是否函数")]
    public bool? Function { get; set; }

    [YhhColumn(ColumnDescription = "是否触发")]
    public bool? Trigger { get; set; }

    [YhhColumn(ColumnDescription = "关系模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel? TriggerModel { get; set; }

    [YhhColumn(ColumnDescription = "关系字段", RelationalType = RelationalType.ManyToOne)]
    public SysField? TriggerField { get; set; }

    [YhhColumn(ColumnDescription = "关系描述")]
    public string? FieldName { get; set; }

    [YhhColumn(ColumnDescription = "名称")]
    public string Name { get; set; }
}