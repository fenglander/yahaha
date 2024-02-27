using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.Models.Dto;
public class GeneralListRes : SqlSugarPagedList<ExpandoObject>
{
    /// <summary>
    /// 字段信息
    /// </summary>
    public List<SysField> fields { get; set; }
}
