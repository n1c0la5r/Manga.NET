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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace MangaNET {
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ChapterPage : Page, INotifyPropertyChanged {
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

        public Visibility ShowPreviousButton {
            get {
                var chapsNums = CurrentManga.ChaptersNumbers.ToList();
                if(CurrentChapter.Num == chapsNums[0])
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }
        public Visibility ShowNextButton {
            get {
                var cn = CurrentManga.ChaptersNumbers.ToList();
                if(CurrentChapter.Num == cn[cn.Count - 1])
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }

        public int ImagesWidth {
            get {
                if(ActualWidth < ActualHeight)
                    return (int)ActualWidth;
                else {
                    return (int)(0.6 * ActualWidth);
                }
            }
        }

        public ChapterPage() {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            try {
                await Lib.LoadChapterPages(CurrentManga.Id, CurrentChapter.Num);
            }
            catch { await MainPage.ConnectionError(); }
            OnPropertyChanged("CurrentChapter");
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e) {
            OnPropertyChanged("ImagesWidth");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var cb = sender as ComboBox;
            (Application.Current as App).CurrentChapter = (float)cb.SelectedItem;
            Page_Loaded(this, new RoutedEventArgs());
            OnPropertyChanged("ShowNextButton");
            OnPropertyChanged("ShowPreviousButton");
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e) {
            ChangeChapter(-1);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e) {
            ChangeChapter(1);
        }

        private void ChangeChapter(int i) {
            var chapsNums = CurrentManga.ChaptersNumbers.ToList();
            var indexOfNext = chapsNums.LastIndexOf(CurrentChapter.Num) + i;
            (Application.Current as App).CurrentChapter = chapsNums[indexOfNext];
            MainPage.FrameNavigateTo(typeof(ChapterPage));
        }

        private async void MarkAsReadButton_Click(object sender, RoutedEventArgs e) {
            Lib.MarkAsRead(CurrentManga.Id, CurrentChapter.Num);
            await Lib.SaveData();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
