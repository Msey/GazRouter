using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using GazRouter.DataServices.ExchangeServices;

namespace GazRouter.DataServices
{
    public partial class Messages : Page
    {
        private IEnumerable<KeyValuePair<string, string>> Files
        {
            get { return XmlMessageFileHelper.Files.Select(f => new KeyValuePair<string, string>(f, f)).ToList(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataList1.DataSource = Files;
                DataList1.DataBind();
            }
        }

        protected void bLoad_OnCommand(object sender, CommandEventArgs e)
        {
            var fileName = (string)e.CommandArgument;
            using (var reader = new StreamReader(XmlMessageFileHelper.FullPath(fileName)))
            {
                tbMessage.Text = reader.ReadToEnd();
            }
        }

        private ChannelFactory<IAsduExchangeService> _channelFactory;
        protected IAsduExchangeService GetChannel()
        {
            if ((_channelFactory == null) || (_channelFactory.State == CommunicationState.Faulted))
            {
                var binding = new BasicHttpBinding(Request.Url.Scheme == "https" ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None)
                {
                    SendTimeout = TimeSpan.FromMinutes(5),
                    ReceiveTimeout = TimeSpan.FromMinutes(5),
                    OpenTimeout = TimeSpan.FromMinutes(1),
                    CloseTimeout = TimeSpan.FromMinutes(1),
                    MaxReceivedMessageSize = 2147483647
                };

                var endpointAddress = new EndpointAddress(new Uri(Request.Url.AbsoluteUri.ToLower().Replace("/messages.aspx", "/ExchangeServices/AsduExchangeService.svc")));

                _channelFactory = new ChannelFactory<IAsduExchangeService>(binding, endpointAddress);
            }

            return _channelFactory.CreateChannel();
        }

        protected void OnSendClick(object sender, EventArgs e)
        {
            var channel = GetChannel();
            var result = channel.GetTask(tbMessage.Text);
            ((ICommunicationObject)channel).Close();
        }

    }

}