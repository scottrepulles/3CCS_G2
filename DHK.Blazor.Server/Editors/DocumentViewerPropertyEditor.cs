using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraRichEdit;

namespace DHK.Blazor.Server.Editors
{
    [PropertyEditor(typeof(IFileData), "DocumentViewEditor", false)]
    public class DocumentViewerPropertyEditor(Type objectType, IModelMemberViewItem info) : BlazorPropertyEditorBase(objectType, info), IComplexViewItem
    {
        private XafApplication application;
        protected override IComponentAdapter CreateComponentAdapter() => new FileDataAdapter(new FileDataModel());

        protected override void ReadValueCore()
        {
            try
            {
                IFileData file = PropertyValue as IFileData;
                if (ComponentAdapter is FileDataAdapter adapter && file != null)
                {
                    adapter.ComponentModel.Value = GetBase64String(file);
                }
            }
            catch (Exception ex)
            {
                Tracing.Tracer.LogError(ex);
                application.ShowViewStrategy.ShowMessage(ex.Message, InformationType.Error);
            }
            base.ReadValueCore();
        }
        protected string GetBase64String(IFileData file)
        {
            string pdfContent = string.Empty;

            if (file != null && !string.IsNullOrEmpty(file.FileName))
            {
                using (MemoryStream fileStream = new MemoryStream())
                {
                    file.SaveToStream(fileStream);
                    fileStream.Position = 0;

                    string filename = file.FileName;
                    string fileExtension = System.IO.Path.GetExtension(filename ?? "").ToLower();
                    if (!string.IsNullOrEmpty(fileExtension))
                    {
                        if (new[] { ".doc", ".docx", ".rtf", ".xml", ".txt" }.Contains(fileExtension))
                        {
                            using (RichEditDocumentServer server = new RichEditDocumentServer())
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    server.LoadDocument(fileStream, DocumentFormat.OpenXml);
                                    server.ExportToPdf(stream);
                                    stream.Position = 0;

                                    string mimeType = "application/pdf";
                                    pdfContent = string.Format("data:{0};base64,", mimeType);
                                    pdfContent += System.Convert.ToBase64String(stream.ToArray());

                                    stream.Dispose();
                                }
                            }
                        }
                        else if (new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(fileExtension))
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                file.SaveToStream(stream);
                                stream.Position = 0;

                                string mimeType = $"image/{fileExtension.TrimStart('.')}";
                                pdfContent = $"data:{mimeType};base64," + Convert.ToBase64String(stream.ToArray());
                            }
                        }
                        else
                        {
                            string mimeType = $"image/{fileExtension.TrimStart('.')}";
                            pdfContent = string.Format("data:{0};base64,", mimeType);
                            pdfContent += System.Convert.ToBase64String(fileStream.ToArray());
                        }
                    }
                }
            }
            return pdfContent;
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            Console.WriteLine("DocumentViewerPropertyEditor.Setup");
        }
    }
}
