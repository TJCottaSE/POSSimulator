using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The p2pe class is used for sending P2PE data blocks to the RESTful interface.
    /// </summary>
    public class p2pe
    {
        public string data { get; set; }
        public string format { get; set; }
        public string ksn { get; set; }
    }
}
