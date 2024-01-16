using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Models.Dto;
public class GeneralExecFuncDto
{
    public string moduleName { get; set; }
    public string className { get; set; }
    public string methodName { get; set; }
    public object data { get; set; }
    public dynamic model { get; set; }
}
