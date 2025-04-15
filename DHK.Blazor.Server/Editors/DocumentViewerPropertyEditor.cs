using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.XtraRichEdit;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Slides;
using Aspose.Slides.Export;


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
            if (file == null || string.IsNullOrEmpty(file.FileName))
                return string.Empty;

            string fileExtension = Path.GetExtension(file.FileName)?.ToLower();
            if (string.IsNullOrEmpty(fileExtension))
                return string.Empty;

            string mimeType;
            byte[] fileBytes;

            using (var fileStream = new MemoryStream())
            {
                file.SaveToStream(fileStream);
                fileStream.Position = 0;

                if (new[] { ".doc", ".docx", ".rtf", ".xml", ".txt" }.Contains(fileExtension))
                {
                    using (var server = new RichEditDocumentServer())
                    using (var pdfStream = new MemoryStream())
                    {
                        server.LoadDocument(fileStream, DocumentFormat.Undefined);
                        server.ExportToPdf(pdfStream);
                        fileBytes = pdfStream.ToArray();
                        mimeType = "application/pdf";
                    }
                }
                else if (fileExtension == ".pptx")
                {
                    fileStream.Position = 0; // Just to be safe
                    using Presentation presentation = new();
                    using var pdfStream = new MemoryStream();
                    presentation.Save(pdfStream, Aspose.Slides.Export.SaveFormat.Pdf);
                    fileBytes = pdfStream.ToArray();
                    mimeType = "application/pdf";
                }
                else
                {
                    fileBytes = fileStream.ToArray();
                    mimeType = GetMimeType(file.FileName);
                }
            }

            return $"data:{mimeType};base64,{Convert.ToBase64String(fileBytes)}";
        }




        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            Console.WriteLine("DocumentViewerPropertyEditor.Setup");
        }

        private byte[] compressimagesize(double scaleFactor, Stream sourcePath)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            using (var image = System.Drawing.Image.FromStream(sourcePath))
            {
                var imgnewwidth = (int)(image.Width * scaleFactor);
                var imgnewheight = (int)(image.Height * scaleFactor);
                var imgthumbnail = new Bitmap(imgnewwidth, imgnewheight);
                var imgthumbgraph = Graphics.FromImage(imgthumbnail);
                imgthumbgraph.CompositingQuality = CompositingQuality.HighQuality;
                imgthumbgraph.SmoothingMode = SmoothingMode.HighQuality;
                imgthumbgraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, imgnewwidth, imgnewheight);
                var stm = new MemoryStream();
                imgthumbgraph.DrawImage(image, imageRectangle);
                imgthumbnail.Save(stm, image.RawFormat);
#pragma warning restore CA1416 // Validate platform compatibility
                return stm.ToArray();
            }
        }

        private string GetMimeType(string fileName)
        {
            string ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".bmp" => "image/bmp",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                ".xml" => "application/xml",
                ".rtf" => "application/rtf",
                _ => "application/octet-stream",
            };
        }
    }
}
