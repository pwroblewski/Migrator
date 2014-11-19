using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Migrator.Helpers
{
    public static class ExtensionMethods
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            var col = new ObservableCollection<T>();
            foreach (var cur in enumerable)
            {
                col.Add(cur);
            }

            return col;
        }

        public static List<T> ToList<T>(this ObservableCollection<T> collection)
        {
            var col = new List<T>();
            foreach (var cur in collection)
            {
                col.Add(cur);
            }

            return col;
        }

        public static T MergeWith<T>(this T primary, T secondary)
        {
            foreach (var pi in typeof(T).GetProperties())
            {
                var priValue = pi.GetGetMethod().Invoke(primary, null);
                var secValue = pi.GetGetMethod().Invoke(secondary, null);

                if (priValue != null && priValue.ToString().Equals(string.Empty))
                    priValue = null;
                if (secValue != null && secValue.ToString().Equals(string.Empty))
                    secValue = null;

                if (priValue == null || (pi.PropertyType.IsValueType && priValue.Equals(Activator.CreateInstance(pi.PropertyType))))
                {
                    pi.GetSetMethod().Invoke(primary, new object[] { secValue });
                }
            }

            return primary;
        }
    }
}
