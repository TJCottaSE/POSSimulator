using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The avs class contains all of the Address Verificaiton System checks.
    /// </summary>
    public class avs
    {
        public string postalCodeVerified { get; set; }
        public string result { get; set; }
        public string streetVerified { get; set; }
        public string valid { get; set; }

    }
}
