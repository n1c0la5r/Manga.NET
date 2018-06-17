using MangaLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MangaNET {
    class Chapter2LPageConverter : IValueConverter {
        public Library Lib {
            get { return (Application.Current as App).library; }
        }
        public Manga CurrentManga {
            get {
                var id = (Application.Current as App).CurrentManga;
                return Lib[id];
            }
        }
        public Chapter CurrentChapter {
            get {
                var id = (Application.Current as App).CurrentManga;
                var num = (Application.Current as App).CurrentChapter;
                return Lib[id][num];
            }
        }
        public object Convert(object value, Type targetType, object parameter, string language) {
            var nums = value as List<int>;
            var lUrl = new List<string>();
            foreach(var num in nums)
                lUrl.Add(CurrentChapter[num].Url);
            return lUrl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return new List<int>();
        }
    }
}
