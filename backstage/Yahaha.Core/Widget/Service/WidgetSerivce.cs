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
using System.Collections;

namespace Yahaha.Core.Widget.Service;
/// <summary>
/// 小组件相关接口
/// </summary>
[ApiDescriptionSettings(Order = 992)]
public class WidgetSerivce : IDynamicApiController, ITransient
{
    private readonly IdentityService _identitySvc;
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysModel> _sysModel;
    private readonly SqlSugarRepository<SysField> _sysField;
    private readonly ISqlSugarClient _db;
    private DataElement _de;

    public WidgetSerivce(IdentityService identitySvc, UserManager userManager, SqlSugarRepository<SysModel> sysModel, SqlSugarRepository<SysField> sysField, ISqlSugarClient db)
    {
        _identitySvc = identitySvc;
        _userManager = userManager;
        _sysModel = sysModel;
        _sysField = sysField;
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
        var Query = _de.Search(input.RelModelName);
        if(input.Keywords != null)
        {
            var lable = _de.GetSysModelLabelInfos(input.RelModelName).LastOrDefault();
            var conModels = new List<IConditionalModel>
            {
                new ConditionalModel { FieldName = lable.Name, ConditionalType = ConditionalType.Like, FieldValue =  input.Keywords}
            };
            Query = Query.Where(conModels);
        }
         var Row = Query.ToPagedList(1, input.PageSize);
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = input.RelModelName,
            items = Row.Items.ToList(),
        };
        var res = _de.DrillDownData(DrillDownParams);
        return res.Select(expando => (dynamic)expando).ToList();
    }
}
