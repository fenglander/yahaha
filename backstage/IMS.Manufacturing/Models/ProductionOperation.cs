using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Manufacturing.Models;
[YhhTable("生产工序")]
public class ProductionOperation : EntityBase
{
    [YhhColumn(ColumnDescription = "编码")]
    public string Code { get; set; }

    [YhhColumn(ColumnDescription = "名称", Display = true)]
    public string Name { get; set; }

    [YhhColumn(ColumnDescription = "简称")]
    public string? ShortName { get; set; }

    [YhhColumn(ColumnDescription = "活动")]
    public bool? Active { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }
}
