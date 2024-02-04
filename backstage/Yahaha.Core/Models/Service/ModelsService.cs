// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Newtonsoft.Json.Linq;
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
            query = query.Where("\"BindingModel\" = @model", new { model = (long)modelid });
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
    /// 获取用户筛选字段信息
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public List<dynamic> GetUserFilterSchemes(long model)
    {
        var query = _de.Search("UserFilterScheme")
            .Where("\"ModelId\" = @ModelId and \"" + nameof(EntityBase.CreateUser) + "\" = @CreateUserId", new { ModelId = model, CreateUserId = _userManager.UserId })
            .ToList();
        return query;
    }

    /// <summary>
    /// 创建用户筛选配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPost("createUserFilterSchemes")]
    public async Task<int> CreateUserFilterSchemes(UserFilterScheme input)
    {
        int res = 0;
        try
        {
            res = await _db.Storageable(input).ExecuteCommandAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        if (res > 0) { return res; } else { throw new Exception("更新失败"); }
    }

    /// <summary>
    /// 通用列表数据接口
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<GeneralListRes> GeneralListData(GeneralListDto input)
    {
        var Model = await _sysModel.GetByIdAsync(input.model);
        var query = _de.Search(Model.Name);
        var Fields = _de.GetSysFields(Model.Name);
        var FilterSchemes = GetUserFilterSchemes(input.model);

        if (input.filters != null)
        {
            var FieldFilters = input.filters;
            var conModels = new List<IConditionalModel>();

            foreach (var field in FieldFilters)
            {
                if (field.filters == null) continue;
                var FieldName = field.name;
                foreach (var item in field.filters)
                {
                    if (item == null) continue;
                    conModels.Add(new ConditionalModel { FieldName = "\"" + FieldName + "\"", ConditionalType = item.conditionalType, FieldValue = item.value, CSharpTypeName = field.tType });
                }
            }
            query = query.Where(conModels);
        }

        var Raw = await query.ToPagedListAsync(input.Page, input.PageSize);

        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = Model.Name,
            items = Raw.Items.ToList(),
            ConnectionId = App.HttpContext.Connection.Id,
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
            userFilterSchemes = FilterSchemes,
        };
        return res;
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
            new ConditionalModel { FieldName = "\"Id\"", ConditionalType = ConditionalType.Equal, FieldValue = input.id.ToString(), CSharpTypeName="long" }
        };
        query = query.Where(conModels);

        var Row = query.ToList();

        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = Model.Name,
            items = Row,
            maxLevel = 3,
            ConnectionId = App.HttpContext.Connection.Id,
        };
        var res = _de.DrillDownData(DrillDownParams).FirstOrDefault();

        return res;
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
            new ConditionalModel { FieldName = "\"Id\"", ConditionalType = ConditionalType.In, FieldValue = ids }
        };
        var list = _de.Search(Model.Name).Where(conModels).ToList();
        return _de.Delete(Model.Name, list);
    }

    /// <summary>
    /// 通用表单新增接口
    /// </summary>
    /// <param name="input">传参</param>
    /// <returns></returns>
    public long GeneralSave(GeneralCreateDto input)
    {
        var Model = _sysModel.GetById(input.model);
        if (Model == null || Model.Name == null)
        {
            throw new Exception("请输入正确表名");
        }
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
        res.Data = _de.ToDictionaryList(updatedRec as Object);
        res.Result = result;

        return res;
    }


}