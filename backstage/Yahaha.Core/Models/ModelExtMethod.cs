using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.Models;
public static class ModelExtMethod
{
    public static Type GetclassType(this SysModel Model)
    {
        Assembly assembly = Assembly.Load(Model.ModuleName);
        Type classType = assembly.GetType(Model.FullName);
        return classType;
    }


    public static List<Dictionary<string, object>> ToDictionaryList(this object obj)
    {
        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

        if (obj is IEnumerable enumerable && !(obj is string))
        {
            // 如果是集合类型，递归处理每个元素
            foreach (var item in enumerable)
            {
                result.Add(ObjectToDictionary(item));
            }
        }
        else
        {
            // 如果是单个对象，直接处理
            result.Add(ObjectToDictionary(obj));
        }

        return result;
    }

    public static Dictionary<string, object> ObjectToDictionary(object obj)
    {
        // 获取对象的类型
        Type type = obj.GetType();

        // 获取对象的所有属性
        PropertyInfo[] properties = type.GetProperties();

        // 创建 Dictionary 存储属性和值
        Dictionary<string, object> result = new Dictionary<string, object>();

        // 遍历属性，将属性名和属性值添加到 Dictionary
        foreach (PropertyInfo property in properties)
        {
            result[property.Name] = property.GetValue(obj, null);
        }

        return result;
    }
}
