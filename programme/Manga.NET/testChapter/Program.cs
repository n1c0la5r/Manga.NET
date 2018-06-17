using System;
using static System.Console;
using MangaLib;
using LocalDataSaverLib;

using System.Collections.Generic;

/// <summary>
/// this test was designed to test the accessibility of a Chapter
/// </summary>
namespace testChapter {
    class Program {
        static void Main(string[] args) {
            Library l = new Library(new MangaEdenAPI(),new LocalDataSaver());
            string id = "55a19992719a1609004acd13";
            WriteLine("Chargement...");
            l.LoadMangaList().Wait();
            l.LoadMangaDetails(id).Wait();
            l.LoadChapterPages(id, 34).Wait();
            Chapter c = l[id][34];

            WriteLine(c);//un Chapitre
            WriteLine("\n\n\n\n");
            //Accès a chaque page
            foreach(var n in c.PagesNumbers)
                WriteLine(c[n].Num);


            ReadKey(true);
        }
    }
}
