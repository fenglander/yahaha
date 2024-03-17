using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Entity;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Models;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ComponentTCBBatchCreateContainerServiceVersionRequest.Types;

namespace Yahaha.Core.SeedData;
public class SysCompanySeedData : IYahahaSeedData<SysCompany>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate(new string[] { "Code", "Name" })]
    public IEnumerable<SysCompany> HasData(DataElement de)
    {
        var Models = de.GetSysModels();
        SysModel? Company = Models.FirstOrDefault(u => u.Name == nameof(SysCompany));
        return new[]
        {
            new SysCompany{ Id=SnowFlakeSingle.Instance.NextId(), ShortName="优特科技", Code="UT", Name="珠海优特电力科技股份有限公司"},
        };
    }
}