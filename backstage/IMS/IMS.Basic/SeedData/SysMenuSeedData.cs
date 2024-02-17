using Yahaha.Core.Models.Entity;

namespace IMS.Basic.SeedData;

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
        // 获取模型ID
        var Models = _de.GetSysModels();
        SysModel? Material = Models.FirstOrDefault(u => u.Name == "Material");
        SysModel? MaterialCategory = Models.FirstOrDefault(u => u.Name == "MaterialCategory");
        SysModel? Uom = Models.FirstOrDefault(u => u.Name == "Uom");
        SysModel? UomCategory = Models.FirstOrDefault(u => u.Name == "UomCategory");

        return new[]
        {
            new SysMenu{ Id=1310000000715, Pid=0, Title="基础档案", Path="/basic", Name="dashboard", Component="Layout", Icon="ele-Grid", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=990 },
            new SysMenu{ Id=1310000000716, Pid=1310000000715, Title="料品", Path="/basic/Material", Name="Material", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100,ModelId=Material?.Id },
            new SysMenu{ Id=1310000000717, Pid=1310000000715, Title="料品分类", Path="/basic/MaterialCategory", Name="MaterialCategory", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=101,ModelId=MaterialCategory?.Id },
            new SysMenu{ Id=1310000000718, Pid=1310000000715, Title="计量单位", Path="/basic/Uom", Name="Uom", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=102,ModelId=Uom?.Id },
            new SysMenu{ Id=1310000000719, Pid=1310000000715, Title="计量单位分类", Path="/basic/UomCategory", Name="UomCategory", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=103, ModelId=UomCategory?.Id },
        };
    }
}