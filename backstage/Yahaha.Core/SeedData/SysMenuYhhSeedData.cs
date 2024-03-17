// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Yahaha.Core.Entity;
using Yahaha.Core.Models;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core;

/// <summary>
/// 系统配置表种子数据
/// </summary>
public class SysMenuYhhSeedData : IYahahaSeedData<SysMenu>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate("Path")]
    public IEnumerable<SysMenu> HasData(DataElement de)
    {
        var Models = de.GetSysModels();
        SysModel? Company = Models.FirstOrDefault(u => u.Name == nameof(SysCompany));
        return new[]
        {
            new SysMenu{ Id=SnowFlakeSingle.Instance.NextId(), Pid=1310000000301, Title="公司档案", Path="/platform/Company", Name="Company", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, Icon="ele-Coin", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=210,ModelId=Company?.Id },
        };
    }
}