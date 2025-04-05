using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.MultiTenancy;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.MultiTenancy;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DHK.Module.BusinessObjects;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System;
using DHK.Module.Constants;
using DKH.Module.Constants;

namespace DHK.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {

    private Program FindProgram(string code) => ObjectSpace.FirstOrDefault<Program>(t => t.Code == code, true);


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
            UpdateStatus("Programs", string.Empty, "Creating programs in the database...");
            this.CreateProgram();
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
            teacher.SetPassword(string.Empty);

            ObjectSpace.CommitChanges();
            ((ISecurityUserWithLoginInfo)teacher).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(teacher));
        }
        PermissionPolicyRole teacherRole = this.CreateTeacherRole();
        teacher.Roles.Add(teacherRole);
    }
    private void CreateProgram()
    {
        DataTable timeZonesTable = GetDataTable($"{nameof(Program)}.xml", nameof(Program));
        List<Program> result = [];
        foreach (DataRow timeZoneData in timeZonesTable.Rows)
        {
            string code = Convert.ToString(timeZoneData[nameof(Program.Code)]);
            string description = Convert.ToString(timeZoneData[nameof(Program.Description)]);
            string name = Convert.ToString(timeZoneData[nameof(Program.Name)]);
            Program program = FindProgram(code);
            if (program == null)
            {
                program = ObjectSpace.CreateObject<Program>();
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

            studentRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
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

            teacherRole.AddTypePermission<Document>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
        }
        return teacherRole;
    }
}
