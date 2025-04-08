using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DHK.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHK.Blazor.Module.Enumerations;

public enum ScheduleAnalyzerExportType
{
    [ImageName("ExportToXLSX")]
    [XafDisplayName(DisplayNames.EXCEL)]
    Excel,
    [ImageName("ExportToCsv")]
    [XafDisplayName(DisplayNames.CSV)]
    Csv
}
