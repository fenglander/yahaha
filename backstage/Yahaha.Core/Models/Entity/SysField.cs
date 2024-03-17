namespace Yahaha.Core.Models.Entity;

[YhhTable("系统字段", "Name desc, Description")]
[SystemTable]
public class SysField : EntityBase
{
    [YhhColumn(ColumnDescription = "名称")]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "描述")]
    public string? Description { get; set; }

    [SugarColumn(ColumnDescription = "帮助")]
    public string? Help { get; set; }

    [SugarColumn(ColumnDescription = "模型ID")]
    public long? ModelId { get; set; }

    /// <summary>
    /// 模型
    /// </summary>
    [YhhColumn(ColumnDescription = "模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel SysModel { get; set; }

    /// <summary>
    /// 模型名称
    /// </summary>
    [YhhColumn(ColumnDescription = "模型名称", RelationalType = RelationalType.Relate, Related = "SysModel.Name")]
    public string? SysModelName { get; set; }

    /// <summary>
    /// 长度
    /// </summary>
    [YhhColumn(ColumnDescription = "长度")]
    public int? Length { set; get; }

    /// <summary>
    /// 小数点长度
    /// </summary>
    [YhhColumn(ColumnDescription = "小数精度")]
    public int? DecimalDigits { set; get; }

    /// <summary>
    /// 是否可以为null默为false
    /// </summary>
    [YhhColumn(ColumnDescription = "非空")]
    public bool NotNull { set; get; }

    [YhhColumn(ColumnDescription = "关系模型", RelationalType = RelationalType.ManyToOne)]
    public SysModel? RelModel { get; set; }

    /// <summary>
    /// 字段列表
    /// </summary>
    [YhhColumn(ColumnDescription = "子表字段", RelationalType = RelationalType.Relate, Related = "RelModel.Fields")]
    public List<SysField>? SubFields { get; set; }

    [YhhColumn(ColumnDescription = "类型")]
    public string? tType { get; set; }

    [SugarColumn(ColumnDescription = "枚举值", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? EnumValue { get; set; }

    [SugarColumn(ColumnDescription = "关系模型名称")]
    public string? RelModelName { get; set; }
    
    [SugarColumn(ColumnDescription = "关联字段")]
    public bool Relate { get; set; } = false;

    [SugarColumn(ColumnDescription = "关联对象")]
    public string? Related { get; set; }

    [SugarColumn(ColumnDescription = "过滤条件")]
    public string? WhereSql { get; set; }

    [SugarColumn(ColumnDescription = "默认显示")]
    public bool? Display { get; set; }

    [SugarColumn(ColumnDescription = "其他属性")]
    public string? ExtendedAttribute { get; set; }

    [YhhColumn(ColumnDescription = "Update操作不处理该列")]
    public bool? IsOnlyIgnoreUpdate { get; set; }

    /// <summary>
    /// @null 删除关联记录后，当前值会设为null。
    /// @restrict 删除关联记录时，如果与本记录存在关联，则不允许删除。
    /// @cascade 删除关联记录后，当前记录也会被删除。
    /// </summary>
    [YhhColumn(ColumnDescription = "删除关联操作")]
    public OnDelete? OnDelete { get; set; }

}