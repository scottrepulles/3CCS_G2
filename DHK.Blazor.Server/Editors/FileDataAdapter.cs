using Aspose.Slides.Export;
using Aspose.Slides;
using DevExpress.ExpressApp.Blazor.Components;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraRichEdit;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace DHK.Blazor.Server.Editors
{
    public class FileDataAdapter : ComponentAdapterBase
    {
        public FileDataAdapter(FileDataModel componentModel)
        {
            ComponentModel = componentModel ?? throw new ArgumentNullException(nameof(componentModel));
            ComponentModel.ValueChanged += ComponentModel_ValueChanged;
        }
        public override FileDataModel ComponentModel { get; }

        public override void SetAllowEdit(bool allowEdit)
        {
            ComponentModel.ReadOnly = !allowEdit;
        }
        public override object GetValue()
        {
            return ComponentModel.Value;
        }

        public override void SetValue(object value)
        {
            if (value is FileData data)
            {
                ComponentModel.Value = GetBase64String(data);
            }
            else if (value is string strValue)
            {
                ComponentModel.SetValueFromUI(strValue);
            }
            else
            {
                ComponentModel.SetValueFromUI(string.Empty);
                ComponentModel.Value = value == null ? string.Empty : value.ToString();
            }
        }

        protected override RenderFragment CreateComponent()
        {
            return ComponentModelObserver.Create(ComponentModel, FileDataRenderer.Create(ComponentModel));
        }
        private void ComponentModel_ValueChanged(object sender, EventArgs e) => RaiseValueChanged();
        public override void SetAllowNull(bool allowNull) { /* ...*/ }
        public override void SetDisplayFormat(string displayFormat) { /* ...*/ }
        public override void SetEditMask(string editMask) { /* ...*/ }
        public override void SetEditMaskType(EditMaskType editMaskType) { /* ...*/ }
        public override void SetErrorMessage(string errorMessage) { /* ...*/ }
        public override void SetIsPassword(bool isPassword) { /* ...*/ }
        public override void SetMaxLength(int maxLength) { /* ...*/ }
        public override void SetNullText(string nullText) { /* ...*/ }
        public override void SetErrorIcon(ImageInfo nullText) { /* ...*/ }

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
                    fileStream.Position = 0;
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
