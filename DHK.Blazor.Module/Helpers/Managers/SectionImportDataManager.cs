using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;
using DHK.Module.Helper;
using Hangfire.Server;
using System.Data;

namespace DHK.Blazor.Module.Helpers.Managers;

public class SectionImportDataManager : BaseImportDataManager<Section, FileImportJob>
{
    private int rowIndex = 0;
    private readonly List<string> parentProperty;
    private readonly ImportMapping importMapping;
    private readonly List<ImportMappingProperty> childrenProperty;


    public SectionImportDataManager(
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
              typeof(Section).FullName)).FirstOrDefault();
        childrenProperty = importMapping.Properties.Where(x => !string.IsNullOrEmpty(x.ChildrenProperty))
            .ToList();
        parentProperty = childrenProperty.Select(x => x.PropertyType).Distinct().ToList();
    }

    protected override Section GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow)
    {
        rowIndex += 1;
        Section Section = objectSpace.GetObjects<Section>(new BinaryOperator(nameof(Section.Code), entityRow[nameof(Section.Code)]?.ToString())).FirstOrDefault();
        if (Section == null)
        {
            return null;
        }
        return Section;
    }

    protected override Section CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow)
    {
        if (string.IsNullOrEmpty(entityRow[nameof(Section.Name)]?.ToString()))
        {
            return null;
        }
        Section newRecord = base.CreateNewRecord(objectSpace, entityRow);
        return newRecord;
    }

    protected override Section MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, Section entity, MapperHelper mapper)
    {

        foreach (string parent in parentProperty.OrderBy(x => x))
        {
            string newObjectName = childrenProperty.Where(x => x.PropertyType == parent).Select(x => x.Property).FirstOrDefault();
        }
        return entity;
    }
}
