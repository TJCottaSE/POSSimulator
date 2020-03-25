using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    public class merchant
    {
        public string name { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string postalCode { get; set; }
        public string phone { get; set; }
        public string dayEndingTime { get; set; }
        public string industry { get; set; }
        public string serialNumber { get; set; }
        public string revision { get; set; }
        public int mid { get; set; }
        public cardTypes[] cardTypes { get; set; }
    }
}
