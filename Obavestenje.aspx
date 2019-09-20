<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Obavestenje.aspx.cs" Inherits="Obavestenje" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Obaveštenje</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
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
                            <asp:Label id="lblpostanaziv" runat="server" style="font-size:17px; color:darkblue">
                                <span style="color:darkblue">                                
                                                                    
                                </span>
                            </asp:Label>
                        </a>           
            </div>
        </div>
        <div class="row">
                
        </div>
        <div class="container-fluid" style="margin-left: 10px;">
            <div class="row">
                    <br />
                    <br />
            </div>
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="spanobavestenje" runat="server" style="COLOR: red; font-weight:bold; font-size:20px;"></asp:Label>&nbsp;
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
        <!---------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                        <div class="col-sm-2">
                            <asp:Label id="lbldugme2" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                        </div>
                        <div class="col-sm-8">
                            <asp:Button ID="btnBack" runat="server" class="btn-lg btn-primary buttonborder" Text="" onclick="btnBack_Click1" onclientclick="unhook()"/>                           
                        </div>
                        <div class="col-sm-2">
                            <asp:Label id="lbldugme3" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                        </div>
            </div>  
        </div>
        <div class="container-fluid" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row">
                    <br />
            </div>
        </div>  
        <div class="row">
                <br />
                <br />
        </div>
    </form>
</body>
</html>
