using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The signature class is used to send or receive a signature block from a device.
    /// </summary>
    public class signature
    {
        public string data { get; set; }
        public string format { get; set; }
    }
}
