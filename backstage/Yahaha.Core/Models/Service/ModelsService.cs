﻿// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using COSXML.Log;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nest;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Org.BouncyCastle.Ocsp;
using SqlSugar;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Service.Role.Dto;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.CgibinUserInfoBatchGetRequest.Types;

namespace Yahaha.Core.Service;

/// <summary>
/// 系统模型服务
/// </summary>
[ApiDescriptionSettings(Order = 460)]
public class ModelsService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysModels> _sysModels;
    private readonly SqlSugarRepository<SysFields> _sysFields;
    private readonly ISqlSugarClient _db;
    private static readonly ICache _cache = Cache.Default;

    public ModelsService(SqlSugarRepository<SysModels> sysModels, SqlSugarRepository<SysFields> sysFields, ISqlSugarClient db)
    {
        _sysModels = sysModels;
        _sysFields = sysFields;
        _db = db;
    }

    /// <summary>
    /// 获取模型列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取模型列表")]
    public async Task<List<SysModels>> GetModelList([FromQuery] PosInput input)
    {
        var res = await _sysModels.AsQueryable().Includes(x => x.Fields, model => model.SysModels)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Model.Contains(input.Code))
            .OrderBy(u => u.Name).ToListAsync();
        return res;
    }

    /// <summary>
    /// 获取字段列表
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [DisplayName("获取字段列表")]
    public async Task<List<SysFields>> GetFieldList(string model)
    {
        // 配置缓存
        var cacheKey = $"GetFieldList,Model:{model}";
        List<SysFields> list = new List<SysFields>();

        if (!_cache.TryGetValue(cacheKey, out list))
        {
            // 如果缓存中没有值，则执行数据库查询
            list = await _db.Queryable<SysFields>()
               .Includes(x => x.SysModels)
               .Where(x => x.SysModels.Model == model)
               .ToListAsync();

            // 将查询结果添加到缓存中
            _cache.Set(cacheKey, list);
        }

        return list;
    }

    /// <summary>
    /// 获取关系字段
    /// </summary>
    /// <param name="model"></param>
    /// <param name="ResList">结果</param>
    /// <param name="Lv">阶</param>
    /// <returns></returns>
    public async Task<List<SysFields>> GetRelFields(string model,List<SysFields> ResList, int Lv = 0)
    {
        var TempList = await _db.Queryable<SysFields>()
           .Includes(x => x.SysModels)
           .Where(x => x.SysModels.Model == model && x.RelFieldName != null && x.RelFieldName != "")
           .ToListAsync();
        if(TempList != null && TempList.Count > 0)
        {
            foreach (var item in TempList)
            {
                if (!ResList.Any(it => it.Id == item.Id))
                {
                    ResList.Add(item);
                    ResList = await GetRelFields(item.tType, ResList, Lv);
                }
            }
        }

        return ResList;
    }

    public async Task<List<SysFields>> GetUserTableViewFields(string model)
    {
        var FieldList = await GetFieldList(model);
        return FieldList;
    }

    /// <summary>
    /// 通用列表数据接口
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<GeneralListRes> GeneralListData(GeneralListDto input)
    {
        
        var Fields = await GetUserTableViewFields(input.model);
        if(input.model == null)
        {
            throw new Exception("请输入正确表名");
        }
        var query =  _db.Queryable<dynamic>().AS(input.model);

        if(input.filters != null)
        {
            var FieldFilters = input.filters;
            var conModels = new List<IConditionalModel>();

            foreach (var field in FieldFilters)
            {
                if (field.filters == null) continue;
                var FieldName = field.name;
                foreach(var item in field.filters)
                {
                    if (item == null) continue;
                    conModels.Add(new ConditionalModel { FieldName = FieldName, ConditionalType = item.conditionalType, FieldValue = item.value, CSharpTypeName = field.tType });
                }
            }
            query = query.Where(conModels);
        }

        var Raw  = await query.ToPagedListAsync(input.Page, input.PageSize);

        DrillDownDataDto DrillDownParams = new DrillDownDataDto {
            model = input.model,
            items = Raw.Items.ToList(),
        };

        
        var expandoList = await DrillDownData(DrillDownParams);

        GeneralListRes res = new GeneralListRes() {
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


    public async Task<List<ExpandoObject>> DrillDownData(DrillDownDataDto Params)
    {
        var expandoList = new List<ExpandoObject>();
        var FieldList = await GetFieldList(Params.model);
        List<dynamic> TempValueList;
        if (Params.items != null)
        {
            TempValueList = Params.items;
        }
        else
        {
            TempValueList = await GetRecById(Params.model, Params.relField, Params.id);
        }

        foreach (var val in TempValueList)
        {
            var valDict = val as IDictionary<string, object>;
            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var Field in FieldList)
            {
                if (Field.RelFieldName != null && Field.RelFieldName != "")
                {
                    if (Params.curLevel >= Params.maxLevel) { continue; }
                    // 使用curLevel + 1而不是curLevel++来控制深度
                    var nextLevel = Params.curLevel + 1;

                    if (Field.NavigatType == "OneToMany")
                    {
                        DrillDownDataDto nextParams = new DrillDownDataDto { 
                            model = Field.tType,
                            id = (long)valDict["id"],
                            relField = Field.RelFieldName,
                            curLevel = nextLevel,
                            maxLevel = Params.maxLevel,
                        };
                        expando[Field.Name] = await DrillDownData(nextParams);
                    }
                    else if (Field.NavigatType == "OneToOne")
                    {
                        DrillDownDataDto nextParams = new DrillDownDataDto()
                        {
                            model = Field.tType,
                            id = (long)valDict[Field.RelFieldName.ToLower()],
                            relField = "id",
                            curLevel = nextLevel,
                            maxLevel = Params.maxLevel,
                        };
                        expando[Field.Name] = await DrillDownData(nextParams);
                    }
                }
                else
                {
                    expando[Field.Name] = valDict[Field.Name.ToLower()];
                }
            }

            expandoList.Add((ExpandoObject)expando);
        }

        return expandoList;
    }


    public async Task<List<dynamic>> GetRecById(string ModelName, string idField = "", long id = 0)
    {

        // 配置缓存
        var cacheKey = $"GetRecById,ModelName:{ModelName},idField:{idField},id:{id}";
        ConcurrentDictionary<Type, LambdaExpression> GetRecCache;
        List<dynamic> res = new List<dynamic>();
        if (!_cache.TryGetValue(cacheKey, out GetRecCache))
        {
            GetRecCache = new ConcurrentDictionary<Type, LambdaExpression>();
            var query = _db.Queryable<dynamic>().AS(ModelName);
            if (id > 0 && idField != "")
            {
                var whereFunc = ObjectFuncModel.Create("Format", idField, "=", "{long}:" + id.ToString());
                query.Where(whereFunc);
            }

            res = await query.ToListAsync();
            var lambdaExpression = Expression.Lambda<Func<List<dynamic>>>(Expression.Constant(res));
            GetRecCache.AddOrUpdate(typeof(List<dynamic>), lambdaExpression, (key, existing) => lambdaExpression);
            _cache.Set(cacheKey, GetRecCache, 10);

        }
        else
        {
            // 如果缓存中有值，则直接返回
            if (GetRecCache.TryGetValue(typeof(List<dynamic>), out var cachedExpression))
            {
                var func = (Func<List<dynamic>>)cachedExpression.Compile();
                res = func();
            }
        }
        return res;
    }

    static ExpandoObject ConvertToExpandoObject(dynamic dynamicObj)
    {
        ExpandoObject expandoObj = new ExpandoObject();
        var expandoDict = (IDictionary<string, object>)expandoObj;

        foreach (var propertyName in GetDynamicMemberNames(dynamicObj))
        {
            var propertyValue = GetPropertyValue(dynamicObj, propertyName);
            expandoDict[propertyName] = propertyValue;
        }

        return expandoObj;
    }

    // 获取 DynamicObject 的属性名称
    static IEnumerable<string> GetDynamicMemberNames(dynamic dynamicObj)
    {
        return ((IDynamicMetaObjectProvider)dynamicObj).GetMetaObject(Expression.Constant(dynamicObj)).GetDynamicMemberNames();
    }

    // 获取 DynamicObject 的属性值
    static object GetPropertyValue(dynamic dynamicObj, string propertyName)
    {
        return ((IDynamicMetaObjectProvider)dynamicObj).GetMetaObject(Expression.Constant(dynamicObj)).GetMember(Expression.Constant(propertyName)).Value;
    }


}