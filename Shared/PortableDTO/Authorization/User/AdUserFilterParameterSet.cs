using System.Runtime.Serialization;
namespace GazRouter.DTO.Authorization.User
{ 
    [DataContract]
    public class AdUserFilterParameterSet
    {
        public AdUserFilterParameterSet(string searchString,
                                        string searchattribute,
                                        bool isEnterprise)
        {
            SearchString = searchString;
            SearchAttribute = searchattribute;
            IsEnterprise = isEnterprise;
            SearchByAccount = GetSearchAttribute() == "samaccountname" ? true : false;
        }

        [DataMember]
        public string SearchString { get; set; }
        [DataMember]
        public bool SearchByAccount { get; set; }

        [DataMember]
        public bool IsEnterprise { get; set; }

        [DataMember]
        public string SearchAttribute { get; set; }

        public string GetSearchAttribute()
        {
            switch (SearchAttribute)
            {
                case "логин"   : return "samaccountname";
                case "Ф.И.О."  : return "displayname";
                case "описание": return "description";

                default: return string.Empty;
            }
        }
    }
}
