using MangaLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AppDataSaverLib {
    public class AppDataSaver : DataSaver {
        static string fileName = "LibraryData.xml";
        static DataContractSerializer serializer = new DataContractSerializer(typeof(Library));
        static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        public AppDataSaver() { }
        public AppDataSaver(string fileName) {
            AppDataSaver.fileName = fileName;
        }

        /// <summary>
        /// Save Library
        /// </summary>
        /// <param name="l">Library object</param>
        /// <returns>Task</returns>
        public override async Task SaveData(Library l) {
            StorageFile sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using(Stream stream = await sampleFile.OpenStreamForWriteAsync()) {
                serializer.WriteObject(stream, l);
            }
        }

        /// <summary>
        /// Deserialize and return library from LibraryData.xml
        /// </summary>
        /// <returns>Task<Libary></returns>
        public override async Task<Library> RestoreData() {
            StorageFile sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            Library lib;

            using(Stream stream = await sampleFile.OpenStreamForReadAsync()) {
                try {
                    lib = serializer.ReadObject(stream) as Library;
                }
                catch {
                    return null;
                }
            }

            return lib;
        }
    }
}
