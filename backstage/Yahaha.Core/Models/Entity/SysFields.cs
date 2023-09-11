using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Models.Entity;

[SugarTable(null, "字段表")]
[SystemTable]
public class SysFields:EntityBase
{

    [SugarColumn(ColumnDescription = "名称")]
    public string Name { get; set; }

    [SugarColumn(ColumnDescription = "描述")]
    public string? Description { get; set; }

    [SugarColumn(ColumnDescription = "帮助")]
    public string? Help { get; set; }

    [SugarColumn(ColumnDescription = "模型ID")]
    public long? ModelId { get; set; }
    /// <summary>
    /// 模型
    /// </summary>
    [SugarColumn(ColumnDescription = "模型")]
    [Navigate(NavigateType.OneToOne, nameof(ModelId))]//一对一 
    public SysModels SysModels { get; set; } //不能赋值只能是null

    [SugarColumn(ColumnDescription = "数据类型")]
    public string? tType { get; set; }

    [SugarColumn(ColumnDescription = "关系类型")]
    public string? NavigatType { get; set; }

    [SugarColumn(ColumnDescription = "关系字段")]
    public string? RelFieldName { get; set; }

    [SugarColumn(ColumnDescription = "关系字段2")]
    public string? RelFieldName2 { get; set; }

    [SugarColumn(ColumnDescription = "映射类型")]
    public string? MappingType { get; set; }

    [SugarColumn(ColumnDescription = "映射对象ID1")]
    public string? MappingAId { get; set; }

    [SugarColumn(ColumnDescription = "映射对象ID2")]
    public string? MappingBId { get; set; }

    [SugarColumn(ColumnDescription = "过滤条件")]
    public string? WhereSql { get; set; }
}
