using MangaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class Bookmarks : Page, INotifyPropertyChanged {
        public Library Lib {
            get {
                var app = Application.Current as App;
                return app.library;
            }
        }
        public string CurrentFolder { get { return Folders.SelectedItem as string; } }
        public List<string> CurrentFolderContent {
            get {
                if(CurrentFolder == null)
                    return new List<string>();
                return Lib.GetBookmarks(CurrentFolder).ToList();
            }
        }

        public Bookmarks() {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            if(Lib.BookmarksFolders.ToList().Count != 0)
                Folders.SelectedIndex = 0;
        }

        private void Folders_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            OnPropertyChanged("CurrentFolder");
            OnPropertyChanged("CurrentFolderContent");
        }

        private async void AddFolder_Click(object sender, RoutedEventArgs e) {
            var inputTextBox = new TextBox {
                AcceptsReturn = false,
                Height = 32,
                BorderThickness = new Thickness(1)
            };
            var newFolderDialog = new ContentDialog {
                Content = inputTextBox,
                Title = "Nouveau dossier",
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
                if(inputTextBox.Text == "") {
                    await new MessageDialog("Le nom que vous avez rentré n'est pas valide.", "Erreur").ShowAsync();
                    return;
                }
                try {
                    Lib.NewFolder(inputTextBox.Text);
                }
                catch(AlreadyExistsException exists) {
                    await new MessageDialog(exists.Message, "Erreur").ShowAsync();
                }
                await Lib.SaveData();
            }
        }

        private void MenuFlyout_Opened(object sender, object e) {
            if(Folders.SelectedItem == null)
                (sender as MenuFlyout).Hide();
        }

        private async void DeleteMenuFlyoutItem_Click(object sender, RoutedEventArgs e) {
            string folder = Folders.SelectedItem as string;

            if(await DeleteValidation($"Voulez-vous supprimer le dossier {folder} et son contenu ?")) {
                Lib.DeleteFolder(folder);
                await Lib.SaveData();
            }
        }

        private async void DeleteBookmark_Click(object sender, RoutedEventArgs e) {
            var mfi = sender as MenuFlyoutItem;
            var id = mfi.Tag as string;
            var title = Lib[id].Title;

            if(await DeleteValidation($"Voulez-vous supprimer le Favoris {title} du dossier {CurrentFolder} ?")) {
                Lib.DeleteBookmark(CurrentFolder, id);
                await Lib.SaveData();
                OnPropertyChanged("CurrentFolderContent");
            }
        }

        private async Task<bool> DeleteValidation(string msg) {
            var dialog = new MessageDialog(msg, "Supprimer");
            dialog.Commands.Add(new UICommand("Oui") { Id = 0 });
            dialog.Commands.Add(new UICommand("Non") { Id = 1 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();
            if((int)result.Id == 0) {
                return true;
            }
            return false;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {
            MainPage.MangaThumbnailClick(e.ClickedItem as Manga);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
