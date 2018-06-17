using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MangaLib;

namespace LocalDataSaverLib {
    public class LocalDataSaver : DataSaver {
        static string xmlFile = "LibraryData.xml";
        static DataContractSerializer serializer = new DataContractSerializer(typeof(Library));

        /// <summary>
        /// Save Library into LibraryData.xml
        /// </summary>
        /// <param name="l">Library object</param>
        /// <returns>Task</returns>
        public override async Task SaveData(Library l) {
            using(Stream s = File.Create("LibraryData.xml")) {
                await Task.Run(() => serializer.WriteObject(s, l));
            }
        }
        
        /// <summary>
        /// Deserialize and return library from LibraryData.xml
        /// </summary>
        /// <returns>Task<Libary></returns>
        public override async Task<Library> RestoreData() {
            Library lib;
            try {
                using(Stream s = File.OpenRead(xmlFile)) {
                    lib = await Task.Run(() => serializer.ReadObject(s) as Library);
                }
            }
            catch(FileNotFoundException) {
                return null;
            }
            return lib;
        }
    }
}