using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Novell.Directory.Ldap;
using SqlSugar;
using StackExchange.Redis;

namespace Yahaha.Core.Ldap;
public class LdapService
{
    //public static string Domain = "apac";//域名称
    //public static string Host = "apac.contoso.com";//域服务器地址
    //public static string BaseDC = "DC=apac,DC=contoso,DC=com";//根据上面的域服务器地址，每个点拆分为一个DC，例如上面的apac.contoso.com，拆分后就是DC=apac,DC=contoso,DC=com
    //public static int Port = 389;//域服务器端口，一般默认就是389
    //public static string DomainAdminUser = "admin";//域管理员账号用户名，如果只是验证登录用户，不对域做修改，可以就是登录用户名
    //public static string DomainAdminPassword = "1qaz!QAZ";//域管理员账号密码，如果只是验证登录用户，不对域做修改，可以就是登录用户的密码

    private readonly List<LdapConn> _ldapConns;

    public LdapService(IOptions<LdapOptions> ldap)
    {
        _ldapConns = ldap.Value.Ldaps;
    }
    public bool Validate(string username, string password, SysTenant tenant)
    {
        try
        {
            using (var conn = new LdapConnection())
            {
                var ldap = _ldapConns.First(x => x.TenantId == tenant.Id);
                conn.Connect(ldap.Host, ldap.Port);
                conn.Bind(ldap.DomainAdminUser, ldap.DomainAdminPassword);//这里用户名或密码错误会抛出异常LdapException

                var entities =
                    conn.Search(ldap.BaseDC, LdapConnection.ScopeSub,
                        $"mail={username}",//注意一个多的空格都不能打，否则查不出来
                        new string[] { "sAMAccountName", "cn", "mail", "telephoneNumber", "Sn", "Givename", "displayName" }, false);

                string userDn = null;
                while (entities.HasMore())
                {
                    var entity = entities.Next();
                    var sAMAccountName = entity.GetAttribute("sAMAccountName")?.StringValue;
                    var cn = entity.GetAttribute("cn")?.StringValue;
                    var mail = entity.GetAttribute("mail")?.StringValue;

                    Console.WriteLine($"User name : {sAMAccountName}");//james
                    Console.WriteLine($"User full name : {cn}");//James, Clark [james]
                    Console.WriteLine($"User mail address : {mail}");//james@contoso.com

                    //If you need to Case insensitive, please modify the below code.
                    if (sAMAccountName != null && (sAMAccountName == username || mail == username))
                    {
                        userDn = entity.Dn;
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(userDn)) return false;
                conn.Bind(userDn, password);//这里用户名或密码错误会抛出异常LdapException
                                            // LdapAttribute passwordAttr = new LdapAttribute("userPassword", password);
                                            // var compareResult = conn.Compare(userDn, passwordAttr);
                conn.Disconnect();
                return true;
            }
        }
        catch (LdapException ldapEx)
        {
            string message = ldapEx.Message;

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

}
