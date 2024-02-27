using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.VisualDev.Dto;
public class GetUserListDesignSchemeDto
{
    /// <summary>
    /// 模型id
    /// </summary>
    public long sysModel { get; set; }
    /// <summary>
    /// 列表设计id
    /// </summary>
    public long? listDesign { get; set; }
    /// <summary>
    /// 用户id
    /// </summary>
    public long user { get; set; }
    /// <summary>
    /// id
    /// </summary>
    public long? id { get; set; }
}