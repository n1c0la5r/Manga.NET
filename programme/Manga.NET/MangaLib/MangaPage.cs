using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaLib {
    public struct MangaPage : IEquatable<MangaPage> {
        public int Num { get; private set; }
        public string Url { get; private set; }

        internal MangaPage(int num, string url) {
            Num = num;
            Url = url;
        }

        public override bool Equals(object p) {
            if(!(p is MangaPage)) {
                return false;
            }
            return Equals((MangaPage)p);
        }
        public bool Equals(MangaPage p) {
            return (Num == p.Num && Url == p.Url);
        }
        public static bool operator ==(MangaPage page1, MangaPage page2) {
            return page1.Equals(page2);
        }
        public static bool operator !=(MangaPage page1, MangaPage page2) {
            return !page1.Equals(page2);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}