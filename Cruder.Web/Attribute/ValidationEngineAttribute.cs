using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cruder.Web
{
    public class ValidationEngineAttribute : System.Attribute
    {
        public string ValidationClass { get; private set; }

        public ValidationEngineAttribute(string validationClass)
        {
            this.ValidationClass = validationClass;
        }
    }
}
