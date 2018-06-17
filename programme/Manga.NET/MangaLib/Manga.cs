using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaLib {
    public class Manga : IEquatable<Manga> {
        public string Id { get; internal set; }
        public string Title { get; internal set; }
        public string Author { get; internal set; }
        public IEnumerable<string> Categories { get; internal set; } = new List<string>();
        public DateTime LastChapDate { get; internal set; }
        public string Synopsis { get; internal set; }
        public string ImageUrl { get; internal set; }
        List<Chapter> Chapters = new List<Chapter>();
        public IEnumerable<float> ChaptersNumbers {
            get {
                List<float> l = new List<float>();
                foreach(var c in Chapters)
                    l.Add(c.Num);
                l.Sort();
                return l;
            }
        }

        internal Manga() { }
        internal Manga(IEnumerable<Chapter> lc) {
            foreach(var c in lc) {
                Chapters.Add(c.DeepCopy());
            }
        }

        public Chapter this[float chapterNum] {
            get {
                try {
                    return (from c in Chapters
                            where c.Num == chapterNum
                            select c).First();
                }
                catch {
                    throw new NoSuchElementException($"Chapter {chapterNum} doesn't exists.");
                }
            }

            internal set {
                int i;
                try {
                    i = (from c in Chapters
                         where c.Num == chapterNum
                         select Chapters.IndexOf(c)).First();
                }
                catch {
                    throw new NoSuchElementException($"Chapter {chapterNum} doesn't exists.");
                }

                Chapters[i] = value;
            }
        }

        //public Manga DeepCopy() {
        //    var l = new List<Chapter>();
        //    foreach (var c in Chaps) {
        //        l.Add(c.DeepCopy());
        //    }
        //    return new Manga(l) {
        //        Id = Id,
        //        Title = Title,
        //        Author = Author,
        //        Categories = Categories,
        //        LastChapDate = LastChapDate,
        //        Synopsis = Synopsis,
        //        ImageUrl = ImageUrl
        //    };
        //}

        public override bool Equals(object o) {
            if(ReferenceEquals(o, null)) return false;
            if(ReferenceEquals(this, o)) return true;
            if(GetType() != o.GetType()) return false;
            return Equals(o as Manga);
        }
        public bool Equals(Manga m) {
            return (Id == m.Id && Title == m.Title && Author == m.Author && Categories == m.Categories && Synopsis == m.Synopsis && ImageUrl == m.ImageUrl);
        }

        public override int GetHashCode() {
            return Title.GetHashCode();
        }

        public override string ToString() {
            string msg = Title + "\t" + ImageUrl;
            foreach(var ch in Chapters) {
                msg = $"{msg}\n\t{ch}";
            }
            return msg;
        }
    }
}
