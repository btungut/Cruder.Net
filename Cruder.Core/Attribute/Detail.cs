using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cruder.Core
{
    public class DetailAttribute : System.Attribute
    {
        private readonly string value;
        public string Value
        {
            get
            {
                return this.value;
            }
        }

        public DetailAttribute(string value)
        {
            this.value = value;
        }
    }
}
