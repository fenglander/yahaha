

using Furion.FriendlyException;
using Mapster;

namespace Yahaha.WMS;
/// <summary>
/// 存货档案服务
/// </summary>
[ApiDescriptionSettings(YahahaWmsConst.GroupName, Order = 100)]
public class InventoryService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<Inventory> _rep;
    public InventoryService(SqlSugarRepository<Inventory> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 分页查询存货档案
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<InventoryOutput>> Page(InventoryInput input)
    {
        var query= _rep.AsQueryable()
                    .WhereIF(!string.IsNullOrWhiteSpace(input.code), u => u.Code.Contains(input.code.Trim()))
                    .WhereIF(input.unit>0, u => u.Unit == input.unit)
                    .WhereIF(input.createuserid>0, u => u.CreateUserId == input.createuserid)

                    .Select<InventoryOutput>()
;
        if(input.createtimeRange != null && input.createtimeRange.Count >0)
        {
                DateTime? start= input.createtimeRange[0]; 
                query = query.WhereIF(start.HasValue, u => u.createtime > start);
                if (input.createtimeRange.Count >1 && input.createtimeRange[1].HasValue)
                {
                    var end = input.createtimeRange[1].Value.AddDays(1);
                    query = query.Where(u => u.createtime < end);
                }
        } 
        if(input.updatetimeRange != null && input.updatetimeRange.Count >0)
        {
                DateTime? start= input.updatetimeRange[0]; 
                query = query.WhereIF(start.HasValue, u => u.updatetime > start);
                if (input.updatetimeRange.Count >1 && input.updatetimeRange[1].HasValue)
                {
                    var end = input.updatetimeRange[1].Value.AddDays(1);
                    query = query.Where(u => u.updatetime < end);
                }
        } 
        query = query.OrderBuilder(input);
        return await query.ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加存货档案
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task Add(AddInventoryInput input)
    {
        var entity = input.Adapt<Inventory>();
        await _rep.InsertAsync(entity);
    }

    /// <summary>
    /// 删除存货档案
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task Delete(DeleteInventoryInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _rep.FakeDeleteAsync(entity);   //假删除
    }

    /// <summary>
    /// 更新存货档案
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateInventoryInput input)
    {
        var entity = input.Adapt<Inventory>();
        await _rep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取存货档案
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<Inventory> Get([FromQuery] QueryByIdInventoryInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.id);
    }

    /// <summary>
    /// 获取存货档案列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<InventoryOutput>> List([FromQuery] InventoryInput input)
    {
        return await _rep.AsQueryable().Select<InventoryOutput>().ToListAsync();
    }





}

