using Yahaha.Core.Models.Entity;
using Yahaha.Core.Models;
using AngleSharp.Mathml.Dom;
using SQLitePCL;
using Yahaha.Core.Service.Role.Dto;
using System.Dynamic;

namespace Yahaha.Core.VisualDev.Service;
/// <summary>
/// 在线设计服务
/// </summary>
[ApiDescriptionSettings(Order = 991)]
public class VisualDevService : IDynamicApiController, ITransient
{
    private readonly IdentityService _identitySvc;
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysModels> _sysModels;
    private readonly SqlSugarRepository<SysField> _sysFields;
    private readonly SqlSugarRepository<VisualDev> _visualDev;
    private readonly ISqlSugarClient _db;
    private DataElement _de;

    public VisualDevService(IdentityService identitySvc, UserManager userManager, SqlSugarRepository<SysModels> sysModels, SqlSugarRepository<SysField> sysFields, SqlSugarRepository<VisualDev> visualDev, ISqlSugarClient db)
    {
        _identitySvc = identitySvc;
        _userManager = userManager;
        _sysModels = sysModels;
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
    public async Task<List<VisualDev>> getList([FromQuery] string name)
    {
        var qery = _visualDev.AsQueryable();
        qery = qery.WhereIF(!name.IsNullOrEmpty(), u => u.FullName.Contains(name));
        var res = await qery.ToListAsync();
        return res;
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
            .WhereIF(id.HasValue || id > 0 , "\"ModelId\" = @modelid", new { modelid = id })
            .WhereIF(!string.IsNullOrEmpty(name), "(SELECT \"name\" FROM \"sysmodels\"  WHERE  \"t\".\"ModelId\"=\"id\"   ) = @Name", new { Name = name })
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
    public async Task<SysModels> getModelInfo(long id)
    {
        var res = await _sysModels.GetFirstAsync(x => x.Id == id);

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
