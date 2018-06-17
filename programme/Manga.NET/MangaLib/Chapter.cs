using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaLib {
    public class Chapter : IEquatable<Chapter> {
        public string Id { get; internal set; }
        public float Num { get; internal set; }
        public string Title { get; internal set; }
        public DateTime Date { get; internal set; }
        List<MangaPage> Pages = new List<MangaPage>();
        public IEnumerable<int> PagesNumbers {
            get {
                List<int> l = new List<int>();
                foreach(var p in Pages)
                    l.Add(p.Num);
                l.Sort();
                return l;
            }
        }

        internal Chapter() { }
        internal Chapter(IEnumerable<MangaPage> lp) => Pages.AddRange(lp);

        public MangaPage this[int PageNum] {
            get {
                try {
                    return (from p in Pages
                            where p.Num == PageNum
                            select p).First();
                }
                catch {
                    throw new NoSuchElementException($"Pages {PageNum} doesn't exist.");
                }
            }

            internal set {
                int i;
                try {
                    i = (from p in Pages
                         where p.Num == PageNum
                         select Pages.IndexOf(p)).First();
                }
                catch {
                    throw new NoSuchElementException($"Pages {PageNum} doesn't exist.");
                }

                Pages[i] = value;
            }
        }


        /// <summary>
        /// Create a deep copy
        /// </summary>
        /// <returns>Chapter copy</returns>
        public Chapter DeepCopy() {
            return new Chapter(Pages) {
                Id = Id,
                Num = Num,
                Title = Title,
                Date = Date
            };
        }

        public override bool Equals(object o) {
            if(ReferenceEquals(o, null)) return false;
            if(ReferenceEquals(this, o)) return true;
            if(GetType() != o.GetType()) return false;
            return Equals(o as Chapter);
        }
        public bool Equals(Chapter c) {
            return (Id == c.Id && Num == c.Num && Title == c.Title);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            string msg = $"Chapter : {Num}";
            foreach(var page in Pages) {
                msg = $"{msg}\n\t\tpage : {page.Num} => {page.Url}";
            }
            return msg;
        }
    }
}
