using System;
using System.Collections.Generic;
using System.Text;

namespace MangaLib {
    public class AlreadyExistsException : Exception {
        public AlreadyExistsException(string msg) : base(msg) { }
    }
    public class NoSuchElementException : Exception {
        public NoSuchElementException(string msg) : base(msg) { }
    }
}
