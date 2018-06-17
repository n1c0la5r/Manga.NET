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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace MangaNET {
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MangaDetail : Page, INotifyPropertyChanged {
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

        public Visibility ShowLoading {
            get { return _ShowLoading; }
            set {
                _ShowLoading = value;
                OnPropertyChanged("ShowLoading");
            }
        }
        private Visibility _ShowLoading;

        public MangaDetail() {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            try {
                await Lib.LoadMangaDetails(CurrentManga.Id);
            }
            catch { await MainPage.ConnectionError(); }
            OnPropertyChanged("CurrentManga");
            ShowLoading = Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            var bookmarksFolders = Lib.BookmarksFolders.ToList();
            if(bookmarksFolders.Count == 0) {
                await new MessageDialog("Pour mettre un manga en favoris, il faut au moins un dossier", "Aucun dossier").ShowAsync();
                return;
            }

            var inputComboBox = new ComboBox {
                Height = 32,
                BorderThickness = new Thickness(1),
                ItemsSource = Lib.BookmarksFolders,
                SelectedItem = bookmarksFolders[0]
            };
            var newFolderDialog = new ContentDialog {
                Content = inputComboBox,
                Title = "Nouveau favoris",
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Cancel",
                SecondaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Secondary
            };

            ContentDialogResult result;
            try {
                result = await newFolderDialog.ShowAsync();
            }
            catch(Exception) {
                return;
            }
            if(result == ContentDialogResult.Secondary) {
                try {
                    Lib.NewBookmark(inputComboBox.SelectedItem as string, CurrentManga.Id);
                }
                catch(AlreadyExistsException exists) {
                    await new MessageDialog(exists.Message, "Erreur").ShowAsync();
                }
                await Lib.SaveData();
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = (float)e.ClickedItem;
            (Application.Current as App).CurrentChapter = item;
            MainPage.FrameNavigateTo(typeof(ChapterPage));
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e) {
            (sender as Image).Source = new BitmapImage(new Uri("http://cssp.org.pk/images/404_img.jpg"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
