using Hangfire;
using DHK.Blazor.Module.Interfaces;
using DHK.Blazor.Module.Model;
using System.Text.Json;
using DHK.Blazor.Module.BusinessObjects.Globals;
using DHK.Blazor.Module.BusinessObjects.Schedules;
using DHK.Module.Interfaces;
using DHK.Blazor.Module.Jobs;
using DHK.Module.BusinessObjects;
using DHK.Blazor.Module.Processes;

namespace DHK.Blazor.Module.Helpers.Globals;

public static class JobProcessHelper
{
    private static bool StartProcess<T, P>(IHangfireJobData job)
        where T : BaseHangfireJob
        where P : IHangfireProcess
    {
        T jobData = job as T;
        if (jobData is not null)
        {
            string parentObjecType = jobData.GetType()?.GetProperty("ParentObjectType")?.GetValue(jobData, null)?.ToString();
            string parentObjectOid = jobData.GetType()?.GetProperty("ParentObjectOid")?.GetValue(jobData, null)?.ToString();
            string mappingId = jobData.GetType()?.GetProperty("ImportMappingOid")?.GetValue(jobData, null)?.ToString();

            BaseHangfireJobParameterModel parameter = new BaseHangfireJobParameterModel { ParentObjectOid = parentObjectOid, ParentObjecType = parentObjecType, Mapping = mappingId };
            string jsonParam = JsonSerializer.Serialize(parameter);
            job.BackgroundJobId = BackgroundJob.Enqueue<P>(p => p.Execute(null, jsonParam));
            return job.BackgroundJobId != null;
        }
        return false;
    }

    private static List<Func<IHangfireJobData, bool>> parsers =
    [
        (job) => StartProcess<TeacherImportJob, ImportFromFileProcess<Teacher, TeacherImportJob>>(job),
        (job) => StartProcess<StudentImportJob, ImportFromFileProcess<Student, StudentImportJob>>(job),
        (job) => StartProcess<CourseImportJob, ImportFromFileProcess<Course, CourseImportJob>>(job),
        (job) => StartProcess<CollegeImportJob, ImportFromFileProcess<College, CollegeImportJob>>(job),
        (job) => StartProcess<ProgramImportJob, ImportFromFileProcess<Program, ProgramImportJob>>(job),
        (job) => StartProcess<SectionImportJob, ImportFromFileProcess<Section, SectionImportJob>>(job),
        (job) => StartProcess<SyllabusImportJob, ImportFromFileProcess<Syllabus, SyllabusImportJob>>(job),
        (job) => StartProcess<EnrollmentImportJob, ImportFromFileProcess<Enrollment, EnrollmentImportJob>>(job),
    ];

    public static void ExecuteOnceFor(IHangfireJobData job)
    {
        foreach (Func<IHangfireJobData, bool> parser in parsers)
        {
            if (parser.Invoke(job)) break;
        }
    }

    private static bool StartRecurringProcess<T, P>(RecurringHangfireJob job)
        where T : RecurringHangfireJob
        where P : IHangfireProcess
    {
        if (job is T jobData)
        {
            var jobId = jobData.GenerateJobId();
            var cron = jobData.GetCron();

            jobData.RecurringJobId = jobId;

            if (!string.IsNullOrEmpty(cron))
            {
                RecurringJob.AddOrUpdate<P>(
                    jobId,
                    p => p.Execute(null, jobId),
                    cron
                );
            }
            else
            {
                RecurringJob.RemoveIfExists(jobId);
            }
            return true;
        }

        return false;
    }

    public static void ExecuteRecurringFor(RecurringHangfireJob job)
    {
        foreach (Func<RecurringHangfireJob, bool> parser in recurringParsers)
        {
            if (parser.Invoke(job)) break;
        }
    }
    private static List<Func<RecurringHangfireJob, bool>> recurringParsers = new List<Func<RecurringHangfireJob, bool>>()
    {
    };
}
