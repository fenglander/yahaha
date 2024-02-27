using System.Dynamic;
using Yahaha.Core.Models;
using Yahaha.Core.Models.Dto;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Service.Role.Dto;
using Yahaha.Core.VisualDev.Dto;
using Yahaha.Core.VisualDev.Entity;

namespace Yahaha.Core.VisualDev.Service;

/// <summary>
/// 在线设计服务
/// </summary>
[ApiDescriptionSettings(Order = 991)]
public class VisualDevService : IDynamicApiController, ITransient
{
    private readonly IdentityService _identitySvc;
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysModel> _sysModel;
    private readonly SqlSugarRepository<SysField> _sysFields;
    private readonly SqlSugarRepository<FormDesign> _formDesign;
    private readonly ISqlSugarClient _db;
    private DataElement _de;

    public VisualDevService(IdentityService identitySvc, UserManager userManager, SqlSugarRepository<SysModel> sysModel, SqlSugarRepository<SysField> sysFields, SqlSugarRepository<FormDesign> visualDev, ISqlSugarClient db)
    {
        _identitySvc = identitySvc;
        _userManager = userManager;
        _sysModel = sysModel;
        _sysFields = sysFields;
        _db = db;
        _formDesign = visualDev;
        _de = new DataElement(_db);
    }

    /// <summary>
    /// 获取设计列表
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [DisplayName("获取表单设计")]
    public List<ExpandoObject> getFormDesginList()
    {
        var query = _de.Search(nameof(FormDesign));

        var Row = query.ToList();
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = nameof(FormDesign),
            items = Row,
        };
        var ObjectRes = _de.DrillDownData(DrillDownParams);
        return ObjectRes;
    }

    /// <summary>
    /// 获取列表设计
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [DisplayName("获取列表设计")]
    public List<ExpandoObject> getListDesginList()
    {
        var query = _de.Search(nameof(ListDesign));

        var Row = query.ToList();
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = nameof(ListDesign),
            items = Row,
        };
        var ObjectRes = _de.DrillDownData(DrillDownParams);
        return ObjectRes;
    }

    /// <summary>
    /// 保存设计方案
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public long SaveFormDesgin(FormDesign input)
    {
        long res;
        try
        {
            res = _de.AddElseUpdate(input);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        if (res > 0) { return res; } else { throw new Exception("保存失败"); }
    }

    /// <summary>
    /// 保存设计方案
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public long SaveListDesgin(ListDesign input)
    {
        long res;
        try
        {
            res = _de.AddElseUpdate(input);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        if (res > 0) { return res; } else { throw new Exception("保存失败"); }
    }


    public int DelFormDesgin(BaseIdInput input)
    {
        try
        {
            List<FormDesign> res = _formDesign.GetList(u=>u.Id == input.Id);
            return _de.Delete(res);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// 获取用户默认筛选字段信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public dynamic GetUserFilterSchemes([FromQuery] GetUserListDesignSchemeDto input)
    {
        var query = _de.Search(nameof(UserFilterScheme));
        var conModels = new List<IConditionalModel>();
        if (input.id != 0 && input.id != null)
        {
            conModels.Add(new ConditionalModel { FieldName = nameof(UserFilterScheme.Id), ConditionalType = ConditionalType.Equal, FieldValue = input.id.ToString(), CSharpTypeName = "long" });
        }
        if (input.sysModel != 0)
        {
            conModels.Add(new ConditionalModel { FieldName = nameof(UserFilterScheme.RelModel), ConditionalType = ConditionalType.Equal, FieldValue = input.sysModel.ToString(), CSharpTypeName = "long" });
        }
        if (input.listDesign != 0 && input.listDesign != null)
        {
            conModels.Add(new ConditionalModel { FieldName = nameof(UserFilterScheme.ListDesign), ConditionalType = ConditionalType.Equal, FieldValue = input.listDesign.ToString(), CSharpTypeName = "long" });
        }
        conModels.Add(new ConditionalModel { FieldName = nameof(UserFilterScheme.RelUser), ConditionalType = ConditionalType.Equal, FieldValue = _userManager.UserId.ToString(), CSharpTypeName = "long" });
        query = query.Where(conModels);

        var Row = query.ToList().FirstOrDefault();

        return Row;
    }

    /// <summary>
    /// 获取用户列表自定义配置
    /// </summary>
    /// <param name="input">输入参数</param>
    /// <returns></returns>
    public dynamic GetUserListDesignScheme([FromQuery] GetUserListDesignSchemeDto input)
    {
        var query = _de.Search(nameof(UserListDesignScheme));
        var conModels = new List<IConditionalModel>();
        if (input.id != 0 && input.id != null)
        {
            conModels.Add(new ConditionalModel { FieldName = nameof(UserListDesignScheme.Id), ConditionalType = ConditionalType.Equal, FieldValue = input.id.ToString(), CSharpTypeName = "long" });
        }
        if(input.sysModel != 0)
        {
            conModels.Add(new ConditionalModel { FieldName = nameof(UserListDesignScheme.RelModel), ConditionalType = ConditionalType.Equal, FieldValue = input.sysModel.ToString(), CSharpTypeName = "long" });
        }
        if (input.listDesign != 0 && input.id != null)
        {
            conModels.Add(new ConditionalModel { FieldName = nameof(UserListDesignScheme.ListDesign), ConditionalType = ConditionalType.Equal, FieldValue = input.listDesign.ToString(), CSharpTypeName = "long" });
        }
        if (input.user != 0)
        {
            conModels.Add(new ConditionalModel { FieldName = nameof(UserListDesignScheme.RelUser), ConditionalType = ConditionalType.Equal, FieldValue = input.user.ToString(), CSharpTypeName = "long" });
        }
        query = query.Where(conModels);

        var Row = query.ToList().FirstOrDefault();

        return Row;
    }
}