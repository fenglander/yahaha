namespace Yahaha.Core;

/// <summary>
/// 支持新实体结构的种子数据接口
/// </summary>
/// <typeparam name="TEntity">需要插入的实体</typeparam>
public interface IYahahaSeedData<TEntity>
    where TEntity : class, new()
{
    /// <summary>
    /// 返回种子数据
    /// </summary>
    /// <returns></returns>
    IEnumerable<TEntity> HasData();
}