using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The error class is used in parsing responses. The presence of the error class indicates
    /// that there was a problem with the transaction and it's properties should be interogated.
    /// </summary>
    public class error
    {
        public string longText { get; set; }
        public int primaryCode { get; set; }
        public int secondaryCode { get; set; }
        public string shortText { get; set; }
    }
}
