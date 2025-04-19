using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.MultiTenancy;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DHK.Module.BusinessObjects;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using DHK.Module.Constants;
using DKH.Module.Constants;
using DHK.Module.Enumerations;
using Microsoft.Extensions.Configuration;
using DHK.Module.Converters;

namespace DHK.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {

    private BusinessObjects.Program FindProgram(string code) => ObjectSpace.FirstOrDefault<BusinessObjects.Program>(t => t.Code == code, true);
    private ImportMappingProperty FindImportMappingProperty(ImportMapping importMapping, string propertyName)
            => ObjectSpace.FirstOrDefault<ImportMappingProperty>(i => i.ImportMapping == importMapping
            && i.Property == propertyName, true);
    private ImportMapping FindImportMapping(string entity) => ObjectSpace.FirstOrDefault<ImportMapping>(i => i.Entity == entity, true);

    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        //string name = "MyName";
        //DomainObject1 theObject = ObjectSpace.FirstOrDefault<DomainObject1>(u => u.Name == name);
        //if(theObject == null) {
        //    theObject = ObjectSpace.CreateObject<DomainObject1>();
        //    theObject.Name = name;
        //}
        if (!ObjectSpace.CanInstantiate(typeof(ApplicationUser))) {
            return;
        }

#if !RELEASE
        //if (TenantName == null) {
        //    _ = CreateTenant("cict", "cict");
        //    ObjectSpace.CommitChanges();
        //}
#endif
        UserManager userManager = ObjectSpace.ServiceProvider.GetRequiredService<UserManager>();

        var adminRole = CreateAdminRole();

        string adminUserName = "Admin";
        if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, adminUserName) == null)
        {
            // Set a ***REMOVED*** if the standard authentication type is used
            string EmptyPassword = string.Empty;
            _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, adminUserName, EmptyPassword, (user) =>
            {
                // Add the Administrators role to the user
                user.Roles.Add(adminRole);
            });
        }

        // The code below creates users and roles for testing purposes only.
        // In production code, you can create users and assign roles to them automatically, as described in the following help topic:
        // https://docs.devexpress.com/eXpressAppFramework/119064/data-security-and-safety/security-system/authentication
#if !RELEASE

        if (TenantName != null) {
            var defaultRole = CreateDefaultRole();

            string userName = $"User";
            // If a user named 'userName' doesn't exist in the database, create this user
            if(userManager.FindUserByName<ApplicationUser>(ObjectSpace, userName) == null) {
                // Set a ***REMOVED*** if the standard authentication type is used
                string EmptyPassword = "";
                _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, userName, EmptyPassword, (user) => {
                    // Add the Users role to the user
                    user.Roles.Add(defaultRole);
                });
            }
            SeedTenantData();
            SeedUserAndRole();
        }

        ObjectSpace.CommitChanges(); //This line persists created object(s).
