using System.Dynamic;
using Yahaha.Core.Models;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Service.Role.Dto;

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
    private readonly SqlSugarRepository<VisualDev> _visualDev;
    private readonly ISqlSugarClient _db;
    private DataElement _de;

    public VisualDevService(IdentityService identitySvc, UserManager userManager, SqlSugarRepository<SysModel> sysModel, SqlSugarRepository<SysField> sysFields, SqlSugarRepository<VisualDev> visualDev, ISqlSugarClient db)
    {
        _identitySvc = identitySvc;
        _userManager = userManager;
        _sysModel = sysModel;
        _sysFields = sysFields;
        _db = db;
        _visualDev = visualDev;
        _de = new DataElement(_db);
    }

    /// <summary>
    /// 获取设计信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [DisplayName("设计信息")]
    public async Task<VisualDev> GetById(long? id)
    {
        var res = await _visualDev.GetByIdAsync(id);
        return res;
    }

    /// <summary>
    /// 获取设计列表
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [DisplayName("获取设计列表")]
    public async Task<List<ExpandoObject>> getList()
    {
        var query = _de.Search(nameof(VisualDev));

        var Row = query.ToList();
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = nameof(VisualDev),
            items = Row,
        };
        var ObjectRes = _de.DrillDownData(DrillDownParams);
        return ObjectRes;
    }

    /// <summary>
    /// 按模型获取字段列表
    /// </summary>
    /// <param name="name">模型</param>
    /// <param name="id">模型</param>
    /// <returns></returns>
    [DisplayName("按模型获取字段列表")]
    public List<ExpandoObject> getFieldsByModel([FromQuery] string? name, [FromQuery] long? id)
    {
        DataElement dataElement = new DataElement(_db);
        var Raw = dataElement.Search("SysField")
            .WhereIF(id.HasValue || id > 0, "\"ModelId\" = @modelid", new { modelid = id })
            .WhereIF(!string.IsNullOrEmpty(name), "(SELECT \"name\" FROM \"sysmodel\"  WHERE  \"t\".\"ModelId\"=\"id\"   ) = @Name", new { Name = name })
            .ToList();

        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = "SysField",
            items = Raw,
        };

        var expandoList = dataElement.DrillDownData(DrillDownParams);
        return expandoList;
    }

    /// <summary>
    /// 获取模型信息
    /// </summary>
    /// <param name="id">模型id</param>
    /// <returns></returns>
    [DisplayName("获取模型信息")]
    public async Task<SysModel> getModelInfo(long id)
    {
        var res = await _sysModel.GetFirstAsync(x => x.Id == id);

        return res;
    }

    /// <summary>
    /// 保存设计方案
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "saveVisualDev")]
    public async Task<long> Save(VisualDev input)
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

    [ApiDescriptionSettings(Name = "delVisualDev")]
    public async Task<Boolean> Del(BaseIdInput input)
    {
        try
        {
            return await _visualDev.DeleteByIdAsync(input.Id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}