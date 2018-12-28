<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Uputstvo.aspx.cs" Inherits="Uputstvo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Uputstvo</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
        function errorOpeningPage() {
            swal({
                title: 'Greška prilikom otvaranja stranice.',
                text: 'Pokušajte ponovo kasnije.',
                type: 'OK'
            });
        }
    </script>
    <style>
        .arrowRight {
            padding: 15px 30px 15px 43px;
            background: url(Content/Images/arrowRight.png) left 12px top 13px no-repeat #ffffff;
            border:2px solid #D3D3D3;
        }
        </style>
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
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo1" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo2" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo3" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo4" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo5" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo6" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo7" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:Label id="lbluputstvo8" runat="server" style="font-size:13px;"></asp:Label>&nbsp;
                </div>
            </div>
        </div>
        <div class="row">
            <br />
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">                                 
                <div class="col-sm-10" style="background-color:white;">
                    <asp:CheckBox ID="CheckBox1" runat="server" style="font-size:14px;" AutoPostBack="true" OnCheckedChanged="CheckBox1_CheckedChanged" onclientclick="unhook()"/>
                </div>
            </div>
        </div>
        <div class="row">
            <br />
        </div>
        <!---------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <asp:Label id="lbldugme2" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                </div>
                <div class="col-sm-8">
                    <asp:Button ID="btnSubmit" runat="server" class="btn btn-default arrowRight" style="margin-right: 8px;" Text="" onclick="btnSubmit_Click1" onclientclick="unhook()"/>                           
                </div>
                <div class="col-sm-2">
                    <asp:Label id="lbldugme3" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                </div>
            </div>  
        </div>
        <div class="container-fluid" id="myDiv6" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
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
