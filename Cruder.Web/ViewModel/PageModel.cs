using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cruder.Web.ViewModel
{
    public class PageModel
    {
        public int CurrentPage { get; set; }

        public int RecordPerPage { get; set; }

        public int TotalRecord { get; set; }

        /* That can set by only constructor */
        public int TotalPage { get; private set; }

        public PageModel(int currentPage, int recordPerPage, int totalRecord)
        {
            this.CurrentPage = currentPage;
            this.RecordPerPage = recordPerPage;
            this.TotalRecord = totalRecord;

            this.TotalPage = Calculate();
        }

        private int Calculate()
        {
            if (TotalRecord % RecordPerPage == 0)
            {
                return TotalRecord / RecordPerPage;
            }
            else
            {
                return (TotalRecord / RecordPerPage) + 1;
            }
        }

    }
}
