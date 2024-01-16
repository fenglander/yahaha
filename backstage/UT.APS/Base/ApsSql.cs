

namespace UT.APS.Base;
public static class ApsSql
{
    /// <summary>
    /// 生成链接
    /// </summary>
    /// <returns></returns>
    public static SqlSugarClient GetDbConn()
    {
        var UtApsOptions = App.GetOptions<UtApsOptions>();

        var db = new SqlSugarClient(new List<ConnectionConfig>()
        {
            new ConnectionConfig(){DbType=UtApsOptions.DbType,ConnectionString=UtApsOptions.ConnectionString,IsAutoCloseConnection=true}
        });

        return db;
    }
}
