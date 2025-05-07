using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DHK.Module.BusinessObjects;
using DHK.Module.Enumerations;
using DHK.Module.Models;
using DKH.Module.Enumerations;
using System;
using System.Data;
using System.Linq;
namespace DHK.Module.Helper;

public class MapperHelper
{
    readonly Session objectSession;

    public MapperHelper(Session session)
    {
        objectSession = session;
    }
    public void MapProperties(XPBaseObject source, XPBaseObject destination, params string[] propertiesToIgnore)
    {
        foreach (XPMemberInfo sourceMemberInfo in source.ClassInfo.PersistentProperties)
        {
            if (sourceMemberInfo is not DevExpress.Xpo.Metadata.Helpers.ServiceField &&
                !sourceMemberInfo.IsKey &&
                !propertiesToIgnore.Contains(sourceMemberInfo.Name))
            {
                XPMemberInfo targetMemberInfo = destination.ClassInfo.FindMember(sourceMemberInfo.Name);
                if (targetMemberInfo != null)
                {
                    object clonedValue = null;
                    if (sourceMemberInfo.ReferenceType != null)
                    {
                        object value = sourceMemberInfo.GetValue(source);
                        if (value != null)
                        {
                            clonedValue = objectSession.GetObjectByKey(objectSession.GetClassInfo(value), objectSession.GetKeyValue(value));
                        }
                    }
                    else
                    {
                        clonedValue = sourceMemberInfo.GetValue(source);
                    }
                    targetMemberInfo.SetValue(destination, clonedValue);
                }
            }
        }
    }

