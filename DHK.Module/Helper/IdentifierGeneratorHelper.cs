using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DHK.Module.Interfaces;
using System.Reflection;

namespace DHK.Module.Helper;

public static class IdentifierGeneratorHelper
{
    public static void GenerateIdentifier<T>(this T entity, string propertyName)
        where T : XPBaseObject, IGeneratedIdentifier
    {
        if (entity.Session is not NestedUnitOfWork &&
        entity.Session.DataLayer != null &&
        entity.Session.ObjectLayer is not SecuredSessionObjectLayer)
        {
            IObjectSpace currentObjectSpace = XPObjectSpace.FindObjectSpaceByObject(entity);
            //AutomationActionHelper.Trigger(entity, currentObjectSpace);
        }

        if (entity.Session is not NestedUnitOfWork &&
            entity.Session.DataLayer != null &&
            entity.Session.IsNewObject(entity) &&
            entity.Session.ObjectLayer is not SecuredSessionObjectLayer)
        {
            PropertyInfo property = entity.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                string currentValue = property.GetValue(entity) as string;
                if (string.IsNullOrEmpty(currentValue))
                {
                    OidGenerator oid = entity.Session.FindObject<OidGenerator>(
                        new BinaryOperator(nameof(OidGenerator.Type), entity.GetType().FullName));
                    string prefix = oid is not null && oid.Prefix is not null ? oid.Prefix : string.Empty;
                    int nextSequence = DistributedIdGeneratorHelper.Generate(
                        entity.Session.DataLayer,
                        entity.GetType().FullName,
                        prefix);
                    property.SetValue(entity, $"{prefix}{nextSequence:D10}");
                }
            }
        }
    }
}