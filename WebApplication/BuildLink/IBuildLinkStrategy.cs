using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.BuildLink
{
    public interface IBuildLinkStrategy
    {
        public string BuildLinkDetail(string strategyName, int strategyId);
        public string GetCurrentURL(int? pageIndex);
        public string BuildLinkImage(string imageName);
    }
}
