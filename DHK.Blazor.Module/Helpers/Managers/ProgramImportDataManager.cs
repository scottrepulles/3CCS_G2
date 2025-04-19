using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;
using DHK.Module.Helper;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Helpers.Managers;
public class ProgramImportDataManager : BaseImportDataManager<Program, FileImportJob>
{
    private int rowIndex = 0;
    private readonly List<string> parentProperty;
    private readonly ImportMapping importMapping;
    private readonly List<ImportMappingProperty> childrenProperty;


    public ProgramImportDataManager(
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
              typeof(Program).FullName)).FirstOrDefault();
        childrenProperty = importMapping.Properties.Where(x => !string.IsNullOrEmpty(x.ChildrenProperty))
            .ToList();
        parentProperty = childrenProperty.Select(x => x.PropertyType).Distinct().ToList();
    }

    protected override Program GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow)
    {
        rowIndex += 1;
        Program Program = objectSpace.GetObjects<Program>(new BinaryOperator(nameof(Program.Code), entityRow[nameof(Program.Code)]?.ToString())).FirstOrDefault();
        if (Program == null)
        {
            return null;
        }
        return Program;
    }

    protected override Program CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow)
    {
        if (string.IsNullOrEmpty(entityRow[nameof(Program.Code)]?.ToString()) ||
           string.IsNullOrEmpty(entityRow[nameof(Program.Name)]?.ToString()))
        {
            return null;
        }
        Program newRecord = base.CreateNewRecord(objectSpace, entityRow);
        return newRecord;
    }

    protected override Program MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, Program entity, MapperHelper mapper)
    {

        foreach (string parent in parentProperty.OrderBy(x => x))
        {
            string newObjectName = childrenProperty.Where(x => x.PropertyType == parent).Select(x => x.Property).FirstOrDefault();
        }
        return entity;
    }
}
