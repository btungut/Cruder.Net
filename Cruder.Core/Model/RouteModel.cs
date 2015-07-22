using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cruder.Core.Model
{
    public class RouteModel
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public string HttpMethod { get; set; }
    }
}
