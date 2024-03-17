using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Yahaha.Core;
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class JsonNamingPolicyAttribute : Attribute
{
    public JsonNamingPolicyAttribute(JsonNamingPolicy? namingPolicy)
    {
        NamingPolicy = namingPolicy;
    }

    public JsonNamingPolicy? NamingPolicy { get; }
}