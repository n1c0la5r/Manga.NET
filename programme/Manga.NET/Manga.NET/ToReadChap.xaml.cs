using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class ToReadChap : UserControl, INotifyPropertyChanged {
        public ToReadChap() {
            this.InitializeComponent();
        }

        public string Id {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); OnPropertyChanged("Id"); }
        }
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(ToReadChap), new PropertyMetadata(null));

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); OnPropertyChanged("Title"); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ToReadChap), new PropertyMetadata("Title"));

        public string ImageUrl {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); OnPropertyChanged("ImageUrl"); }
        }
        public static readonly DependencyProperty ImageUrlProperty =
            DependencyProperty.Register("ImageUrl", typeof(string), typeof(ToReadChap), new PropertyMetadata("http://cssp.org.pk/images/404_img.jpg"));

        public float ChapterNumber {
            get { return (float)GetValue(ChapterNumberProperty); }
            set { SetValue(ChapterNumberProperty, value); OnPropertyChanged("ChapterNumber"); }
        }
        public static readonly DependencyProperty ChapterNumberProperty =
            DependencyProperty.Register("ChapterNumber", typeof(float), typeof(ToReadChap), new PropertyMetadata(0.0f));
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
