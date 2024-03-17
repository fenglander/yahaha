using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.Models.SeedData;

/// <summary>
/// 系统菜单表种子数据
/// </summary>
public class SysMenuSeedData : IYahahaSeedData<SysMenu>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate(nameof(SysMenu.Path))]
    public IEnumerable<SysMenu> HasData(DataElement de)
    {
        // 获取模型ID
        var Models = de.GetSysModels();
        SysModel? Model = Models.FirstOrDefault(u => u.Name == nameof(SysModel));
        SysModel? Field = Models.FirstOrDefault(u => u.Name == nameof(SysField));
        SysModel? Access = Models.FirstOrDefault(u => u.Name == nameof(SysAccess));
        return new[]
        {
            new SysMenu{ Id=1310000000654, Pid=1310000000301, Title="系统模型", Path="/platform/sysModel", Name="sysModel", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, Icon="ele-Coin", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=210,ModelId=Model?.Id },
            new SysMenu{ Id=1310000000655, Pid=1310000000301, Title="系统字段", Path="/platform/sysField", Name="sysField", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, Icon="fa fa-circle-o", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=220,ModelId=Field?.Id },
            new SysMenu{ Id=SnowFlakeSingle.Instance.NextId(), Pid=1310000000301, Title="基础权限", Path="/platform/sysAccess", Name="sysAccess", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, Icon="ele-Refrigerator", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=209,ModelId=Access?.Id },
        };
    }
}