using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The device class is used to identify the PIN Pad / Swipe / Dip / Tap device terminal.
    /// </summary>
    public class device
    {
        public string terminalID { get; set; }
        public capability capability { get; set; }
        public string[] info { get; set; }
        public string[] lineitems { get; set; }
        public prompt prompt { get; set; }
        public promptInput promptInput { get; set; }
        public promptConfirmation promptConfirmation { get; set; }
    }
}
