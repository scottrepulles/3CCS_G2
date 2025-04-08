using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.BaseImpl;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.BusinessObjects;
using DHK.Module.Helper;
using DKH.Module.Constants;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Helpers.Managers
{
    public class TeacherImportDataManager : BaseImportDataManager<Teacher, FileImportJob>
    {
        private int rowIndex = 0;
        PermissionPolicyRole role = null;
        private readonly List<string> parentProperty;
        private readonly ImportMapping importMapping;
        private readonly List<ImportMappingProperty> childrenProperty;


        public TeacherImportDataManager(
            IServiceProvider serviceProvider,
            IObjectSpace objectSpace,
            PerformContext performContext,
            string backgroundJobId,
            string parentObjectOid,
            string parentObjectType,
            string mappingId
        ) : base(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId)
        {
            GetTeacherRole(objectSpace);
            importMapping = objectSpace.GetObjects<ImportMapping>(CriteriaOperator.Parse(
                  $"{nameof(ImportMapping.Entity)} = ? ",
                  typeof(Teacher).FullName)).FirstOrDefault();
            childrenProperty = importMapping.Properties.Where(x => !string.IsNullOrEmpty(x.ChildrenProperty))
                .ToList();
            parentProperty = childrenProperty.Select(x => x.PropertyType).Distinct().ToList();
        }

        protected override Teacher GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow)
        {
            rowIndex += 1;
            Teacher courier = objectSpace.GetObjects<Teacher>(new BinaryOperator(nameof(Teacher.Email), entityRow[nameof(Teacher.Email)]?.ToString())).FirstOrDefault();
            if (courier == null)
            {
                return null;
            }
            return courier;
        }

        protected override Teacher CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow)
        {
            if (string.IsNullOrEmpty(entityRow[nameof(Teacher.FirstName)]?.ToString()) ||
               string.IsNullOrEmpty(entityRow[nameof(Teacher.LastName)]?.ToString()))
            {
                return null;
            }

            Teacher newRecord = base.CreateNewRecord(objectSpace, entityRow);
            return newRecord;
        }

        // Helper method to assign roles and parent relationships
        private void AssignRolesAndParentRelationships(IObjectSpace objectSpace, Teacher teacher)
        {
            if (role != null)
                teacher.Roles.Add(role);
            ((ISecurityUserWithLoginInfo)teacher).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(teacher));
        }

        private PermissionPolicyRole GetTeacherRole(IObjectSpace objectSpace)
        {
            role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == RoleNames.TEACHERS);
            return role;
        }

        protected override Teacher MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, Teacher entity, MapperHelper mapper)
        {

            foreach (string parent in parentProperty.OrderBy(x => x))
            {
                string newObjectName = childrenProperty.Where(x => x.PropertyType == parent).Select(x => x.Property).FirstOrDefault();
            }
            return entity;
        }
    }
}
