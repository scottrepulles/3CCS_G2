using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DHK.Blazor.Module.Helpers.Globals;
using DHK.Blazor.Module.Model;
using DHK.Module.BusinessObjects;
using DHK.Module.Interfaces;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Processes
{
    public class ImportFromFileProcess<T, D> : BaseHangfireProcess
    where T : BaseObject
    where D : IHangfireJobData
    {
        [ActivatorUtilitiesConstructor]
        public ImportFromFileProcess(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory) { }

        public override void Execute(PerformContext performContext, string parameter)
        {
            string jobId = performContext.BackgroundJob.Id;

            performContext.WriteLine("Job Id={0}", jobId);

            RunActionUsing((serviceProvider, objectSpace) =>
            {
                BaseHangfireJobParameterModel param = JsonSerializer.Deserialize<BaseHangfireJobParameterModel>(parameter);
                Session session = ((XPObjectSpace)objectSpace).Session;
                ImportMapping importMapping=null;
                if (param.Mapping == null)
                {
                    //Company company = objectSpace.GetObjects<Company>().FirstOrDefault();
                    //string propertyName = $"{typeof(T).Name}Mapping";
                    //PropertyInfo propertyInfo = typeof(Company).GetProperty(propertyName);
                    //object map = propertyInfo?.GetValue(company);
                    //importMapping = map == null ? null : (ImportMapping)map;
                }
                else
                {
                    ImportMapping map = objectSpace.GetObjects<ImportMapping>(CriteriaOperator.Parse(
                        $"Oid = '{param.Mapping}'")).FirstOrDefault();
                    importMapping = map;
                }

                if (importMapping != null)
                {
                    Type key = typeof(T);
                    if (FileImportHelper.Managers.TryGetValue(key, out Func<IServiceProvider, DevExpress.ExpressApp.IObjectSpace, PerformContext, string, string, string, string, DHK.Module.Interfaces.Globals.IImportDataManager> value))
                    {
                        value?.Invoke(serviceProvider, objectSpace, performContext, jobId, param.ParentObjectOid, param.ParentObjecType, importMapping.Oid.ToString())
                            ?.ImportData();
                        objectSpace.CommitChanges();
                    }
                }
            });
        }
    }
}
