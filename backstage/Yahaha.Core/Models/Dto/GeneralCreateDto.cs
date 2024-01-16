using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Models.Dto;
public class GeneralCreateDto
{
    /// <summary>
    /// 模型id
    /// </summary>
    public long model { get; set; }

    public object data { get; set; }
}
