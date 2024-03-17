using System.Dynamic;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.Models.Dto;

public class GeneralListRes : SqlSugarPagedList<ExpandoObject>
{
    /// <summary>
    /// 字段信息
    /// </summary>
    public List<SysField> fields { get; set; }
}