    public virtual object GetPropertyValue(XPMemberInfo targetMemberInfo, object columnValue, IObjectSpace objectSpace)
    {
        if (targetMemberInfo.ReferenceType != null && !string.IsNullOrEmpty(columnValue.ToString()))
        {
            if (targetMemberInfo.ReferenceType.ClassType == typeof(Program))
            {
                Program program  = objectSpace.GetObjects<Program>(new BinaryOperator(nameof(Program.Code), columnValue)).FirstOrDefault();
                return program;
            }
            else if (targetMemberInfo.ReferenceType.ClassType == typeof(Course))
            {
                Course course = objectSpace.GetObjects<Course>(new BinaryOperator(nameof(Course.Code), columnValue)).FirstOrDefault();
                return course;
            }
            else if (targetMemberInfo.ReferenceType.ClassType == typeof(Teacher))
            {
                Teacher teacher = objectSpace.GetObjects<Teacher>(new BinaryOperator(nameof(Teacher.EmployeeNumber), columnValue)).FirstOrDefault();
                return teacher;
            }
            else if (targetMemberInfo.ReferenceType.ClassType == typeof(Student))
            {
                Student student = objectSpace.GetObjects<Student>(new BinaryOperator(nameof(Student.StudentNumber), columnValue)).FirstOrDefault();
                return student;
            }
            else if (targetMemberInfo.ReferenceType.ClassType == typeof(AcademicYear))
            {
                AcademicYear academicYear = objectSpace.GetObjects<AcademicYear>(new BinaryOperator(nameof(AcademicYear.Year), columnValue)).FirstOrDefault();
                return academicYear;
            }
            else if (targetMemberInfo.ReferenceType.ClassType == typeof(College))
            {
                College college = objectSpace.GetObjects<College>(new BinaryOperator(nameof(College.Code), columnValue)).FirstOrDefault();
                return college;
            }
            else if (targetMemberInfo.ReferenceType.ClassType == typeof(Section))
            {
                Section section = objectSpace.GetObjects<Section>(new BinaryOperator(nameof(Section.Code), columnValue)).FirstOrDefault();
                return section;
            }
        }
        else
        {
            if (targetMemberInfo.MemberType == typeof(string))
            {
                Attribute criteriaAttribute = targetMemberInfo.FindAttributeInfo("CriteriaOptionsAttribute");
                if (criteriaAttribute != null)
                    ParseCriteriaString(Convert.ToString(columnValue));
                return Convert.ToString(columnValue);
            }
            else if (targetMemberInfo.MemberType == typeof(bool))
            {
                if (columnValue.ToString() == "")
                {
                    columnValue = false;
                }
                return Convert.ToBoolean(columnValue);
            }
            else if (targetMemberInfo.MemberType == typeof(DateTime))
            {
                if (columnValue.ToString() != "")
                {
                    return Convert.ToDateTime(columnValue);
                }
            }
            else if (targetMemberInfo.MemberType == typeof(decimal))
            {
                if (columnValue.ToString() != "")
                {
                    return Convert.ToDecimal(columnValue);
                }
            }
            else if (targetMemberInfo.MemberType == typeof(double))
            {
                if (columnValue.ToString() != "")
                {
                    return Convert.ToDouble(columnValue);
                }
            }
            else if (targetMemberInfo.MemberType == typeof(short))
            {
                if (columnValue.ToString() != "")
                {
                    return Convert.ToInt16(columnValue);
                }
            }
            else if (targetMemberInfo.MemberType == typeof(int))
            {
                if (columnValue != null && columnValue.GetType() == typeof(string) && columnValue.ToString() != "")
                    return Convert.ToInt32(columnValue);
            }
            else if (targetMemberInfo.MemberType == typeof(long))
            {
                if (columnValue.ToString() != "")
                {
                    return Convert.ToInt64(columnValue);
                }
            }
            else if (targetMemberInfo.MemberType == typeof(long?))
            {
                return columnValue != null && columnValue.ToString() != "" ? System.Convert.ToInt64(columnValue) : null;
            }
            else if (targetMemberInfo.MemberType == typeof(TimeSpan))
            {
                if (columnValue.ToString() != "")
                {
                    DateTime parsedDateTime = DateTime.Parse(columnValue.ToString());
                    TimeSpan timeSpan = parsedDateTime.TimeOfDay;
                    return timeSpan;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(TimeSpan?))
            {
                if (columnValue.ToString() != "")
                {
                    DateTime parsedDateTime = DateTime.Parse(columnValue.ToString());
                    TimeSpan timeSpan = parsedDateTime.TimeOfDay;
                    return timeSpan;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(TimeOnly))
            {
                if (columnValue.ToString() != "")
                {
                    DateTime parsedDateTime = DateTime.Parse(columnValue.ToString());
                    TimeOnly timeOnly = TimeOnly.FromTimeSpan(parsedDateTime.TimeOfDay);
                    return timeOnly;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(TimeOnly?))
            {
                if (columnValue.ToString() != "")
                {
                    DateTime parsedDateTime = DateTime.Parse(columnValue.ToString());
                    TimeOnly timeOnly = TimeOnly.FromTimeSpan(parsedDateTime.TimeOfDay);
                    return timeOnly;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(DateOnly))
            {
                if (columnValue.ToString() != "")
                {
                    DateTime parsedDateTime = DateTime.Parse(columnValue.ToString());
                    DateOnly dateOnly = DateOnly.FromDateTime(parsedDateTime);
                    return dateOnly;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(DateOnly?))
            {
                if (columnValue.ToString() != "")
                {
                    DateTime parsedDateTime = DateTime.Parse(columnValue.ToString());
                    DateOnly? dateOnly = DateOnly.FromDateTime(parsedDateTime);
                    return dateOnly;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(EmploymentStatusType))
            {
                if (columnValue.ToString() != "")
                {
                    if (Enum.TryParse<EmploymentStatusType>(columnValue.ToString(), true, out var status))
                    {
                        // Successfully parsed
                        return status;
                    }
                    return EmploymentStatusType.FULLTIME;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(GenderType))
            {
                if (columnValue.ToString() != "")
                {
                    if (Enum.TryParse<GenderType>(columnValue.ToString(), true, out var status))
                    {
                        // Successfully parsed
                        return status;
                    }
                    return GenderType.Male;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(EnrollmentStatusType))
            {
                if (columnValue.ToString() != "")
                {
                    if (Enum.TryParse<EnrollmentStatusType>(columnValue.ToString(), true, out var status))
                    {
                        return status;
                    }
                    return EnrollmentStatusType.INACTIVE;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(SemesterType))
            {
                if (columnValue.ToString() != "")
                {
                    if (Enum.TryParse<SemesterType>(columnValue.ToString(), true, out var status))
                    {
                        return status;
                    }
                    return SemesterType.First;
                }
            }
            else if (targetMemberInfo.MemberType == typeof(YearLevelType))
            {
                if (columnValue.ToString() != "")
                {
                    if (Enum.TryParse<YearLevelType>(columnValue.ToString(), true, out var status))
                    {
                        return status;
                    }
                    return YearLevelType.FIRST;
                }
            }
        }
        return null;
    }
    public static NameValueModel GetCountries(string searchTerm)
    {
        List<NameValueModel> countries = new List<NameValueModel>();
        countries.Add(new NameValueModel { Value = "United States", Name = "USA" });
        countries.Add(new NameValueModel { Value = "United States", Name = "United States of America" });
        countries.Add(new NameValueModel { Value = "United States", Name = "US" });
        countries.Add(new NameValueModel { Value = "United Kingdom", Name = "UK" });
        return countries.Where(c => c.Name.ToLower().Contains(searchTerm.ToLower())).FirstOrDefault();
    }
    public void MapHeader(DataColumn leadTemplateColumn, IObjectSpace objectSpace, List<ImportMappingProperty> importMappingProperties)
    {
        ImportMappingProperty column = importMappingProperties.Find(x => x.MapTo == leadTemplateColumn.ColumnName);
        if (column != null)
        {
            leadTemplateColumn.ColumnName = column.ChildrenProperty != null ? column.Property + "." + column.ChildrenProperty : column.Property;
        }
    }
    public virtual List<string> ValidateRequiredProperty(DataTable dataTable, List<ImportMappingProperty> importMappingProperties)
    {
        List<Guid> processedProperty = new List<Guid>();
        foreach (ImportMappingProperty property in importMappingProperties.Where(x => !x.Required).ToList())
        {
            string mapToName = property.MapTo;
            if (mapToName == null || mapToName == string.Empty)
            {
                string newColumnName = property.ChildrenProperty != null ? property.Property + "." + property.ChildrenProperty : property.Property;
                DataColumn newColumn = dataTable.Columns.Add(newColumnName, typeof(string));
                foreach (DataRow row in dataTable.Rows)
                {
                    row[newColumnName] = property.DefaultValue;
                }
            }
            else
            {
                int dtableColumn = dataTable.Columns.IndexOf(mapToName);
                if (dtableColumn < 0)
                {
                    string newColumnName = property.ChildrenProperty != null ? property.Property + "." + property.ChildrenProperty : property.Property;
                    DataColumn newColumn = dataTable.Columns.Add(newColumnName, typeof(string));
                    foreach (DataRow row in dataTable.Rows)
                    {
                        row[newColumnName] = property.DefaultValue;
                    }
                }
                else
                {
                    IEnumerable<ImportMappingProperty> importProperties = importMappingProperties.Where(x => x.MapTo == mapToName && !processedProperty.Contains(x.Oid));
                    if (importProperties.Count() > 1)
                    {
                        string newColumnName = property.ChildrenProperty != null ? property.Property + "." + property.ChildrenProperty : property.Property;
                        DataColumn newColumn = dataTable.Columns.Add(newColumnName, typeof(string));
                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (string.IsNullOrEmpty(row[mapToName].ToString()))
                                row[newColumnName] = property.DefaultValue;
                            else
                                row[newColumnName] = row[mapToName].ToString();
                        }
                    }
                    else
                    {
                        string newColumnName = property.ChildrenProperty != null ? property.Property + "." + property.ChildrenProperty : property.Property;
                        dataTable.Columns[dtableColumn].ColumnName = newColumnName;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (string.IsNullOrEmpty(row[newColumnName].ToString()))
                                row[newColumnName] = property.DefaultValue;

                        }
                    }
                }
            }
            processedProperty.Add(property.Oid);
        }
        List<string> mappedRequired = new List<string>();
        //map required
        foreach (DataColumn leadTemplateColumn in dataTable.Columns)
        {
            ImportMappingProperty column = importMappingProperties.Find(x => x.MapTo == leadTemplateColumn.ColumnName && x.Required);
            if (column != null)
            {
                leadTemplateColumn.ColumnName = column.ChildrenProperty != null ? column.Property + "." + column.ChildrenProperty : column.Property;
                mappedRequired.Add(column.MapTo);
                foreach (DataRow row in dataTable.Rows)
                {
                    if (string.IsNullOrEmpty(row[leadTemplateColumn.ColumnName].ToString()))
                        row[leadTemplateColumn.ColumnName] = column.DefaultValue;

                }
            }
        }

        int reqCount = importMappingProperties.Count(x => x.Required);
        if (mappedRequired.Count != reqCount)
        {
            return importMappingProperties.Where(x => !mappedRequired.Contains(x.MapTo) && x.Required).Select(x => x.MapTo).ToList();
        }
        return new List<string>();
    }
    public void ParseCriteriaString(string criteria)
    {
        try
        {
            CriteriaOperator.Parse(Convert.ToString(criteria));
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("Cannot parse criteria string: " + ex.Message);
        }
    }
}
