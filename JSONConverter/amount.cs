using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The Amount class contains various types of amounts that are commonly sent and received.
    /// </summary>
    public class amount
    {
        public double tip { get; set; }
        public double total { get; set; }
        public double tax { get; set; }
        public double cashback { get; set; }
        public double surcharge { get; set; }
    }
}
