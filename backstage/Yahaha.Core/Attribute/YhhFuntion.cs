using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core;
[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class YhhFunction : Attribute
{
    public string Name { get; set; }
    public FuntionType Type { get; set; }
    public YhhFunction(string name)
    {
        this.Name = name;
    }

}

public enum FuntionType
{
    Button = 1,
    trigger = 2,
}