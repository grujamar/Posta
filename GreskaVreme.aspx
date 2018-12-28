<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GreskaVreme.aspx.cs" Inherits="GreskaVreme" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Greška-isteklo vreme</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
        function erroralertTime() {
            swal({
                title: 'Greška prilikom kreiranja sertifikata.',
                text: 'Isteklo je vreme za kreiranje.',
                type: 'OK'
            });
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
                            <asp:Label id="lblpostanaziv" runat="server" style="font-size:17px; color:darkblue">
                                <span style="color:darkblue">                                
                                                                    
                                </span>
                            </asp:Label>
                        </a>           
            </div>
        </div>
        <div class="row">
                <br />
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">                                 
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:Label id="spangreskavreme" runat="server" style="COLOR: red; font-weight:bold; font-size:20px;"></asp:Label>&nbsp;
                        </div>
                        <div class="col-sm-2" style="background-color:white;">

                        </div>
            </div>
        </div>
        <div class="container-fluid" style="margin-left: 10px;">
            <div class="row">
                    <br />
                    <br />
            </div>
        </div>
    </form>
</body>
</html>
