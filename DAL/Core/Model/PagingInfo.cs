using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.Model
{
    [Serializable]
    public class PagingInfo
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string LinkSite { get; set; }
        public int Sort { get; set; }
        public int HasImage { get; set; }
        public int Count { get; set; }
    }
}
