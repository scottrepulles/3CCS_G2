using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;

namespace DHK.Module.Helper
{
    public static class ChoiceActionHelper
    {
        public static void FillItemWithEnumValues(SingleChoiceAction parentItem, Type enumType)
        {
            EnumDescriptor ed = new(enumType);
            foreach (object current in Enum.GetValues(enumType))
            {
                ChoiceActionItem item = new(ed.GetCaption(current), current);
                parentItem.Items.Remove(item);
                item.ImageName = ImageLoader.Instance.GetEnumValueImageName(current);
                parentItem.Items.Add(item);
            }
        }
    }
}
