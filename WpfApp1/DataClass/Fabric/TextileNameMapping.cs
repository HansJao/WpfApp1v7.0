using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Fabric
{
    public class TextileNameMapping
    {
        public IEnumerable<string> ProcessOrder { get; set; }
        public IEnumerable<string> Inventory { get; set; }
        public IEnumerable<string> Account { get; set; }
    }
}
