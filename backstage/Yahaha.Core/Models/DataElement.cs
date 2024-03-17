using AngleSharp.Text;
using Newtonsoft.Json.Linq;
using RazorEngine.Compilation.ImpromptuInterface.Dynamic;
using System.Dynamic;
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

    public string GetSearchColumns(string Model)
    {
        List<string> relationalTypes = Enum.GetValues(typeof(RelationalType))
                                           .Cast<RelationalType>()
                                           .Where(u => u.ToString() != "ManyToOne")
                                           .Select(enumValue => enumValue.ToString())
                                           .ToList();

        var Fields = GetSysFields(Model).Where(u => !relationalTypes.Contains(u.tType) && !u.Relate);
        List<string> FieldNames = Fields
            .Select(it => string.Format("\"{0}\" as \"{1}\" ", it.Name.ToLower(), it.Name)).ToList();
        return string.Join(", ", FieldNames);
    } 

    public ISugarQueryable<dynamic> Search(string ModelTableName, string? ShortName = "")
    {
        var columns = GetSearchColumns(ModelTableName);
        var Model = GetSysModels(null, ModelTableName).FirstOrDefault();

        var query = _db.Queryable<dynamic>().AS(ModelTableName, ShortName).Select(columns);
        if (Model != null && !string.IsNullOrEmpty(Model.DefaultSort))
        {
            query = query.OrderBy(Model.DefaultSort);
        }
        return query;
    }

    public ISugarQueryable<T> Search<T>(string? ShortName = "") where T : new()
    {
        string typeName = typeof(T).Name;
        var Model = GetSysModels(null, typeName).FirstOrDefault();
        var Columns = GetSearchColumns(typeName);
        var query = _db.Queryable<T>().AS(typeName, ShortName).Select(Columns);
        if(Model != null && !string.IsNullOrEmpty(Model.DefaultSort))
        {
            query = query.OrderBy(Model.DefaultSort);
        }
        return query;
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

    public Dictionary<string, object> Update(string Model, Dictionary<string, object> UpdateObj)
    {
        var dt = MatchingValue(Model, UpdateObj, 2);
        _db.Updateable(dt).AS(Model).WhereColumns("Id").ExecuteCommand();

        return dt;
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
    /// <summary>
    /// 匹配数据生成SQL参数值
    /// </summary>
    /// <param name="Model">模型名</param>
    /// <param name="Obj">数据</param>
    /// <param name="OpType">操作类型1：新增，2：修改</param>
    /// <returns></returns>
    public Dictionary<string, object> MatchingValue(string Model, Dictionary<string, object> Obj, int OpType)
    {
        var Fields = GetSysFields(Model).FindAll(it => it.Name != "Id");
        var dt = new Dictionary<string, object>();
        Dictionary<string, object> Delted = new Dictionary<string, object>();
        bool IsUpdate = OpType == 2;
        if (!IsUpdate)
        {
            long id = SnowFlakeSingle.Instance.NextId();
            if (dt.ContainsKey("Id")) { dt["Id"] = id; } else { dt.Add("Id", id); }
        }
        else
        {
            dt["Id"] = Obj["Id"];
            // 查询旧数据
            var conModels = new List<IConditionalModel>
            {
                new ConditionalModel { FieldName = "Id", ConditionalType = ConditionalType.Equal, FieldValue =  Obj["Id"].ToString(), CSharpTypeName = "long" }
            };
            var raw = Search(Model).Where(conModels).ToList();
            DrillDownDataDto DrillDownParams = new DrillDownDataDto
            {
                model = Model,
                items = raw,
                maxLevel = 1,
            };
            Delted = ToDictionaryList(DrillDownData(DrillDownParams)).FirstOrDefault();
            if (Delted == null) { IsUpdate = false; }
        }
        for (int i = 0; i < Fields.Count; i++)
        {
            var Field = Fields[i];
            dynamic Value;
            dynamic DelValue;
            // 需要忽略
            if (Field.Relate == true) { continue; }
            if (IsUpdate && Field.IsOnlyIgnoreUpdate == true) { continue; }
            // 获取新值
            if (Obj.TryGetValue(Field.Name, out var val)) { Value = val; } else { Value = null; }
            // 校验必填
            //if (Field.ForceRequired && (Value == null || (long.TryParse(Value.ToString(), out long v) && v == 0) || string.IsNullOrEmpty(Value.ToString())))
            //{
            //    throw new Exception(Field.Description + '[' + Field.Name + ']' + "不能为空");
            //}
            // 获取原值
            if (IsUpdate && Delted.TryGetValue(Field.Name, out var delVal)) { DelValue = delVal; } else { DelValue = null; }
            // 对空值的处理
            var NumType = new string[] { "Int64", "Int32", "Double", "Select" };
            if (Field.tType == "Boolean" && (Value == null || string.IsNullOrEmpty(Value.ToString())))
            {
                Value = false;
            }
            else if (NumType.Contains(Field.tType) && (Value == null || string.IsNullOrEmpty(Value.ToString())))
            {
                Value = null;
            }
            else if (Value != null && string.IsNullOrEmpty(Value.ToString()))
            {
                Value = null;
            }
            // 匹配值
            if (IsUpdate || Obj.ContainsKey(Field.Name))
            {
                if (IsUpdate || (Value != null && !string.IsNullOrEmpty(Value.ToString())))
                {
                    if (Field.tType == "OneToMany")
                    {
                        if (Value != null && !string.IsNullOrEmpty(Value.ToString()))
                            continue;
                        List<Dictionary<string, object>> resultList = Value != null ? ToDictionaryList(Value) : new List<Dictionary<string, object>>();
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
                            List<long> ids = resultList.Where(it => it.ContainsKey("Id") && it["Id"] != null && long.TryParse(it["Id"].ToString(), out long id) && id != 0).Select(it => (long)it["Id"]).ToList();
                            List<Dictionary<string, object>> DelValueList = ((List<ExpandoObject>)DelValue)
                            .Select(e => (IDictionary<string, object>)e)
                            .Select(d => d.ToDictionary(k => k.Key, k => k.Value))
                            .ToList();
                            var DeleteRecs = DelValueList.Where(it => it.ContainsKey("Id") && !ids.Contains(Convert.ToInt64(it["Id"]))).ToList();
                            Delete(Field.RelModelName, (List<Dictionary<string, object>>)DeleteRecs);
                        }
                        BatchAddElseUpdate(Field.RelModelName, resultList);
                        continue;
                    }
                    else if (Field.tType == "ManyToOne")
                    {
                        if (Value is Dictionary<string, object> dictionaryValue)
                        {
                            // 访问 Id 属性
                            Value = (long)Value["Id"];
                            // 进行相应的处理
                        }
                        else if (Value is long)
                        {
                            Value = (long)Value;
                        }
                        else if (Value != null)
                        {
                            Value = (long)Value.Id;
                        }
                    }
                }
            }
            // 默认值
            if (IsUpdate && Field.Name == "UpdateUser" && App.User != null)
            {
                Value = long.Parse(App.User?.FindFirst(ClaimConst.UserId)?.Value);
            }
            else if (IsUpdate && Field.Name == "UpdateTime")
            {
                Value = DateTime.Now;
            }
            else if (Field.Name == "CreateTime")
            {
                Value = IsUpdate && Value == null ? DelValue : DateTime.Now;
            }
            else if (!IsUpdate && Field.Name == "CreateUser" && App.User != null)
            {
                Value = IsUpdate && Value == null ? DelValue : long.Parse(App.User?.FindFirst(ClaimConst.UserId)?.Value);
            }

            dt.Add(Field.Name, Value);
            continue;
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

    public Dictionary<string, object> MatchingValue(string Model, ExpandoObject Obj, int OpType)
    {
        var dictionary = new Dictionary<string, object>();

        foreach (var property in Obj)
        {
            dictionary[property.Key] = property.Value;
        }

        return MatchingValue(Model, dictionary, OpType);
    }

    public Dictionary<string, object> Create(string Model, Dictionary<string, object> CreateObj)
    {
        var dt = MatchingValue(Model, CreateObj, 1);
        _db.Insertable(dt).AS(Model).ExecuteCommand();
        return dt;
    }

    public long BatchCreate(string Model, object Obj)
    {
        var DictList = Obj.ToDictionaryList();
        int res = 0;
        for (var i = 0; i < DictList.Count(); i++)
        {
            Create(Model, DictList[i]);
            res++;
        }
        return res;
    }

    public int BatchAddElseUpdate(string Model, List<Dictionary<string, object>> Obj)
    {
        int res = 0;
        for (int i = 0; i < Obj.Count(); i++)
        {
            if (Obj[i].ContainsKey("Id") && Obj[i]["Id"] != null && long.TryParse(Obj[i]["Id"].ToString(), out long id) && id != 0)
            {
                var existingItem = _db.Queryable<dynamic>().AS(Model).Where("Id = @Id", new { Id = id }).ToList().FirstOrDefault();
                if (existingItem == null)
                {
                    Create(Model, Obj[i]);
                }
                else
                {
                    Update(Model, Obj[i]);
                }
            }
            else
            {
                Create(Model, Obj[i]);
            }
            res++;
        }
        return res;
    }

    public int AddElseUpdate(string Model, object Obj)
    {
        var DictList = Obj.ToDictionaryList();
        return BatchAddElseUpdate(Model, DictList);
    }


    public List<T> BatchAddElseUpdate<T>(List<T> Obj)
    {
        Type type = typeof(T);
        string typeName = typeof(T).Name;
        var res = new List<T>();
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        DataTable PostMatchDatas = new DataTable();
        foreach (var obj in Obj)
        {
            // 获取对象的所有属性
            PropertyInfo[] properties = type.GetProperties();

            // 创建 Dictionary 存储属性和值
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

            // 遍历属性，将属性名和属性值添加到 Dictionary
            foreach (PropertyInfo property in properties)
            {
                keyValuePairs[property.Name] = property.GetValue(obj);
            }
            bool IsUpdate = keyValuePairs.ContainsKey("Id") && keyValuePairs["Id"] != null && long.TryParse(keyValuePairs["Id"].ToString(), out long id) && id != 0;
            int OpType = IsUpdate ? 2 : 1;
            var PostMatchData = MatchingValue(typeName, keyValuePairs, OpType);

            keyValuePairs["Id"] = PostMatchData["Id"];
            string resultJson = JsonConvert.SerializeObject(keyValuePairs, settings);
            T resultObj = JsonConvert.DeserializeObject<T>(resultJson, settings);
            res.Add(resultObj);

            foreach(var key in PostMatchData.Keys)
            {
                if (!PostMatchDatas.Columns.Contains(key))
                {
                    PostMatchDatas.Columns.Add(key, typeof(object));
                }
            }

            DataRow row = PostMatchDatas.NewRow();
            foreach (var pair in PostMatchData)
            {
                row[pair.Key] = pair.Value;
            }

            PostMatchDatas.Rows.Add(row);
        }
        List<string> LowerStrKeys = PostMatchDatas.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower()).ToList();

        string TempTableName = "temp_" + typeName + SnowFlakeSingle.Instance.NextId().ToString();
        string keysAsString = string.Join(", ", LowerStrKeys.Select(key => $"\"{key}\""));
        string ColumnsAssignment = string.Join(", ", LowerStrKeys.Select(key => $"\"{key}\" = EXCLUDED.\"{key}\""));
        string CreateTempTableSql = $"CREATE TEMP TABLE {TempTableName} AS SELECT * FROM {typeName} WITH NO DATA;";
        string UpdateSql = $@"
        INSERT INTO {typeName} ({keysAsString})
        SELECT {keysAsString}
        FROM {TempTableName}
        ON CONFLICT (id) 
        DO UPDATE SET {ColumnsAssignment}
        WHERE {typeName}.id = EXCLUDED.id;";
        using (_db.Ado.OpenAlways())
        {
            _db.Ado.ExecuteCommand(CreateTempTableSql);
            _db.Fastest<System.Data.DataTable>().AS(TempTableName).BulkCopy(PostMatchDatas);
            _db.Ado.ExecuteCommand(UpdateSql);
        }

        return res;
    }

    public Dictionary<string, object> AddElseUpdate<T>(T Obj)
    {
        string typeName = typeof(T).Name;
        Dictionary<string, object> keyValuePairs = ObjectToDictionary(Obj);
        if (keyValuePairs.ContainsKey("Id") && keyValuePairs["Id"] != null && (long)keyValuePairs["Id"] != 0)
        {
            return Update(typeName, keyValuePairs);
        }
        else
        {
            return Create(typeName, keyValuePairs);
        }
    }

    public Dictionary<string, object> ObjectToDictionary(object obj, bool DeepTrans = true)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        if (obj != null)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj);

                if (value != null && !IsPrimitiveType(property.PropertyType))
                {
                    // 递归处理非基元数据类型，但如果是List类型，递归转换为List<Dictionary<string, object>>
                    if (value is IEnumerable && !(value is string) && DeepTrans)
                    {
                        value = value.ToDictionaryList();
                    }
                    else
                    {
                        value = ObjectToDictionary(value, DeepTrans);
                    }
                }

                dict.Add(property.Name, value);
            }
        }

        return dict;
    }


    private static bool IsPrimitiveType(Type type)
    {
        return type.IsPrimitive || type.IsValueType || type == typeof(string);
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
            var cacheKey = $"DrillDownData,Model:{Params.model},RelField:{Params.relField},Id:{Params.id},ConnectionId:{Params.ConnectionId}";
            if (!_cache.TryGetValue(cacheKey, out TempValueList))
            {
                // 如果缓存中没有值，则执行数据库查询
                TempValueList = Search(Params.model).Where(string.Format("\"{0}\" = {1}", Params.relField.ToLower(), Params.id)).ToList();
                // 将查询结果添加到缓存中
                _cache.Set(cacheKey, TempValueList, 100);
            }
        }

        foreach (var val in TempValueList)
        {
            var valDict = val as IDictionary<string, object>;
            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var Field in FieldList)
            {
                if (Field.Relate)
                {
                    expando[Field.Name] = GetRelateValue(expando, Field);
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
                                model = Field.RelModel.Name,
                                id = (long)valDict["Id"],
                                relField = Field.Related,
                                curLevel = nextLevel,
                                maxLevel = Params.maxLevel,
                                ConnectionId = Params.ConnectionId,
                            };
                            expando[Field.Name] = DrillDownData(nextParams);
                        }
                        else if (Field.tType == "ManyToOne")
                        {
                            if (valDict.TryGetValue(Field.Name, out object? value) && value != null)
                            {
                                DrillDownDataDto nextParams = new DrillDownDataDto
                                {
                                    model = Field.RelModel.Name,
                                    id = (long)value,
                                    relField = "Id",
                                    curLevel = nextLevel,
                                    maxLevel = Params.maxLevel,
                                    ConnectionId = Params.ConnectionId,
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

    public object GetRelateValue(IDictionary<string, object> obj, SysField property)
    {
        string propertyPath = property.Related;
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

    public List<SysModel> GetSysModels(long? ModelId = null, string? ModelName = null)
    {
        List<SysModel> res = new List<SysModel>();
        // 缓存
        var cacheKey = $"GetSysModels";
        ConcurrentDictionary<Type, LambdaExpression> GetRecCache;
        if (!_cache.TryGetValue(cacheKey, out GetRecCache))
        {
            GetRecCache = new ConcurrentDictionary<Type, LambdaExpression>();

            res = _db.Queryable<SysModel>().ToList();
            var lambdaExpression = Expression.Lambda<Func<List<SysModel>>>(Expression.Constant(res));
            GetRecCache.AddOrUpdate(typeof(List<SysModel>), lambdaExpression, (key, existing) => lambdaExpression);
            _cache.Set(cacheKey, GetRecCache, 10000);
        }
        else
        {
            if (GetRecCache.TryGetValue(typeof(List<SysModel>), out var cachedExpression))
            {
                var func = (Func<List<SysModel>>)cachedExpression.Compile();
                res = func();
            }
        }
        if (ModelId != null)
        {
            res = res.FindAll(it => it.Id == ModelId);
        }
        if (ModelName != null)
        {
            res = res.FindAll(it => it.Name == ModelName);
        }
        if (!res.Any())
        {
            throw new Exception("未能匹配到模型，请联系管理员");
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
            var Modelid = GetSysModels().FirstOrDefault(u => u.Name == Model)?.Id;
            if (Modelid == null) { throw new Exception("无效Model名"); }
            res = res.FindAll(it => it.ModelId == Modelid);
        }
        return res;
    }

    public List<SysField> GetSysModelLabelInfos(string name)
    {
        string[] defaultLabels = { "Id", "Code", "Name" };
        var res = GetSysFields(name);

        // 使用 LINQ 查询获取 Display 字段为 true 的字段名
        List<SysField> DisplayFields = res.Where(it => it.Display == true).ToList();

        List<SysField> DefaultFields = res.Where(it => defaultLabels.Contains(it.Name)).ToList();
        // 如果没有符合条件的字段，使用默认字段
        if (DisplayFields.Count == 0)
        {
            DisplayFields.AddRange(DefaultFields);
        }

        return DisplayFields;
    }

    private static List<dynamic> ToDynamicList(List<Dictionary<string, object>> dictionaryList)
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

    private static List<T> ConvertToList<T>(List<ExpandoObject> expandoList) where T : new()
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

    private static T ConvertToClass<T>(ExpandoObject expandoObject) where T : new()
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

    private static object ConvertValue(object value, Type targetType)
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

    private static List<Dictionary<string, object>> ToDictionaryList(List<ExpandoObject> expandoList, string[]? Columns = null)
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

    private static List<Dictionary<string, object>> ToDictionaryList(List<dynamic> dynamicList, string[]? Columns = null)
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

    private static List<Dictionary<string, object>> ToDictionaryList(JArray jsonArray)
    {
        List<Dictionary<string, object>> resultList = jsonArray
            .Where(item => item != null)
            .Select(item => item.ToObject<Dictionary<string, object>>())
            .ToList();

        return resultList;
    }

}