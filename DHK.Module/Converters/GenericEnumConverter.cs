using DevExpress.Xpo.Metadata;

namespace DHK.Module.Converters;

public class GenericEnumConverter<T> : ValueConverter
    where T : Enum
{
    public override Type StorageType
        => typeof(string);

    public override object ConvertFromStorageType(object value)
    {
        if (value == null) { return null; }
        return (T)Enum.Parse(typeof(T), (string)value);
    }

    public override object ConvertToStorageType(object value)
    {
        if (value == null) return null;

        if (value is T enumValue)
        {
            return enumValue.ToString();
        }

        return null;
    }
}
