using IMS.Basic.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Basic.SeedData;
public class UomCategorySeedData : ISqlSugarEntitySeedData<UomCategory>
{
    [IgnoreUpdate]
    public IEnumerable<UomCategory> HasData()
    {
        return new[]{
            new UomCategory{Id=1310000000715,Name="Unit"}
        };
    }
}
