using System;
using System.Collections.Generic;
using System.Text;

namespace MangaLib {
    public class IsLoadedEventArgs : EventArgs {
        public string LoadedData { get; set; }

        public IsLoadedEventArgs(string loadedData) {
            LoadedData = loadedData;
        }
    }
}
