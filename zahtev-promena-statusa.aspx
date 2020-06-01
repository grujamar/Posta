<%@ Page Language="C#" AutoEventWireup="true" CodeFile="zahtev-promena-statusa.aspx.cs" Inherits="zahtev_promena_statusa" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Zahtev za promenu statusa elektronskog sertifikata</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
        <!--Confirmation box before closing tab. This code will always display alert message (because it's always true) at the time of both tab closing and reloading.-->
        $(document).ready(function () {
            $('#btnEnterRequest, #btnSubmit').click(function () {
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
                title:'Proverite tačnost unetih podataka.',
                text: 'Pritisnite odgovarajuće dugme na dnu forme.',
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
        function keydownFunctionJik01() {
            var jikposta = document.getElementById('<%=txtjik01.ClientID %>');

            if (jikposta.value.length == 0)
                document.getElementById('errLabel01').style.display = 'none';
        }
        function RadioButtonUnknown() {
            document.getElementById("rbUnknownJik").checked = true;
        }
        function RadioButtonCheck1() {
            document.getElementById("rbUnknownJik").checked = true;
            document.getElementById("rbAutomatikJik").checked = false;
            document.getElementById("rbManualJik").checked = false;
        }
        function RadioButtonCkeckAutomatik() {
            document.getElementById("rbAutomatikJik").checked = true;
            document.getElementById("rbManualJik").checked = false;
            document.getElementById("rbUnknownJik").checked = false;
        }
        function RadioButtonCheck2() {
            document.getElementById("rbIndividual").checked = true;
            document.getElementById("rbLegal").checked = false;
        }
        function RadioButtonCheck3() {
            document.getElementById("rbIndividual").checked = false;
            document.getElementById("rbLegal").checked = true;
        }
        function DisableButton() {
            document.getElementById('btnReadCardInfo').disabled = true;
        }
        function EnableButton() {
            document.getElementById('btnReadCardInfo').disabled = false;
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
        function keydownFunctionJmbg(){
            var jmbgpost = document.getElementById('<%=txtjmbg.ClientID %>');

            if (jmbgpost.value.length == 0 || jmbgpost.value.length == 1)
                document.getElementById('errLabelJMBG').style.display = 'none';
        }
        function keydownFunctionTelefon(){
            var telefonpost = document.getElementById('<%=txttelefon.ClientID %>');

            if (telefonpost.value.length == 0 || telefonpost.value.length == 1)
                document.getElementById('errLabelNumber').style.display = 'none';
        }
        function keydownFunctionMail(){
            var mailpost = document.getElementById('<%=txtadresaeposte.ClientID %>');

            if (mailpost.value.length == 0 || mailpost.value.length == 1)
                document.getElementById('errLabelMail').style.display = 'none';
        }
        function keydownFunctionPIB(){
            var pib = document.getElementById('<%=txtpib.ClientID %>');

            if (pib.value.length == 0 || pib.value.length == 1)
                document.getElementById('errLabelPIB').style.display = 'none';
        }
        function DisableCalendar() {
            $("[id$=txtdatumgubitka], [id$=txtdatumcompromise]").datepicker('disable');
            return false;
        }
        function pickdate() {
            $("[id$=txtdatumgubitka], [id$=txtdatumcompromise]").datepicker({
                showOn: 'button',
                buttonText: 'Izaberite datum',
                buttonImageOnly: true,
                buttonImage: "Content/Images/calendar.png",
                dayNames: ['Nedelja', 'Ponedeljak', 'Utorak', 'Sreda', 'Četvrtak', 'Petak', 'Subota'],
                dayNamesMin: ['Ned', 'Pon', 'Uto', 'Sre', 'Čet', 'Pet', 'Sub'],
                dateFormat: 'dd.mm.yy',
                monthNames: ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun', 'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'],
                monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Maj', 'Jun', 'Jul', 'Avg', 'Sep', 'Okt', 'Nov', 'Dec'],
                firstDay: 1,
                constrainInput: true,
                changeMonth: true,
                changeYear: true,
                yearRange: '1900:2100',
                showButtonPanel: false,
                closeText: "Zatvori",
                beforeShow: function () { try { FixIE6HideSelects(); } catch (err) { } },
                onClose: function () { try { FixIE6ShowSelects(); } catch (err) { } }
            });
            $(".ui-datepicker-trigger").mouseover(function () {
                $(this).css('cursor', 'pointer');
            });
            $(".ui-datepicker-trigger").css("margin-bottom", "3px");
            $(".ui-datepicker-trigger").css("margin-left", "3px");
        };
    </script>
    <style>
        .ui-priority-secondary, .ui-widget-content .ui-priority-secondary, .ui-widget-header .ui-priority-secondary {
            font-weight: bold;
            opacity: 1;
        }
    </style>
    <style type="text/css">
        .submit {
            border: 1px solid #FFA500;
            border-radius: 5px;
            color: white;
            padding: 5px 30px 5px 45px;
            background: url(Content/Images/certificate.gif) left 3px top 2px no-repeat #FFA500;
        }
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
            <asp:HyperLink id="pdfhyperlink1" runat="server" NavigateUrl="~/dokumentacija/Uputstvo-promena-statusa.pdf" target="_blank" style="vertical-align:bottom"><asp:Label id="lblkorisniskouputstvo" runat="server" style="font-size:15px;"></asp:Label></asp:HyperLink>
        </div>
        <!-------------------------------------------Hidden------------------------------------------------>
        <div class="container-fluid" id="myDiv1" runat="server" style="margin-left: 10px">
            <div class="row">
                <br />
            </div>
            <div class="row top10">
                <div class="col-sm-2">
                    <asp:Label id="lbldatumzahteva" runat="server" style="font-weight:bold;font-size:13px;"></asp:Label>
                </div>
                <div class="col-sm-10">
                        <asp:TextBox ID="txtdatumzahteva" runat="server" class="txtbox1" style="font-size:13px; background-color: #e2e2e2;" ReadOnly="true"></asp:TextBox>
                </div>
            </div>  
        </div>
        <!---------------------------------------------------------------------------------------------------------->
        <!------------------------------------------------------------------------------------------------>
        <!--AJAX ToolkitScriptManager-->
        <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
        <div class="container-fluid" style="margin-left: 10px">         
            <div class="row top10">
                <div class="col-sm-12">
                    <br/><p class="notification"><asp:Label id="lblnotification" runat="server" style="font-size:13px;"></asp:Label></p>
                </div>
            </div>
            <div class="row top10" style="margin-left: 10px">
                <div class="col-sm-12">
                    <asp:RadioButton ID="rbUnknownJik" runat="server" Text="" OnCheckedChanged="rbUnknownJik_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp1"/>
                </div>                            
            </div>
            <div class="row top10" style="margin-left: 10px"> 
                <div class="col-sm-12">
                    <asp:RadioButton ID="rbManualJik" runat="server" Text="" OnCheckedChanged="rbManualJik_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp1"/>
                </div>
            </div>
            <div class="row" style="margin-left: 10px">
            </div>
                <div class="row top10" style="margin-left: 10px">
                    <div class="col-sm-12">
                        <asp:RadioButton ID="rbAutomatikJik" runat="server" Text="" OnCheckedChanged="rbAutomatikJik_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp1"/>&nbsp;&nbsp;
                        <asp:Button ID="btnReadCardInfo" runat="server" class="btn btn-warning btn-sm submit" Text="" style="border: 4px solid #FFA500;" OnClick="btnReadCardInfo_Click" OnClientClick="unhook()" GroupName="btnGrp1"/>
                    </div>
                </div>
        </div>
        <!---------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="Container00" runat="server" style="margin-left: 10px">
            <div class="page-header" style="color:darkblue">
                <h4><asp:Label id="lblnotification2" runat="server" style="font-size:19px;"></asp:Label></h4>
            </div>   
        </div>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <asp:UpdatePanel id="UpdatePanel7" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="container-fluid" id="Container01" runat="server" style="margin-left: 10px">           
                        <div class="row">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanjik01" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lbljik01" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>  
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtjik01" runat="server" class="txtbox1" style="font-size:13px;" maxlength="9" AutoPostBack="True" ontextchanged="txtjik01_TextChanged" OnClientClick="return CheckIfChannelHasChanged1();" onkeydown="keydownFunctionJik01(); return true;"></asp:TextBox>
                                <asp:Label ID="errLabel01" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvjik01" controltovalidate="txtjik01" errormessage="" OnServerValidate="cvjik01_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                <br/><p class="notification"><asp:Label id="lblnotification5" runat="server" style="font-size:13px;"></asp:Label></p>
                            </div> 
                        </div>
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
                            </div>
                        </div>
                        <div class="row">
                            <br />
                        </div>
                    </div>
                </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
          <div class="container-fluid" id="Container04" runat="server" style="margin-left: 10px">
            <div class="row">                                 
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                       <asp:Label id="spanjik" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;             
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lbljik" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>          
                    </div>  
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:TextBox ID="txtjik" runat="server" class="txtbox1" style="font-size:13px;" maxlength="9"></asp:TextBox>
                    <asp:CustomValidator runat="server" id="cvjik" controltovalidate="txtjik" errormessage="" OnServerValidate="cvjik_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                    <br/><p class="notification"><asp:Label id="lblnotification6" runat="server" style="font-size:13px;"></asp:Label></p>
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanime" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;         
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblime" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>            
                    </div>
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:TextBox ID="txtime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30"></asp:TextBox>
                    <asp:CustomValidator runat="server" id="cvime" controltovalidate="txtime" errormessage="" OnServerValidate="cvime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>
            <div class="row top10">                                 
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanprezime" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;         
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblprezime" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>            
                    </div>  
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:TextBox ID="txtprezime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30"></asp:TextBox>
                    <asp:CustomValidator runat="server" id="cvprezime" controltovalidate="txtprezime" errormessage="" OnServerValidate="cvprezime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>
              <asp:UpdatePanel id="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanjmbg" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lbljmbg" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtjmbg" runat="server" class="txtbox" style="font-size:13px;" maxlength="30"></asp:TextBox>
                                <asp:CustomValidator runat="server" id="cvjmbg" controltovalidate="txtjmbg" errormessage="" OnServerValidate="cvjmbg_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                <br><p class="notification"><asp:Label id="lblnotification12" runat="server" style="font-size:13px;"></asp:Label></p>
                            </div>
                        </div>
                   </fieldset>
                </ContentTemplate>
              </asp:UpdatePanel>
              <asp:UpdatePanel id="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanadresaeposte" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lbladresaeposte" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtadresaeposte" runat="server" class="txtbox3" style="font-size:13px;" maxlength="50" ontextchanged="txtadresaeposte_TextChanged" OnClientClick="return CheckIfChannelHasChanged2();" AutoPostBack="true" onkeydown="keydownFunctionMail(); return true;"></asp:TextBox>
                                <asp:Label ID="errLabelMail" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvadresaeposte" controltovalidate="txtadresaeposte" errormessage="" OnServerValidate="cvadresaeposte_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                     </fieldset>
                  </ContentTemplate>
              </asp:UpdatePanel>
              <asp:UpdatePanel id="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spantelefon" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lbltelefon" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txttelefon" runat="server" class="txtbox" style="font-size:13px;" maxlength="12" ontextchanged="txttelefon_TextChanged" OnClientClick="return CheckIfChannelHasChanged3();" AutoPostBack="true" onkeydown="keydownFunctionTelefon(); return true;"></asp:TextBox>
                                <asp:Label id="TelefonPrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                                <asp:Label ID="errLabelNumber" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvtelefon" controltovalidate="txttelefon" errormessage="" OnServerValidate="cvtelefon_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                      </fieldset>
                  </ContentTemplate>
              </asp:UpdatePanel>
          </div>
        <asp:UpdatePanel id="UpdatePanel6" runat="server">
         <ContentTemplate>
          <fieldset>
            <div class="container-fluid" id="Container06" runat="server" style="margin-left: 10px">
                <div class="row">
                    <div class="col-sm-12">
                        <br/><p class="notification"><asp:Label id="lblnotification3" runat="server" style="font-size:13px;"></asp:Label></p>
                    </div>
                </div>
                <div class="row" style="margin-left: 10px"> 
                    <div class="col-sm-12">
                        <asp:RadioButton ID="rbIndividual" runat="server" Text="" OnCheckedChanged="rbIndividual_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp2"/>
                    </div>
                </div>
                <div class="row top10" style="margin-left: 10px"> 
                    <div class="col-sm-12">
                        <asp:RadioButton ID="rbLegal" runat="server" Text="" OnCheckedChanged="rbLegal_CheckedChanged" AutoPostBack="True" onclick="unhook()" GroupName="btnGrp2"/>
                    </div>
                </div>
            </div>
            <div class="container-fluid" id="Container07" runat="server" style="margin-left: 10px">
                <div class="page-header" style="color:darkblue">
                    <h4><asp:Label id="lblnotification4" runat="server" style="font-size:19px;"></asp:Label></h4>
                </div>
                    <div class="row">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanmaticnibroj" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;    
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblmaticnibroj" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>   
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtmaticnibroj" runat="server" class="txtbox1" style="font-size:13px;" maxlength="8" ontextchanged="txtmaticnibroj_TextChanged" OnClientClick="return CheckIfChannelHasChanged7();" AutoPostBack="true"></asp:TextBox>
                            <asp:Label ID="errLabelIN" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                            <asp:CustomValidator runat="server" id="cvmaticnibroj" controltovalidate="txtmaticnibroj" errormessage="" OnServerValidate="cvmaticnibroj_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row top10" id="rowLegalEntityDDL" runat="server">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spannazivpravnoglicaDDL" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;    
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblnazivpravnoglicaDDL" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>   
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:DropDownList ID="ddlLegalEntityName" runat="server" AppendDataBoundItems="False" AutoPostBack="True" class="txtbox5" OnSelectedIndexChanged="ddlLegalEntityName_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:CustomValidator runat="server" id="cvLegalEntityName" controltovalidate="ddlLegalEntityName" errormessage="" OnServerValidate="cvLegalEntityName_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row top10" id="rowLegalEntityName" runat="server">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spannazivpravnoglica" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;   
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                               <asp:Label id="lblnazivpravnoglica" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>      
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtnazivpravnoglica" runat="server" class="txtbox5" style="font-size:13px;" maxlength="250"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvnazivpravnoglica" controltovalidate="txtnazivpravnoglica" errormessage="" OnServerValidate="cvnazivpravnoglica_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row top10">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                 <asp:Label id="spanpib" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;   
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                 <asp:Label id="lblpib" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>   
                            </div>  
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtpib" runat="server" class="txtbox1" style="font-size:13px;" maxlength="9" ontextchanged="txtpib_TextChanged" OnClientClick="return CheckIfChannelHasChanged5();" AutoPostBack="true" onkeydown="keydownFunctionPIB(); return true;"></asp:TextBox>
                            <asp:Label ID="errLabelPIB" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                            <asp:CustomValidator runat="server" id="cvpib" controltovalidate="txtpib" errormessage="" OnServerValidate="cvpib_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                <div class="page-header" style="color:darkblue">
                    <h4><asp:Label id="lblnotification7" runat="server" style="font-size:19px;"></asp:Label></h4>
                </div>
                <div class="row top10">                                 
                    <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                        <div class="w-2-forme w-8-forme-md">
                            <asp:Label id="spanimezz" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;       
                        </div>
                        <div class="w-98-forme w-92-forme-md">
                            <asp:Label id="lblimezz" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>        
                        </div>
                    </div>
                    <div class="col-sm-10" style="background-color:white;">
                        <asp:TextBox ID="txtimezz" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" onkeyup="change(this,'btnAddAuthorizedPersonalUser');"></asp:TextBox>
                        <asp:CustomValidator runat="server" id="cvimezz" controltovalidate="txtimezz" errormessage="" OnServerValidate="cvimezz_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                    </div>
                </div>
                <div class="row top10">                                 
                    <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                        <div class="w-2-forme w-8-forme-md">
                            <asp:Label id="spanprezimezz" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;       
                        </div>
                        <div class="w-98-forme w-92-forme-md">
                            <asp:Label id="lblprezimezz" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>        
                        </div>
                    </div>
                    <div class="col-sm-10" style="background-color:white;">
                        <asp:TextBox ID="txtprezimezz" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" onkeyup="change(this,'btnAddAuthorizedPersonalUser');"></asp:TextBox>
                        <asp:CustomValidator runat="server" id="cvprezimezz" controltovalidate="txtprezimezz" errormessage="" OnServerValidate="cvprezimezz_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                    </div>
                </div>
             </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel id="UpdatePanel4" runat="server">
      <ContentTemplate>
        <fieldset>
        <div class="container-fluid" id="Container10" runat="server" style="margin-left: 10px">
            <div class="page-header" style="color:darkblue">
                <h4><asp:Label id="lblnotification8" runat="server" style="font-size:19px;"></asp:Label></h4>
            </div>
            <div class="row">
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spannacinpromene" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;          
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                       <asp:Label id="lblnacinpromene" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>             
                    </div>  
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:DropDownList ID="ddlnacinpromene" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox4" DataSourceID="odsNacinPromene" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlnacinpromene_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsNacinPromene" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="zahtev-promena-statusa.aspx" Name="filename" Type="String" />
                                            <asp:Parameter DefaultValue="ddlnacinpromene" Name="controlid" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                    <asp:CustomValidator runat="server" id="cvnacinpromene" controltovalidate="ddlnacinpromene" errormessage="" OnServerValidate="cvnacinpromene_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>
        </div>
        <div class="container-fluid" id="Container08" runat="server" style="margin-left: 10px">
            <div class="row top10">
                <div class="col-sm-12">
                    <br/><p class="notification"><asp:Label id="lblnotification9" runat="server" style="font-size:13px;"></asp:Label></p>
                </div>
            </div>
            <div class="row top10" style="margin-left: 10px"> 
                <div class="col-sm-12">
                    <asp:RadioButton ID="rblosstoken" runat="server" Text="" OnCheckedChanged="rblosstoken_CheckedChanged" AutoPostBack="True" onclick="unhook()"/>
                </div>
            </div>
            <div class="row top10" id="Container11" runat="server" style="margin-left: 10px">
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spandatumgubitka" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;            
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lbldatumgubitka" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>          
                    </div>
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:TextBox ID="txtdatumgubitka" runat="server" class="txtbox2" style="font-size:13px;" ReadOnly="true"></asp:TextBox>                                                                                                                               
                    <asp:Label id="LostExample" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                    <asp:CustomValidator runat="server" id="cvdatumgubitka" controltovalidate="txtdatumgubitka" errormessage="" OnServerValidate="cvdatumgubitka_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div> 
            <div class="row top10" style="margin-left: 10px"> 
                <div class="col-sm-12">
                    <asp:RadioButton ID="rbcompromise" runat="server" Text="" OnCheckedChanged="rbcompromise_CheckedChanged" AutoPostBack="True" onclick="unhook()"/>
                </div>
            </div>
            <div class="row top10" id="Container12" runat="server" style="margin-left: 10px">
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                         <asp:Label id="spandatumcompromise" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;           
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lbldatumcompromise" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>            
                    </div>     
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:TextBox ID="txtdatumcompromise" runat="server" class="txtbox2" style="font-size:13px;" ReadOnly="true"></asp:TextBox>
                    <asp:Label id="CompromiseExample" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                    <asp:CustomValidator runat="server" id="cvdatumcompromise" controltovalidate="txtdatumcompromise" errormessage="" OnServerValidate="cvdatumcompromise_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>
            <div class="row top10" style="margin-left: 10px"> 
                <div class="col-sm-12">
                    <asp:RadioButton ID="rbchangedata" runat="server" Text="" OnCheckedChanged="rbchangedata_CheckedChanged" AutoPostBack="True" onclick="unhook()"/>
                </div>
            </div>
            <div class="row top10" style="margin-left: 10px"> 
                <div class="col-sm-12">
                    <asp:RadioButton ID="rbcessation" runat="server" Text="" OnCheckedChanged="rbcessation_CheckedChanged" AutoPostBack="True" onclick="unhook()"/>
                </div>
            </div>
            <div class="row top10" style="margin-left: 10px"> 
                <div class="col-sm-12">
                    <asp:RadioButton ID="rbcessationofneed" runat="server" Text="" OnCheckedChanged="rbcessationofneed_CheckedChanged" AutoPostBack="True" onclick="unhook()"/>
                </div>
            </div>
            <div class="row top10" style="margin-left: 10px"> 
                <div class="col-sm-3">
                    <asp:RadioButton ID="rbother" runat="server" Text="" OnCheckedChanged="rbother_CheckedChanged" AutoPostBack="True" onclick="unhook()"/>
                </div>
            </div>
            <div class="row top10" id="Container13" runat="server" style="margin-left: 10px">
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spandrugo" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;            
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lbldrugo" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>            
                    </div>
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:TextBox ID="txtdrugo" runat="server" class="txtbox7" style="font-size:13px;" maxlength="100"></asp:TextBox>
                    <asp:CustomValidator runat="server" id="cvdrugo" controltovalidate="txtdrugo" errormessage="" OnServerValidate="cvdrugo_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>      
        </div>
        <div class="container-fluid" id="Container09" runat="server" style="margin-left: 10px">
            <div class="row">
                <br />
            </div>
            <div class="row top10">                                 
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanostalo" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;           
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                       <asp:Label id="lblostalo" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>             
                    </div>
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:TextBox ID="txtostalo" runat="server" class="txtbox7" style="font-size:13px;" maxlength="100"></asp:TextBox>
                    <asp:CustomValidator runat="server" id="cvostalo" controltovalidate="txtostalo" errormessage="" OnServerValidate="cvostalo_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>
        </div>
    </fieldset>
</ContentTemplate>
</asp:UpdatePanel>
    <div class="container-fluid" runat="server" style="margin-left: 10px">
        <div class="row top10">
            <div class="col-sm-12">
                <br/><p class="notification"><asp:Label id="lblnotification11" runat="server" style="font-size:13px; font-weight:bold;"></asp:Label></p>
            </div>
        </div>
    </div>
    <div class="row">
        <br />
    </div>
    <div class="row">
        <br />
    </div>
        <!-------------------------------------------Hidden------------------------------------------------>
        <div class="container-fluid" id="myDiv6" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <label for="lbldugme" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                </div>
                <div class="col-sm-8">
                    <asp:Button ID="btnEnterRequest" runat="server" class="btn btn-primary save" Text="" style="margin-right: 8px;" OnClick="btnEnterRequest_Click1" onclientclick="unhook()" ValidationGroup="EnabledValidation"/>
                    <asp:Button ID="btnReEnterRequest" runat="server" class="btn btn-warning edit" Text="" style="margin-right: 8px;" OnClick="btnReEnterRequest_Click1" onclientclick="unhook()" ValidationGroup="EnabledValidation"/>
                </div>
                <div class="col-sm-2">
                    <label for="lbldugme1" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                </div>
            </div>
        </div>  

        <!---------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <label for="lbldugme" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                </div>
                <div class="col-sm-8">
                    <asp:Button ID="btnSubmit" runat="server" class="btn btn-primary save" Text="" onclick="btnSubmit_Click" onclientclick="unhook()"/>                           
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
        <div id="throbber" style="display:none;">
            <p style="font-size:20px; font-weight: bold;"><b>Molimo sačekajte...</b></p>
            <asp:Image ID="imgThrobber" imageurl="~/Content/Gif/throbber.gif" runat="server" style="width:35px;height:35px;"/>
        </div>
    </form>
</body>
</html>
