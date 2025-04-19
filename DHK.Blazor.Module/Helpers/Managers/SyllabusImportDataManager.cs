using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;
using DHK.Module.Helper;
using Hangfire.Server;
using System.Data;

namespace DHK.Blazor.Module.Helpers.Managers;

public class SyllabusImportDataManager : BaseImportDataManager<Syllabus, FileImportJob>
{
    private int rowIndex = 0;
    private readonly List<string> parentProperty;
    private readonly ImportMapping importMapping;
    private readonly List<ImportMappingProperty> childrenProperty;


    public SyllabusImportDataManager(
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
              typeof(Syllabus).FullName)).FirstOrDefault();
        childrenProperty = importMapping.Properties.Where(x => !string.IsNullOrEmpty(x.ChildrenProperty))
            .ToList();
        parentProperty = childrenProperty.Select(x => x.PropertyType).Distinct().ToList();
    }

    protected override Syllabus GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow)
    {
        rowIndex += 1;
        Syllabus Syllabus = objectSpace.GetObjects<Syllabus>(new BinaryOperator(nameof(Syllabus.Name), entityRow[nameof(Syllabus.Name)]?.ToString())).FirstOrDefault();
        if (Syllabus == null)
        {
            return null;
        }
        return Syllabus;
    }

    protected override Syllabus CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow)
    {
        if (string.IsNullOrEmpty(entityRow[nameof(Syllabus.Name)]?.ToString()))
        {
            return null;
        }
        Syllabus newRecord = base.CreateNewRecord(objectSpace, entityRow);
        return newRecord;
    }

    protected override Syllabus MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, Syllabus entity, MapperHelper mapper)
    {

        foreach (string parent in parentProperty.OrderBy(x => x))
        {
            string newObjectName = childrenProperty.Where(x => x.PropertyType == parent).Select(x => x.Property).FirstOrDefault();
        }
        return entity;
    }
}
