using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Hangfire.Server;
using DHK.Module.Interfaces.Globals;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Module.Interfaces;
using DHK.Module.BusinessObjects;
using DHK.Blazor.Module.Helpers.Managers;
using DHK.Blazor.Module.Jobs;

namespace DHK.Blazor.Module.Helpers.Globals;

public static class FileImportHelper
{
    public static Dictionary<Type, Func<IServiceProvider, IObjectSpace, PerformContext, string, string, string, string, IImportDataManager>> Managers = new Dictionary<Type, Func<IServiceProvider, IObjectSpace, PerformContext, string, string, string, string, IImportDataManager>>()
    {
        { typeof(Teacher), (serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) => new TeacherImportDataManager(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) },
        { typeof(Student), (serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) => new StudentImportDataManager(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) },
        { typeof(Course), (serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) => new CourseImportDataManager(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) },
        { typeof(College), (serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) => new CollegeImportDataManager(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) },
        { typeof(Program), (serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) => new ProgramImportDataManager(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) },
        { typeof(Section), (serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) => new SectionImportDataManager(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) },
        { typeof(Syllabus), (serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) => new SyllabusImportDataManager(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) },
        { typeof(Enrollment), (serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) => new EnrollmentImportDataManager(serviceProvider, objectSpace, performContext, backgroundJobId, parentObjectOid, parentObjectType, mappingId) },
    };

