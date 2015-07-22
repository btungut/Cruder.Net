using System;
using System.Collections.Generic;
using System.Linq;

namespace Cruder.Core.Repository
{
    public class QueryOrdering : IEnumerable<QueryOrderItem>
    {
        private readonly List<QueryOrderItem> list = null;

        public QueryOrdering()
        {
            this.list = new List<QueryOrderItem>();
        }

        public QueryOrderItem this[int index]
        {
            get { return this.ElementAt(index); }
        }

        public IEnumerator<QueryOrderItem> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Insert(QueryOrderItem queryOrderItem)
        {
            if (queryOrderItem == null) throw new ArgumentNullException("queryOrderItem");
            if (list.Any(q => q.OrderBy == queryOrderItem.OrderBy)) throw new ArgumentException("An order item with the same key has already been added.");

            list.Add(queryOrderItem);
        }

        public void Add(QueryOrderItem queryOrderItem)
        {
            Insert(queryOrderItem);
        }

        public void Add(string orderBy)
        {
            Insert(new QueryOrderItem(orderBy));
        }

        public void Add(string orderBy, OrderType orderType)
        {
            Insert(new QueryOrderItem(orderBy, orderType));
        }
    }

    public class QueryOrderItem
    {
        public string OrderBy { get; set; }
        public OrderType OrderType { get; set; }

        public QueryOrderItem(string orderBy):this(orderBy, OrderType.Ascending)
        {
        }

        public QueryOrderItem(string orderBy, OrderType orderType)
        {
            this.OrderBy = orderBy;
            this.OrderType = orderType;
        }

        public override bool Equals(object obj)
        {
            bool retVal = false;

            QueryOrderItem item = obj as QueryOrderItem;

            if (item != null &&
                item.OrderBy == this.OrderBy &&
                item.OrderType == this.OrderType)
            {
                retVal = true;
            }

            return retVal;
        }

        public override string ToString()
        {
            string retVal = null;

            if (!string.IsNullOrEmpty(OrderBy))
            {
                switch (OrderType)
                {
                    case OrderType.Ascending: retVal = string.Format("{0} asc", OrderBy);
                        break;
                    case OrderType.Descending: retVal = string.Format("{0} desc", OrderBy);
                        break;
                }
            }

            return retVal;
        }
    }
}
