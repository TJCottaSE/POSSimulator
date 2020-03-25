using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The purchascard class is what represnet Level 2 card data.
    /// </summary>
    public class purchaseCard
    {
        public string customerReference { get; set; }
        public string destinationPostalCode { get; set; }
        public string[] productDescriptors { get; set; }

    }
}
