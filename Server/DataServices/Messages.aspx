<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Messages.aspx.cs" Inherits="GazRouter.DataServices.Messages" EnableEventValidation="true" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head >
    <title>Messages</title>
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    </style>
</head>
    <body>
        <form id="form1" runat="server" >
            <div>
                        <label style="font-size: 50px">Эмулятор PSIComCentre</label>
                        <br/>
                        <asp:TextBox runat="server" TextMode="MultiLine" Width="500" Height="500" ID="tbMessage"></asp:TextBox>
                        <br/>
                        <asp:Button runat="server" Text="Отправить в ИУС П" ID="bSend" OnClick="OnSendClick"/>
                        <br/>
                <asp:DataList ID="DataList1" runat="server" >
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:HiddenField ID="Id" runat="server" Value='<%#Eval("Key") %>'/>
                        <asp:Label ID="fileName" runat="server" Text='<%#Eval("Value") %>' />
                        <asp:Button ID="bLoad" Text="Загрузить" runat="server" CommandArgument='<%#Eval("Key") %>' CommandName="Select" OnCommand="bLoad_OnCommand"/> 
                    </ItemTemplate>
                </asp:DataList>
            </div>
        </form>
    </body>
</html>
