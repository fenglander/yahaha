﻿// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Minio.DataModel;
using NewLife.Reflection;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections.Generic;
using System.Dynamic;
using Yahaha.Core.Models;
using Yahaha.Core.Models.Dto;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Service.Role.Dto;

namespace Yahaha.Core.Service;

/// <summary>
/// 系统模型服务
/// </summary>
[ApiDescriptionSettings(Order = 990)]
public class ModelsService : IDynamicApiController, ITransient
{
    private readonly IdentityService _identitySvc;
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysModel> _sysModel;
    private readonly SqlSugarRepository<SysField> _sysField;
    private readonly ISqlSugarClient _db;
    private static readonly ICache _cache = Cache.Default;
    private readonly IHttpContextAccessor _context;
    private DataElement _de;

    public ModelsService(SqlSugarRepository<SysModel> sysModel,
        SqlSugarRepository<SysField> sysField,
        IdentityService identityService,
        UserManager userManager,
        IHttpContextAccessor context,
        ISqlSugarClient db)
    {
        _identitySvc = identityService;
        _userManager = userManager;
        _sysModel = sysModel;
        _sysField = sysField;
        _db = db;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _de = new DataElement(_db);
    }

    /// <summary>
    /// 获取模型动作信息
    /// </summary>
    /// <param name="modelid">模型ID</param>
    /// <returns></returns>
    public List<dynamic> getActionList(long? modelid = null)
    {
        var query = _de.Search(nameof(SysAction));
        if (modelid != null)
        {
            query = query.Where("BindingModel = @model", new { model = (long)modelid });
        }
        var Raw = query.ToList();
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = nameof(SysAction),
            items = Raw,
        };
        var Res = _de.DrillDownData(DrillDownParams);
        return Res.Select(expando => (dynamic)expando).ToList();
    }

    /// <summary>
    /// 获取模型列表
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [DisplayName("获取模型列表")]
    public List<dynamic> GetModelList([FromQuery] string? name)
    {
        var cacheKey = $"Frontend.Init.ModelList";
        List<ExpandoObject> ObjectRes = new List<ExpandoObject>();
        if (!_cache.TryGetValue(cacheKey, out ObjectRes))
        {
            var query = _de.Search("SysModel");

            var Row = query.OrderBy("\"Description\"").ToList();
            DrillDownDataDto DrillDownParams = new DrillDownDataDto
            {
                model = "SysModel",
                items = Row,
                maxLevel = 2,
            };
            ObjectRes = _de.DrillDownData(DrillDownParams);
            _cache.Set(cacheKey, ObjectRes, 0);
        }

        if (!string.IsNullOrEmpty(name))
        {
            ObjectRes = ObjectRes.Where(item => (string)item.GetType().GetProperty("Name").GetValue(item) == name).ToList();
        }
        return ObjectRes.Select(expando => (dynamic)expando).ToList(); ;
    }

    /// <summary>
    /// 获取字段列表
    /// </summary>
    /// <returns></returns>
    [DisplayName("前端初始化字段列表")]
    public List<dynamic> getFieldList()
    {
        List<ExpandoObject> ObjectRes = new List<ExpandoObject>();
        var cacheKey = $"Frontend.Init.FieldList";
        if (!_cache.TryGetValue(cacheKey, out ObjectRes))
        {
            var query = _de.Search("SysField");

            var Row = query.OrderBy("\"Description\"").ToList();
            DrillDownDataDto DrillDownParams = new DrillDownDataDto
            {
                model = "SysField",
                items = Row,
                maxLevel = 3,
            };
            ObjectRes = _de.DrillDownData(DrillDownParams);
            // 将查询结果添加到缓存中
            _cache.Set(cacheKey, ObjectRes, 0);
        }
        List<dynamic> Res = ObjectRes.Select(expando => (dynamic)expando).ToList();
        return Res;
    }

    /// <summary>
    /// 通用列表数据接口
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<GeneralListRes> GeneralListData(GeneralListDto input)
    {
        var Model = await _sysModel.GetByIdAsync(input.model);
        var Query = _de.Search(Model.Name, "t1");
        var Fields = _de.GetSysFields(Model.Name);
        if (input.filters != null)
        {
            var FieldFilters = input.filters;
            var ConModels = new List<IConditionalModel>();
            foreach (var Filter in FieldFilters)
            {
                if (Filter.filters == null) continue;
                var FieldName = Filter.name;
                var CSharpTypeName = Filter.tType == nameof(RelationalType.ManyToOne) ? "long" : Filter.tType;
                var ConditionalModelArray = new JArray();
                var FieldInfo = _de.GetSysFields().FirstOrDefault(x => x.Id == Filter.id);

                if (Filter.tType == nameof(RelationalType.OneToMany))
                {
                    var LabelInfo = _de.GetSysModelLabelInfos(Model.Name).LastOrDefault();
                    FieldName = LabelInfo.Name;
                    CSharpTypeName = LabelInfo.tType;
                }

                foreach (var item in Filter.filters)
                {
                    if (item == null) continue;
                    var SubConditionalModelObj = new JObject();

                    if (item.conditionalType == ConditionalType.IsNullOrEmpty || item.conditionalType == ConditionalType.IsNot)
                    {
                        string StrEsxists = item.conditionalType == ConditionalType.IsNot ? "NOT EXISTS" : "EXISTS";
                        Query = Query.Where(string.Format("{0} ( SELECT * FROM {1} WHERE ( {2} = t1.id ))", StrEsxists, FieldInfo.RelModel.Name, FieldInfo.Related.ToLower()));
                        continue;
                    }
                    SubConditionalModelObj["FieldName"] = FieldName;
                    SubConditionalModelObj["FieldValue"] = item.value;
                    SubConditionalModelObj["ConditionalType"] = (int)item.conditionalType;
                    SubConditionalModelObj["CSharpTypeName"] = CSharpTypeName;
                    int WhereType = 0;
                    ConditionalModelArray.Add(new JObject { { "Key", WhereType }, { "Value", SubConditionalModelObj } });
                }
                var finalObj = new JArray { new JObject(new JProperty("ConditionalList", ConditionalModelArray)) };
                string jsonWithArray = finalObj.ToString();
                var whereList = _db.Utilities.JsonToConditionalModels(jsonWithArray);

                // 转为EXISTS子查询
                if (Filter.tType == nameof(RelationalType.OneToMany))
                {
                    var SubQuery = _db.Queryable<dynamic>().AS(FieldInfo.RelModel.Name);
                    var SubSqlStr = SubQuery.Where(whereList).ToSqlString();
                    Query = Query.Where(string.Format("EXISTS ( {0} AND ( {1} = t1.id ))", SubSqlStr, FieldInfo.Related.ToLower()));
                }
                else
                {
                    Query = Query.Where(whereList);
                }

                var sql = Query.ToSqlString();
            }
        }

        var Raw = await Query.ToPagedListAsync(input.Page, input.PageSize);

        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = Model.Name,
            items = Raw.Items.ToList(),
            ConnectionId = App.HttpContext.Connection.Id,
            maxLevel = 1,
        };

        var expandoList = _de.DrillDownData(DrillDownParams);

        GeneralListRes res = new GeneralListRes()
        {
            Page = Raw.Page,
            PageSize = Raw.PageSize,
            Total = Raw.Total,
            TotalPages = Raw.TotalPages,
            HasPrevPage = Raw.HasPrevPage,
            HasNextPage = Raw.HasNextPage,
            Items = expandoList,
            fields = Fields,
        };
        return res;
    }

    /// <summary>
    /// 获取模型标题
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public dynamic GetModelTitle(GeneralCreateDto input)
    {
        var Model = _de.GetSysModels(input.model,null).FirstOrDefault();
        Assembly assembly = Assembly.Load(Model.ModuleName);
        Type classType = assembly.GetType(Model.FullName);
        string resultJson = JsonConvert.SerializeObject(input.data);
        var typedObject = JsonConvert.DeserializeObject(resultJson, classType);
        var resProperty = classType.GetProperty(nameof(EntityBase.ModelTitle));
        var Title = resProperty?.GetValue(typedObject);
        return Title;
    }

    /// <summary>
    /// 获取模型标题
    /// </summary>
    /// <param name="model">模型ID</param>
    /// <returns></returns>
    public dynamic GetModelEmptyData([FromQuery] long model)
    {
        var Model = _de.GetSysModels(model, null).FirstOrDefault();
        Type classType = Model.GetclassType();
        object instance = Activator.CreateInstance(classType);
        return _de.ObjectToDictionary(instance as Object);
    }

    /// <summary>
    /// 通用表单数据接口
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<dynamic> GeneralFormData(GeneralFormDto input)
    {
        var Model = await _sysModel.GetByIdAsync(input.model);
        if (Model == null || Model.Name == null)
        {
            throw new Exception("请输入正确表名");
        }
        var query = _de.Search(Model.Name);
        var conModels = new List<IConditionalModel>
        {
            new ConditionalModel { FieldName = "Id", ConditionalType = ConditionalType.Equal, FieldValue = input.id.ToString(), CSharpTypeName="long" }
        };
        query = query.Where(conModels);

        var Row = query.ToList();

        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = Model.Name,
            items = Row,
            maxLevel = 2,
            ConnectionId = App.HttpContext.Connection.Id,
        };
        var res = _de.DrillDownData(DrillDownParams).FirstOrDefault();
        Type classType = Model.GetclassType();

        string resultJson = JsonConvert.SerializeObject(res);
        var typedObject = JsonConvert.DeserializeObject(resultJson, classType);
        return _de.ObjectToDictionary(typedObject as Object);
    }

    /// <summary>
    /// 通用表单删除接口
    /// </summary>
    /// <param name="input">传参</param>
    /// <returns></returns>
    public int GeneralDelete(GeneralDeleteDto input)
    {
        var Model = _sysModel.GetById(input.model);
        if (Model == null || Model.Name == null)
        {
            throw new Exception("请输入正确表名");
        }
        string ids = string.Join(",", input.ids);
        var conModels = new List<IConditionalModel>
        {
            new ConditionalModel { FieldName = "Id", ConditionalType = ConditionalType.In, FieldValue = ids }
        };
        var list = _de.Search(Model.Name).Where(conModels).ToList();
        return _de.Delete(Model.Name, list);
    }

    /// <summary>
    /// 通用表单新增接口
    /// </summary>
    /// <param name="input">传参</param>
    /// <returns></returns>
    public Dictionary<string, object> GeneralSave(GeneralCreateDto input)
    {
        var Model = _de.GetSysModels(input.model,null).FirstOrDefault();
        // 将 object 转换为 JToken
        JToken jToken = JToken.FromObject(input.data);
        // 将 JToken 转换为 Dictionary<string, object>
        Dictionary<string, object> result = jToken.ToObject<Dictionary<string, object>>();
        bool hasId = result.ContainsKey("Id") && result["Id"] != null && long.TryParse(result["Id"].ToString(), out long id) && id != 0;
        if (hasId)
        {
            return _de.Update(Model.Name, result);
        }
        else
        {
            return _de.Create(Model.Name, result);
        }
    }

    /// <summary>
    /// 通用执行函数
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public dynamic GeneralExecFunc(GeneralExecFuncDto input)
    {
        dynamic res = new ExpandoObject();
        if (input.moduleName == null) { return res; }
        // 加载程序集
        Assembly assembly = Assembly.Load(input.moduleName);

        Type classType = assembly.GetType(input.className);
        object instance = Activator.CreateInstance(classType);

        // 获取 Res 字段
        var resProperty = classType.GetField("Rec");
        Type recType = assembly.GetType(input.model.Name.Value);
        // 获取 List<> 的类型
        Type listType = typeof(List<>).MakeGenericType(recType);
        // 创建 List<> 实例
        IList resultList = (IList)Activator.CreateInstance(listType);

        // 将 object 转换为 JToken
        JToken jToken = JToken.FromObject(input.data);
        // 将 JToken 转换为 Dictionary<string, object>
        Dictionary<string, object> Dict = jToken.ToObject<Dictionary<string, object>>();
        var item = _de.ConvertToClass(Dict, recType);

        resultList.Add(item);

        resProperty.SetValue(instance, resultList);
        // 获取 AuditRes 方法  调用 AuditRes 方法
        MethodInfo methodInfo = instance.GetType().GetMethod(input.methodName);
        object[] porp = null; // 生成参数 new object[] { "Prop1Value", "Prop2Value" }
        var result = methodInfo?.Invoke(instance, porp);

        var updatedRec = resProperty?.GetValue(instance);

        // 输出结果
        res.Data = updatedRec.ToDictionaryList();
        res.Result = result;

        return res;
    }
}