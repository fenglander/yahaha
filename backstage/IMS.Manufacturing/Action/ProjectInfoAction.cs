using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahaha.Core.Entity;

namespace IMS.Manufacturing.Action;
public class ProjectInfoAction : ModelAction<ProjectInfo>
{
    public List<ProjectInfo>? Rec { get; set; }
    private DataElement _de;
    private IServiceProvider _serviceProvider;
    public ProjectInfoAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _de = serviceProvider.GetRequiredService<DataElement>();
    }

    public void SyncU9ProjectInfo()
    {

        List<ProjectInfo> ProjectInfoExists = _de.Search<ProjectInfo>().ToList();
        var maxTime = ProjectInfoExists.Select(p => p.ExternalUpdateTime).Max();

        SqlSugarClient U9Conn = U9Sql.GetDbConn();
        List<SysCompany> Companys = _de.Search<SysCompany>().ToList();
        foreach (SysCompany company in Companys)
        {
            string U9_Project_Sql = @"SELECT TOP 10000 T1.Id,T1.Code,T2.Name,T1.ShortName,T2.Description,T1.State,
            ISNULL(T1.ModifiedOn,T1.CreatedOn) AS ModifiedOn,T1.Effective_IsEffective
            FROM CBO_Project  T1
            INNER JOIN CBO_Project_Trl T2 ON T1.Id = T2.Id AND T2.SysMLFlag = 'zh-CN' WHERE Org = @org  ";
            string OrderbySql = "ORDER BY ISNULL(T1.ModifiedOn,T1.CreatedOn) ";
            List<dynamic> U9Project = new List<dynamic>();
            if (maxTime == null)
            {
                U9Project = U9Conn.Ado.SqlQuery<dynamic>(U9_Project_Sql + OrderbySql, new { org = company.ExternalId });
            }
            else
            {
                U9Project = U9Conn.Ado.SqlQuery<dynamic>(U9_Project_Sql + "AND ISNULL(T1.ModifiedOn, T1.CreatedOn) >= @max " + OrderbySql, new { org = company.ExternalId, max = maxTime });
            }
            List<ProjectInfo> projectInfos = new List<ProjectInfo>();
            foreach (dynamic item in U9Project)
            {
                ProjectInfo newItem = ProjectInfoExists.FirstOrDefault(t => t.ExternalId == item.Id.ToString()) ?? new ProjectInfo();
                newItem.Code = item.Code;
                newItem.Name = item.Name;
                newItem.Description = item.Description;
                newItem.State = (ProjectStateEnum)item.State;
                newItem.ShortName = item.ShortName;
                newItem.Active = item.Effective_IsEffective;
                newItem.ExternalId = item.Id.ToString();
                newItem.ExternalUpdateTime = item.ModifiedOn;
                newItem.Company = company;
                projectInfos.Add(newItem);
            }
            _de.BatchAddElseUpdate(projectInfos);
        }


    }

}
