using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Manufacturing.Action;
public class UomAction : ModelAction<Uom>
{
    public List<Uom>? Rec { get; set; }
    private DataElement _de;
    private IServiceProvider _serviceProvider;
    public UomAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _de = serviceProvider.GetRequiredService<DataElement>();
    }

    public void SyncU9Uom()
    {
        SqlSugarClient U9Conn = U9Sql.GetDbConn();
        string U9_UomCategory_sql = @"
        SELECT t1.Id,t1.EValue,t1.Code,t2.Name FROM [dbo].[UBF_Sys_ExtEnumValue] t1
        join UBF_Sys_ExtEnumValue_Trl t2 on t1.id = t2.id
        WHERE [ExtEnumType] = N'1001101151281854'"; 
        List<dynamic> U9UomCategory = U9Conn.Ado.SqlQuery<dynamic>(U9_UomCategory_sql);
        List<UomCategory> UomCategoryExists = _de.Search<UomCategory>().ToList();
        List<UomCategory> UomCategorys = new List<UomCategory>();
        foreach (dynamic Category in U9UomCategory)
        {
            UomCategory NewCategory = UomCategoryExists.FirstOrDefault(t => t.ExternalId == Category.EValue.ToString()) ?? new UomCategory();
            NewCategory.Name = Category.Name;
            NewCategory.Code = Category.Code;
            NewCategory.ExternalId = Category.EValue.ToString();
            UomCategorys.Add(NewCategory);
        }
        UomCategorys = _de.BatchAddElseUpdate(UomCategorys);


        double QueryUomRatio(dynamic Cur, List<dynamic> queryUomUomRes)
        {
            if (Cur.IsBase)
            {
                return Convert.ToDouble(Cur.RatioToBase); // 没有参考单位就是当前比率  
            }
            else
            {
                var result = queryUomUomRes.FirstOrDefault(item => item.Id.Equals(Cur.BaseUOM));
                if (result == null)
                {
                    throw new ArgumentException("BaseUOM not found in queryUomUomRes");
                }
                return Convert.ToDouble(Cur.RatioToBase) * QueryUomRatio(result, queryUomUomRes); // 有参考单位则叠加比率  
            }
        }

        string U9_Uom_sql = @"
        SELECT t1.Id,t1.Code,t1.UOMClass,t1.IsBase,t2.Name,t1.RatioToBase,t1.Round_Precision,t1.Round_RoundType,t1.Round_RoundValue,t1.BaseUOM
        FROM [dbo].[Base_UOM] t1 
        join [dbo].[Base_UOM_Trl] t2 on t1.id = t2.id
        where 1=1 and len(Name)>0 
        order by t1.UOMClass,t1.IsBase desc";
        List<dynamic> U9Uom = U9Conn.Ado.SqlQuery<dynamic>(U9_Uom_sql);
        List<Uom> UomExists = _de.Search<Uom>().ToList();
        List<Uom> Uoms = new List<Uom>();
        foreach (dynamic Uom in U9Uom)
        {
            Uom NewUom = UomExists.FirstOrDefault(t => t.ExternalId == Uom.Id.ToString()) ?? new Uom();
            UomCategory? uomCategory = UomCategorys.FirstOrDefault(t => t.ExternalId == Uom.UOMClass.ToString());
            if (uomCategory == null) { continue; }
            NewUom.Name = Uom.Name;
            NewUom.Code = Uom.Code;
            NewUom.ExternalId = Uom.Id.ToString();
            NewUom.Category = uomCategory;
            NewUom.Rounding = Uom.Round_Precision;
            NewUom.UomType = Uom.IsBase ? UomType.Reference : UomType.Bigger;
            NewUom.Ratio = 1 / QueryUomRatio(Uom, U9Uom);
            Uoms.Add(NewUom);
        }

        _de.BatchAddElseUpdate(Uoms);

    }
}
