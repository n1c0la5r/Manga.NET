using MangaLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaNET {
    public class RecentsIds2MangasConverter : IValueConverter {
        public Library Lib {
            get {
                var app = Application.Current as App;
                return app.library;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language) {
            var ies = value as IEnumerable<string>;
            var ls = ies.ToList();
            List<Manga> lm = new List<Manga>();
            for(int i = 0; i < 20; i++) {
                lm.Add(Lib[ls[i]]);
            }
            return lm;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            var lm = value as List<Manga>;
            List<string> ls = new List<string>();
            foreach(var m in lm) {
                ls.Add(m.Id);
            }
            return lm;
        }
    }
}
