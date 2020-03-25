using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The securityCode class is used to send or receive CVV2 information.
    /// </summary>
    public class securityCode
    {
        public string indicator { get; set; }
        public string value { get; set; }
        public string result { get; set; }
        public string valid { get; set; }
    }
}
