using HtmlAgilityPack;
using Limilabs.Mail;
using Limilabs.Mail.MIME;
using System.Data;
using System.Text.RegularExpressions;

namespace DHK.Module.Converters
{
    public static class DataConverter
    {
        #region DataRow Extensions

        /// <summary>
        /// Provides strongly-typed access to each of the column values in the specified row.
        /// Automatically applies type conversion to the column values.
        /// </summary>
        /// <typeparam name="T">A generic parameter that specifies the return type of the column.</typeparam>
        /// <param name="row">The input <see cref="DataRow"/>, which acts as the this instance for the extension method.</param>
        /// <param name="field">The name of the column to return the value of.</param>
        /// <returns>The value, of type T, of the <see cref="DataColumn"/> specified by <paramref name="field"/>.</returns>
        public static T ConvertField<T>(this DataRow row, string field)
        {
            return row.ConvertField(field, default(T));
        }

        /// <summary>
        /// Provides strongly-typed access to each of the column values in the specified row.
        /// Automatically applies type conversion to the column values.
        /// </summary>
        /// <typeparam name="T">A generic parameter that specifies the return type of the column.</typeparam>
        /// <param name="row">The input <see cref="DataRow"/>, which acts as the this instance for the extension method.</param>
        /// <param name="field">The name of the column to return the value of.</param>
        /// <param name="defaultValue">The value to be substituted if <see cref="DBNull.Value"/> is retrieved.</param>
        /// <returns>The value, of type T, of the <see cref="DataColumn"/> specified by <paramref name="field"/>.</returns>
        public static T ConvertField<T>(this DataRow row, string field, T defaultValue)
        {
            object value = row.Field<object>(field);

            if (value == null || value == DBNull.Value)
                return defaultValue;

            // If the value is an instance of the given type,
            // no type conversion is necessary
            if (value is T typeValue)
                return typeValue;

            Type type = typeof(T);

            // Nullable types cannot be used in type conversion, but we can use Nullable.GetUnderlyingType()
            // to determine whether the type is nullable and convert to the underlying type instead
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            // Handle Guids as a special case since they do not implement IConvertible
            if (underlyingType == typeof(Guid))
                return (T)(object)Guid.Parse(value.ToString());

            // Handle enums as a special case since they do not implement IConvertible
            if (underlyingType.IsEnum)
                return (T)Enum.Parse(underlyingType, value.ToString());

            // Handle DateOnly as a special case since it does not implement IConvertible
            if (underlyingType == typeof(DateOnly))
                return (T)(object)DateOnly.Parse(value.ToString());

            // Handle TimeOnly as a special case since it does not implement IConvertible
            if (underlyingType == typeof(TimeOnly))
                return (T)(object)TimeOnly.Parse(value.ToString());

            return (T)Convert.ChangeType(value, underlyingType);
        }

        /// <summary>
        /// Automatically applies type conversion to column values when only a type is available.
        /// </summary>
        /// <param name="row">The input <see cref="DataRow"/>, which acts as the this instance for the extension method.</param>
        /// <param name="field">The name of the column to return the value of.</param>
        /// <param name="type">Type of the column.</param>
        /// <returns>The value of the <see cref="DataColumn"/> specified by <paramref name="field"/>.</returns>
        public static object ConvertField(this DataRow row, string field, Type type)
        {
            return row.ConvertField(field, type, null);
        }

        /// <summary>
        /// Automatically applies type conversion to column values when only a type is available.
        /// </summary>
        /// <param name="row">The input <see cref="DataRow"/>, which acts as the this instance for the extension method.</param>
        /// <param name="field">The name of the column to return the value of.</param>
        /// <param name="type">Type of the column.</param>
        /// <param name="defaultValue">The value to be substituted if <see cref="DBNull.Value"/> is retrieved.</param>
        /// <returns>The value of the <see cref="DataColumn"/> specified by <paramref name="field"/>.</returns>
        public static object ConvertField(this DataRow row, string field, Type type, object defaultValue)
        {
            object value = row.Field<object>(field);

            if (value == null || value == DBNull.Value)
                return defaultValue ?? (type.IsValueType ? Activator.CreateInstance(type) : null);

            // If the value is an instance of the given type,
            // no type conversion is necessary
            if (type.IsInstanceOfType(value))
                return value;

            // Nullable types cannot be used in type conversion, but we can use Nullable.GetUnderlyingType()
            // to determine whether the type is nullable and convert to the underlying type instead
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            // Handle Guids as a special case since they do not implement IConvertible
            if (underlyingType == typeof(Guid))
                return Guid.Parse(value.ToString());

            // Handle enums as a special case since they do not implement IConvertible
            if (underlyingType.IsEnum)
                return Enum.Parse(underlyingType, value.ToString());

            return Convert.ChangeType(value, underlyingType);
        }

        /// <summary>
        /// Provides strongly-typed access to each of the column values in the specified row.
        /// Automatically applies type conversion to the column values.
        /// </summary>
        /// <typeparam name="T">A generic parameter that specifies the return type of the column.</typeparam>
        /// <param name="row">The input <see cref="DataRow"/>, which acts as the this instance for the extension method.</param>
        /// <param name="field">The name of the column to return the value of.</param>
        /// <returns>The value, of type T, of the <see cref="DataColumn"/> specified by <paramref name="field"/>.</returns>
        public static T? ConvertNullableField<T>(this DataRow row, string field) where T : struct
        {
            object value = row.Field<object>(field);

            if (value == null)
                return null;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Parses a Guid from a database field that is a Guid type or a string representing a Guid.
        /// </summary>
        /// <param name="row">The input <see cref="DataRow"/>, which acts as the this instance for the extension method.</param>
        /// <param name="field">The name of the column to return the value of.</param>
        /// <param name="defaultValue">The value to be substituted if <see cref="DBNull.Value"/> is retrieved; defaults to <see cref="Guid.Empty"/>.</param>
        /// <returns>The <see cref="Guid"/> value of the <see cref="DataColumn"/> specified by <paramref name="field"/>.</returns>
        public static Guid ConvertGuidField(this DataRow row, string field, Guid? defaultValue = null)
        {
            object value = row.Field<object>(field);

            if (value == null || value == DBNull.Value)
                return defaultValue ?? Guid.Empty;

            if (value is Guid guid)
                return guid;

            return Guid.Parse(value.ToString());
        }

        #endregion

        #region MailBuilder Extensions

        public static void ConvertInlineImageToVisual(this MailBuilder builder)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(builder.Html);

            IEnumerable<string> images = document.DocumentNode.Descendants("img")
                .Select(e => e.GetAttributeValue("src", null))
                .Where(s => !string.IsNullOrEmpty(s));

            foreach (string image in images)
            {
                string imageData = Regex.Replace(image, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                string contentId = Guid.NewGuid().ToString();

                builder.Html = builder.Html.Replace(image, $"cid:{contentId}");

                MimeData visual = builder.AddVisual(Convert.FromBase64String(imageData));
                visual.ContentId = contentId;
            }
        }

        #endregion
    }
}
