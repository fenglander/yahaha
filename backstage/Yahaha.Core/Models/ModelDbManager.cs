﻿using AngleSharp.Mathml.Dom;
using Microsoft.CodeAnalysis;
using Yahaha.Core.Models.Entity;
using Yahaha.Core.Service.Role.Dto;

namespace Yahaha.Core.Models;

public static class ModelDbManager
{
    public static void UpdateModelInfo(SqlSugarScope db, DbConnectionConfig config)
    {
        SqlSugarScopeProvider dbProvider = db.GetConnectionScope(config.ConfigId);
        DataElement dataElement = new DataElement(db);

        // 获取所有实体表-初始化表结构
        var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
        && (u.IsDefined(typeof(SugarTable), false) || u.IsDefined(typeof(YhhTable), false))).ToList();
        if (!entityTypes.Any()) return;
        List<SysModel> ExistModelRecs = dbProvider.Queryable<SysModel>().ToList();
        List<SysField> ExistFieldInfoRecs = dbProvider.Queryable<SysField>().ToList();
        List<long> RetainedModelIds = new List<long>();
        List<long> RetainedFieldIds = new List<long>();
        List<SysField> UpdateFieldInfoRecs = new List<SysField>();
        List<string> relationalTypes = Enum.GetValues(typeof(RelationalType))
                                           .Cast<RelationalType>()
                                           .Select(enumValue => enumValue.ToString())
                                           .ToList();
        foreach (var entityType in entityTypes)
        {
            var TableAttr = entityType.GetCustomAttribute<SugarTable>();
            var YhhTableAttr = entityType.GetCustomAttribute<YhhTable>();

            SysModel Model = new SysModel();
            Model.Name = entityType.Name;
            Model.FullName = entityType.FullName;
            Model.ModuleName = Path.GetFileNameWithoutExtension(entityType.Module.Name);
            Model.Description = YhhTableAttr != null ? YhhTableAttr?.Description : TableAttr?.TableDescription;
            Model.DefaultSort = YhhTableAttr != null ? YhhTableAttr?.DefaultSort : null;
            Model.DefaultSort = Model.DefaultSort == null && entityType.IsSubclassOf(typeof(EntityBase)) ? nameof(EntityBase.CreateTime).ToLower() + " desc" : Model.DefaultSort;
            Model.IsVirtual = entityType.IsSubclassOf(typeof(VirtualBase));
            Model.CreateUser = null;

            List<SysModel> ExistCurRecs = ExistModelRecs.Where(it => it.FullName == Model.FullName).ToList();
            if (ExistCurRecs != null && ExistCurRecs.Count > 0)
            {
                Model.Id = ExistCurRecs.FirstOrDefault().Id;
                RetainedModelIds.Add((long)Model.Id);
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
                    if (YhhColumnAttr != null && YhhColumnAttr.IsIgnore) continue;
                    SysField Field = ExistFieldInfoRecs.FirstOrDefault(it => it.Name == item.Name && it.ModelId == Model.Id);
                    if (Field != null)
                    {
                        RetainedFieldIds.Add((long)Field.Id);
                    }
                    else
                    {
                        Field = new SysField();
                    }
                    //var x = item.PropertyType.GenericTypeArguments[0].Name;
                    Field = GetType(item, Field, ColumnAttr, YhhColumnAttr);
                    Field.NotNull = IsNullable(item, YhhColumnAttr, Field);
                    Field.ModelId = Model.Id;
                    Field.SysModel = Model;
                    Field.Name = item.Name;
                    Field.Description = YhhColumnAttr?.ColumnDescription != null ? YhhColumnAttr.ColumnDescription : ColumnAttr?.ColumnDescription;
                    Field.ExtendedAttribute = ColumnAttr?.ExtendedAttribute != null ? ColumnAttr?.ExtendedAttribute.ToString() : null;

                    Field.IsOnlyIgnoreUpdate = YhhColumnAttr?.IsOnlyIgnoreUpdate;
                    Field.DecimalDigits = YhhColumnAttr?.DecimalDigits;
                    Field.Length = YhhColumnAttr?.Length;
                    Field.Help = YhhColumnAttr?.Help;
                    Field.Display = YhhColumnAttr?.Display != null ? YhhColumnAttr.Display : false;
                    Field.Related = YhhColumnAttr?.Related;
                    Field.OnDelete = YhhColumnAttr?.OnDelete;

                    if (Field.Display == true && relationalTypes.Contains(Field.tType))
                    {
                        throw new Exception(string.Format("模型：{0}，字段：{1}，关系类型字段不能作为标题，请选择字符串类型或其他类型字段。"));
                    }
                    UpdateFieldInfoRecs.Add(Field);
                }
            }
        }

        List<SysModel> DeleteModelInfoRecs = ExistModelRecs.Where(it => !RetainedModelIds.Contains((long)it.Id)).ToList();

        // 更新关联表id 更新关联字段类型
        foreach (SysField Field in UpdateFieldInfoRecs.Where(it => !string.IsNullOrEmpty(it.RelModelName)))
        {
            SysModel RelModel = dbProvider.Queryable<SysModel>().First(it => it.Name == Field.RelModelName);
            if (RelModel != null)
            {
                Field.RelModel = RelModel;
                var SubFields = UpdateFieldInfoRecs.Where(it => it.RelModelName == Field.SysModel.Name).ToList();
                if (SubFields.Any()) { Field.SubFields = SubFields; }
            }
        }

        //dbProvider.Storageable(UpdateFieldInfoRecs).DefaultAddElseUpdate().ExecuteCommand();
        dataElement.SetSysFieldsCache(UpdateFieldInfoRecs);
        dataElement.BatchAddElseUpdate(UpdateFieldInfoRecs);
        List<SysField> DeleteFieldInfoRecs = ExistFieldInfoRecs.Where(it => !RetainedFieldIds.Contains((long)it.Id)).ToList();
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
        var Row = dataElement.Search("SysModel").ToList();
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = nameof(SysModel),
            items = Row,
        };
        List<SysModel> Models = dataElement.DrillDownData<SysModel>(DrillDownParams);
        var Fields = dataElement.GetSysFields();
        List<long> RetainedActionIds = new List<long>();
        foreach (var entityType in entityTypes)
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
                    Model = Models.Where(it => it.FullName == BindingModelFullName).FirstOrDefault();
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
                    RetainedActionIds.Add((long)Action.Id);
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
                if (TriggerAttr != null)
                {
                    Action.Trigger = true;
                    var FieldPropertyInfo = GetPropertyInfo(genericArgument, TriggerAttr.FieldName);
                    var TriggerModel = Models.Where(it => it.FullName == FieldPropertyInfo.ClassType.FullName).FirstOrDefault();
                    var TriggerField = Fields.Where(it => it.Name == FieldPropertyInfo.PropertyInfo.Name && it.SysModel.FullName == FieldPropertyInfo.ClassType.FullName).FirstOrDefault();
                    Action.TriggerField = TriggerField;
                    Action.TriggerModel = TriggerModel;
                    Action.FieldName = TriggerAttr.FieldName;
                }
                actionList.Add(Action);
            }
        }
        dataElement.BatchAddElseUpdate(actionList);
        List<SysAction> DeleteActionRecs = ExistActionRecs.Where(it => !RetainedActionIds.Contains((long)it.Id)).ToList();
        dataElement.Delete(DeleteActionRecs);
    }

    public static void SeedDataCreation(SqlSugarScope db, DbConnectionConfig config)
    {
        DataElement de = new DataElement(db);
        var seedDataTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
            && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(IYahahaSeedData<>)))).ToList();
        if (!seedDataTypes.Any()) return;
        foreach (var seedType in seedDataTypes)
        {
            var instance = Activator.CreateInstance(seedType);

            var hasDataMethod = seedType.GetMethod("HasData");
            var seedData = ((IEnumerable)hasDataMethod?.Invoke(instance, new object[] { de }))?.Cast<object>();
            if (seedData == null) continue;
            var seedDataList = seedData.ToList();
            var entityType = seedType.GetInterfaces().First().GetGenericArguments().First();

            var Model = de.GetSysModels(null, entityType.Name).FirstOrDefault();
            if (Model != null)
            {
                List<SysField> Fields = de.GetSysFields(entityType.Name);
                IgnoreUpdateAttribute ignoreUpdate = hasDataMethod.GetCustomAttribute<IgnoreUpdateAttribute>();
                if (ignoreUpdate == null)
                {
                    de.AddElseUpdate(Model.Name, seedDataList);
                }
                else
                {
                    List<string> Keys = ignoreUpdate.Keys?.ToList() ?? new List<string> { "Id" };
                    foreach (var item in seedData.ToList())
                    {
                        var conModels = new List<IConditionalModel>();
                        foreach (var Key in Keys)
                        {
                            string tType = Fields.FirstOrDefault(t => t.Name == Key).tType;
                            object idValue = item.GetType().GetProperty(Key).GetValue(item);
                            conModels.Add(new ConditionalModel
                            {
                                FieldName = Key,
                                ConditionalType = ConditionalType.Equal,
                                FieldValue = idValue.ToString(),
                                CSharpTypeName = tType
                            });
                        }
                        var ExistRes = de.Search(Model.Name).Where(conModels).ToList().Count() > 0;
                        if (ExistRes)
                        {
                            seedDataList.Remove(item); // 如果ExistRes为true，则从seedData中移除当前项
                        }
                    }
                    de.BatchCreate(Model.Name, seedDataList);
                }
            }
        }
    }

    public static SysField GetType(PropertyInfo item, SysField Field, SugarColumn SugarAttr, YhhColumn YhhAttr)
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
                if (RelYhhAttr?.RelationalType == RelationalType.Relate)
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
            if (field.FieldType.IsEnum)
            {
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

    private static string GetDataTypeName(PropertyInfo TypeInfo, SugarColumn ColumnAttr)
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

    public static bool IsNullable(PropertyInfo prop, YhhColumn yhh, SysField field)
    {
        if (prop.Name == "Category")
        {
            Console.WriteLine("");
        }

        bool isNullable = (new NullabilityInfoContext().Create(prop).WriteState is NullabilityState.Nullable) == false;

        if (prop.Name.ToLower() == "id" || field.tType == "Boolean")
        {
            return false;
        }
        else if (yhh != null && yhh.NotNull == true)
        {
            return yhh.NotNull || isNullable;
        }
        // C#字符串允许null暂时没找到文档支持判断
        //else if (prop.PropertyType == typeof(string))
        //{
        //    return false;
        //}
        return isNullable;
    }
}