using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Models.Dto;

/// <summary>
/// 删除接口传参
/// </summary>
public class GeneralDeleteDto
{
    /// <summary>
    /// 模型id
    /// </summary>
    public long model { get; set; }

    public List<long> ids { get; set; }

}
