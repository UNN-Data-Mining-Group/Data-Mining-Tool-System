using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace dms.services.archivation
{
    class ArchivatorService
    {
        private static ArchivatorService sharedManager;

        public static ArchivatorService SharedManager
        {
            get
            {
                if (sharedManager == null)
                {
                    sharedManager = new ArchivatorService();
                }
                return sharedManager;
            }
        }

        public void exportSystem(string path)
        {
            var archive = ExportService.SharedManager.generateArchiveSystem();
            BinaryFormatter serializer = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, archive);
                File.WriteAllBytes(path, ms.ToArray());                
            }
        }

        public void importSystem(string path)
        {
            BinaryFormatter deserializer = new BinaryFormatter();
            byte[] byteArray = File.ReadAllBytes(path);
            using (var ms = new MemoryStream(byteArray, 0, byteArray.Length))
            {
                ms.Write(byteArray, 0, byteArray.Length);
                ms.Position = 0;
                models.archive.Archive obj = (models.archive.Archive)deserializer.Deserialize(ms);
                ImportService.SharedManager.importArchive(obj);
            }
        }
    }
}
