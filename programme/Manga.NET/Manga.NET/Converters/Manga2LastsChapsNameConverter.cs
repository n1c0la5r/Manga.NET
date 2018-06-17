using MangaLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaNET {
    public class Manga2LastsChapsNameConverter : IValueConverter {
        public Library Lib {
            get {
                var app = Application.Current as App;
                return app.library;
            }
        }
        string id;

        public object Convert(object value, Type targetType, object parameter, string language) {
            var m = value as Manga;
            id = m.Id;
            var ls = new List<string>();
            if(Lib[m.Id].ChaptersNumbers == null) return ls;

            var lc = Lib[m.Id].ChaptersNumbers.ToList();
            lc.Reverse();

            var end = (lc.Count < 4) ? lc.Count : 4;

            for(int i = 0; i < end; i++) {
                var chap = Lib[m.Id][lc[i]];

                if(chap.Num.ToString() != chap.Title && !string.IsNullOrWhiteSpace(chap.Title))
                    ls.Add(chap.Num + " : " + chap.Title);
                else ls.Add(chap.Num.ToString());
            }

            return ls;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return Lib[id];
        }
    }
}
