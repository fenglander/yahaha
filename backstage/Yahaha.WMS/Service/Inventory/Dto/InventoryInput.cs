
namespace Yahaha.WMS;

    /// <summary>
    /// 存货档案基础输入参数
    /// </summary>
    public class InventoryBaseInput
    {
        /// <summary>
        /// Completed Quantity
        /// </summary>
        public virtual float qty_operation_wip { get; set; }
        
        /// <summary>
        /// Code
        /// </summary>
        public virtual string? code { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public virtual int? unit { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? createtime { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime? updatetime { get; set; }
        
        /// <summary>
        /// 创建者Id
        /// </summary>
        public virtual long? createuserid { get; set; }
        
        /// <summary>
        /// 修改者Id
        /// </summary>
        public virtual long? updateuserid { get; set; }
        
        /// <summary>
        /// 软删除
        /// </summary>
        public virtual bool isdelete { get; set; }
        
    }

    /// <summary>
    /// 存货档案分页查询输入参数
    /// </summary>
    public class InventoryInput : BasePageInput
    {
        /// <summary>
        /// Completed Quantity
        /// </summary>
        public float qty_operation_wip { get; set; }
        
        /// <summary>
        /// Code
        /// </summary>
        public string? code { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public int? unit { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createtime { get; set; }
        
        /// <summary>
         /// 创建时间范围
         /// </summary>
         public List<DateTime?> createtimeRange { get; set; } 
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? updatetime { get; set; }
        
        /// <summary>
         /// 更新时间范围
         /// </summary>
         public List<DateTime?> updatetimeRange { get; set; } 
        /// <summary>
        /// 创建者Id
        /// </summary>
        public long? createuserid { get; set; }
        
        
        /// <summary>
        /// 软删除
        /// </summary>
        public bool isdelete { get; set; }
        
    }

    /// <summary>
    /// 存货档案增加输入参数
    /// </summary>
    public class AddInventoryInput : InventoryBaseInput
    {
    }

    /// <summary>
    /// 存货档案删除输入参数
    /// </summary>
    public class DeleteInventoryInput : BaseIdInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public long id { get; set; }
        
    }

    /// <summary>
    /// 存货档案更新输入参数
    /// </summary>
    public class UpdateInventoryInput : InventoryBaseInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public long id { get; set; }
        
    }

    /// <summary>
    /// 存货档案主键查询输入参数
    /// </summary>
    public class QueryByIdInventoryInput : DeleteInventoryInput
    {

    }
