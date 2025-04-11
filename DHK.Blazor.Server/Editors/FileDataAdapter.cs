﻿using DevExpress.ExpressApp.Blazor.Components;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.BaseImpl;
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
        public override void SetValue(object value) => ComponentModel.Value = (value as FileData)?.ToString() ?? string.Empty;

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
    }
}
