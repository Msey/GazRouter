<%@ Page Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="GazRouter.DataServices.Infrastructure" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>gazRouter</title>
      <link rel="shortcut icon" type="image/x-icon" href="/favicon.ico"  />
    <link rel="icon" type="image/png" href="/favicon-32x32.png" sizes="32x32" />
    <link rel="icon" type="image/png" href="/favicon-96x96.png" sizes="96x96" />
    <link rel="icon" type="image/png" href="/favicon-16x16.png" sizes="16x16" />
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    #silverlightControlHost {
	    height: 100%;
	    text-align:center;
    }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
              appSource = sender.getHost().Source;
            }
            
            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
              return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " +  appSource + "\n" ;

            errMsg += "Code: "+ iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {           
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " +  args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="height:100%">
    <div id="silverlightControlHost">
        <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
<%
    const string orgSourceValue = @"ClientBin/Client.xap";
    string param;
    if (Debugger.IsAttached)
        param = "<param name=\"source\" value=\"" + orgSourceValue + "\" />";
    else
    {
        var xappath = HttpContext.Current.Server.MapPath("") + @"\" + orgSourceValue;
        var xapCreationDate = File.GetLastWriteTime(xappath);
        param = "<param name=\"source\" value=\"" + orgSourceValue + "?ignore=" + xapCreationDate.Ticks.ToString() + "\" />";
    }
    Response.Write(param);
%>

          <param name="initParams" value="DispatherDayStartHour =<%= AppSettingsManager.DispatherDayStartHour%>,
                                            ServerTimeUtcOffset =<%= AppSettingsManager.ServerTimeUtcOffset %>,
                                          ServerAssemblyVersion =<%= AppSettingsManager.ServerAssemblyVersion%>,
                                          ServerAssemblyDateInTicks =<%= AppSettingsManager.ServerAssemblyDate.Ticks%>,EnterpriseId =<%= AppSettingsManager.CurrentEnterpriseId%>"/>
		  <param name="onError" value="onSilverlightError" />
		  <param name="background" value="white" />
		  <param name="minRuntimeVersion" value="5.0.61118.0" />
		  <param name="autoUpgrade" value="true" />
		  <param name="Windowless" value="true" />
		  <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		  </a>
	    </object><iframe id="_sl_historyFrame" style="visibility:hidden;height:0;width:0;border:0"></iframe></div>
    </form>
</body>
</html>
