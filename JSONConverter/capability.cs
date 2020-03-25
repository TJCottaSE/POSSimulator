using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    public class capability
    {
        public string manualEntry { get; set; }
        public string magstripe { get; set; }
        public string contactlessMSR { get; set; }
        public string EMV { get; set; }
        public string quickChip { get; set; }
        public string contactlessEMV { get; set; }
    }
}
