using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;

namespace WpfApp1.DataClass.Fabric
{
    public class ProcessSequenceCost : ProcessSequence
    {
        private float _cost { get; set; }
        public float Cost { get { return _cost; } set { _cost = value; OnPropertyChanged(); } }

    }
}
