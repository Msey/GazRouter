using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GazRouter.DataServices.ExchangeServices.DispatcherTaskHandlers
{
    public partial class CorrectedTaskReceived : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("CorrectedTaskReceived - > success");
        }
    }
}