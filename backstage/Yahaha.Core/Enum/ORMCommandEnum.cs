using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core;
public enum ORMCommandEnum
{
    /// <summary>
    /// 覆盖记录，不支持与其他指令同时使用，只要任意记录存在该值即按Set处理。
    /// 先执行Clear，再把当前记录进行Create。
    /// </summary>
    Set = 0,
    /// <summary>
    /// 新增记录
    /// </summary>
    Create = 1, 
    /// <summary>
    /// 修改记录
    /// </summary>
    Update = 2, 
    /// <summary>
    /// 删除记录
    /// </summary>
    Delete = 3,
    /// <summary>
    /// 清空记录
    /// <para>删除已关联的所有记录</para>
    /// </summary>
    Clear = 4,
}