#endif
    }
    private PermissionPolicyRole CreateAdminRole() {
        PermissionPolicyRole adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
        if(adminRole == null) {
            adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            adminRole.Name = "Administrators";
            adminRole.IsAdministrative = true;
        }
        return adminRole;
    }
    private PermissionPolicyRole CreateDefaultRole() {
        PermissionPolicyRole defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Default");
        if(defaultRole == null) {
            defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            defaultRole.Name = "Default";

            defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
            defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            defaultRole.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            defaultRole.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            defaultRole.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.Read, SecurityPermissionState.Deny);
            defaultRole.AddObjectPermissionFromLambda<AuditDataItemPersistent>(SecurityOperations.Read, a => a.UserId == CurrentUserIdOperator.CurrentUserId().ToString(), SecurityPermissionState.Allow);
            defaultRole.AddTypePermission<AuditedObjectWeakReference>(SecurityOperations.Read, SecurityPermissionState.Allow);
        }
        return defaultRole;
    }
    string TenantName {
        get {
            return ObjectSpace.ServiceProvider.GetRequiredService<ITenantProvider>().TenantName;
        }
    }

    private void SeedTenantData()
    {
        // ************** //
        // SEED DATA
        // ************** //
        try
        {
            // Create OidGenerators
            //UpdateStatus("Programs", string.Empty, "Creating programs in the database...");
            //this.CreateProgram();

            // Create Import Mappings
            UpdateStatus("CreateImportMappings", string.Empty, "Creating default import mappings in the database...");
            this.CreateImportMappings();

            // Create Import Mapping Properties
            UpdateStatus("CreateImportMappingProperties", string.Empty, "Creating default import mapping properties in the database...");
            this.CreateImportMappingProperties();
        }
        catch (Exception e)
        {
            Tracing.Tracer.LogText("Cannot initialize default data from the XML files.");
            Tracing.Tracer.LogError(e);
        }
    }

    private void SeedUserAndRole()
    {
        UpdateStatus("CreateStudentRole", string.Empty, "Creating student role in the database...");
        Student student = FindUserByName<Student>(DefaultValues.STUDENT_USER_NAME);
        if (student == null)
        {
            student = ObjectSpace.CreateObject<Student>();
            student.UserName = DefaultValues.STUDENT_USER_NAME;
            student.FirstName = DefaultValues.STUDENT_FIRST_NAME;
            student.LastName = DefaultValues.STUDENT_LAST_NAME;
            student.SetPassword(string.Empty);

            ObjectSpace.CommitChanges();
            ((ISecurityUserWithLoginInfo)student).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(student));
        }
        PermissionPolicyRole studentRole = this.CreateStudentRole();
        student.Roles.Add(studentRole);

        UpdateStatus("CreateTeacherRole", string.Empty, "Creating teacher role in the database...");
        Teacher teacher = FindUserByName<Teacher>(DefaultValues.TEACHER_USER_NAME);
        if (teacher == null)
        {
            teacher = ObjectSpace.CreateObject<Teacher>();
            teacher.UserName = DefaultValues.TEACHER_USER_NAME;
            teacher.FirstName = DefaultValues.TEACHER_FIRST_NAME;
            teacher.LastName = DefaultValues.TEACHER_LAST_NAME;
            teacher.Status = EmploymentStatusType.FULLTIME;

            teacher.SetPassword(string.Empty);

            ObjectSpace.CommitChanges();
            ((ISecurityUserWithLoginInfo)teacher).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(teacher));
        }
        PermissionPolicyRole teacherRole = this.CreateTeacherRole();
        teacher.Roles.Add(teacherRole);
        var configuration = ObjectSpace.ServiceProvider.GetRequiredService<IConfiguration>();
        string environment = configuration["ASPNETCORE_ENVIRONMENT"];
        bool isLocalDeployment = ApplicationEnvironmentType.Development.ToString().Equals(environment, StringComparison.InvariantCultureIgnoreCase);
        var serviceUserName = configuration["Services:UserName"];
        var servicePassword = configuration["Services:Password"];
        Teacher servicesUser = FindUserByName<Teacher>("Services");
        if (servicesUser == null)
        {
            servicesUser = ObjectSpace.CreateObject<Teacher>();
            servicesUser.UserName = serviceUserName;
            servicesUser.FirstName = "Background";
            servicesUser.LastName = "Services";
            // Set a ***REMOVED*** if the standard authentication type is used
            servicesUser.SetPassword(servicePassword);

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)servicesUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(servicesUser));
        }
    }
    private void CreateProgram()
    {
        DataTable timeZonesTable = GetDataTable($"{nameof(BusinessObjects.Program)}.xml", nameof(Program));
        List<BusinessObjects.Program> result = [];
        foreach (DataRow timeZoneData in timeZonesTable.Rows)
        {
            string code = Convert.ToString(timeZoneData[nameof(BusinessObjects.Program.Code)]);
            string description = Convert.ToString(timeZoneData[nameof(BusinessObjects.Program.Description)]);
            string name = Convert.ToString(timeZoneData[nameof(BusinessObjects.Program.Name)]);
            BusinessObjects.Program program = FindProgram(code);
            if (program == null)
            {
                program = ObjectSpace.CreateObject<BusinessObjects.Program>();
                program.Code = code;
                program.Description = description;
                program.Name = name;
                result.Add(program);
            }
        }
    }

    private DataTable GetDataTable(string shortName, string tableName)
    {
        Stream stream = GetResourceByName(shortName);
        DataSet ds = new();
        ds.ReadXml(stream);
        return ds.Tables[tableName];
    }
    private Stream GetResourceByName(string shortName)
    {
        string embeddedResourceName = Array.Find<string>(this.GetType().Assembly.GetManifestResourceNames(), (s) => { return s.Contains(shortName); });
        Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedResourceName);
        if (stream == null)
        {
            throw new Exception(string.Format("Cannot read data from the {0} file!", shortName));
        }
        return stream;
    }
    private byte[] GetResourceRawData(string shortName)
    {
        Stream stream = GetResourceByName(shortName);
        return stream.GetBytes();
    }
    private T FindUserByName<T>(string userName)
        where T : ApplicationUser
    {
        UserManager userManager = ObjectSpace.ServiceProvider.GetRequiredService<UserManager>();
        return userManager.FindUserByName<T>(ObjectSpace, userName);
    }

    private PermissionPolicyRole CreateStudentRole()
    {
        PermissionPolicyRole studentRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == RoleNames.STUDENTS);
        if (studentRole == null)
        {
            studentRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            studentRole.Name = RoleNames.STUDENTS;

            //Navigation
            studentRole.AddNavigationPermission(@"Application/NavigationItems/Items/Document_ListView", SecurityPermissionState.Allow);

            studentRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.ReadWriteAccess, cm => cm.Oid != (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Deny);
            studentRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            studentRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            studentRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            studentRole.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            studentRole.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            studentRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            studentRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            studentRole.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.Read, SecurityPermissionState.Deny);
            studentRole.AddObjectPermissionFromLambda<AuditDataItemPersistent>(SecurityOperations.Read, a => a.UserId == CurrentUserIdOperator.CurrentUserId().ToString(), SecurityPermissionState.Allow);
            studentRole.AddTypePermission<AuditedObjectWeakReference>(SecurityOperations.Read, SecurityPermissionState.Allow);
            studentRole.AddTypePermission<Document>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            studentRole.AddTypePermission<Course>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            studentRole.AddTypePermission<Program>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            studentRole.AddTypePermission<FileData>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            studentRole.AddObjectPermissionFromLambda<Student>(SecurityOperations.ReadWriteAccess, cm => cm.Oid != (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Deny);


        }
        return studentRole;
    }

    private PermissionPolicyRole CreateTeacherRole()
    {
        PermissionPolicyRole teacherRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == RoleNames.TEACHERS);
        if (teacherRole == null)
        {
            teacherRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            teacherRole.Name = RoleNames.TEACHERS;

            //Navigation
            teacherRole.AddNavigationPermission(@"Application/NavigationItems/Items/Document_ListView", SecurityPermissionState.Allow);
            teacherRole.AddNavigationPermission(@"Application/NavigationItems/Items/Course_ListView", SecurityPermissionState.Allow);

            teacherRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            teacherRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            teacherRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            teacherRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            teacherRole.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            teacherRole.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            teacherRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            teacherRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            teacherRole.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.Read, SecurityPermissionState.Deny);
            teacherRole.AddObjectPermissionFromLambda<AuditDataItemPersistent>(SecurityOperations.Read, a => a.UserId == CurrentUserIdOperator.CurrentUserId().ToString(), SecurityPermissionState.Allow);
            teacherRole.AddTypePermission<AuditedObjectWeakReference>(SecurityOperations.Read, SecurityPermissionState.Allow);
            teacherRole.AddObjectPermissionFromLambda<Document>(SecurityOperations.ReadOnlyAccess, o => o.CreatedBy.Oid != (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Deny);
            teacherRole.AddTypePermission<Document>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
            teacherRole.AddTypePermission<Course>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            teacherRole.AddTypePermission<Program>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            teacherRole.AddTypePermission<FileData>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
            teacherRole.AddObjectPermissionFromLambda<Student>(SecurityOperations.ReadWriteAccess, cm => cm.Oid != (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Deny);
            teacherRole.AddObjectPermissionFromLambda<Section>(SecurityOperations.ReadWriteAccess, cm => cm.Teacher.Oid != (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Deny);
        }
        return teacherRole;
    }

    private void CreateImportMappings()
    {
        DataTable importMappingTable = GetDataTable($"{nameof(ImportMapping)}.xml", nameof(ImportMapping));
        List<ImportMapping> result = [];
        foreach (DataRow row in importMappingTable.Rows)
        {
            string entity = row.ConvertField<string>(nameof(ImportMapping.Entity));
            ImportMapping importMapping = FindImportMapping(entity);
            Type type = XafTypesInfo.Instance.FindTypeInfo(entity)?.Type;
            string name = XafTypesInfo.Instance.FindTypeInfo(entity)?.Name;
            if (importMapping==null)
            {
                importMapping = ObjectSpace.CreateObject<ImportMapping>();
                importMapping.EntityDataType = type;
                importMapping.Entity = entity;
                importMapping.Description = $"{name} Mapping";
                importMapping.Code = $"{name.ToUpper()}MAPPING";
                result.Add(importMapping);
            }
        }
        ObjectSpace.CommitChanges();
    }

    private void CreateImportMappingProperties()
    {
        try
        {
            DataTable importMappingPropertyTable = GetDataTable($"{nameof(ImportMappingProperty)}.xml", nameof(ImportMappingProperty));
            List<ImportMappingProperty> result = [];
            foreach (DataRow row in importMappingPropertyTable.Rows)
            {
                try
                {
                    string entity = row.ConvertField<string>(nameof(ImportMappingProperty.ImportMapping));
                    string property = row.ConvertField<string>(nameof(ImportMappingProperty.Property));
                    string propertyType = row.ConvertField<string>(nameof(ImportMappingProperty.PropertyType));
                    int sortOrder = row.ConvertField<int>(nameof(ImportMappingProperty.SortOrder));
                    string sampleValue = row.ConvertField<string>(nameof(ImportMappingProperty.SampleValue));

                    ImportMapping importMapping = FindImportMapping(entity);

                    if (importMapping == null)
                        continue;

                    ImportMappingProperty importMappingProperty = FindImportMappingProperty(importMapping, property);

                    if (importMappingProperty != null)
                        continue;

                    importMappingProperty = ObjectSpace.CreateObject<ImportMappingProperty>();
                    importMappingProperty.ImportMapping = importMapping;
                    importMappingProperty.Property = property;
                    importMappingProperty.PropertyType = propertyType;
                    importMappingProperty.SortOrder = sortOrder;
                    importMappingProperty.Required = false;
                    importMappingProperty.MapTo = property;
                    importMappingProperty.SampleValue = sampleValue;
                    result.Add(importMappingProperty);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            ObjectSpace.CommitChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