    private static Dictionary<Type, Action<XafApplication, IObjectSpace, CustomizePopupWindowParamsEventArgs, object>> DetailViewCustomizers = new Dictionary<Type, Action<XafApplication, IObjectSpace, CustomizePopupWindowParamsEventArgs, object>>()
    {
        { typeof(Teacher), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<TeacherImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Student), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<StudentImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Course), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<CourseImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(College), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<CollegeImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Program), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<ProgramImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Section), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<SectionImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Syllabus), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<SyllabusImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Enrollment), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<EnrollmentImportJob>(application, objectSpace, popupWindowParams, currentObject) },
    };

    private static Dictionary<Type, Action<XafApplication, IObjectSpace, SingleChoiceActionExecuteEventArgs, object>> ListViewCustomizers = new Dictionary<Type, Action<XafApplication, IObjectSpace, SingleChoiceActionExecuteEventArgs, object>>()
    {
        { typeof(Teacher), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<TeacherImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Student), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<StudentImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Course), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<CourseImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(College), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<CollegeImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Program), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<ProgramImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Section), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<SectionImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Syllabus), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<SyllabusImportJob>(application, objectSpace, popupWindowParams, currentObject) },
        { typeof(Enrollment), (application, objectSpace, popupWindowParams, currentObject) => CreateCustomPopupWindow<EnrollmentImportJob>(application, objectSpace, popupWindowParams, currentObject) },
    };

    private static Dictionary<Type, Action<XafApplication, SingleChoiceActionExecuteEventArgs>> AuditLogListViewCustomizers = new Dictionary<Type, Action<XafApplication, SingleChoiceActionExecuteEventArgs>>()
    {
        { typeof(Teacher), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<TeacherImportJob>(application, popupWindowParams) },
        { typeof(Student), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<StudentImportJob>(application, popupWindowParams) },
        { typeof(Course), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<CourseImportJob>(application, popupWindowParams) },
        { typeof(College), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<CollegeImportJob>(application, popupWindowParams) },
        { typeof(Program), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<ProgramImportJob>(application, popupWindowParams) },
        { typeof(Section), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<SectionImportJob>(application, popupWindowParams) },
        { typeof(Syllabus), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<SyllabusImportJob>(application, popupWindowParams) },
        { typeof(Enrollment), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<EnrollmentImportJob>(application, popupWindowParams) },
    };

    private static Dictionary<Type, Action<XafApplication, CustomizePopupWindowParamsEventArgs>> AuditLogDetailViewCustomizers = new Dictionary<Type, Action<XafApplication, CustomizePopupWindowParamsEventArgs>>()
    {
        { typeof(Teacher), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<TeacherImportJob>(application, popupWindowParams) },
        { typeof(Student), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<StudentImportJob>(application, popupWindowParams) },
        { typeof(Course), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<CourseImportJob>(application, popupWindowParams) },
        { typeof(College), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<CollegeImportJob>(application, popupWindowParams) },
        { typeof(Program), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<ProgramImportJob>(application, popupWindowParams) },
        { typeof(Section), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<SectionImportJob>(application, popupWindowParams) },
        { typeof(Syllabus), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<SyllabusImportJob>(application, popupWindowParams) },
        { typeof(Enrollment), (application, popupWindowParams) => CreateCustomPopupAuditLogsWindow<EnrollmentImportJob>(application, popupWindowParams) },
    };

    private static void CreateCustomPopupWindow<J>(
        XafApplication application,
        IObjectSpace objectSpace,
        CustomizePopupWindowParamsEventArgs e,
        object parentObject
    )
        where J : FileImportJob
    {
        J hangfireJob = objectSpace.CreateObject<J>();
        if (parentObject != null)
        {
            Session session = ((XPObjectSpace)objectSpace).Session;
            XPBaseObject source = (XPBaseObject)parentObject;
            string parentObjectOid = source.ClassInfo.KeyProperty.GetValue(source)?.ToString();
            hangfireJob.GetType()?.GetProperty("ParentObjectOid")?.SetValue(hangfireJob, parentObjectOid);
            hangfireJob.GetType()?.GetProperty("ParentObjectType")?.SetValue(hangfireJob, parentObject.GetType().FullName);
        }
        DetailView detailView = application.CreateDetailView(objectSpace, hangfireJob);
        if (detailView is not null)
        {
            e.DialogController.SaveOnAccept = true;
            detailView.Caption = @"Import";
            detailView.ViewEditMode = ViewEditMode.Edit;

            // Get the file property control (replace "YourFileProperty" with the actual property name)
            PropertyEditor filePropertyControl = detailView.FindItem("File") as PropertyEditor;

            if (filePropertyControl != null)
            {
                e.DialogController.AcceptAction.Enabled.SetItemValue("OK", false);
                // Handle the ControlValueChanged event
                filePropertyControl.ControlValueChanged += (sender, args) =>
                {
                    object filePropertyValue = filePropertyControl.PropertyValue;
                    e.DialogController.AcceptAction.Enabled.SetItemValue("OK", true);
                };
            }

            e.View = detailView;

        }
    }
    private static void CreateCustomPopupWindow<J>(
        XafApplication application,
        IObjectSpace objectSpace,
        SingleChoiceActionExecuteEventArgs e,
        object parentObject
    )
        where J : FileImportJob
    {
        J hangfireJob = objectSpace.CreateObject<J>();
        if (parentObject != null)
        {
            Session session = ((XPObjectSpace)objectSpace).Session;
            XPBaseObject source = (XPBaseObject)parentObject;
            string parentObjectOid = source.ClassInfo.KeyProperty.GetValue(source)?.ToString();
            hangfireJob.GetType()?.GetProperty("ParentObjectOid")?.SetValue(hangfireJob, parentObjectOid);
            hangfireJob.GetType()?.GetProperty("ParentObjectType")?.SetValue(hangfireJob, parentObject.GetType().FullName);
        }
        DetailView detailView = application.CreateDetailView(objectSpace, hangfireJob);
        if (detailView is not null)
        {
            DialogController dialog = application.CreateController<DialogController>();
            dialog.SaveOnAccept = true;
            detailView.Caption = @"Import";
            detailView.ViewEditMode = ViewEditMode.Edit;
            // Get the file property control (replace "YourFileProperty" with the actual property name)
            PropertyEditor filePropertyControl = detailView.FindItem("File") as PropertyEditor;

            if (filePropertyControl != null)
            {
                dialog.AcceptAction.Enabled.SetItemValue("OK", false);
                // Handle the ControlValueChanged event
                filePropertyControl.ControlValueChanged += (sender, args) =>
                {
                    object filePropertyValue = filePropertyControl.PropertyValue;
                    dialog.AcceptAction.Enabled.SetItemValue("OK", true);
                };
            }
            e.ShowViewParameters.Controllers.Add(dialog);
            e.ShowViewParameters.CreatedView = detailView;
        }
    }
    private static void CreateCustomPopupAuditLogsWindow<J>(
        XafApplication application,
        SingleChoiceActionExecuteEventArgs e
    )
        where J : IHangfireJobData
    {
        ListView lv = application.CreateListView(application.CreateObjectSpace(typeof(J)), typeof(J), true);
        lv.Caption = "Import Logs";
        e.ShowViewParameters.CreatedView = lv;
    }
    private static void CreateCustomPopupAuditLogsWindow<J>(
        XafApplication application,
        CustomizePopupWindowParamsEventArgs e
    )
        where J : IHangfireJobData
    {
        ListView lv = application.CreateListView(application.CreateObjectSpace(typeof(J)), typeof(J), true);
        lv.Caption = "Import Logs";
        e.View = lv;
    }

    public static void ShowPopupListlView(XafApplication application, IObjectSpace objectSpace, SingleChoiceActionExecuteEventArgs e, Type type, object CurrentObject)
    {
        if (ListViewCustomizers.ContainsKey(type))
        {
            ListViewCustomizers[type]?.Invoke(application, objectSpace, e, CurrentObject);
        }
    }

    public static void ShowPopupDetailView(XafApplication application, IObjectSpace objectSpace, CustomizePopupWindowParamsEventArgs e, Type type, object CurrentObject)
    {
        if (DetailViewCustomizers.ContainsKey(type))
        {
            DetailViewCustomizers[type]?.Invoke(application, objectSpace, e, CurrentObject);
        }
    }

    public static void ShowPopupAuditLogs(XafApplication application, SingleChoiceActionExecuteEventArgs e, Type type)
    {
        if (AuditLogListViewCustomizers.ContainsKey(type))
        {
            AuditLogListViewCustomizers[type]?.Invoke(application, e);
        }
    }
    public static void ShowPopupAuditLogsDetail(XafApplication application, CustomizePopupWindowParamsEventArgs e, Type type)
    {
        if (AuditLogDetailViewCustomizers.ContainsKey(type))
        {
            AuditLogDetailViewCustomizers[type]?.Invoke(application, e);
        }
    }
}
