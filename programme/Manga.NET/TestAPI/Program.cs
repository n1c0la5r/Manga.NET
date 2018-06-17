using System;
using static System.Console;
using MangaLib;
using LocalDataSaverLib;

/// <summary>
/// test the methods of MangaLoader
/// </summary>
namespace TestAPI {
    class Program {
        static void Main(string[] args) {
            MangaEdenAPI loader = new MangaEdenAPI();
            Library l = new Library(loader, new LocalDataSaver());
            WriteLine("Loading...");
            string id = "55a19992719a1609004acd13";
            l.LoadMangaList().Wait();
            l.LoadMangaDetails(id).Wait();
            l.LoadChapterPages(id, 34).Wait();


            //foreach (Manga m in l.mangas) {
            //    WriteLine(m.Title);
            //}

            //ReadKey(true);
        }
    }
}
