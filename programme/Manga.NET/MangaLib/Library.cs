using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace MangaLib {
    [DataContract]
    public class Library : INotifyPropertyChanged {
        Dictionary<string, Manga> Mangas;
        public IEnumerable<string> MangasNames {
            get { return Mangas.Values.Select(m => m.Title); }
        }

        [DataMember]
        Dictionary<string, List<String>> Bookmarks = new Dictionary<string, List<string>>();
        public IEnumerable<string> BookmarksFolders {
            get {
                return Bookmarks.Keys.ToList();
            }
        }

        [DataMember]
        Dictionary<string, float> Readed = new Dictionary<string, float>();

        /// <summary>
        /// List the last Manga updated
        /// </summary>
        /// <returns>list manga id</returns>
        public IEnumerable<string> Recents {
            get {
                return Mangas.Values.OrderBy(m => m.LastChapDate).Reverse().Select(m => m.Id);
            }
        }

        MangaLoader api;
        DataSaver saver;

        public Library(MangaLoader mangaLoader, DataSaver dataSaver) {
            api = mangaLoader;
            saver = dataSaver;
        }

        public Manga this[string id] {
            get {
                if(!Mangas.ContainsKey(id))
                    throw new NoSuchElementException($"Manga with id {id} doesn't exists");
                return Mangas[id];
            }

            private set {
                if(!Mangas.ContainsKey(id))
                    throw new NoSuchElementException($"Manga with id {id} doesn't exists");
                Mangas[id] = value;
            }
        }

        /// <summary>
        ///     Get List Manga Id for a folder
        /// </summary>
        /// <param name="folder">Name of the folder</param>
        /// <returns>List manga id</returns>
        public IEnumerable<string> GetBookmarks(string folder) {
            List<string> bookmarks = new List<string>();
            bookmarks.AddRange(Bookmarks[folder]);
            return bookmarks;
        }

        /// <summary>
        /// Load Manga list from api into mangas
        /// </summary>
        public async Task LoadMangaList() {
            var mangas = await api.GetMangaList();
            Mangas = mangas.ToDictionary(kv => kv.Key, kv => kv.Value);
            OnLoaded("library");
        }

        /// <summary>
        /// Load manga details from api into the manga
        /// </summary>
        /// <param name="id">id of the manga to load</param>
        public async Task LoadMangaDetails(string id) {
            this[id] = await api.GetMangaDetails(this[id]);
        }

        /// <summary>
        /// Load chapter pages from api into the chapter
        /// </summary>
        /// <param name="mangaID">id of the manga</param>
        /// <param name="chapNum">num of the chapter to load</param>
        public async Task LoadChapterPages(string mangaId, float chapterNum) => this[mangaId][chapterNum] = await api.GetChapter(this[mangaId][chapterNum]);

        /// <summary>
        /// Save the a chapter as last read for a Manga
        /// </summary>
        /// <param name="mangaId">Manga id</param>
        /// <param name="num">Chapter number</param>
        public void MarkAsRead(string mangaId, float num) {
            if(!Readed.ContainsKey(mangaId)) {
                Readed.Add(mangaId, num);
            }
            else {
                Readed[mangaId] = num;
            }
        }

        /// <summary>
        /// Add a new folder
        /// </summary>
        /// <param name="name">New folder name</param>
        public void NewFolder(string name) {
            if(Bookmarks.ContainsKey(name))
                throw new AlreadyExistsException("Folder already exists.");
            Bookmarks.Add(name, new List<string>());
            OnPropertyChanged("BookmarksFolders");
        }

        /// <summary>
        /// Delete a folder
        /// </summary>
        /// <param name="name">name of the folder to delete</param>
        public void DeleteFolder(string name) {
            Bookmarks.Remove(name);
            OnPropertyChanged("BookmarksFolders");
        }

        /// <summary>
        /// Add a manga as bookmarked into a folder
        /// </summary>
        /// <param name="folder">Folder name</param>
        /// <param name="mangaId">Manga id</param>
        public void NewBookmark(string folder, string mangaId) {
            if(Bookmarks[folder].Contains(mangaId))
                throw new AlreadyExistsException("Bookmark already exists in this folder");
            Bookmarks[folder].Add(mangaId);
        }

        /// <summary>
        /// Delete a bookmarked manga from a folder
        /// </summary>
        /// <param name="folder">Folder name</param>
        /// <param name="mangaId">Manga id</param>
        public void DeleteBookmark(string folder, string mangaId) => Bookmarks[folder].Remove(mangaId);

        /// <summary>
        /// Search mangas by title
        /// </summary>
        /// <param name="search">Manga name to search</param>
        /// <returns></returns>
        public IEnumerable<string> SearchManga(string research) {
            var mangaList = Mangas.Values;
            var em = mangaList.Where(m => m.Title.ToLower().Contains(research.ToLower())).Select(m => m.Id);
            return em;
        }

        /// <summary>
        /// Find the next chapters to read from already readed chapters
        /// </summary>
        /// <returns>Dictionary key: string MangaId , value: float ChapterNumber</returns>
        public async Task<Dictionary<string, float>> ToRead() {
            Dictionary<string, float> toRead = new Dictionary<string, float>();
            foreach(var kv in Readed) {
                await LoadMangaDetails(kv.Key);
                var chapsNums = this[kv.Key].ChaptersNumbers.ToList();
                if(chapsNums.Max() > kv.Value)
                    toRead.Add(kv.Key, chapsNums[chapsNums.IndexOf(kv.Value) + 1]);
            }
            return toRead;

        }

        /// <summary>
        /// Save asyncronously Bookmarks and Readed
        /// </summary>
        /// <returns>Task</returns>
        public async Task SaveData() {
            await saver.SaveData(this);
        }

        /// <summary>
        /// Restore asyncronously Bookmarks and Readed
        /// </summary>
        /// <returns>Task</returns>
        public async Task RestoreData() {
            var l = await saver.RestoreData();
            if(l == null) {
                OnLoaded("appData");
                return;
            }
            Bookmarks = l.Bookmarks;
            Readed = l.Readed;
            OnPropertyChanged("BookmarksFolders");
            OnLoaded("appData");
        }

        /// <summary>
        /// Triggered every time a loading data methode end
        /// </summary>
        public event EventHandler<IsLoadedEventArgs> Loaded;
        public virtual void OnLoaded(string name) {
            Loaded?.Invoke(this, new IsLoadedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
