using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The card class is used to identify card properties and sub-components.
    /// </summary>
    public class card
    {
        public string entryMode { get; set; }
        public int expirationDate { get; set; }
        public string levelResult { get; set; }
        public string number { get; set; }
        public string present { get; set; }
        public string trackData { get; set; }
        public string type { get; set; }

        public securityCode securityCode;

        public balance balance;

        public token token;



    }
}
