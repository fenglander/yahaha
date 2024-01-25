
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System.Reflection;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Service.Role.Dto;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ProductSPUUpdateRequest.Types;

namespace Yahaha.Core.Models;

public static class ModelDbManager
{
    
    public static void UpdateModelInfo(SqlSugarScope db, DbConnectionConfig config)
    {
        SqlSugarScopeProvider dbProvider = db.GetConnectionScope(config.ConfigId);
        DataElement dataElement = new DataElement(db);

        // 获取所有实体表-初始化表结构
        var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass 
        && (u.IsDefined(typeof(SugarTable), false) || u.IsDefined(typeof(YhhTableAttribute), false))).ToList();
        if (!entityTypes.Any()) return;
        List<SysModels> ExistModelRecs = dbProvider.Queryable<SysModels>().ToList();
        List<SysField> ExistFieldInfoRecs = dbProvider.Queryable<SysField>().ToList();
        List<long> RetainedModelIds = new List<long>();
        List<long> RetainedFieldIds = new List<long>();
        List<SysField> UpdateFieldInfoRecs = new List<SysField>();
        foreach (var entityType in entityTypes)
        {
            var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
            var TableAttr = entityType.GetCustomAttribute<SugarTable>();
            var YhhTableAttr = entityType.GetCustomAttribute<YhhTableAttribute>();

            SysModels Model = new SysModels();
            Model.Name = entityType.FullName;
            Model.TableName = entityType.Name;
            Model.Description = YhhTableAttr != null ? YhhTableAttr?.Description : TableAttr?.TableDescription;
            Model.IsTenant = tAtt != null;
            Model.CreateUser = null;

            List<SysModels> ExistCurRecs = ExistModelRecs.Where(it => it.Name == Model.Name).ToList();
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

            foreach (PropertyInfo item in properties)
            {
                SugarColumn ColumnAttr = item.GetCustomAttribute<SugarColumn>();
                var ColumnNavigateAttr = item.GetCustomAttribute<Navigate>();
                var YhhColumnAttr = item.GetCustomAttribute<YhhColumn>();
                if (YhhColumnAttr != null || ColumnAttr != null || ColumnNavigateAttr != null)
                {
                    SysField Field = ExistFieldInfoRecs.FirstOrDefault(it => it.Name == item.Name && it.ModelId == Model.Id);
                    if (Field != null)
                    {
                        RetainedFieldIds.Add(Field.Id);
                    }
                    else
                    {
                        Field = new SysField();
                    }
                    //var x = item.PropertyType.GenericTypeArguments[0].Name;
                    Field = GetType(item, Field, ColumnAttr, YhhColumnAttr);
                    Field.ModelId = Model.Id;
                    Field.SysModel = Model;
                    Field.Name = item.Name;
                    Field.Description = YhhColumnAttr?.ColumnDescription != null ? YhhColumnAttr.ColumnDescription : ColumnAttr.ColumnDescription;
                    Field.ExtendedAttribute = ColumnAttr?.ExtendedAttribute != null ? ColumnAttr?.ExtendedAttribute.ToString() : null;
                    
                    
                    Field.DecimalDigits = YhhColumnAttr?.DecimalDigits;
                    Field.Length = YhhColumnAttr?.Length;
                    Field.NotNull = YhhColumnAttr?.NotNull != null ? YhhColumnAttr.NotNull : false;
                    Field.Help = YhhColumnAttr?.Help;
                    Field.Display = YhhColumnAttr?.Display != null ? YhhColumnAttr.Display : false;
                    Field.Related = YhhColumnAttr?.Related;
                    Field.OnDelete = YhhColumnAttr?.OnDelete;
                    Field.ForceRequired = YhhColumnAttr?.ForceRequired != null ? YhhColumnAttr.ForceRequired : false;
                    UpdateFieldInfoRecs.Add(Field); 
                }
            }
        }

        List<SysModels> DeleteModelInfoRecs = ExistModelRecs.Where(it => !RetainedModelIds.Contains(it.Id)).ToList();

        // 更新关联表id 更新关联字段类型
        foreach (SysField Field in UpdateFieldInfoRecs.Where(it => !string.IsNullOrEmpty(it.RelModelName)))
        {
            SysModels RelModel = dbProvider.Queryable<SysModels>().First(it => it.TableName == Field.RelModelName);
            if(RelModel != null ) { 
                Field.RelModel = RelModel;
                var SubFields = UpdateFieldInfoRecs.Where(it => it.RelModelName == Field.SysModel.TableName).ToList();
                if (SubFields.Any()) { Field.SubFields = SubFields; }
            }
            
        }

