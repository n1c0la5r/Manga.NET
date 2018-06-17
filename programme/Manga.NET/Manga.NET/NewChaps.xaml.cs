using MangaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace MangaNET {
    public sealed partial class NewChaps : UserControl, INotifyPropertyChanged {
        public NewChaps() {
            this.InitializeComponent();
        }

        public string Id {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); OnPropertyChanged("Id"); }
        }
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(NewChaps), new PropertyMetadata(null));

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); OnPropertyChanged("Title"); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NewChaps), new PropertyMetadata("Title"));

        public string ImageUrl {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); OnPropertyChanged("ImageUrl"); }
        }
        public static readonly DependencyProperty ImageUrlProperty =
            DependencyProperty.Register("ImageUrl", typeof(string), typeof(NewChaps), new PropertyMetadata("http://cssp.org.pk/images/404_img.jpg"));

        public List<string> Chapters {
            get { return (List<string>)GetValue(ChaptersProperty); }
            set { SetValue(ChaptersProperty, value); OnPropertyChanged("ImageUrl"); }
        }
        public static readonly DependencyProperty ChaptersProperty =
            DependencyProperty.Register("Chapters", typeof(List<string>), typeof(NewChaps), new PropertyMetadata(null));
        

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            var s = e.ClickedItem as string;
            float f;
            if(s.Contains(":")) {
                var num = s.Remove(s.IndexOf(":") - 1);
                f = float.Parse(num);
            }
            else {
                f = float.Parse(s);
            }
            (Application.Current as App).CurrentManga = Id;
            (Application.Current as App).CurrentChapter = f;

            MainPage.FrameNavigateTo(typeof(ChapterPage));
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e) {
            ImageUrl = "http://cssp.org.pk/images/404_img.jpg";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
