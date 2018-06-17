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
using Windows.System;
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
    public sealed partial class ListeMangas : Page, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Library Lib {
            get {
                var app = Application.Current as App;
                return app.library;
            }
        }
        private List<String> search = new List<string>();

        public Visibility ToMany {
            get { return _toMany; }
            set { _toMany = value; OnPropertyChanged("ToMany"); }
        }
        private Visibility _toMany = Visibility.Collapsed;

        public ListeMangas() {
            this.InitializeComponent();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
                if(sender.Text.Length < 3) {
                    sender.ItemsSource = new List<string>();
                    return;
                }
                var found = Lib.MangasNames.Where(m => m.ToLower().Contains(sender.Text.ToLower())).ToList();
                found.Sort();
                sender.ItemsSource = found;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            ToMany = Visibility.Collapsed;
            var s = sender as AutoSuggestBox;
            s.IsFocusEngaged = false;
            search = Lib.SearchManga(s.Text).ToList();
            
            if(search.Count > 350) {
                search.Clear();
                ResultText.Text = "Trop de Résultats.\nAffichage Impossible.";
                ToMany = Visibility.Visible;
            }
            if(search.Count == 0 && ToMany != Visibility.Visible) {
                ResultText.Text = "Aucun résultat.";
                ToMany = Visibility.Visible;
            }

            OnPropertyChanged("search");
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {
            MainPage.MangaThumbnailClick(e.ClickedItem as Manga);
        }
    }
}
