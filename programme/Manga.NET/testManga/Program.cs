using System;
using static System.Console;
using MangaLib;
using LocalDataSaverLib;

using System.Collections.Generic;

/// <summary>
/// This test was designed to test the accessibility of a Manga
/// </summary>
namespace testManga {
    class Program {
        static void Main(string[] args) {
            Library l = new Library(new MangaEdenAPI(), new LocalDataSaver());
            string id = "55a19992719a1609004acd13";
            WriteLine("Chargement...");
            l.LoadMangaList().Wait();
            l.LoadMangaDetails(id).Wait();
            l.LoadChapterPages(id, 34).Wait();
            Manga m = l[id];

            WriteLine(m);//un Manga
            WriteLine("\n\n\n\n");
            //Accès a chaque chapitre
            foreach(var n in m.ChaptersNumbers)
                WriteLine(m[n].Num);

            ReadKey(true);
        }
    }
}
