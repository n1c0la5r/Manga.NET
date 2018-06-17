using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MangaNET {
    class Categories2StringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var lc = value as List<string>;
            string categories = "";
            foreach(var c in lc)
                categories += c + " ";
            return categories;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return new List<string>();
        }
    }
}
