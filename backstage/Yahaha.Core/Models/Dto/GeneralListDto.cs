using System.Dynamic;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.Service;

/// <summary>
/// 模型列表传入参数
/// </summary>
public class GeneralListDto: BasePageInput
{
    /// <summary>
    /// 模型名称
    /// </summary>
    public string model { get; set; }
        
    public List<fieldFilter>? filters { get; set;}
        
}


public class GeneralListRes : SqlSugarPagedList<ExpandoObject>
{
    /// <summary>
    /// 字段信息
    /// </summary>
    public List<SysFields> fields { get; set; }
    /// <summary>
    /// 查询方案
    /// </summary>
    public List<UserFilterScheme> userFilterSchemes { get; set; }
}

public class fieldFilter
{
    public long? id { get; set; }
    public string? name { get; set; }
    public string? description { get; set; }
    public string? tType { get; set; }
    public List<Filter>? filters { get; set; }
}

public class Filter
{
    public ConditionalType conditionalType { get; set; }
    public string? value { get; set; }
}