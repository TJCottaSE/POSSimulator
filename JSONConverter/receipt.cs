using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    ///  The receipt part class is used to parse out and hold certain parts of the receipt
    ///  text that gets returned from the RESTful interface.
    /// </summary>
    public class receipt
    {
        public string key { get; set; }
        public string printName { get; set; }
        public string printValue { get; set; }
    }
}
