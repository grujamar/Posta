<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sertifikaciono telo Pošte</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
        <!--Confirmation box before closing tab. This code will always display alert message (because it's always true) at the time of both tab closing and reloading.-->
        var new_var = true;
        window.onbeforeunload = function () {
            if (new_var) {
                return "You have unsaved changes, if you leave they will be lost!"                
            }
        }
        function unhook() {
            new_var = false;
        }
    </script>
</head>
<body>
   <form id="form1" runat="server">
        <div class="navbar navbar-default" id="navbar" style="background-color:#dbdbdb;">
            <div class="navbar-container" id="navbar-container">
               <div class="navbar-header pull-left">
                    <asp:Image ID="logo" imageurl="~/Content/Images/Posta.gif" runat="server" style="height:35px; margin-top: 4px;"/>
                </div>
                        <a class="navbar-brand">
                            <span style="color:darkblue">
                                Sertifikaciono telo Pošte
                            </span>
                        </a>           
            </div>
        </div>
        <div class="row">
                <br />
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <div style="margin:0px; padding:5px 0px;margin-bottom:5px;">
                1. Izdavanje elektronskog sertifikata:
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <!--_blank  Opens the linked document in a new window or tab-->
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/zahtev-izdavanje-fizicko-lice.aspx" onclick="unhook()" target="_blank">1. Zahtev za izdavanje kvalifikovanog elektronskog sertifikata za fizičko lice</asp:HyperLink>
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG.aspx" onclick="unhook()" target="_blank">2. Zahtev za izdavanje kvalifikovanog elektronskog sertifikata bez JMBG za stranca kao fizičkog lica</asp:HyperLink>
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/zahtev-izdavanje-pravno-lice.aspx" onclick="unhook()" target="_blank">3. Zahtev za izdavanje kvalifikovanog elektronskog sertifikata za pravno lice ili državni organ</asp:HyperLink>
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/zahtev-izdavanje-pravno-lice-stranac-bez-JMBG.aspx" onclick="unhook()" target="_blank">4. Zahtev za izdavanje kvalifikovanog elektronskog sertifikata bez JMBG za stranca kao pripadnika pravnog lica</asp:HyperLink>
            </div>
        </div>
            <br />
        <div class="container-fluid" style="margin-left: 10px">
            <div style="margin:0px; padding:5px 0px;margin-bottom:5px;">
                2. Podnošenje zahteva za promenu statusa elektronskog sertifikata:
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/zahtev-promena-statusa.aspx" onclick="unhook()" target="_blank">Zahtev za promenu statusa elektronskog sertifikata</asp:HyperLink>
            </div>
        </div>
            <br />
        <div class="container-fluid" style="margin-left: 10px">
            <div style="margin:0px; padding:5px 0px;margin-bottom:5px;">
                3. Podnošenje zahteva za deblokadu smart kartice/USB tokena:
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="~/zahtev-deblokada.aspx" onclick="unhook()" target="_blank">Podnošenje zahteva za deblokadu smart kartice/USB tokena</asp:HyperLink>
            </div>
        </div>
            <br />
        <div class="container-fluid" style="margin-left: 10px">
            <div style="margin:0px; padding:5px 0px;margin-bottom:5px;">
                4. Dobijanje koda (Response) za deblokadu smart kartice/USB tokena:
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink7" runat="server" NavigateUrl="~/kod-za-deblokadu.aspx" onclick="unhook()" target="_blank">Dobijanje koda (Response) za deblokadu smart kartice/USB tokena</asp:HyperLink>
            </div>
        </div>
            <br />
        <div class="container-fluid" style="margin-left: 10px">
            <div style="margin:0px; padding:5px 0px;margin-bottom:5px;">
                5. Provera statusa zahteva:
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink8" runat="server" NavigateUrl="~/provera-statusa-zahteva.aspx" onclick="unhook()" target="_blank">Provera statusa zahteva</asp:HyperLink>
            </div>
        </div>
            <br />
        <div class="container-fluid" style="margin-left: 10px">
            <div style="margin:0px; padding:5px 0px;margin-bottom:5px;">
                6. Provera datuma isticanja elektronskog sertifikata:
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink9" runat="server" NavigateUrl="~/provera-datuma-isticanja.aspx" onclick="unhook()" target="_blank">Provera datuma isticanja elektronskog sertifikata</asp:HyperLink>
            </div>
        </div>
            <br />
        <div class="container-fluid" style="margin-left: 10px">
            <div style="margin:0px; padding:5px 0px;margin-bottom:5px;">
                7. Provera opozvanosti elektronskog sertifikata:
            </div>
            <div style="margin: 0px; padding: 5px 0px;">
                <asp:HyperLink ID="HyperLink10" runat="server" NavigateUrl="~/provera-opozvanosti-sertifikata.aspx" onclick="unhook()" target="_blank">Provera opozvanosti elektronskog sertifikata</asp:HyperLink>
            </div>
        </div>
            <br />
        <div class="container-fluid" style="margin-left: 10px">
            <div style="margin:0px; padding:5px 0px;margin-bottom:5px;">
                8. Preuzimanje softverskog elektronskog sertifikata kao PKCS#12 datoteke:
            </div>
            <div style="margin:0px; padding:5px 0px;">
                <asp:HyperLink ID="HyperLink12" runat="server" NavigateUrl="~/preuzimanje-sertifikata-pkcs12.aspx" onclick="unhook()" target="_blank">Preuzimanje softverskog elektronskog sertifikata kao PKCS#12 datoteke</asp:HyperLink>
            </div>
        </div>
            <br />
    </form>
</body>
</html>
