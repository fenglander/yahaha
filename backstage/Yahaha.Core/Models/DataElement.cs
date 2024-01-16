using AngleSharp.Text;
using Elasticsearch.Net;
using Nest;
using NetTaste;
using NewLife.Reflection;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Service.Role.Dto;

namespace Yahaha.Core.Models;
public class DataElement
{
    private readonly ISqlSugarClient _db;
    private static readonly ICache _cache = Cache.Default;
    public DataElement(ISqlSugarClient db)
    {
        _db = db;
    }

    public ISugarQueryable<dynamic> Search(string Model)
    {
        List<string> relationalTypes = Enum.GetValues(typeof(RelationalType))
                                           .Cast<RelationalType>()
                                           .Where(u => u.ToString() != "ManyToOne")
                                           .Select(enumValue => enumValue.ToString())
                                           .ToList();

        var Fields = GetSysFields(Model).Where(u => !relationalTypes.Contains(u.tType) && !u.Relate);
        List<string> FieldNames = Fields
            .Select(it => string.Format("\"{0}\" as \"{1}\" ", it.Name.ToLower(), it.Name)).ToList();
        string columns = string.Join(", ", FieldNames);
        string sql = string.Format("select {0} from {1} ", columns, Model);

        //_db.Queryable<dynamic>().AS(Model);

        var query = _db.SqlQueryable<dynamic>(sql).OrderBy("\"Id\"");
        return query;
    }

    public ISugarQueryable<dynamic> Search<T>() where T : new()
    {
        string typeName = typeof(T).Name;
        return Search(typeName);
    }

    public int Delete<T>(List<T> DeteleObjs)
    {
        string typeName = typeof(T).Name;

        var Objs = ToDictionaryList(DeteleObjs);
        return Delete(typeName, ToDynamicList(Objs));
    }

    public int Delete(string Model, List<Dictionary<string, object>> DeleteObjs)
    {
        return Delete(Model, ToDynamicList(DeleteObjs));
    }

    public int Delete(string Model, List<dynamic> DeleteObjs)
    {
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = Model,
            items = DeleteObjs,
            maxLevel = 1,
        };
        var objs = DrillDownData(DrillDownParams);

