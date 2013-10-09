<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Leitor.EditorWeb.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link type="text/css" rel="stylesheet" href="/css/style.css" />
    <script src="http://code.jquery.com/jquery-1.10.1.min.js"></script>

<%--    <script>
        function textAreaAdjust(o) {
            o.style.width = "700px"
            o.style.height = "1px";
            o.style.height = (25 + o.scrollHeight) + "px";
        }

        $(document).ready(function () {
            $("#txtXML").css('height', '1000')
            
        })
    </script>--%>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            Documento:
            <asp:TextBox ID="txtDocumento" runat="server" />
        </div>
        <div>
            Prefeitura:
            <asp:DropDownList ID="ddlPrefeituras" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPrefeituras_SelectedIndexChanged" />
        </div>
        <div>
            </p>Expressão regular:</p>
            <asp:TextBox ID="txtRegex" runat="server" TextMode="MultiLine" />
        </div>
        <div>
            Parsear Documentento:
            <asp:Button ID="btParse" runat="server" OnClick="btParse_Click" Text="Parse" />
        </div>
            
    </div>
        <p></p>
        <asp:Panel ID="pnlResult" runat="server" CssClass="boxResult">
                <asp:Literal ID="litControl" runat="server"></asp:Literal>          
        </asp:Panel>

        <asp:Panel ID="pnlXml" runat="server" CssClass="boxResult">       
                <asp:TextBox ID="txtXML" runat="server" TextMode="MultiLine" CssClass="boxXML" 
                    onkeyup="textAreaAdjust(this)"></asp:TextBox>
        </asp:Panel>

    </form>
</body>
</html>
