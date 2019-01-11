using System;
using System.Collections.Generic;
using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.DTO.ManualInput
{
    public class GetSiteStatusListParameters
    {
        public GetSiteStatusListParameters()
        {
            Sites = new List<SiteDTO>();
        }

        public int? SerieId { get; set; }
        public List<SiteDTO> Sites { get; set; }
        public DateTime? KeyDate { get; set; }
    }
}