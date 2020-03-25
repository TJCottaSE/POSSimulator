using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The result class makes up everything that could be contained within a response 
    /// from the RESTful API.
    /// </summary>
    public class result
    {
        public string dateTime { get; set; }
        public amount amount { get; set; }
        public card card { get; set; }
        public clerk clerk { get; set; }
        public credential credential { get; set; }
        public customer customer { get; set; }
        public device device { get; set; }
        public emv emv { get; set; }
        public error error { get; set; }
        public merchant merchant { get; set; }
        public receipt[] receipt { get; set; }
        public server server { get; set; }
        public signature signature { get; set; }
        public transaction transaction { get; set; }
    }
}
