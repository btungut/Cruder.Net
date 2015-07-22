using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cruder.Core.Repository
{
    public class QueryOptions
    {
        public QueryOrdering Ordering { get; set; }
        public QueryCriterias Criterias { get; set; }

        public QueryOptions()
        {
            this.Ordering = new QueryOrdering();
            this.Criterias = new QueryCriterias();
        }

        public QueryOptions(QueryOrdering ordering, QueryCriterias criterias)
        {
            this.Ordering = ordering;
            this.Criterias = criterias;
        }
    }
}
