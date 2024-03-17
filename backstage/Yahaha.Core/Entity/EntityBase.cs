
using Nest;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Yahaha.Core.Entity;
using Yahaha.Core.Models;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ComponentTCBBatchCreateContainerServiceVersionRequest.Types;

namespace Yahaha.Core;

/// <summary>
/// 框架实体基类Id
/// </summary>
public abstract class EntityBaseId
{
    /// <summary>
    /// 雪花Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true, IsIdentity = false)]
    public virtual long Id { get; set; }

    public virtual string? ModelTitle
    {
        get
        {
            return this.Id.ToString();
        }
    }
}

/// <summary>
/// 框架实体基类
/// </summary>
public abstract class EntityBase : EntityBaseId
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsOnlyIgnoreUpdate = true)]
    public virtual DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", IsOnlyIgnoreInsert = true)]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    [YhhColumn(ColumnDescription = "创建者", RelationalType = RelationalType.ManyToOne)]
    public SysUser? CreateUser { get; set; }

    /// <summary>
    /// 修改者
    /// </summary>
    [YhhColumn(ColumnDescription = "修改者", RelationalType = RelationalType.ManyToOne)]
    public SysUser? UpdateUser { get; set; }
    /// <summary>
    /// ORM指令，用于判断如何处理OneToMany,ManyToMany字段，其他情况不起作用
    /// </summary>
    [YhhColumn(ColumnDescription = "ORM指令", IsIgnore = true)]
    public ORMCommandEnum ORMCommand { get; set; }
}


public abstract class VirtualBase
{
}


public interface ModelAction<T> where T : class, new()
{
    List<T>? Rec { get; set; }
}

/// <summary>
/// 业务数据实体基类(数据权限)
/// </summary>
public abstract class EntityBaseData : EntityBase, IOrgIdFilter
{
    /// <summary>
    /// 创建者部门Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者部门Id", IsOnlyIgnoreUpdate = true)]
    public virtual long? CreateOrgId { get; set; }
}

/// <summary>
/// 租户基类实体
/// </summary>
public abstract class EntityTenant : EntityBase, ITenantIdFilter
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsOnlyIgnoreUpdate = true)]
    public virtual long? TenantId { get; set; }
}

/// <summary>
/// 租户基类实体Id
/// </summary>
public abstract class EntityTenantId : EntityBaseId, ITenantIdFilter
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsOnlyIgnoreUpdate = true)]
    public virtual long? TenantId { get; set; }
}

public abstract class EntityCompany : EntityBase, ICompanyFilter
{
    /// <summary>
    /// 公司
    /// </summary>
    [YhhColumn(ColumnDescription = "公司", RelationalType = RelationalType.ManyToOne)]
    public virtual SysCompany? Company { get; set; }
}

