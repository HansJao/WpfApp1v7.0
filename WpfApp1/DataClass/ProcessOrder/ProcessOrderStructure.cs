using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;

namespace WpfApp1.DataClass.ProcessOrder
{
    public class ProcessOrderStructure
    {
        public Entity.ProcessOrder ProcessOrder { get; set; }
        public IEnumerable<ProcessOrderColorGroup> ProcessOrderColorGroups { get; set; }
    }

    public class ProcessOrderColorGroup
    {
        public ProcessOrderColorDetail ProcessOrderColorDetail { get; set; }
        public IEnumerable<ProcessOrderFlowDateDetail> ProcessOrderFlowDateDetails { get; set; }
        public IEnumerable<FactoryShippingName> FactoryShippings { get; set; }

    }

}
