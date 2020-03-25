using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConverter
{
    /// <summary>
    /// The credential class is used in requesting an AccessToken from an AuthToken, clientGUID pair.
    /// </summary>
    public class credential
    {
        public string accessToken { get; set; }
        public string authToken { get; set; }
        public string clientGuid { get; set; }
    }
}
