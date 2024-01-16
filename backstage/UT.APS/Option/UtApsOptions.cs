
namespace UT.APS;
public sealed class UtApsOptions : IConfigurableOptions
{
    /// <summary>
    /// 服务链接字符
    /// </summary>
    public string? ConnectionString { get; set; }
    /// <summary>
    /// 数据类型
    /// </summary>
    public DbType DbType { get; set; }
}
