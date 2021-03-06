﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="zahtev-deblokada-podnet.aspx.cs" Inherits="zahtev_deblokada_podnet" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Zahtev za deblokadu smart kartice/USB tokena</title>
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
        function requestsend() {
            swal("Zahtev je uspešno poslat!", "Odštampane i potpisane dokumente dostavite u Sertifikaciono telo Pošte.");
        }
    </script>
    <style type="text/css">
        .submit1 {
        padding: 15px 30px 15px 43px;
        background: url(Content/Images/pdf_icon.png) left 5px top 10px no-repeat #e5e5e5;
        border-style: solid;
        border-width: 0.5px;
        cursor: pointer;
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
        <div class="container-fluid">
            <h4><b style="color:#6b6b6b;"><asp:Label id="lblstranicanaziv" runat="server" style="font-size:18px;"></asp:Label></b></h4>
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row">
                <br />
            </div>
            <div class="row">
                <br />
            </div>
            <div class="row">                                 
                        <div class="col-sm-2" style="background-color:white;">
                            <asp:Label id="lblbrojzahteva" runat="server" style="font-size:13px;"></asp:Label>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtbrojzahteva" runat="server" class="txtbox1" style="font-size:13px;" ReadOnly="true" BackColor="#F5F5F5"></asp:TextBox>
                        </div>
            </div>
            <div class="row top10">

            </div>
            <div class="row">                                 
                        <div class="col-sm-2" style="background-color:white;">
                           <asp:Label id="lbldatumzahteva" runat="server" style="font-size:13px;"></asp:Label> 
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtdatumzahteva" runat="server" class="txtbox1" style="font-size:13px;" ReadOnly="true" BackColor="#F5F5F5"></asp:TextBox>
                        </div>
            </div>
            <div class="row top10">

            </div>
            <div class="row">                                 
                        <div class="col-sm-2" style="background-color:white;">
                           <asp:Label id="lblcenasaporezom" runat="server" style="font-size:13px;"></asp:Label> 
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtcenasaporezom" runat="server" class="txtbox1" style="font-size:13px;" ReadOnly="true" BackColor="#F5F5F5"></asp:TextBox>
                            <asp:Label id="lbldinara" runat="server" style="font-weight:bold;font-size:13px;"></asp:Label>
                        </div>
            </div>
        </div>
        <div class="row">
                <br />
                <br />
        </div> 
        <div class="container-fluid" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <label for="lbldugme" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                </div>
                <div class="col-sm-8">
                    <asp:HyperLink ID="btnPrintRequest" runat="server" download="" target="_blank" class="btn-lg btn-default submit1" onclick="unhook();" style="margin-right: 8px;" Text=""/>
                    <asp:HyperLink ID="btnPrintPaymentOrder" runat="server" download="" target="_blank" class="btn-lg btn-default submit1" onclick="unhook();" style="margin-right: 8px;" Text=""/>                                                 
                </div>
                <div class="col-sm-2">
                    <label for="lbldugme1" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
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
        </div>
        <div class="container" style="margin-left: 10px;">
            <div class="row top10">
            <table class="table table-condensed">
                <p class="notification_price">
                   <asp:Label id="lblnotification10" runat="server" style="font-size:13px;">
                                    
                    </asp:Label> 
                </p>
                <tbody>
                  <tr>
                    <td><asp:Label id="spannotification11" runat="server" style="font-size:13px;">                                    
                    </asp:Label></td>
                    <td><asp:Label id="lblnotification11" runat="server" style="font-size:13px;">                                    
                    </asp:Label></td>
                  </tr>
                  <tr>
                    <td><asp:Label id="spannotification12" runat="server" style="font-size:13px;">                                    
                    </asp:Label></td>
                    <td><asp:Label id="lblnotification12" runat="server" style="font-size:13px;">                                    
                    </asp:Label></td>
                  </tr>
                  <tr>
                    <td><asp:Label id="spannotification13" runat="server" style="font-size:13px;">                                    
                    </asp:Label></td>
                    <td><asp:Label id="lblnotification13" runat="server" style="font-size:13px;">                                    
                    </asp:Label></td>
                  </tr>
                </tbody>                 
            </table>
            </div>
        </div>  
        <div class="row">
                <br />
                <br />
        </div> 
        <div class="row">
                <br />
        </div>
    </form>
</body>
</html>
