using MangaLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaNET {
    class DictToRead2dictMangaToRead : IValueConverter {
        public Library Lib {
            get {
                var app = Application.Current as App;
                return app.library;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language) {
            var tr = value as Dictionary<string, float>;
            var toRead = new Dictionary<Manga, float>();
            foreach(var sf in tr) {
                toRead.Add(Lib[sf.Key], sf.Value);
            }
            toRead = toRead.OrderBy(kv => kv.Key.Title).ToDictionary(kv => kv.Key, kv => kv.Value);
            return toRead;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return null;
        }
    }
}
