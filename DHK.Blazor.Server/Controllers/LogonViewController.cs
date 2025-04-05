using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DHK.Module.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DHK.Blazor.Server.Controllers
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class LogonViewController : ObjectViewController<DetailView, CustomLogonParametersForStandardAuthenticationModel>
    {
        public LogonViewController()
        {
            InitializeComponent();
        }
        protected override void OnViewControlsCreated()
        {
            //SetInputNullText(nameof(MultiTenantLogonParametersModel.Company), "Company");
            SetInputNullText(nameof(PermissionPolicyUser.UserName), "User name or email");
            SetInputNullText("Password", "Password");
            base.OnViewControlsCreated();
        }

        private void SetInputNullText(string propertyName, string hintText)
        {
            StringPropertyEditor stringPropertyEditor = (StringPropertyEditor)View.FindItem(propertyName);
            if (stringPropertyEditor.Control is DxTextBoxAdapter dxTextBoxAdapter)
            {
                dxTextBoxAdapter.SetNullText(hintText);
            }
        }
    }
}
