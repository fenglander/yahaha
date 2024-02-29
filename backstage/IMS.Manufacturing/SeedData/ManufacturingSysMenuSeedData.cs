using IMS.Manufacturing.Models;
using Yahaha.Core.Models.Entity;

namespace IMS.Manufacturing.SeedData;
public class ManufacturingSysMenuSeedData : IYahahaSeedData<SysMenu>
{
    private readonly DataElement _de;

    public ManufacturingSysMenuSeedData(DataElement dataElement)
    {
        _de = dataElement;
    }

    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysMenu> HasData()
    {
        // 获取模型ID
        var Models = _de.GetSysModels();
        SysModel? ManufacturingOrderModel = Models.FirstOrDefault(u => u.Name == nameof(ManufacturingOrder));

        return new[]
        {
            new SysMenu{ Id=1310000000720, Pid=0, Title="制造", Path="/manufacturing", Name="manufacturing", Component="Layout", Icon="ele-Tools", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=900 },
            new SysMenu{ Id=1310000000721, Pid=1310000000720, Title="生产订单", Path="/manufacturing/ManufacturingOrder", Name="ManufacturingOrder", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100,ModelId=ManufacturingOrderModel?.Id },
        };
    }
}
