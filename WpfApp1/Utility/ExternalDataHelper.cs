using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Fabric;

namespace WpfApp1.Utility
{
    public class ExternalDataHelper
    {
        public IEnumerable<TextileNameMapping> GetTextileNameMappings()
        {
            var textileNameMappingFilePath = string.Concat(AppSettingConfig.TextileNameMappingFilePath());
            //this code segment read data from the file.
            FileStream fs2 = new FileStream(textileNameMappingFilePath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs2);
            var cacheJson = reader.ReadToEnd();
            IEnumerable<TextileNameMapping> textileNameMappings = JsonConvert.DeserializeObject<IEnumerable<TextileNameMapping>>(cacheJson);
            reader.Close();
            return textileNameMappings;
        }
    }
}
