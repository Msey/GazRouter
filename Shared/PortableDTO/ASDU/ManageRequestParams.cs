using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.ASDU
{
    public enum ManageRequestAction
    {
        /* In */
        Load = 1,
        Read = 2,
        Apply = 3,
        /* Out */
        Create = 100,
        UpdateName = 101,
        Delete = 102,
        GenerateXml = 103,
        Send = 104,
        /* Metadata */
        LoadMeta = 200
    }
    public class ManageRequestParams
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Xml { get; set; }
        public string UserId { get; set; }
        public string EnterpriseId { get; set; }
        public ManageRequestAction Action { get; set; }
    }
}
