using MangaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MangaNET {
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged {

        public Library Lib {
            get {
                var app = Application.Current as App;
                return app.library;
            }
        }

        public Page FrameContent { get { return Page.Content as Page; } }

        /* ToRead est dans la vue plutôt que dans le model car 
         * dans le model ToRead() est une fonction asynchrone.
         * ToRead seras donc Actualisé à chaque chargement de la page Home.
         * Cela permet donc aussi de catch les problème de connexions.
         */
        public Dictionary<string, float> ToRead {
            get {
                var app = Application.Current as App;
                return app.ToRead;
            }
            set {
                var app = Application.Current as App;
                app.ToRead = value;
            }
        }

        public MainPage() {
            this.InitializeComponent();

            Lib.Loaded += AuthorizeAccess;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            try {
                await Lib.LoadMangaList();
                await Lib.RestoreData();
                await Load20RecentsDetails();
            }
            catch {
                await ConnectionError();
            }
        }

        private async Task Load20RecentsDetails() {
            var recents = Lib.Recents.ToList();
            for(int i = 0; i < 20; i++) await Lib.LoadMangaDetails(recents[i]);
            Lib.OnLoaded("recents");
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args) {
            var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
            NavView_Navigate(item as NavigationViewItem);
        }

        private void NavView_Navigate(NavigationViewItem item) {
            switch(item.Tag) {
                case "home":
                    Page.Navigate(typeof(Home));
                    break;

                case "lasts":
                    Page.Navigate(typeof(Lasts));
                    break;

                case "listManga":
                    Page.Navigate(typeof(ListeMangas));
                    break;

                case "bookmarks":
                    Page.Navigate(typeof(Bookmarks));
                    break;
            }
        }

        public static void FrameNavigateTo(Type type) {
            var winFrame = Window.Current.Content as Frame;
            var mp = winFrame.Content as MainPage;
            mp.PageNavigateTo(type);
        }

        public void PageNavigateTo(Type page) {
            Page.Navigate(page);
        }

        private void Nav_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) {
            OnBackRequested();
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            OnBackRequested();
            args.Handled = true;
        }

        private bool OnBackRequested() {
            bool navigated = false;

            if(NavView.IsPaneOpen && (NavView.DisplayMode == NavigationViewDisplayMode.Compact || NavView.DisplayMode == NavigationViewDisplayMode.Minimal)) {
                return false;
            }
            else {
                if(Page.CanGoBack) {
                    Page.GoBack();
                    navigated = true;
                }
            }
            return navigated;
        }

        private void Page_Navigated(object sender, NavigationEventArgs e) {
            NavView.IsBackEnabled = Page.CanGoBack;

            Dictionary<Type, string> lookup = new Dictionary<Type, string>() {
                {typeof(Home), "home"},
                {typeof(Lasts), "lasts"},
                {typeof(ListeMangas), "listManga"},
                {typeof(Bookmarks), "bookmarks"},
                {typeof(MangaDetail), "mangaDetails"},
                {typeof(ChapterPage), "chapter"}
            };

            String stringTag = lookup[Page.SourcePageType];
            
            foreach(NavigationViewItemBase item in NavView.MenuItems) {
                if(item is NavigationViewItem && item.Tag.Equals(stringTag)) {
                    item.IsSelected = true;
                    break;
                }
            }

        }

        public static void MangaThumbnailClick(Manga m) {
            (Application.Current as App).CurrentManga = m.Id;
            FrameNavigateTo(typeof(MangaDetail));
        }

        public static void ToReadClick(Manga m, float chapNum) {
            var app = (Application.Current as App);
            app.CurrentManga = m.Id;
            app.CurrentChapter = chapNum;
            FrameNavigateTo(typeof(ChapterPage));
        }



        bool IsAppDataLoaded {
            get { return mIsAppDataLoaded; }
            set {
                mIsAppDataLoaded = value;
                OnPropertyChanged("IsAppDataLoaded");
            }
        }
        bool mIsAppDataLoaded;

        bool IsLibraryLoaded {
            get { return mIsLibraryLoaded; }
            set {
                mIsLibraryLoaded = value;
                OnPropertyChanged("IsLibraryLoaded");
            }
        }
        bool mIsLibraryLoaded;

        bool IsRecentsLoaded {
            get { return mIsRecentsLoaded; }
            set {
                mIsRecentsLoaded = value;
                OnPropertyChanged("IsRecentsLoaded");
            }
        }
        bool mIsRecentsLoaded;

        private void AuthorizeAccess(object sender, IsLoadedEventArgs args) {
            if(args.LoadedData == "appData") {
                IsAppDataLoaded = true;
                Page.Navigate(typeof(Home));
                NavView.SelectedItem = NavView.MenuItems[0];
            }
            if(args.LoadedData == "library") IsLibraryLoaded = true;
            if(args.LoadedData == "recents") IsRecentsLoaded = true;
        }

        public static async Task ConnectionError() {
            await new MessageDialog("Aucune connexion internet", "Erreur").ShowAsync();
            Application.Current.Exit();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