        //dbProvider.Storageable(UpdateFieldInfoRecs).DefaultAddElseUpdate().ExecuteCommand();
        dataElement.SetSysFieldsCache(UpdateFieldInfoRecs);
        dataElement.AddElseUpdate(UpdateFieldInfoRecs);
        List<SysField> DeleteFieldInfoRecs = ExistFieldInfoRecs.Where(it => !RetainedFieldIds.Contains(it.Id)).ToList();
        dataElement.Delete(DeleteFieldInfoRecs);
        dataElement.Delete(DeleteModelInfoRecs);
    }

    public static void UpdateModelAction(SqlSugarScope db, DbConnectionConfig config)
    {
        SqlSugarScopeProvider dbProvider = db.GetConnectionScope(config.ConfigId);
        DataElement dataElement = new DataElement(db);
        var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(YhhAction), false)).ToList();
        if (!entityTypes.Any()) return;

        List<SysAction> actionList = new List<SysAction>();
        List<SysAction> ExistActionRecs = dbProvider.Queryable<SysAction>().ToList();
        var Row = dataElement.Search<SysModels>().ToList();
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = nameof(SysModels),
            items = Row,
        };
        List<SysModels> Models = dataElement.DrillDownData<SysModels>(DrillDownParams);
        var Fields = dataElement.GetSysFields();
        List<long> RetainedActionIds = new List<long>();
        foreach ( var entityType in entityTypes)
        {
            YhhAction ActionAttr = entityType.GetCustomAttribute<YhhAction>();
            Type genericArgument = null;
            var resProperty = entityType.GetField("Rec");
            string BindingModelFullName = string.Empty;
            dynamic Model = null;
            if (resProperty != null)
            {
                var resType = resProperty.FieldType;
                if (resType.IsGenericType && resType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    genericArgument = resType.GetGenericArguments()[0];
                    BindingModelFullName = genericArgument.FullName;
                    Model = Models.Where(it => it.Name == BindingModelFullName).FirstOrDefault();
                }
            }

            var methods = entityType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                YhhFunction FunctionAttr = method.GetCustomAttribute<YhhFunction>();
                YhhTrigger TriggerAttr = method.GetCustomAttribute<YhhTrigger>();
                SysAction Action = ExistActionRecs.FirstOrDefault(it => it.ActionClassName == entityType.FullName && it.ActionName == method.Name);
                if (Action != null)
                {
                    RetainedActionIds.Add(Action.Id);
                }
                else
                {
                    Action = new SysAction();
                }
                Action.ActionModuleName = Path.GetFileNameWithoutExtension(entityType.Module.Name);
                Action.ActionClassName = entityType.FullName;
                Action.BindingModel = Model;
                Action.ActionName = method.Name;
                Action.Name = FunctionAttr?.Name;
                Action.Function = FunctionAttr != null;
                if(TriggerAttr != null)
                {
                    Action.Trigger = true;
                    var FieldPropertyInfo = GetPropertyInfo(genericArgument, TriggerAttr.FieldName);
                    var TriggerModel = Models.Where(it => it.Name == FieldPropertyInfo.ClassType.FullName).FirstOrDefault();
                    var TriggerField = Fields.Where(it => it.Name == FieldPropertyInfo.PropertyInfo.Name && it.SysModel.Name == FieldPropertyInfo.ClassType.FullName).FirstOrDefault();
                    Action.TriggerField = TriggerField;
                    Action.TriggerModel = TriggerModel;
                    Action.FieldName = TriggerAttr.FieldName;
                }
                actionList.Add(Action);
            }
        }
        dataElement.AddElseUpdate(actionList);
        List<SysAction> DeleteActionRecs = ExistActionRecs.Where(it => !RetainedActionIds.Contains(it.Id)).ToList();
        dataElement.Delete(DeleteActionRecs);
    }

    public static SysField GetType(PropertyInfo item,SysField Field, SugarColumn SugarAttr, YhhColumn YhhAttr)
    {
        Type declaringType = item.DeclaringType;
        // 处理类型问题
        if (YhhAttr != null && YhhAttr?.RelationalType != 0)
        {
            //关联的
            if (YhhAttr?.RelationalType == RelationalType.Relate)
            {
                var RelatedType = GetRelatedType(declaringType, YhhAttr?.Related);
                var RelSugarAttr = RelatedType.GetCustomAttribute<SugarColumn>();
                var RelYhhAttr = RelatedType.GetCustomAttribute<YhhColumn>();
                if(RelYhhAttr?.RelationalType == RelationalType.Relate)
                {
                    //throw new Exception("被关联字段不能再次关联");
                }
                if (RelatedType != null)
                { 
                    Field = GetType(RelatedType, Field, RelSugarAttr, RelYhhAttr); 
                    Field.Relate = true;
                }
            }
            else
            {
                Field.tType = YhhAttr?.RelationalType.ToString();
                Field.RelModelName = GetDataTypeName(item, SugarAttr);
            }
        }
        else
        {
            Field.tType = GetDataTypeName(item, SugarAttr);
        }
        // 枚举值
        var EnumValue = CheckEnum(item.PropertyType);
        if (EnumValue.Count() > 0)
        {
            Field.EnumValue = JSON.Serialize(EnumValue);
            Field.tType = "Select";
        }
        return Field;
    }

    public static PropertyInfo GetRelatedType(Type baseType, string relatedPropertyPath)
    {
        string[] propertyNames = relatedPropertyPath.Split('.');
        Type currentType = baseType;

        PropertyInfo propertyInfo = null;

        foreach (string name in propertyNames)
        {
            propertyInfo = currentType.GetProperty(name);

            if (propertyInfo == null)
            {
                // 如果找不到属性，返回 null
                return null;
            }

            currentType = propertyInfo.PropertyType; // 更新当前类型为属性的类型
        }

        return propertyInfo;
    }

    private static List<dynamic> CheckEnum(Type enumType)
    {
        List<dynamic> res = new List<dynamic>();
        if (enumType.IsEnum)
        {
            res = GetEnumDescription(enumType);
        }
        if (!enumType.IsGenericType)
        {
            return res;
        }
        var fields = ((System.Reflection.TypeInfo)enumType).DeclaredFields;
        foreach (var field in fields)
        {
            if(field.FieldType.IsEnum) {
                res = GetEnumDescription(field.FieldType);
            }
        }
        return res;
    }

    private static List<dynamic> GetEnumDescription(Type enumType)
    {
        //Type enumType = item.PropertyType;
        var fields = ((System.Reflection.TypeInfo)enumType).DeclaredFields;
        var enumList = new List<dynamic>();
        foreach (var value in Enum.GetValues(enumType))
        {
            int intValue = Convert.ToInt32(value);
            string enumItemName = Enum.GetName(enumType, value);
            // 获取描述属性值
            FieldInfo fieldInfo = enumType.GetField(enumItemName);
            DescriptionAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            string DescriptionValue = (attributes.Length > 0) ? attributes[0].Description : enumItemName;

            var enumInfo = new
            {
                Name = enumItemName,
                Description = DescriptionValue,
                Value = intValue
            };
            // 在这里可以处理枚举项的信息
            enumList.Add(enumInfo);
        }
        
        return enumList;
    }

    static string GetDataTypeName(PropertyInfo TypeInfo, SugarColumn ColumnAttr)
    {
        if (ColumnAttr?.ColumnDataType == StaticConfig.CodeFirst_BigString)
        {
            return "BigString";
        }

        string assemblyQualifiedName = TypeInfo.PropertyType.AssemblyQualifiedName;
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

    public class PropertyInfoWithClass
    {
        public Type ClassType { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }


    public static PropertyInfoWithClass GetPropertyInfo(Type classType, string propertyName)
    {
        string[] propertyNames = propertyName.Split('.');
        Type currentType = classType;

        PropertyInfo propertyInfo = null;

        foreach (string name in propertyNames)
        {
            propertyInfo = currentType.GetProperty(name);

            if (propertyInfo == null)
            {
                // 如果找不到属性，返回 null
                return null;
            }

            currentType = propertyInfo.PropertyType; // 更新当前类型为属性的类型
        }

        return new PropertyInfoWithClass
        {
            ClassType = classType,
            PropertyInfo = propertyInfo
        };
    }


}