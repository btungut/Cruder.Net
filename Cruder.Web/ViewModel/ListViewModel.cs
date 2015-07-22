using System;
using System.Collections.Generic;

namespace Cruder.Web.ViewModel
{
    public class ListViewModel<T>
    {
        public PageModel PageModel { get; set; }

        public List<T> Data { get; set; }

        public Type EntityType { get { return typeof(T); } }

        public T Entity { get; set; }
    }
}
