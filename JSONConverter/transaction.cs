using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The transaction class contains the important properties of a given transaction.
    /// </summary>
    public class transaction
    {
        // Required
        public string invoice { get; set; }

        public purchaseCard purchaseCard;

        // Optional
        public string authorizationCode { get; set; }
        public string authSource { get; set; }
        public string notes { get; set; }
        public string responseCode { get; set; }
        public string businessDate { get; set; }
        public hotel hotel { get; set; }
        public auto auto { get; set; }
        public pin pin { get; set; }
        public avs avs { get; set; }

    }
}
