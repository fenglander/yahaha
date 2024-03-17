using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahaha.Core.Entity;
[YhhTable("公司档案")]
[SystemTable]
public class SysCompany : EntityBase
{

    [YhhColumn(ColumnDescription = "编码")]
    public string Code { get; set; } = string.Empty;

    [YhhColumn(ColumnDescription = "名称")]
    public string Name { get; set; } = string.Empty;

    [YhhColumn(ColumnDescription = "简称")]
    public string? ShortName { get; set; }

    [YhhColumn(ColumnDescription = "描述", Length = 2000)]
    public string? Description { get; set; }

    [YhhColumn(ColumnDescription = "外部标识")]
    public string? ExternalId { get; set; }

    public override string ModelTitle => this.Name;
}
