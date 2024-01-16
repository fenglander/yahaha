using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Models;
using Yahaha.Core.Widget.Dto;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using Yahaha.Core.Service.Role.Dto;

namespace Yahaha.Core.Widget.Service;
/// <summary>
/// 小组件相关接口
/// </summary>
[ApiDescriptionSettings(Order = 992)]
public class WidgetSerivce : IDynamicApiController, ITransient
{
    private readonly IdentityService _identitySvc;
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysModels> _sysModels;
    private readonly SqlSugarRepository<SysField> _sysFields;
    private readonly ISqlSugarClient _db;
    private DataElement _de;

    public WidgetSerivce(IdentityService identitySvc, UserManager userManager, SqlSugarRepository<SysModels> sysModels, SqlSugarRepository<SysField> sysFields, ISqlSugarClient db)
    {
        _identitySvc = identitySvc;
        _userManager = userManager;
        _sysModels = sysModels;
        _sysFields = sysFields;
        _db = db;
        _de = new DataElement(_db);
    }

    /// <summary>
    /// 关联字段组件查询接口
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public List<dynamic> SelRelObjectQuery(SelRelObjectQueryDto input)
    {
        var Row = _de.Search(input.RelModel).ToList();
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = input.RelModel,
            items = Row,
        };
        var res = _de.DrillDownData(DrillDownParams);
        return res.Select(expando => (dynamic)expando).ToList();
    }
}
