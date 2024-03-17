using IMS.Manufacturing.Models;
using SqlSugar;
using Yahaha.Core.Models.Entity;

namespace IMS.Manufacturing.SeedData;

public class ManufacturingSysMenuSeedData : IYahahaSeedData<SysMenu>
{

    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate("Path")]
    public IEnumerable<SysMenu> HasData(DataElement de)
    {
        // 获取模型ID
        var Models = de.GetSysModels();
        SysModel? material = Models.FirstOrDefault(u => u.Name == nameof(Material));
        SysModel? materialCategory = Models.FirstOrDefault(u => u.Name == nameof(MaterialCategory));
        SysModel? uom = Models.FirstOrDefault(u => u.Name == nameof(Uom));
        SysModel? uomCategory = Models.FirstOrDefault(u => u.Name == nameof(UomCategory));
        SysModel? ManufacturingOrderModel = Models.FirstOrDefault(u => u.Name == nameof(ManufacturingOrder));
        SysModel? ProductionProcessModel = Models.FirstOrDefault(u => u.Name == nameof(ProductionProcess));
        SysModel? ProductionOperationModel = Models.FirstOrDefault(u => u.Name == nameof(ProductionOperation));
        SysModel? ManufacturingOrderTypeModel = Models.FirstOrDefault(u => u.Name == nameof(ManufacturingOrderType));
        SysModel? WarehouseModel = Models.FirstOrDefault(u => u.Name == nameof(Warehouse)); 
        SysModel? StorageBinModel = Models.FirstOrDefault(u => u.Name == nameof(StorageBin));
        SysModel? ProjectInfoModel = Models.FirstOrDefault(u => u.Name == nameof(ProjectInfo));

        return new[]
        {
            new SysMenu{ Id=1310000000720, Pid=0, Title="制造", Path="/manufacturing", Name="manufacturing", Component="Layout", Icon="ele-Tools", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=900 },
            new SysMenu{ Id=1310000000721, Pid=1310000000720, Title="生产订单", Path="/manufacturing/ManufacturingOrder", Name="ManufacturingOrder", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100,ModelId=ManufacturingOrderModel?.Id },

            new SysMenu{ Id=1310000000715, Pid=0, Title="基础档案", Path="/basic", Name="basic", Component="Layout", Icon="ele-Grid", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=990 },
            new SysMenu{ Id=1310000000716, Pid=1310000000715, Title="料品", Path="/basic/Material", Name="Material", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100,ModelId=material?.Id },
            new SysMenu{ Id=1310000000717, Pid=1310000000715, Title="料品分类", Path="/basic/MaterialCategory", Name="MaterialCategory", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=101,ModelId=materialCategory?.Id },
            new SysMenu{ Id=1310000000718, Pid=1310000000715, Title="计量单位", Path="/basic/Uom", Name="Uom", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=102,ModelId=uom?.Id },
            new SysMenu{ Id=1310000000719, Pid=1310000000715, Title="计量单位分类", Path="/basic/UomCategory", Name="UomCategory", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=103, ModelId=uomCategory?.Id },
            new SysMenu{ Id=SnowFlakeSingle.Instance.NextId(), Pid=1310000000715, Title="生产制程", Path="/basic/ProductionProcess", Name="ProductionProcess", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=104, ModelId=ProductionProcessModel?.Id },
            new SysMenu{ Id=SnowFlakeSingle.Instance.NextId(), Pid=1310000000715, Title="生产工序", Path="/basic/ProductionOperation", Name="ProductionOperation", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=105, ModelId=ProductionOperationModel?.Id },
            new SysMenu{ Id=SnowFlakeSingle.Instance.NextId(), Pid=1310000000715, Title="生产订单类型", Path="/basic/ManufacturingOrderType", Name="ManufacturingOrderType", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=106, ModelId=ManufacturingOrderTypeModel?.Id },
            new SysMenu{ Id=SnowFlakeSingle.Instance.NextId(), Pid=1310000000715, Title="仓库档案", Path="/basic/Warehouse", Name="Warehouse", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=107, ModelId=WarehouseModel?.Id },
            new SysMenu{ Id=SnowFlakeSingle.Instance.NextId(), Pid=1310000000715, Title="库位档案", Path="/basic/StorageBin", Name="StorageBin", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=108, ModelId=StorageBinModel?.Id },
            new SysMenu{ Id=SnowFlakeSingle.Instance.NextId(), Pid=1310000000715, Title="项目档案", Path="/basic/ProjectInfo", Name="ProjectInfo", Component="/system/generalView/index", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=109, ModelId=ProjectInfoModel?.Id },
        };
    }
}