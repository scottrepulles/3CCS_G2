using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;
using DHK.Module.Constants;
using DHK.Module.Helper;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Helpers.Managers;

public class CollegeImportDataManager : BaseImportDataManager<College, FileImportJob>
{
    private int rowIndex = 0;
    private readonly List<string> parentProperty;
    private readonly ImportMapping importMapping;
    private readonly List<ImportMappingProperty> childrenProperty;


    public CollegeImportDataManager(
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
              typeof(College).FullName)).FirstOrDefault();
        childrenProperty = importMapping.Properties.Where(x => !string.IsNullOrEmpty(x.ChildrenProperty))
            .ToList();
        parentProperty = childrenProperty.Select(x => x.PropertyType).Distinct().ToList();
    }

    protected override College GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow)
    {
        rowIndex += 1;
        College College = objectSpace.GetObjects<College>(new BinaryOperator(nameof(College.Code), entityRow[nameof(College.Code)]?.ToString())).FirstOrDefault();
        if (College == null)
        {
            return null;
        }
        return College;
    }

    protected override College CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow)
    {
        if (string.IsNullOrEmpty(entityRow[nameof(College.Code)]?.ToString()) ||
           string.IsNullOrEmpty(entityRow[nameof(College.Name)]?.ToString()))
        {
            return null;
        }
        College newRecord = base.CreateNewRecord(objectSpace, entityRow);
        return newRecord;
    }

    protected override College MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, College entity, MapperHelper mapper)
    {

        foreach (string parent in parentProperty.OrderBy(x => x))
        {
            string newObjectName = childrenProperty.Where(x => x.PropertyType == parent).Select(x => x.Property).FirstOrDefault();
        }
        return entity;
    }
}
