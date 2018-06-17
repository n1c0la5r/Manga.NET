using MangaLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaNET {
    class LId2LMangaConverter : IValueConverter {
        public Library Lib {
            get {
                var app = Application.Current as App;
                return app.library;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language) {
            var ls = value as List<string>;
            List<Manga> lm = new List<Manga>();
            foreach(var s in ls) {
                lm.Add(Lib[s]);
            }
            lm = lm.OrderBy(m => m.Title).ToList();
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
