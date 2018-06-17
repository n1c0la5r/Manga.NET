using System;
using static System.Console;
using MangaLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LocalDataSaverLib;


/// <summary>
/// Test the mains functions of the library
/// </summary>
namespace testLibrary {
    class Program {
        static Library l = new Library(new MangaEdenAPI(), new LocalDataSaver());
        static void Main(string[] args) {
            WriteLine("Loading...");
            try {
                l.LoadMangaList().Wait();
            }
            catch {
                WriteLine("Problème de connexion");
                return;
            }

            l.RestoreData().Wait();

            var s = Menu();
            while(s != "9") {
                switch(s) {
                    case "1":
                        Search();
                        break;
                    case "2":
                        Recents();
                        break;
                    case "3":
                        Favoris();
                        break;
                    case "8":
                        l.SaveData().Wait();
                        break;
                }
                WriteLine("\nAppuyez sur une touche pour continuer...");
                ReadKey(true);
                s = Menu();
            }
        }

        static string Menu() {
            Clear();
            WriteLine("1. Recherche");
            WriteLine("2. Dèrnière sortie");
            WriteLine("3. Favoris");
            WriteLine("8. Sauvegarder");
            WriteLine("9. quitter");
            Write("> ");
            var s = ReadLine();
            Clear();
            return s;
        }

        static void Search() {
            SetWindowSize(180, WindowHeight);
            Write("Recherche : ");
            var s = ReadLine();
            List<string> find = l.SearchManga(s).ToList();
            WriteLine($"Search : {s}");
            var count = 1;
            foreach(var id in find) {
                WriteLine("{0,-5}{1,-50}{2,-50}", count, l[id].Title, l[id].ImageUrl);
                count++;
            }
            if(find.Count == 0) {
                WriteLine("Aucun resultat.");
                return;
            }
            WriteLine("\nQuel manga voulez-vous voir ?");
            Write("> ");
            var choix = CheckChoix(find);
            if(choix == -1) return;
            var cId = find[choix];
            l.LoadMangaDetails(cId).Wait();
            WriteLine(l[cId]);
            Write("\nAjouter au favoris ? o/n : ");
            s = ReadLine();
            count = 1;
            if(s == "o") {
                foreach(var d in l.BookmarksFolders) {
                    WriteLine($"{count}.{d}");
                    count++;
                }
                WriteLine("\nDans quel dossier ?");
                Write("> ");
                choix = CheckChoix(l.BookmarksFolders.ToList());
                if(choix == -1) return;
                var folders = l.BookmarksFolders.ToList();
                var folder = folders[choix];
                try {
                    l.NewBookmark(folder, cId);
                }
                catch {
                    WriteLine("Ce manga est déjà dans ce dossier");
                }
            }
        }

        static void Recents() {
            var recents = l.Recents.ToList();
            for(int i = 0; i < 20; i++) {
                l.LoadMangaDetails(recents[i]).Wait();
                var m = l[recents[i]];
                var chapNums = m.ChaptersNumbers.ToList();
                WriteLine("{0,-50}{1,-10}{2,10}", m.Title, chapNums[chapNums.Count - 1], m.LastChapDate);
            }
        }

        private static void Favoris() {
            foreach(var s in l.BookmarksFolders) {
                WriteLine(s);
                foreach(var f in l.GetBookmarks(s))
                    WriteLine("\t" + l[f].Title);
            }
        }

        static int CheckChoix(List<string> list) {
            int choix = -1;
            try {
                choix = Int32.Parse(ReadLine());
            }
            catch {
                WriteLine("Ceci n'est pas un chiffre");
                return -1;
            }
            string cId;
            try {
                cId = list[choix - 1];
            }
            catch {
                WriteLine("Ce numéro n'était pas proposé (t'es con ou quoi ?)");
                return -1;
            }
            return choix - 1;
        }
    }
}
