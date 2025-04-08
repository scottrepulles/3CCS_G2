using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace DHK.Module.Helper
{
    public class SortingHelper
    {
        public static void Sort(XPBaseCollection collection, string property, SortingDirection direction)
        {
            bool isSortingAdded = false;
            foreach (SortProperty sortProperty in collection.Sorting)
            {
                if (sortProperty.Property.Equals(DevExpress.Data.Filtering.CriteriaOperator.Parse(property)))
                {
                    isSortingAdded = true;
                }
            }
            if (!isSortingAdded)
            {
                collection.Sorting.Add(new SortProperty(property, direction));
            }
        }
    }
}
