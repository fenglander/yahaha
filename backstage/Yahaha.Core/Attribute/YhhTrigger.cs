using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core;
[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class YhhTrigger : Attribute
{
    /// <summary>
    /// 字段名
    /// 支持ManyToOne，如"CreateUser.SysOrg.Name",最大支持三级
    /// 不建议加入长耗时的触发逻辑，影响体验。
    /// 前端表单组件失焦时触发
    /// </summary>
    public string FieldName { get; set; }

    public YhhTrigger(string name)
    {
        this.FieldName = name;
    }

}
