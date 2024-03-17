using Elasticsearch.Net;

namespace Yahaha.Core.Models.Entity;

[SystemTable]
[YhhTable("系统模型", "Name")]
public class SysModel : EntityBase
{
    [YhhColumn(ColumnDescription = "名称", Display = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 模型
    /// </summary>
    [YhhColumn(ColumnDescription = "模型", Help = "模型名称")]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "类名")]
    public string? FullName { get; set; }

    [YhhColumn(ColumnDescription = "程序集名")]
    public string? ModuleName { get; set; }

    [YhhColumn(ColumnDescription = "信息")]
    public string? Info { get; set; }

    [YhhColumn(ColumnDescription = "排序")]
    public string? DefaultSort { get; set; }

    [YhhColumn(ColumnDescription = "虚拟")]
    public bool IsVirtual { get; set; }

    [YhhColumn(ColumnDescription = "字段", RelationalType = RelationalType.OneToMany, Related = "SysModel")]
    public List<SysField> Fields { get; set; }
}