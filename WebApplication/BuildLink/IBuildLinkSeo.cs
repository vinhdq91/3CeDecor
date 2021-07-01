using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.BuildLink
{
    public interface IBuildLinkSeo
    {
        public string AddMetaFacebook(string title, string description, string url, string image, string domainAndCropSize = "", int imgWidth = 620, int imgHeight = 324);
    }
}
