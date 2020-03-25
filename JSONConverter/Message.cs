using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The message class represents a class structure of all potential JSON messages
    /// that could be sent or received from the RESTful interface. 
    /// </summary>
    public class Message
    {
        public string dateTime { get; set; }
        public string[] apiOptions { get; set; }
        public amount amount { get; set; }
        public clerk clerk { get; set; }
        public card card { get; set; }
        public customer customer { get; set; }
        public emv emv { get; set; }
        public transaction transaction { get; set; }
        public result[] result { get; set; }
        public p2pe p2pe { get; set; }
        public device device { get; set; }
        public credential credential { get; set; }
        public signature signature { get; set; }



        static void Main(string[] args) { } 
    }
}
