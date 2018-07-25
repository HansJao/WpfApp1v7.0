using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.StoreSearch
{
    public class StoreSearchData<T>
    {
        public string TextileName { get; set; }
        public List<T> StoreSearchColorDetails { get; set; }
    }
}
