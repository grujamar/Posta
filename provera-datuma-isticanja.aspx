<%@ Page Language="C#" AutoEventWireup="true" CodeFile="provera-datuma-isticanja.aspx.cs" Inherits="provera_datuma_isticanja" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Provera datuma isticanja</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
        <!--Confirmation box before closing tab. This code will always display alert message (because it's always true) at the time of both tab closing and reloading.-->
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
                title:'Uspešno dobijen datum isticanja sertifikata.',
                text: 'Za navedeni JIK je uspešno dobijen datum isticanja.',
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
        function erroralertSendSOAP() {
            swal({
                title: 'Greška prilikom slanja zahteva.',
                text: 'Kontaktirajte tehničku podršku ili pokušajte ponovo kasnije.',
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
                text: 'Za navedeni JIK nema statusa.',
                type: 'OK'
            });
        }
        function ErrorNotification() {
            swal({
                title: 'Greška prilikom očitavanja sertifikata. Proverite instalaciju i pokušajte ponovo.',
                text: '',
                type: 'OK'
            });
        }
        function keydownFunction()
        {
            var jikposta = document.getElementById('<%=txtjik.ClientID %>');

        if (jikposta.value.length == 0 || jikposta.value.length == 1)
                document.getElementById('errLabel').style.display = 'none';
        }
        function RadioButtonCkeck() {
            document.getElementById("rbManualJik").checked = true;
        }
        function RadioButtonCkeckAutomatik() {
            document.getElementById("rbAutomatikJik").checked = true;
        }
        function DisableButton() {
            document.getElementById('btnReadCardInfo').disabled = true;
        }
        function DisableSubmitButton() {
            document.getElementById('btnSubmit').disabled = true;
        }
        function EnableButton() {
            document.getElementById('btnReadCardInfo').disabled = false;
        }
        function EnableSubmitButton() {
            document.getElementById('btnSubmit').disabled = false;
        }
        function ExplorerLogout() {
            document.execCommand("ClearAuthenticationCache",false);
        }
        function ChromeLogout() {
            if (window.crypto) window.crypto.logout();
        }
    </script>
    <style type="text/css">
        .submit {
        border: 1px solid #FFA500;
        border-radius: 5px;
        color: white;
        padding: 5px 30px 5px 45px;
        background: url(Content/Images/certificate.gif) left 3px top 2px no-repeat #FFA500;
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
                        <asp:Label id="lblpostanaziv" runat="server" style="font-size:17px; color:darkblue"> <span style="color:darkblue"></span></asp:Label>
                    </a>             
            </div>
        </div>
        <div class="container-fluid">
            <h4><b style="color:#6b6b6b;"><asp:Label id="lblstranicanaziv" runat="server" style="font-size:18px;"></asp:Label></b></h4>
        </div>
        <div class="container-fluid top10">
            <asp:Image ID="pdfdocument" imageurl="~/Content/Images/pdf_icon.png" runat="server" style="width:20px;" />
            <asp:HyperLink id="pdfhyperlink1" runat="server" NavigateUrl="~/dokumentacija/Uputstvo-provera-datuma-isticanja.pdf" target="_blank" style="vertical-align:bottom"><asp:Label id="lblkorisniskouputstvo" runat="server" style="font-size:15px;"></asp:Label></asp:HyperLink>
        </div>
        <!--AJAX ToolkitScriptManager-->
        <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <div class="row">
                <br />
            </div>
        <div class="row top10" style="margin-left: 10px">
            <div class="col-sm-12">
                <br/><p class="notification"><asp:Label id="lblnotification" runat="server" style="font-size:13px;"></asp:Label></p>
            </div>
        </div> 
        <div class="container-fluid" id="Container1" runat="server" style="margin-left: 10px">
            <asp:UpdatePanel id="UpdatePanel1" runat="server">
              <ContentTemplate>
                 <fieldset>
                    <div class="row top10">
                        <div class="col-sm-12">
                            <asp:RadioButton ID="rbManualJik" runat="server" Text="" OnCheckedChanged="rbManualJik_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp"/>
                        </div>                            
                    </div>
                </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel2" runat="server">
              <ContentTemplate>
                 <fieldset>
                    <div class="row top10">
                        <div class="col-sm-12">
                            <asp:RadioButton ID="rbAutomatikJik" runat="server" Text="" OnCheckedChanged="rbAutomatikJik_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp" TabIndex="0"/>
                            &nbsp;&nbsp;
                            <asp:Button ID="btnReadCardInfo" runat="server" class="btn btn-warning btn-sm submit" Text="" style="border: 4px solid #FFA500;" OnClick="btnReadCardInfo_Click" OnClientClick="unhook();ExplorerLogout();ChromeLogout();" TabIndex="0"/>                               
                        </div>
                    </div>
                 </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row">
                <div class="col-sm-12">
                    
                </div>
            </div>   
        </div>
        <div class="row">
            <br />
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <asp:UpdatePanel id="UpdatePanel3" runat="server">
              <ContentTemplate>
                 <fieldset>
                    <div class="row top10">                                 
                         <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                             <div class="w-2-forme w-8-forme-md">
                                 <asp:Label id="spanjik" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                             </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lbljik" runat="server" style="font-weight:bold; font-size:13px;"> </asp:Label>
                            </div> 
                         </div>
                         <div class="col-sm-10" style="background-color:white;">
                             <asp:TextBox ID="txtjik" runat="server" class="txtbox1" style="font-size:13px;" maxlength="9" AutoPostBack="true" ontextchanged="txtjik_TextChanged" OnClientClick="return CheckIfChannelHasChanged();" onkeydown="keydownFunction(); return true;" TabIndex="0"></asp:TextBox>
                             <asp:Label ID="errLabel" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                             <asp:CustomValidator runat="server" id="cvjik" controltovalidate="txtjik" errormessage="" OnServerValidate="cvjik_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                             <br/><p class="notification"><asp:Label id="lblnotification1" runat="server" style="font-size:13px;"></asp:Label></p>
                          </div>
                    </div>
                  </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row">
            </div>
            <asp:UpdatePanel id="UpdatePanel4" runat="server">
              <ContentTemplate>
                 <fieldset>
                     <div class="row top10">
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanserijskibroj02" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblserijskibroj02" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                            </div>     
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtserijskibroj02" runat="server" class="txtbox" style="font-size:13px;" maxlength="30"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvserijskibroj02" controltovalidate="txtserijskibroj02" errormessage="" OnServerValidate="cvserijskibroj02_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row top10">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanname" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblime" runat="server" style="font-weight:bold; font-size:13px;"> </asp:Label>
                            </div>     
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" AutoPostBack="true"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvime" controltovalidate="txtime" errormessage="" OnServerValidate="cvime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row">
                    </div>
                    <div class="row top10">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spansurname" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblprezime" runat="server" style="font-weight:bold; font-size:13px;"> </asp:Label>
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtprezime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" AutoPostBack="true"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvprezime" controltovalidate="txtprezime" errormessage="" OnServerValidate="cvprezime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row">
                    </div>
                    <div class="row top10">                                 
                         <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                             <div class="w-2-forme w-8-forme-md">
                                 <asp:Label id="spandatumizdavanja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                             </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lbldatumizdavanja" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                            </div>  
                         </div>
                         <div class="col-sm-10" style="background-color:white;">
                             <asp:TextBox ID="txtdatumizdavanja" runat="server" class="txtbox" style="font-size:13px;" maxlength="15" AutoPostBack="true"></asp:TextBox>
                             <asp:CustomValidator runat="server" id="cvdatumizdavanja" controltovalidate="txtdatumizdavanja" errormessage="" OnServerValidate="cvdatumizdavanja_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                          </div>
                    </div>
                    <div class="row">
                    </div>
                    <div class="row top10">                                 
                         <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spandatumisteka" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lbldatumisteka" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                            </div>
                         </div>
                         <div class="col-sm-10" style="background-color:white;">
                             <asp:TextBox ID="txtdatumsiteka" runat="server" class="txtbox" style="font-size:13px;" maxlength="10" AutoPostBack="true"></asp:TextBox>
                             <asp:CustomValidator runat="server" id="cvdatumisteka" controltovalidate="txtdatumsiteka" errormessage="" OnServerValidate="cvdatumisteka_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                          </div>
                    </div>
                 </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
          </div>
        <!---------------------------------------------------------------------------------------------------------->
        <!---------------------------------------------------------------------------------------------------------->
        <div class="row">
            <br />
            <br />
        </div>
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <label for="lbldugme" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                </div>
                <div class="col-sm-8">
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
        <div id="throbber" style="display:none;">
            <p style="font-size:20px; font-weight: bold;"><b>Molimo sačekajte...</b></p>
            <asp:Image ID="imgThrobber" imageurl="~/Content/Gif/throbber.gif" runat="server" style="width:35px;height:35px;"/>
        </div>
        <!---------------------------------------------------------------------------------------------------------->
    </form>
</body>
</html>
