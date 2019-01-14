<%@ Page Language="C#" AutoEventWireup="true" CodeFile="provera-opozvanosti-sertifikata.aspx.cs" Inherits="provera_opozvanosti_sertifikata" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Provera opozvanosti elektronskog sertifikata</title>
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
        function successalertOCSP() {
            swal({
                title:'Uspešno dobijeni rezultati provere opozvanosti sertifikata.',
                text: '',
                type: 'OK'
            });
        }
        function erroralertOCSP() {
            swal({
                title: 'Postoji problem u komunikaciji sa serverom za proveru opozvanosti sertifikata.',
                text: '',
                type: 'OK'
            });
        }
        function erroralert() {
            swal({
                title: 'Greška.',
                text: 'Ispravite podatke i pokušajte ponovo.',
                type: 'OK'
            });
        }
        function errorDownloadingCER() {
            swal({
                title: 'Greška.',
                text: 'Nemate ovlašćenja za snimanje .cer sertifikata u izabranom direktorijumu. Izaberite drugi direktorijum.',
                type: 'OK'
            });
        }
        function errorDownloadingOCR() {
            swal({
                title: 'Greška.',
                text: 'Nemate ovlašćenja za snimanje .ocr ili .crl sertifikata u izabranom direktorijumu. Izaberite drugi direktorijum.',
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
        function Test3Functions() {
            document.getElementById("rbAutomatikSerialNo").checked = true;
            document.getElementById('btnReadCardInfo').disabled = false;
            document.getElementById('btnSubmit').disabled = false;
        }
        function RadioButtonCkeck() {
            document.getElementById("rbManualSerialNo").checked = true;
        }
        function RadioButtonCkeckAutomatik() {
            document.getElementById("rbAutomatikSerialNo").checked = true;
        }
        function RadioButtonCheck1() {
            document.getElementById("rbManualSerialNo").checked = true;
            document.getElementById("rbAutomatikSerialNo").checked = false;
        }
        function DisableButton() {
            document.getElementById('btnReadCardInfo').disabled = true;
        }
        function EnableButton() {
            document.getElementById('btnReadCardInfo').disabled = false;
        }
        function DisableButtonOCSP() {
            document.getElementById('btnGetRevocationOCSP').disabled = true;
        }
        function EnableButtonOCSP() {
            document.getElementById('btnGetRevocationOCSP').disabled = false;
        }
        function DisableButtonRevocation() {
            document.getElementById('btnGetRevocationCertificate').disabled = true;
        }
        function EnableButtonRevocation() {
            document.getElementById('btnGetRevocationCertificate').disabled = false;
        }
        function keydownFunctionSerialNo()
        {
            var serialnoposta = document.getElementById('<%=txtserialno.ClientID %>');

            if (serialnoposta.value.length == 0 || serialnoposta.value.length == 1)
                   document.getElementById('errLabelSerialNo').style.display = 'none';
        }
        function DisableSubmitButton() {
            document.getElementById('btnSubmit').disabled = true;
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
            border-radius: 5px;
            color: white;
            padding: 5px 30px 5px 45px;
            background: url(Content/Images/certificate.gif) left 3px top 2px no-repeat #FFA500;
            border: 4px solid #FFA500; 
            margin-bottom: 20px;
            font-size:13px;
        }
        .crl {
            border: 1px solid #FFA500;
            border-radius: 5px;
            color: white;
            padding: 5px 30px 5px 45px;
            background: url(Content/Images/crl.jpg) left 3px top -3px no-repeat #FFA500;
            border: 4px solid #FFA500;
            font-size:13px;
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
                <asp:HyperLink id="pdfhyperlink1" runat="server" NavigateUrl="~/dokumentacija/Uputstvo-provera-opozvanosti.pdf" target="_blank" style="vertical-align:bottom"><asp:Label id="lblkorisniskouputstvo" runat="server" style="font-size:15px;"></asp:Label></asp:HyperLink>
        </div>
        <!------------------------------------------------------------------------------------------------>
        <!--AJAX ToolkitScriptManager-->
        <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
        <div class="container-fluid" id="Container00" runat="server" style="margin-left: 10px">
                <div class="row top10">
                    <div class="col-sm-12">
                        <br/><p class="notification"><asp:Label id="lblnotification" runat="server" style="font-size:13px;"></asp:Label></p>
                    </div>
                </div>
                <div class="row top10" style="margin-left: 10px"> 
                    <div class="col-sm-12">
                        <asp:RadioButton ID="rbManualSerialNo" runat="server" Text="" OnCheckedChanged="rbManualSerialNo_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp"/>
                    </div>
                </div>
                <div class="row top10" style="margin-left: 10px">                    
                </div>
                <div class="row top10" style="margin-left: 10px">
                    <div class="col-sm-12">
                        <asp:RadioButton ID="rbAutomatikSerialNo" runat="server" Text="" OnCheckedChanged="rbAutomatikSerialNo_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp"/>&nbsp;&nbsp;
                        <asp:Button ID="btnReadCardInfo" runat="server" class="btn btn-warning btn-sm submit" Text="" style="border: 4px solid #FFA500;" OnClick="btnReadCardInfo_Click" OnClientClick="unhook()"/>
                    </div>
                </div>                
        </div>
        <!---------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="Container10" runat="server" style="margin-left: 10px;">
            <div class="row top10">
                    
            </div>
        </div>
        <asp:UpdatePanel id="UpdatePanel5" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="container-fluid" id="Container01" runat="server" style="margin-left: 10px">
                        <div class="row">
                            <div class="col-sm-12">
                                <br/><p class="notification"><asp:Label id="lblnotification1" runat="server" style="font-size:13px;"></asp:Label></p>
                            </div>
                        </div>  
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanserialno" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblserialno" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>  
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtserialno" runat="server" class="txtbox" style="font-size:13px;" AutoPostBack="true" ontextchanged="txtserialno_TextChanged" OnClientClick="return CheckIfChannelHasChanged1();" onkeydown="keydownFunctionSerialNo(); return true;"></asp:TextBox>
                                <asp:Label id="SerialNoPrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"> </asp:Label>
                                <asp:Label ID="errLabelSerialNo" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvserialno" controltovalidate="txtserialno" errormessage="" OnServerValidate="cvserialno_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                    </div>
                    <div class="container-fluid" id="Container02" runat="server" style="margin-left: 10px">
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
                                    <asp:Label id="spanime02" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblime02" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>            
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtime02" runat="server" class="txtbox" style="font-size:13px;" maxlength="30"></asp:TextBox>
                                <asp:CustomValidator runat="server" id="cvime02" controltovalidate="txtime02" errormessage="" OnServerValidate="cvime02_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>                    
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanprezime02" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblprezime02" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtprezime02" runat="server" class="txtbox" style="font-size:13px;" maxlength="30"></asp:TextBox>
                                <asp:CustomValidator runat="server" id="cvprezime02" controltovalidate="txtprezime02" errormessage="" OnServerValidate="cvprezime02_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                     <asp:Label id="spanjik01" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lbljik01" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtjik01" runat="server" class="txtbox" style="font-size:13px;" maxlength="9" AutoPostBack="true"></asp:TextBox>
                                <asp:CustomValidator runat="server" id="cvjik01" controltovalidate="txtjik01" errormessage="" OnServerValidate="cvjik01_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        </asp:UpdatePanel>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="Container11" runat="server" style="margin-left: 10px;">
            <div class="row top10">
                <br />
            </div>
        </div>
        <asp:UpdatePanel id="UpdatePanel1" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="container-fluid" id="Container06" runat="server" style="margin-left: 10px">
                        <div class="row top10">
                            <div class="col-sm-12">
                                <br/><p class="notification"><asp:Label id="lblnotification2" runat="server" style="font-size:13px;"></asp:Label></p>
                            </div>
                        </div>
                        <div class="row top10" style="margin-left: 10px"> 
                            <div class="col-sm-12">
                                <asp:RadioButton ID="rbOCSPRevocation" runat="server" Text="" OnCheckedChanged="rbOCSPRevocation_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp1"/>
                            </div>
                        </div>
                        <div class="row top10" style="margin-left: 10px">
                        </div>
                        <div class="row top10" style="margin-left: 10px">
                            <div class="col-sm-12">
                                <asp:RadioButton ID="rbCRLRevocation" runat="server" Text="" OnCheckedChanged="rbCRLRevocation_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp1"/>
                            </div>
                        </div>
                        <div class="row top10" id="Container04" runat="server" style="margin-left: 20px">
                            <div class="col-sm-12">
                                <asp:RadioButton ID="rbLDAP" runat="server" Text="" OnCheckedChanged="rbLDAP_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp2"/>
                            </div>
                        </div>
                        <div class="row top10" id="Container05" runat="server" style="margin-left: 20px">
                            <div class="col-sm-12">
                                <asp:RadioButton ID="rbHTTP" runat="server" Text="" OnCheckedChanged="rbHTTP_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp2"/>
                            </div>
                        </div>
                    </div>
                    <!---------------------------------------------------------------------------------------------------------->
                    <!---------------------------------------------------------------------------------------------------------->
                    <!-------------------------------------------------------------------------------------------------------------------------------------->
                    <div class="container-fluid" id="Container12" runat="server" style="margin-left: 10px;">
                        <div class="row top10">
                                <br />
                        </div>
                    </div>
                    <div class="container-fluid" id="Container03" runat="server" style="margin-left: 10px">
                        <div class="row top15">
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanimeizdavaoca" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblimeizdavaoca" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>     
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:DropDownList ID="ddlimeizdavaoca" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox3" DataSourceID="odsImeIzdavaoca" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlimeizdavaoca_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:ObjectDataSource ID="odsImeIzdavaoca" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemRevocationCheck" TypeName="ItemBLL">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="filename" SessionField="provera-opozvanosti-sertifikata-filename" Type="String" />
                                        <asp:SessionParameter Name="controlid" SessionField="provera-opozvanosti-sertifikata-controlid" Type="String" />
                                        <asp:SessionParameter Name="RevocationCheckType" SessionField="provera-opozvanosti-sertifikata-revocationchecktype" Type="Int32" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                <asp:CustomValidator runat="server" id="cvimeizdavaoca" controltovalidate="ddlimeizdavaoca" errormessage="" OnServerValidate="cvimeizdavaoca_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>                            
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanrevocationaddress" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblrevocationaddress" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtrevocationaddress" runat="server" class="txtboxMultiline" style="font-size:13px;" AutoPostBack="true" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                <asp:CustomValidator runat="server" id="cvrevocationaddress" controltovalidate="txtrevocationaddress" errormessage="" OnServerValidate="cvrevocationaddress_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                      </div>
                   </div>
                </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!---------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="Container09" runat="server" style="margin-left: 10px;">
            <br />
            <br />
        </div> 
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <label for="lbldugme" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                </div>
                <div class="col-sm-8">
                    <asp:Button ID="btnSubmit" runat="server" class="btn-lg btn-primary buttonborder" Text="" onclick="btnSubmit_Click" onclientclick="unhook()"/>                           
                </div>
                <div class="col-sm-2">
                    <label for="lbldugme1" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                </div>
            </div>  
        </div>
        <div class="container-fluid" id="Container08" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row">
                <br />
            </div>
        </div>  
        <div class="container-fluid" id="Container07" runat="server" style="margin-left: 10px">
            <div class="row top20">
                <div class="col-sm-12">
                    <h4><asp:Label id="lblnotification4" runat="server" style="font-size:19px;color:darkblue;"></asp:Label></h4>
                </div>
            </div>
            <div class="row top20">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanserijskibrojsert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblserijskibrojsert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtserijskibrojsert" runat="server" class="txtbox4" style="font-size:13px;" maxlength="30"></asp:TextBox>
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanstatussert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblstatussert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtstatussert" runat="server" class="txtbox4" style="font-size:13px;" maxlength="30"></asp:TextBox>
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanrazlogopozivasert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblrazlogopozivasert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtrazlogopozivasert" runat="server" class="txtbox4" style="font-size:13px;" maxlength="9" AutoPostBack="true"></asp:TextBox>                                         
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spandatumvremeopozivasert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lbldatumvremeopozivasert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtdatumvremeopozivasert" runat="server" class="txtbox4" style="font-size:13px;" maxlength="9" AutoPostBack="true"></asp:TextBox>                                         
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spandatumvremekompromitovanjasert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lbldatumvremekompromitovanjasert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtdatumvremekompromitovanjasert" runat="server" class="txtbox4" style="font-size:13px;" maxlength="9" AutoPostBack="true"></asp:TextBox>                                         
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanimeizdavaoca1" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblimeizdavaoca1" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox id="txtimeizdavaoca" runat="server" class="txtboxMultiline" style="font-size:13px; margin-right: 10px;" AutoPostBack="true" TextMode="MultiLine" Rows="5"></asp:TextBox>                                       
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanimeservera" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblimeservera" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtimeservera" runat="server" class="txtboxMultilineGray" style="font-size:13px; margin-bottom: 5px;" AutoPostBack="true" TextMode="MultiLine" Rows="5"></asp:TextBox>
                    <asp:Button ID="btnGetRevocationCertificate" runat="server" CssClass="submit" Text="" onclick="btnGetRevocationCertificate_Click" onclientclick="unhook()"/>
                </div>
            </div>           
            <div class="row top10">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spannacinprovere" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblnacinprovere" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtnacinprovere" runat="server" class="txtbox4" style="font-size:13px; margin-bottom: 5px;" maxlength="9" AutoPostBack="true"></asp:TextBox>
                    <asp:Button ID="btnGetRevocationOCSP" runat="server" CssClass="crl" Text="" onclick="btnGetRevocationOCSP_Click" onclientclick="unhook()"/>
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spancheckingurl" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblcheckingurl" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtcheckingurl" runat="server" class="txtbox4gray" style="font-size:13px;" maxlength="50" AutoPostBack="true"></asp:TextBox>
                </div>
            </div>
            <div class="row top10">
                <div class="col-sm-3 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spandatumvremesprovedeneprovere" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lbldatumvremesprovedeneprovere" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-9" style="background-color:white;">
                    <asp:TextBox ID="txtdatumvremesprovedeneprovere" runat="server" class="txtbox4" style="font-size:13px;" maxlength="9" AutoPostBack="true"></asp:TextBox>                                         
                </div>
            </div>
            <div class="row">
                <br />
                <br />
            </div>
            <div class="container-fluid" runat="server" style=" background-color:#f5f5f5;">
                <div class="row top20" >
                    <div class="col-sm-3">
                        <label for="lbldugme" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                    </div>
                    <div class="col-sm-9">
                        <asp:Button ID="btnBack" runat="server" class="btn-lg btn-warning buttonborder1" Text="" onclick="btnQuit_Click3" onclientclick="unhook()" Width="150px"/>                            
                    </div>
                    <div class="col-sm-2" style="background-color:#f5f5f5;">
                        <label for="lbldugme1" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                    </div>
                </div>
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
    </form>
</body>
</html>
