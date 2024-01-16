using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Models.Dto;

/// <summary>
/// 模型表单查询传入参数
/// </summary>
public class GeneralFormDto
{
    /// <summary>
    /// 模型id
    /// </summary>
    public long model { get; set; }

    public long id { get; set; }

}