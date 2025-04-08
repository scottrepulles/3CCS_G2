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
    public class StudentImportDataManager : BaseImportDataManager<Student, FileImportJob>
    {
        private int rowIndex = 0;
        PermissionPolicyRole role = null;
        private readonly List<string> parentProperty;
        private readonly ImportMapping importMapping;
        private readonly List<ImportMappingProperty> childrenProperty;


        public StudentImportDataManager(
            IServiceProvider serviceProvider,
            IObjectSpace objectSpace,
            PerformContext performContext,
            string backgroundJobId,
            string parentObjectOid,
            string parentObjectType,
            string mappingId
        ) : base(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId)
        {
            GetStudentRole(objectSpace);
            importMapping = objectSpace.GetObjects<ImportMapping>(CriteriaOperator.Parse(
                  $"{nameof(ImportMapping.Entity)} = ? ",
                  typeof(Student).FullName)).FirstOrDefault();
            childrenProperty = importMapping.Properties.Where(x => !string.IsNullOrEmpty(x.ChildrenProperty))
                .ToList();
            parentProperty = childrenProperty.Select(x => x.PropertyType).Distinct().ToList();
        }

        protected override Student GetMatchFromDb(IObjectSpace objectSpace, DataRow entityRow)
        {
            rowIndex += 1;
            Student student = objectSpace.GetObjects<Student>(new BinaryOperator(nameof(Student.Email), entityRow[nameof(Student.Email)]?.ToString())).FirstOrDefault();
            if (student == null)
            {
                return null;
            }
            return student;
        }

        protected override Student CreateNewRecord(IObjectSpace objectSpace, DataRow entityRow)
        {
            if (string.IsNullOrEmpty(entityRow[nameof(Student.FirstName)]?.ToString()) ||
               string.IsNullOrEmpty(entityRow[nameof(Student.LastName)]?.ToString()))
            {
                return null;
            }

            Student newRecord = base.CreateNewRecord(objectSpace, entityRow);
            return newRecord;
        }

        // Helper method to assign roles and parent relationships
        private void AssignRolesAndParentRelationships(IObjectSpace objectSpace, Student Student)
        {
            if (role != null)
                Student.Roles.Add(role);
            ((ISecurityUserWithLoginInfo)Student).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(Student));
        }

        private PermissionPolicyRole GetStudentRole(IObjectSpace objectSpace)
        {
            role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == RoleNames.STUDENTS);
            return role;
        }

        protected override Student MatchSubProperty(IObjectSpace objectSpace, DataRow entityRow, Student entity, MapperHelper mapper)
        {

            foreach (string parent in parentProperty.OrderBy(x => x))
            {
                string newObjectName = childrenProperty.Where(x => x.PropertyType == parent).Select(x => x.Property).FirstOrDefault();
            }
            return entity;
        }
    }
}
