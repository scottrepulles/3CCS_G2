using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;
using DHK.Module.Helper;
using Hangfire.Server;
using System.Data;

namespace DHK.Blazor.Module.Helpers.Managers;

public class EnrollmentImportDataManager : BaseImportDataManager<Enrollment, FileImportJob>
{
    private int rowIndex = 0;
    PermissionPolicyRole role = null;
    private readonly List<string> parentProperty;
    private readonly ImportMapping importMapping;
    private readonly List<ImportMappingProperty> childrenProperty;


    public EnrollmentImportDataManager(
        IServiceProvider serviceProvider,
        IObjectSpace objectSpace,
        PerformContext performContext,
        string backgroundJobId,
        string parentObjectOid,
        string parentObjectType,
        string mappingId
    ) : base(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId)
    {
        importMapping = objectSpace.GetObjects<ImportMapping>(CriteriaOperator.Parse(
              $"{nameof(ImportMapping.Entity)} = ? ",
              typeof(Enrollment).FullName)).FirstOrDefault();
        childrenProperty = importMapping.Properties.Where(x => !string.IsNullOrEmpty(x.ChildrenProperty))
            .ToList();
        parentProperty = childrenProperty.Select(x => x.PropertyType).Distinct().ToList();
    }

    protected override Enrollment GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow)
    {
        rowIndex += 1;
        Enrollment Enrollment = objectSpace.GetObjects<Enrollment>().Where(o => o.Student.StudentNumber.Equals(entityRow[nameof(Enrollment.Student)]) 
        && o.Section.Name.Equals(entityRow[nameof(Enrollment.Section)])).FirstOrDefault();
        if (Enrollment == null)
        {
            return null;
        }
        return Enrollment;
    }

    protected override Enrollment CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow)
    {
        if (string.IsNullOrEmpty(entityRow[nameof(Enrollment.Student)]?.ToString()) || string.IsNullOrEmpty(entityRow[nameof(Enrollment.Section)]?.ToString()))
        {
            return null;
        }

        Enrollment newRecord = base.CreateNewRecord(objectSpace, entityRow);
        return newRecord;
    }

    protected override Enrollment MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, Enrollment entity, MapperHelper mapper)
    {

        foreach (string parent in parentProperty.OrderBy(x => x))
        {
            string newObjectName = childrenProperty.Where(x => x.PropertyType == parent).Select(x => x.Property).FirstOrDefault();
        }
        return entity;
    }
}
