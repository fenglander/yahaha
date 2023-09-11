﻿// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

namespace Yahaha.Core;

/// <summary>
/// 缓存相关常量
/// </summary>
public class CacheConst
{
    /// <summary>
    /// 用户缓存
    /// </summary>
    public const string KeyUser = "user:";

    /// <summary>
    /// 菜单缓存
    /// </summary>
    public const string KeyMenu = "menu:";

    /// <summary>
    /// 权限缓存（按钮集合）
    /// </summary>
    public const string KeyPermission = "permission:";

    /// <summary>
    /// 机构Id集合缓存
    /// </summary>
    public const string KeyOrgIdList = "org:";

    /// <summary>
    /// 角色最大数据范围缓存
    /// </summary>
    public const string KeyMaxDataScope = "maxDataScope:";

    /// <summary>
    /// 验证码缓存
    /// </summary>
    public const string KeyVerCode = "verCode:";

    /// <summary>
    /// 所有缓存关键字集合
    /// </summary>
    public const string KeyAll = "keys";

    /// <summary>
    /// 定时任务缓存
    /// </summary>
    public const string KeyTimer = "timer:";

    /// <summary>
    /// 在线用户缓存
    /// </summary>
    public const string KeyOnlineUser = "onlineuser:";

    /// <summary>
    /// 常量下拉框
    /// </summary>
    public const string KeyConst = "const:";

    /// <summary>
    /// swagger登录缓存
    /// </summary>
    public const string SwaggerLogin = "swaggerLogin:";

    /// <summary>
    /// 租户缓存
    /// </summary>
    public const string KeyTenant = "tenant:list";
}