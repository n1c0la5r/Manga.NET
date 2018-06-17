using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaLib {
    public abstract class MangaLoader {
        public abstract Task<Dictionary<string, Manga>> GetMangaList();
        public abstract Task<Manga> GetMangaDetails(Manga m);
        public abstract Task<Chapter> GetChapter(Chapter c);
    }
}
