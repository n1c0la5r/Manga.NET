using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace MangaLib {
    public class MangaEdenAPI : MangaLoader {
        static readonly HttpClient client = new HttpClient();
        static readonly string MangaListURL = "https://www.mangaeden.com/api/list/0/";
        static readonly string MangaURL = "https://www.mangaeden.com/api/manga/";
        static readonly string ChapterURL = "https://www.mangaeden.com/api/chapter/";
        static readonly string CoverURL = "https://cdn.mangaeden.com/mangasimg/200x/";
        static readonly string PageURL = "https://cdn.mangaeden.com/mangasimg/";

        /// <summary>
        /// Create a manga list from mangaEden mangalist json
        /// </summary>
        /// <returns>List<Manga></returns>
        public override async Task<Dictionary<string, Manga>> GetMangaList() {
            var mangaListJson = await GetStringFromAPI(MangaListURL);
            JObject MangaListParsed = JObject.Parse(mangaListJson);

            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            List<Manga> lmanga = MangaListParsed["manga"].Select(m => new Manga {
                Id = (string)m["i"],
                Title = (string)m["t"],
                LastChapDate = (m["ld"] != null) ? d.AddSeconds(Convert.ToDouble(m["ld"])) : d,
                Categories = m["c"].Select(c => (string)c).ToList(),
                ImageUrl = CoverURL + m["im"]
            }).ToList();
            Dictionary<string, Manga> mangas = lmanga.ToDictionary(m => m.Id, m => m);

            return mangas;
        }

        /// <summary>
        /// Load details of the manga passed as parameter
        /// </summary>
        /// <param name="m">Manga to load</param>
        /// <returns>Manga with details added</returns>
        public override async Task<Manga> GetMangaDetails(Manga m) {
            var mangaDetailJson = await GetStringFromAPI(MangaURL + m.Id);
            JObject mangaParsed = JObject.Parse(mangaDetailJson);

            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            IEnumerable<Chapter> chaps = mangaParsed["chapters"].Select(c => new Chapter {
                Num = (float)c[0],
                Date = d.AddSeconds(Convert.ToDouble(c[1])),
                Title = (string)c[2],
                Id = (string)c[3]
            });

            return new Manga(chaps) {
                Id = m.Id,
                Title = m.Title,
                Author = mangaParsed["author"].ToString(),
                Categories = m.Categories,
                LastChapDate = m.LastChapDate,
                Synopsis = mangaParsed["description"].ToString(),
                ImageUrl = m.ImageUrl
            };
        }

        /// <summary>
        /// Load pages of the chapter passed as parameter
        /// </summary>
        /// <param name="c">Chapter to load</param>
        /// <returns>Chapter loaded</returns>
        public override async Task<Chapter> GetChapter(Chapter c) {
            var pagesJson = await GetStringFromAPI(ChapterURL + c.Id);
            JObject pagesParsed = JObject.Parse(pagesJson);

            IEnumerable<MangaPage> pages = pagesParsed["images"].Select(p => new MangaPage((int)p[0], PageURL + p[1]));

            return new Chapter(pages) {
                Id = c.Id,
                Num = c.Num,
                Title = c.Title,
                Date = c.Date
            };
        }

        /// <summary>
        /// Makes an HTTP request to a given url
        /// </summary>
        /// <param name="url">URL to make a request to</param>
        /// <returns>HTTP response string</returns>
        private static async Task<string> GetStringFromAPI(string url) {
            client.DefaultRequestHeaders.Accept.Clear();
            var stringTask = client.GetStringAsync(url);
            var msg = await stringTask;
            return msg;
        }
    }
}
