using Furion.RemoteRequest;
using System.Text.Json;
using Yahaha.Core.Models;
using Yahaha.Core.Widget.Dto;

namespace Yahaha.Core.Widget.Service;

/// <summary>
/// 小组件相关接口
/// </summary>
[ApiDescriptionSettings(Order = 992)]
public class WidgetSerivce : IDynamicApiController, ITransient
{
    private readonly IdentityService _identitySvc;
    private readonly UserManager _userManager;
    private readonly ISqlSugarClient _db;
    private DataElement _de;

    public WidgetSerivce(IdentityService identitySvc, UserManager userManager, ISqlSugarClient db, DataElement de)
    {
        _identitySvc = identitySvc;
        _userManager = userManager;
        _db = db;
        _de = de;
    }

    /// <summary>
    /// 关联字段组件查询接口
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public object SelRelObjectQuery(SelRelObjectQueryDto input)
    {
        var Model = _de.GetSysModels(null, input.RelModelName).FirstOrDefault();

        Type classType = Model.GetclassType();

        MethodInfo genericSearchMethod = typeof(DataElement).GetMethods()
            .FirstOrDefault(m => m.Name == nameof(DataElement.Search) && m.IsGenericMethod && m.GetParameters().Length == 1);

        MethodInfo constructedSearchMethod = genericSearchMethod.MakeGenericMethod(classType);
        dynamic Query = constructedSearchMethod.Invoke(_de, new object[] { null });

       var resultList = Query.ToList();

        // 构建属性访问表达式
        var parameter = Expression.Parameter(classType, "item");
        var property = Expression.Property(parameter, "ModelTitle");
        var constant = Expression.Constant(input.Keywords); // 替换 someValue 为实际的值
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var containsCall = Expression.Call(property, containsMethod, constant);
        var lambda = Expression.Lambda(containsCall, parameter);

        // 构建 Where 方法调用
        var whereCallExpression = Expression.Call(
            typeof(Enumerable),
            "Where",
            new Type[] { classType },
            Expression.Constant(resultList),
            lambda
        );

        // 执行查询并转换结果为列表
        var queryable = Expression.Lambda(whereCallExpression).Compile().DynamicInvoke() as IEnumerable<dynamic>;
        object res = queryable.Take(20).ToDictionaryList();
        return res;

    }
}