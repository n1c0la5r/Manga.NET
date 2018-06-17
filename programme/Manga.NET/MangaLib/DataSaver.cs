using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MangaLib {
    public abstract class DataSaver {
        public abstract Task SaveData(Library l);
        public abstract Task<Library> RestoreData();
    }
}