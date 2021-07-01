using DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.BuildLink
{
    public interface IBuildLinkProduct
    {
        public string BuildLinkDetail(string title, int id);
        public string GetCurrentURL(int? pageIndex);
        public Task<string> BuildLinkProductSearch(ProductSearch objSearch);
    }
}