        objs.ForEach(obj => DrillDownDel(Model, obj));
        List<Dictionary<string, object>> updateObjList = ToDictionaryList(objs, new string[] { "Id" });
        return _db.Deleteable<object>().AS(Model).WhereColumns(updateObjList).ExecuteCommand();
    }

    public long Update(string Model, Dictionary<string, object> UpdateObj)
    {
        var dt = MatchingValue(Model, UpdateObj);
        _db.Updateable(dt).AS(Model).WhereColumns("Id").ExecuteCommand();

        return (long)dt["Id"];
    }

    public void DrillDownDel(string Model, Dictionary<string, object> Obj)
    {
        var Fields = GetSysFields(Model).FindAll(it => it.Name != "Id");
        for (int i = 0; i < Fields.Count; i++)
        {
            var Field = Fields[i];
            if (Field.tType == "OneToMany" && Obj.ContainsKey(Field.Name) && !Field.Relate)
            {
                dynamic value = Obj[Field.Name];
                List<Dictionary<string, object>> resultList = ToDictionaryList(value);
                Delete(Field.RelModelName, resultList);
            }
        }
    }

    public Dictionary<string, object> MatchingValue(string Model, Dictionary<string, object> Obj)
    {
        var Fields = GetSysFields(Model).FindAll(it => it.Name != "Id");
        var dt = new Dictionary<string, object>();
        bool IsUpdate = Obj.ContainsKey("Id") && Obj["Id"] != null && long.TryParse(Obj["Id"].ToString(), out long id) && id != 0;
        if (!IsUpdate)
        {
            id = SnowFlakeSingle.Instance.NextId();
            if (dt.ContainsKey("Id")) { dt["Id"] = id; } else { dt.Add("Id", id); }
        }
        else { dt["Id"] = Obj["Id"]; }
        for (int i = 0; i < Fields.Count; i++)
        {
            var Field = Fields[i];
            // 匹配值
            if (Obj.ContainsKey(Field.Name))
            {
                dynamic value = Obj[Field.Name];
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    if (Field.Relate == true)
                    {
                        continue;
                    }
                    else if (Field.tType == "OneToMany")
                    {
                        List<Dictionary<string, object>> resultList = ToDictionaryList(value);
                        for (int it = 0; it < resultList.Count; it++)
                        {   // 绑定主表
                            if (Field.tType == "Int64")
                            {
                                resultList[it][Field.Related] = (long)dt["Id"];
                            }
                            else
                            {
                                resultList[it][Field.Related] = dt;
                            }
                        }
                        //筛选出被删除的记录
                        if (IsUpdate)
                        {
                            string ids = string.Join(",", resultList.Where(it => it.ContainsKey("Id") && it["Id"] != null && long.TryParse(it["Id"].ToString(), out long id) && id != 0).Select(it => it["Id"].ToString()).ToList());
                            var conModels = new List<IConditionalModel>
                            {
                                new ConditionalModel { FieldName = "\"Id\"", ConditionalType = ConditionalType.NotIn, FieldValue = ids },
                                new ConditionalModel { FieldName = "\"" + Field.Related + "\"", ConditionalType = ConditionalType.Equal, FieldValue = dt["Id"].ToString(), CSharpTypeName = "long" }
                            };
                            var DeleteRecs = Search(Field.RelModelName).Where(conModels).ToList();
                            Delete(Field.RelModelName, DeleteRecs);
                        }
                        AddElseUpdate(Field.RelModelName, resultList);

                        continue;
                    }
                    else if (Field.tType == "ManyToOne")
                    {
                        if (value is Dictionary<string, object> dictionaryValue)
                        {
                            // 访问 Id 属性
                            value = (long)value["Id"];
                            // 进行相应的处理
                        }
                        else if (value is long)
                        {

                            value = (long)value;
                        }
                        else
                        {
                            value = (long)value.Id;
                        }

                    }
                    dt.Add(Field.Name, value);
                    continue;
                }
            }
            // 默认值
            if (Field.tType == "Boolean")
            {
                dt.Add(Field.Name, false);
            }
            if (IsUpdate && Field.Name == "UpdateUser" && App.User != null)
            {
                dt.Add(Field.Name, long.Parse(App.User?.FindFirst(ClaimConst.UserId)?.Value));
            }
            else if (IsUpdate && Field.Name == "UpdateTime")
            {
                dt.Add(Field.Name, DateTime.Now);
            }
            else if (!IsUpdate && Field.Name == "CreateTime")
            {
                dt.Add(Field.Name, DateTime.Now);
            }
            else if (!IsUpdate && Field.Name == "CreateUser" && App.User != null)
            {
                dt.Add(Field.Name, long.Parse(App.User?.FindFirst(ClaimConst.UserId)?.Value));
            }
        }
        return dt;
    }

    public void DrillDownDel(string Model, ExpandoObject Obj)
    {
        var dictionary = new Dictionary<string, object>();

        foreach (var property in Obj)
        {
            dictionary[property.Key] = property.Value;
        }

        DrillDownDel(Model, dictionary);
    }

    public Dictionary<string, object> MatchingValue(string Model, ExpandoObject Obj)
    {
        var dictionary = new Dictionary<string, object>();

        foreach (var property in Obj)
        {
            dictionary[property.Key] = property.Value;
        }

        return MatchingValue(Model, dictionary);
    }

    public long Create(string Model, Dictionary<string, object> CreateObj)
    {
        var dt = MatchingValue(Model, CreateObj);
        _db.Insertable(dt).AS(Model).ExecuteCommand();
        return (long)dt["Id"];
    }


    public int AddElseUpdate(string Model, List<Dictionary<string, object>> Obj)
    {
        int res = 0;
        for (int i = 0; i < Obj.Count(); i++)
        {
            if (Obj[i].ContainsKey("Id") && Obj[i]["Id"] != null && long.TryParse(Obj[i]["Id"].ToString(), out long id) && id != 0)
            {
                Update(Model, Obj[i]);
            }
            else
            {
                Create(Model, Obj[i]);
            }
            res++;
        }
        return 0;
    }


    public int AddElseUpdate<T>(List<T> Obj)
    {
        string typeName = typeof(T).Name;
        int res = 0;
        for (int i = 0; i < Obj.Count(); i++)
        {
            Dictionary<string, object> keyValuePairs = ObjectToDictionary(Obj[i]);
            if (keyValuePairs.ContainsKey("Id") && keyValuePairs["Id"] != null && (long)keyValuePairs["Id"] != 0)
            {
                Update(typeName, keyValuePairs);
            }
            else
            {
                Create(typeName, keyValuePairs);
            }
            res++;
        }
        return 0;
    }

    public long AddElseUpdate<T>(T Obj)
    {
        string typeName = typeof(T).Name;
        Dictionary<string, object> keyValuePairs = ObjectToDictionary(Obj);
        long res;
        if (keyValuePairs.ContainsKey("Id") && keyValuePairs["Id"] != null && (long)keyValuePairs["Id"] != 0)
        {
            res = Update(typeName, keyValuePairs);
        }
        else
        {
            res = Create(typeName, keyValuePairs);
        }

        return res;
    }


    public Dictionary<string, object> ObjectToDictionary(object obj)
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
            result[property.Name] = property.GetValue(obj);
        }

        return result;
    }

    public List<ExpandoObject> DrillDownData(DrillDownDataDto Params)
    {
        var expandoList = new List<ExpandoObject>();
        var FieldList = GetSysFields(Params.model)
                        .OrderBy(u => u.tType == "ManyToOne" ? 0 : 1)  // "ManyToOne" 排在最前
                        .ThenBy(u => u.Relate ? 1 : 0)        // "Relate" 排在最后
                        .ToList(); ;
        List<dynamic> TempValueList;
        if (Params.items != null)
        {
            TempValueList = Params.items;
        }
        else
        {
            var cacheKey = $"GetFieldList,Model:{Params.model},RelField:{Params.relField},Id:{Params.id}";
            if (!_cache.TryGetValue(cacheKey, out TempValueList))
            {
                // 如果缓存中没有值，则执行数据库查询
                TempValueList = Search(Params.model).Where(string.Format("{0} = {1}", "\"" + Params.relField + "\"", Params.id)).ToList();
                // 将查询结果添加到缓存中
                _cache.Set(cacheKey, TempValueList, 10);
            }
            // TempValueList = Search(Params.model).Where(string.Format("{0} = {1}", "\""+Params.relField+ "\"", Params.id)).ToList();
        }

        foreach (var val in TempValueList)
        {
            var valDict = val as IDictionary<string, object>;
            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var Field in FieldList)
            {
                if (Field.Relate)
                {
                    expando[Field.Name] = GetRelateValue(expando, Field.Related);
                }
                else
                {
                    if (Field.RelModel != null && Field.RelModelName != "")
                    {
                        if (Params.curLevel >= Params.maxLevel) { continue; }
                        // 使用curLevel + 1而不是curLevel++来控制深度
                        var nextLevel = Params.curLevel + 1;

                        if (Field.tType == "OneToMany")
                        {
                            DrillDownDataDto nextParams = new DrillDownDataDto
                            {
                                model = Field.RelModel.TableName,
                                id = (long)valDict["Id"],
                                relField = Field.Related,
                                curLevel = nextLevel,
                                maxLevel = Params.maxLevel,
                            };
                            expando[Field.Name] = DrillDownData(nextParams);
                        }
                        else if (Field.tType == "ManyToOne")
                        {
                            if (valDict.TryGetValue(Field.Name, out object? value) && value != null)
                            {
                                DrillDownDataDto nextParams = new DrillDownDataDto
                                {
                                    model = Field.RelModel.TableName,
                                    id = (long)value,
                                    relField = "Id",
                                    curLevel = nextLevel,
                                    maxLevel = Params.maxLevel,
                                };
                                var result = DrillDownData(nextParams)?.FirstOrDefault();
                                expando[Field.Name] = result;
                            }
                        }
                    }
                    else if (valDict.TryGetValue(Field.Name, out object? value))
                    {
                        expando[Field.Name] = value;
                    }
                }
            }
            expandoList.Add((ExpandoObject)expando);
        }

        return expandoList;
    }


    public List<T> DrillDownData<T>(DrillDownDataDto Params) where T : new()
    {
        List<ExpandoObject> row = DrillDownData(Params);
        List<T> DynamicRes = ConvertToList<T>(row);
        return DynamicRes;
    }


    public object GetRelateValue(IDictionary<string, object> obj, string propertyPath)
    {
        string[] pathSegments = propertyPath.Split('.');
        object currentObject = obj;

        foreach (var segment in pathSegments)
        {
            if (currentObject is ExpandoObject dictionary)
            {
                var valDict = dictionary as IDictionary<string, object>;
                // 如果当前对象是字典类型，则尝试获取属性值
                if (valDict.TryGetValue(segment, out var value))
                {
                    currentObject = value;
                }
                else
                {
                    // 属性不存在，返回默认值
                    return null;
                }
            }
            else
            {
                // 当前对象不是字典类型，无法继续深入
                return null;
            }
        }
        return currentObject;
    }


    public List<SysModels> GetSysModels(long? ModelId = null, string? ModelTableName = null)
    {
        List<SysModels> res = new List<SysModels>();
        // 缓存
        var cacheKey = $"GetSysModels";
        ConcurrentDictionary<Type, LambdaExpression> GetRecCache;
        if (!_cache.TryGetValue(cacheKey, out GetRecCache))
        {
            GetRecCache = new ConcurrentDictionary<Type, LambdaExpression>();

            res = _db.Queryable<SysModels>().ToList();
            var lambdaExpression = Expression.Lambda<Func<List<SysModels>>>(Expression.Constant(res));
            GetRecCache.AddOrUpdate(typeof(List<SysModels>), lambdaExpression, (key, existing) => lambdaExpression);
            _cache.Set(cacheKey, GetRecCache, 10000);
        }
        else
        {
            if (GetRecCache.TryGetValue(typeof(List<SysModels>), out var cachedExpression))
            {
                var func = (Func<List<SysModels>>)cachedExpression.Compile();
                res = func();
            }
        }
        if (ModelId != null)
        {
            res = res.FindAll(it => it.Id == ModelId);
        }
        if (ModelTableName != null)
        {
            res = res.FindAll(it => it.TableName == ModelTableName);
        }
        return res;
    }

    public void SetSysFieldsCache(List<SysField> list)
    {
        var cacheKey = $"GetSysFields";
        ConcurrentDictionary<Type, LambdaExpression> GetRecCache;
        GetRecCache = new ConcurrentDictionary<Type, LambdaExpression>();

        var lambdaExpression = Expression.Lambda<Func<List<SysField>>>(Expression.Constant(list));
        GetRecCache.AddOrUpdate(typeof(List<SysField>), lambdaExpression, (key, existing) => lambdaExpression);
        _cache.Set(cacheKey, GetRecCache, 0);
    }

    public List<SysField> GetSysFields(string? Model = null)
    {
        List<SysField> res = new List<SysField>();
        // 缓存
        var cacheKey = $"GetSysFields";
        ConcurrentDictionary<Type, LambdaExpression> GetRecCache;
        if (!_cache.TryGetValue(cacheKey, out GetRecCache))
        {
            // 避免循环调用，只能用原始方法处理这部分
            res = _db.Queryable<SysField>().ToList();
            foreach (var item in res)
            {
                var SysModel = GetSysModels(item.ModelId, null).FirstOrDefault();
                item.SysModel = SysModel;
                item.SysModelName = SysModel.Name;
                if (!string.IsNullOrEmpty(item.RelModelName) && string.IsNullOrEmpty(item.Related))
                {
                    var RelModel = GetSysModels(null, item.RelModelName).FirstOrDefault();
                    item.RelModel = RelModel;
                    item.SubFields = RelModel.Fields;
                }
            }
            //res = ConvertToList<SysFields>(DrillDownDataRes);
            SetSysFieldsCache(res);
        }
        else
        {
            if (GetRecCache.TryGetValue(typeof(List<SysField>), out var cachedExpression))
            {
                var func = (Func<List<SysField>>)cachedExpression.Compile();
                res = func();
            }
        }
        if (Model != null)
        {
            var Modelid = GetSysModels().FirstOrDefault(u => u.TableName == Model)?.Id;
            if (Modelid == null) { throw new Exception("无效Model名"); }
            res = res.FindAll(it => it.ModelId == Modelid);
        }
        return res;
    }

    static List<dynamic> ToDynamicList(List<Dictionary<string, object>> dictionaryList)
    {
        return dictionaryList.Select(dict =>
        {
            dynamic dynamicObj = new ExpandoObject();

            foreach (var kvp in dict)
            {
                ((IDictionary<string, object>)dynamicObj)[kvp.Key] = kvp.Value;
            }

            return dynamicObj;
        }).ToList();
    }

    static List<T> ConvertToList<T>(List<ExpandoObject> expandoList) where T : new()
    {
        List<T> resultList = new List<T>();

        foreach (var expando in expandoList)
        {
            T result = ConvertToClass<T>(expando);
            resultList.Add(result);
        }

        return resultList;
    }

    public static List<T> ConvertToList<T>(List<dynamic> dynamicList) where T : new()
    {
        List<T> resultList = new List<T>();

        foreach (var dynamicObject in dynamicList)
        {
            T result = ConvertToClass<T>(dynamicObject);
            resultList.Add(result);
        }

        return resultList;
    }

    public object ConvertToClass(Dictionary<string, object> expandoObject, Type targetType)
    {
        object itemObject = Activator.CreateInstance(targetType);

        // 将 ExpandoObject 转换为字典
        var dictionary = (IDictionary<string, object>)expandoObject;

        foreach (var entry in dictionary)
        {
            var property = targetType.GetProperty(entry.Key);
            if (property != null)
            {
                // 将字典中的值递归转换为属性的类型
                object value = ConvertValue(entry.Value, property.PropertyType);
                property.SetValue(itemObject, value);
            }
        }
        return itemObject;
    }

    static T ConvertToClass<T>(ExpandoObject expandoObject) where T : new()
    {
        T obj = new T();

        // 将 ExpandoObject 转换为字典
        var dictionary = (IDictionary<string, object>)expandoObject;

        foreach (var entry in dictionary)
        {
            var property = typeof(T).GetProperty(entry.Key);
            if (property != null)
            {
                // 将字典中的值递归转换为属性的类型
                object value = ConvertValue(entry.Value, property.PropertyType);
                property.SetValue(obj, value);
            }
        }
        return obj;
    }

    static object ConvertValue(object value, Type targetType)
    {
        if (value == null || targetType.IsAssignableFrom(value.GetType()))
        {
            return value;
        }

        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            // 处理 List<T> 类型
            var listItemType = targetType.GetGenericArguments()[0];

            // 创建一个 List<> 实例
            var listInstance = typeof(List<>).MakeGenericType(listItemType);
            var list = Activator.CreateInstance(listInstance);

            // 获取 Add 方法
            var addMethod = list.GetType().GetMethod("Add");

            // 递归处理 List 中的每个元素
            foreach (var item in (IEnumerable)value)
            {
                var listItem = ConvertValue(item, listItemType);
                addMethod.Invoke(list, new[] { listItem });
            }

            return list;
        }
        else
        {
            // 处理普通类型
            if (value != null && targetType != typeof(string) && targetType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            // 将 ExpandoObject 转换为 JSON 字符串
            string json = JsonConvert.SerializeObject(value);

            // 将 JSON 字符串转换为目标类型
            return JsonConvert.DeserializeObject(json, targetType);
        }
    }

    /// <summary>
    /// 转类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="Columns">保留字段</param>
    /// <returns></returns>
    public List<Dictionary<string, object>> ToDictionaryList<T>(List<T> list, string[]? Columns = null)
    {
        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (T obj in list)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>(properties.Length);

            foreach (PropertyInfo property in properties)
            {
                if (Columns == null)
                {
                    dictionary[property.Name] = property.GetValue(obj);
                }
                else
                {
                    if (Columns.Contains(property.Name)) { dictionary[property.Name] = property.GetValue(obj); }
                }
            }

            result.Add(dictionary);
        }

        return result;
    }


    static List<Dictionary<string, object>> ToDictionaryList(List<ExpandoObject> expandoList, string[]? Columns = null)
    {
        return expandoList.Select(expando =>
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var property in expando)
            {
                if (Columns == null || Columns.Contains(property.Key))
                {
                    dictionary[property.Key] = property.Value;
                }
            }

            return dictionary;
        }).ToList();
    }

    static List<Dictionary<string, object>> ToDictionaryList(List<dynamic> dynamicList, string[]? Columns = null)
    {
        List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();

        foreach (var dynamicObj in dynamicList)
        {
            var dictionaryObj = new Dictionary<string, object>();

            if (dynamicObj is IDictionary<string, object> dictionary)
            {
                // 使用字典接口获取动态对象的属性和值
                foreach (var kvp in dictionary)
                {
                    // 检查是否应处理指定字段
                    if (Columns == null || Columns.Contains(kvp.Key))
                    {
                        dictionaryObj[kvp.Key] = kvp.Value;
                    }
                }
            }
            else
            {
                // 如果不是字典，可以考虑其他方式获取属性和值，这里示例使用反射
                foreach (var property in dynamicObj.GetType().GetProperties())
                {
                    string propertyName = property.Name as string;
                    // 检查是否应处理指定字段
                    if (Columns == null || (propertyName != null && Columns.Contains(propertyName)))
                    {
                        dictionaryObj[property.Name] = property.GetValue(dynamicObj);
                    }
                }
            }

            dictionaryList.Add(dictionaryObj);
        }

        return dictionaryList;
    }


    static List<Dictionary<string, object>> ToDictionaryList(JArray jsonArray)
    {
        List<Dictionary<string, object>> resultList = jsonArray
            .Where(item => item != null)
            .Select(item => item.ToObject<Dictionary<string, object>>())
            .ToList();

        return resultList;
    }

}
