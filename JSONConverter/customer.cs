using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The customer class contains all of the common properties of a customer.
    /// </summary>
    public class customer
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string postalCode { get; set; }
        public string streetAddress { get; set; }
        public string addressLine1 { get; set; }
    }
}
