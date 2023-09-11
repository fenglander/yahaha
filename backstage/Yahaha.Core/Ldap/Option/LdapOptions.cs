using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core;
public sealed class LdapOptions : IConfigurableOptions
{
    public List<LdapConn> Ldaps { set; get; }
    
}

public class LdapConn
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public long TenantId { get; set; }
    /// <summary>
    /// 域名称
    /// </summary>
    public string Domain { get; set; }
    /// <summary>
    /// 域服务器地址
    /// </summary>
    public string Host { get; set; }
    /// <summary>
    /// 域服务器端口
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// 基础DC
    /// </summary>
    public string BaseDC { get; set; }
    /// <summary>
    /// 域账号用户名
    /// </summary>
    public string DomainAdminUser { get; set; }
    /// <summary>
    /// 域账号密码
    /// </summary>
    public string DomainAdminPassword { get; set; }
}