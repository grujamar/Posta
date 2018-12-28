<%@ Page Language="C#" AutoEventWireup="true" CodeFile="provera-statusa-zahteva.aspx.cs" Inherits="provera_statusa_zahteva" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Provera statusa zahteva</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
        $(document).ready(function () {
            $('#btnSubmit').click(function () {
                $.blockUI({
                    message: '<p style="font-size:20px; font-weight: bold;"><b>Molimo sačekajte...</b></p><img src="throbber.gif" runat="server" style="width:35px;height:35px;"/>',
                    css: {
                        border: 'none',
                        padding: '15px',
                        backgroundColor: '#000',
                        '-webkit-border-radius': '10px',
                        '-moz-border-radius': '10px',
                        opacity: .5,
                        color: '#fff',
                        left: '25%',
                        width: '50%',
                        onBlock: function () {
                            pageBlocked = true;
                        }
                    }
                });
            });
        });
        var new_var = true;
        window.onbeforeunload = function () {
            if (new_var) {
                return "You have unsaved changes, if you leave they will be lost!"                
            }
        }
        function unhook() {
            new_var = false;
        }
        function successalert() {
            swal({
                title:'Uspešno dobijen status zahteva.',
                text: 'Za navedeni broj zahteva je uspešno dobijen status zahteva.',
                type: 'OK'
            });
        }
        function erroralertSendSOAP() {
            swal({
                title: 'Greška prilikom slanja zahteva.',
                text: 'Kontaktirajte tehničku podršku ili pokušajte ponovo kasnije.',
                type: 'OK'
            });
        }
        function erroralert() {
            swal({
                title: 'Greška prilikom podnošenja zahteva.',
                text: 'Ispravite podatke i pokušajte ponovo.',
                type: 'OK'
            });
        }
        function errorSOAPalert() {
            swal({
                title: 'Greška u slanju zahteva, nije uspelo slanje SOAP poruke.',
                text: 'Popunite zahtev kasnije i pokušajte ponovo.',
                type: 'OK'
            });
        }
        function Notification() {
            swal({
                title: 'Obaveštenje.',
                text: 'Za navedeni broj zahteva nema statusa.',
                type: 'OK'
            });
        }
        function keydownFunctionRequest()
           {
               var requestpost = document.getElementById('<%=txtbrojzahteva.ClientID %>');

            if (requestpost.value.length == 0 || requestpost.value.length == 1)
                   document.getElementById('errLabel').style.display = 'none';
        }
    </script>
    <style type="text/css">
    .save {
    padding: 15px 30px 15px 43px;
    background: url(Content/Images/save.png) left 12px top 13px no-repeat #4275c9;
    }
    .edit {
        padding: 15px 30px 15px 43px;
        background: url(Content/Images/edit.png) left 12px top 13px no-repeat #FFA500;
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
        <div class="container-fluid top10">
                <asp:Image ID="pdfdocument" imageurl="~/Content/Images/pdf_icon.png" runat="server" style="width:20px;" />
                <asp:HyperLink id="pdfhyperlink1" runat="server" NavigateUrl="~/dokumentacija/Uputstvo-provera-statusa-zahteva.pdf" target="_blank" style="vertical-align:bottom"><asp:Label id="lblkorisniskouputstvo" runat="server" style="font-size:15px;"></asp:Label></asp:HyperLink>
        </div>
        <div class="row">
                    <br />
        </div> 
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">
                        <div class="col-sm-1">
                            <asp:Label id="spanbrojzahteva" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblbrojzahteva" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                        </div>
                        <div class="col-sm-11">
                             <asp:TextBox ID="txtbrojzahteva" runat="server" class="txtbox1" style="font-size:13px;" maxlength="8"></asp:TextBox>                            
                             <asp:CustomValidator runat="server" id="cvbrojzahteva" controltovalidate="txtbrojzahteva" errormessage="" OnServerValidate="cvbrojzahteva_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>                       
            </div>
            <div class="row top10">
                <div class="col-sm-1">
                </div>
                <div class="col-sm-11">
                        <asp:Label ID="lblnotification" runat="server" style="font-size:13px;"></asp:Label>
                </div>
            </div>
        </div>
        <!-------------------------------------------OVO SE NE VIDI------------------------------------------------>       
        <cc1:ToolkitScriptManager ID="ToolkitScriptManager2" runat="server"></cc1:ToolkitScriptManager>
            <asp:UpdatePanel id="UpdatePanel5" runat="server">
              <ContentTemplate>
                 <fieldset>
                         <div class="container-fluid" id="Container00" runat="server" style="margin-left: 10px">
                            <div class="row top10">
                                        <div class="col-sm-1">
                                            <asp:Label id="spanstatus" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblstatus" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                        </div>
                                        <div class="col-sm-11">
                                             <asp:DropDownList ID="ddlListaSertifikata" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox7" OnSelectedIndexChanged="ddlListaSertifikata_SelectedIndexChanged">
                                             </asp:DropDownList>
                                            <asp:CustomValidator runat="server" id="cvlistasertifikata" controltovalidate="ddlListaSertifikata" errormessage="" OnServerValidate="cvlistasertifikata_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                        </div>
                            </div>
                         </div>
                         <div class="container-fluid" id="Container0" runat="server" style="margin-left: 10px">
                            <div class="row top10">
                                        <div class="col-sm-1">
                                            <asp:Label id="spancertificate" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblcertificate" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                        </div>
                                        <div class="col-sm-11">
                                             <asp:TextBox ID="txtSertifikat" runat="server" class="txtbox7" style="font-size:13px; background-color: #f5f5f5;" maxlength="100" ReadOnly="true"></asp:TextBox>
                                             <asp:CustomValidator runat="server" id="cvsertifikat" controltovalidate="txtSertifikat" errormessage="" OnServerValidate="cvsertifikat_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                        </div>
                            </div>
                         </div>
                         <div class="container-fluid" id="Container000" runat="server" style="margin-left: 10px">
                            <div class="row top10">
                                        <div class="col-sm-1">
                                            <asp:Label id="spanpostexpressid" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblpostexpressid" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                        </div>
                                        <div class="col-sm-11">
                                             <asp:TextBox ID="txtPostExpressBroj" runat="server" class="txtbox7" style="font-size:13px; background-color: #f5f5f5;" maxlength="50" ReadOnly="true"></asp:TextBox>
                                        </div>
                            </div>
                         </div>   
                        <div class="row">
                                <br />
                                <br />
                        </div>
                        <!-------------------------------------------OVO SE NE VIDI------------------------------------------------>
                        <div class="container-fluid" id="Container1" runat="server" style="margin-left: 10px">
                            <div class="row top10">
                                    <div class="col-sm-1">
                                        <asp:Label id="spanstatuszahteva" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblstatuszahteva" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> </label>
                                    </div>
                                    <div class="col-sm-11">
                                            <asp:TextBox ID="txtstatus" runat="server" class="txtboxMultiline" AutoPostBack="true" TextMode="MultiLine" Rows="5" style="font-size:13px; background-color: #f5f5f5;" maxlength="50" ReadOnly="true"></asp:TextBox>
                                            <asp:CustomValidator runat="server" id="cvstatus" controltovalidate="txtstatus" errormessage="" OnServerValidate="cvstatus_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                    </div>
                            </div>
                        </div>
                   </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
        <!---------------------------------------------------------------------------------------------------------->
        <!---------------------------------------------------------------------------------------------------------->
        <!---------------------------------------------------------------------------------------------------------->
        <!---------------------------------------------------------------------------------------------------------->
        <div class="row">
                <br />
                <br />
        </div>
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                        <div class="col-sm-1">
                            <label for="lbldugme" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                        </div>
                        <div class="col-sm-9">
                            <asp:Button ID="btnSubmit" runat="server" class="btn-lg btn-primary buttonborder" Text="" onclick="btnSubmit_Click1" onclientclick="unhook()"/>                           
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
                <br />
        </div>
        <div class="row">
                <br />
                <br />
        </div>
        <!---------------------------------------------------------------------------------------------------------->
        <!---------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" style="margin-left: 10px;">
            <div class="row">
                    <br />
            </div>
        </div>  
        <div class="row">
                <br />
                <br />
        </div>
        <div id="throbber" style="display:none;">
            <p style="font-size:20px; font-weight: bold;"><b>Molimo sačekajte...</b></p>
            <asp:Image ID="imgThrobber" imageurl="~/Content/Gif/throbber.gif" runat="server" style="width:35px;height:35px;"/>
        </div>
    </form>
</body>
</html>
