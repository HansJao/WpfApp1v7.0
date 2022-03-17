using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.Utility
{
    public class ExternalDataHelper
    {
        public IEnumerable<TextileNameMapping> GetTextileNameMappings()
        {
            string textileNameMappingFilePath = string.Concat(AppSettingConfig.TextileNameMappingFilePath());
            //this code segment read data from the file.
            FileStream fs2 = new FileStream(textileNameMappingFilePath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs2);
            string cacheJson = reader.ReadToEnd();
            IEnumerable<TextileNameMapping> textileNameMappings = JsonConvert.DeserializeObject<IEnumerable<TextileNameMapping>>(cacheJson);
            reader.Close();
            return textileNameMappings;
        }

        public TrashItem GetTrashItemFromInventoryMapping(IEnumerable<TrashItem> trashItems, string textileName, string textileColor, IEnumerable<TextileNameMapping> textileNameMappings)
        {
            TextileNameMapping textileNameMapping = textileNameMappings.ToList().Find(f => f.Inventory.Contains(textileName)) ?? new TextileNameMapping();
            string accountTextileNameMapping = textileNameMapping.Account == null ? string.Empty : textileNameMapping.Account.FirstOrDefault().Split('*')[0];

            TrashItem trashItem = new TrashItem();
            trashItem = trashItems.Where(w => w.I_03 == string.Concat(accountTextileNameMapping, textileColor)).FirstOrDefault();
            if (accountTextileNameMapping != string.Empty && (trashItem == null || trashItem.I_03 == null))
                trashItem = trashItems.Where(w => w.I_03.Contains(accountTextileNameMapping) && w.I_03.Contains(textileColor)).FirstOrDefault();
            return trashItem;
        }
    }
}
