

using Nest;
using Org.BouncyCastle.Ocsp;
using System.Collections.Generic;
using System.Reflection;
using Yahaha.Core.Models.Entity;

namespace Yahaha.Core.Models;

public static class ModelDbManager
{
    
    public static void UpdateModelInfo(SqlSugarScope db, DbConnectionConfig config)
    {

        SqlSugarScopeProvider dbProvider = db.GetConnectionScope(config.ConfigId);

        // 获取所有实体表-初始化表结构
        var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false)).ToList();
        if (!entityTypes.Any()) return;
        List<SysModels> ExistModelRecs = dbProvider.Queryable<SysModels>().ToList();
        List<SysFields> ExistFieldInfoRecs = dbProvider.Queryable<SysFields>().ToList();
        List<long> RetainedModelIds = new List<long>();
        List<long> RetainedFieldIds = new List<long>();
        List<SysFields> UpdateFieldInfoRecs = new List<SysFields>();
        foreach (var entityType in entityTypes)
        {
            var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
            var TableAttr = entityType.GetCustomAttribute<SugarTable>();

            SysModels Model = new SysModels();
            Model.Model = entityType.Name;
            Model.Name = TableAttr?.TableDescription; ;
            Model.IsTenant = tAtt != null; ;

            List<SysModels> ExistCurRecs = ExistModelRecs.Where(it => it.Model == Model.Model).ToList();
            if (ExistCurRecs != null && ExistCurRecs.Count > 0)
            {
                Model.Id = ExistCurRecs.FirstOrDefault().Id;
                RetainedModelIds.Add(Model.Id);
                dbProvider.Updateable(Model).ExecuteCommand();
            }
            else
            {
                Model.Id = dbProvider.Insertable(Model).ExecuteReturnSnowflakeId();
            }
            PropertyInfo[] properties = ((System.Reflection.TypeInfo)entityType).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            foreach (var item in properties)
            {
                var ColumnAttr = item.GetCustomAttribute<SugarColumn>();
                var ColumnNavigateAttr = item.GetCustomAttribute<Navigate>();
                if (ColumnAttr != null || ColumnNavigateAttr != null)
                {
                    SysFields Field = ExistFieldInfoRecs.FirstOrDefault(it => it.Name == item.Name && it.ModelId == Model.Id);
                    if (Field != null)
                    {
                        RetainedFieldIds.Add(Field.Id);
                    }
                    else
                    {
                        Field = new SysFields();
                    }
                    //var x = item.PropertyType.GenericTypeArguments[0].Name;
                    Field.ModelId = Model.Id;
                    Field.Name = item.Name;
                    Field.Description =  ColumnAttr?.ColumnDescription;
                    Field.tType = GetDataTypeName(item.PropertyType.AssemblyQualifiedName);
                    Field.NavigatType = ColumnNavigateAttr?.NavigatType.ToString();
                    Field.RelFieldName = ColumnNavigateAttr?.Name;
                    Field.RelFieldName2 = ColumnNavigateAttr?.Name2;
                    Field.MappingType = ColumnNavigateAttr?.MappingType != null ? ColumnNavigateAttr?.MappingType.ToString(): null ;
                    Field.MappingAId = ColumnNavigateAttr?.MappingAId;
                    Field.MappingBId = ColumnNavigateAttr?.MappingBId;
                    Field.NavigatType = ColumnNavigateAttr?.NavigatType != null ? ColumnNavigateAttr?.NavigatType.ToString() : null;
                    UpdateFieldInfoRecs.Add(Field);

                }
            }

        }

        dbProvider.Storageable(UpdateFieldInfoRecs).DefaultAddElseUpdate().ExecuteCommand();

        List<SysFields> DeleteFieldInfoRecs = ExistFieldInfoRecs.Where(it => !RetainedFieldIds.Contains(it.Id)).ToList();
        dbProvider.Deleteable(DeleteFieldInfoRecs).ExecuteCommand();
        List<SysModels> DeleteModelInfoRecs = ExistModelRecs.Where(it => !RetainedModelIds.Contains(it.Id)).ToList();
        dbProvider.Deleteable(DeleteModelInfoRecs).ExecuteCommand();

    }

    static string GetDataTypeName(string assemblyQualifiedName)
    {
        // Split the string by '['
        string[] parts = assemblyQualifiedName.Split('[');

        // Get the first part after the split
        string typePart = parts[parts.Length - 1];

        // Extract the type name before ','
        int commaIndex = typePart.IndexOf(',');
        string dataType = typePart.Substring(0, commaIndex);

        string lastPart = dataType.Split('.').Last(); // 获取最后一个部分

        return lastPart;
    }



}