using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Balances.Routes.Exceptions;
using GazRouter.DTO.Dictionaries.EntityTypes;


namespace GazRouter.DTO.Balances.Routes
{
    [DataContract]
	public class RouteDTO
    {
        public RouteDTO()
        {
            SectionList = new List<RouteSectionDTO>();
            ExceptionList = new List<RouteExceptionDTO>();
        }


        [DataMember]
        public int? RouteId { get; set; }
        
        [DataMember]
        public Guid InletId { get; set; }

        [DataMember]
        public Guid OutletId { get; set; }

        [DataMember]
        public string OutletName { get; set; }

        [DataMember]
        public EntityType OutletType { get; set; }

        [DataMember]
        public double? Length { get; set; }
        
        [DataMember]
        public double? CalcLength { get; set; }


        public int? ExceptionsCount => HasExceptions ? ExceptionList.Count : (int?)null;

        public bool HasExceptions => ExceptionList.Count > 0;

        [DataMember]
        public List<RouteSectionDTO> SectionList { get; set; }

        [DataMember]
        public List<RouteExceptionDTO> ExceptionList { get; set; }
    }
}