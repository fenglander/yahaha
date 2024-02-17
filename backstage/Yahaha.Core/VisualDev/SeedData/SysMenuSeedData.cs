using Yahaha.Core;
using Yahaha.Core.Models;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.VisualDev.SeedData;

/// <summary>
/// 系统菜单表种子数据
/// </summary>
public class SysMenuSeedData : IYahahaSeedData<SysMenu>
{
    private readonly DataElement _de;

    public SysMenuSeedData(DataElement dataElement)
    {
        _de = dataElement;
    }

    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysMenu> HasData()
    {

        return new[]
        {
            new SysMenu{ Id=1310000000652, Pid=1310000000601, Title="表单设计", Path="/develop/formDesign", Name="formDesign", Component="/system/visualDev/formDesignList", Type=MenuTypeEnum.Menu, Icon="ele-Memo", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=1310000000653, Pid=1310000000601, Title="列表设计", Path="/develop/listDesign", Name="listDesign", Component="/system/visualDev/listDesignList", Type=MenuTypeEnum.Menu, Icon="ele-Tickets", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
        };
    }
}