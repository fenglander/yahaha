namespace Yahaha.WMS;

    /// <summary>
    /// 存货档案输出参数
    /// </summary>
    public class InventoryDto
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
        /// 更新时间
        /// </summary>
        public DateTime? updatetime { get; set; }
        
        /// <summary>
        /// 创建者Id
        /// </summary>
        public long? createuserid { get; set; }
        
        /// <summary>
        /// 修改者Id
        /// </summary>
        public long? updateuserid { get; set; }
        
        /// <summary>
        /// 软删除
        /// </summary>
        public bool isdelete { get; set; }
        
        /// <summary>
        /// Id
        /// </summary>
        public long id { get; set; }
        
    }
