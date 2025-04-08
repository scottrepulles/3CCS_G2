using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;
using DHK.Module.Helper;
using DKH.Module.Constants;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Helpers.Managers
{
    public class CourseImportDataManager : BaseImportDataManager<Course, FileImportJob>
    {
        private int rowIndex = 0;
        PermissionPolicyRole role = null;
        private readonly List<string> parentProperty;
        private readonly ImportMapping importMapping;
        private readonly List<ImportMappingProperty> childrenProperty;


        public CourseImportDataManager(
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
                  typeof(Course).FullName)).FirstOrDefault();
            childrenProperty = importMapping.Properties.Where(x => !string.IsNullOrEmpty(x.ChildrenProperty))
                .ToList();
            parentProperty = childrenProperty.Select(x => x.PropertyType).Distinct().ToList();
        }

        protected override Course GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow)
        {
            rowIndex += 1;
            Course Course = objectSpace.GetObjects<Course>(new BinaryOperator(nameof(Course.Code), entityRow[nameof(Course.Code)]?.ToString())).FirstOrDefault();
            if (Course == null)
            {
                return null;
            }
            return Course;
        }

        protected override Course CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow)
        {
            if (string.IsNullOrEmpty(entityRow[nameof(Course.Code)]?.ToString()))
            {
                return null;
            }

            Course newRecord = base.CreateNewRecord(objectSpace, entityRow);
            return newRecord;
        }

        protected override Course MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, Course entity, MapperHelper mapper)
        {

            foreach (string parent in parentProperty.OrderBy(x => x))
            {
                string newObjectName = childrenProperty.Where(x => x.PropertyType == parent).Select(x => x.Property).FirstOrDefault();
            }
            return entity;
        }
    }
}
