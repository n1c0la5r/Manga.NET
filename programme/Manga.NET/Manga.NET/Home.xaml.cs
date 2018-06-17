using MangaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace MangaNET {
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Home : Page, INotifyPropertyChanged {
        public Library Lib {
            get {
                var app = Application.Current as App;
                return app.library;
            }
        }
        public Dictionary<string, float> ToRead {
            get {
                return (Application.Current as App).ToRead;
            }
            set {
                (Application.Current as App).ToRead = value;
            }
        }

        public Home() {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            await LoadToRead();
        }

        private async Task LoadToRead() {
            try {
                ToRead = await Lib.ToRead();
            }
            catch { await MainPage.ConnectionError(); }
            OnPropertyChanged("ToRead");
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {
            var s = (KeyValuePair<Manga, float>)e.ClickedItem;
            MainPage.ToReadClick(s.Key, s.Value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
