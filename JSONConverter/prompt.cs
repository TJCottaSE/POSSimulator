using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The prompt class is used in the context of sending a prompt to a particular device.
    /// </summary>
    public class prompt
    {
        public string cardSecurityCode { get; set; }
        public string postalCode { get; set; }
        public string streetAddress { get; set; }
    }
}
