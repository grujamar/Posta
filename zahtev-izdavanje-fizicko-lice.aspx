<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="zahtev-izdavanje-fizicko-lice.aspx.cs" Inherits="zahtev_izdavanje_fizicko_lice" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Podnošenje zahteva za izdavanje sertifikata</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
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
        function keydownFunctionJmbg()
           {
               var jmbgpost = document.getElementById('<%=txtjmbg.ClientID %>');

               if (jmbgpost.value.length == 0 || jmbgpost.value.length == 1)
                   document.getElementById('errLabel').style.display = 'none';
        }
        function keydownFunctionTelefon()
           {
               var telefonpost = document.getElementById('<%=txttelefon.ClientID %>');

            if (telefonpost.value.length == 0 || telefonpost.value.length == 1)
                   document.getElementById('errLabelNumber').style.display = 'none';
        }
        function keydownFunctionMail()
           {
               var mailpost = document.getElementById('<%=txtadresaeposte.ClientID %>');

            if (mailpost.value.length == 0 || mailpost.value.length == 1)
                   document.getElementById('errLabelMail').style.display = 'none';
        }
        function DisableCalendar() {
            $("[id$=txtdatumisteka], [id$=txtdatumizdavanja]").datepicker('disable');
            return false;
        }
        function pickdate() {
            $("[id$=txtdatumisteka], [id$=txtdatumizdavanja]").datepicker({
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
        //eventArgs.get_value() The method will return id of selected country (acecountry).
        //TargetControlID: The textbox control where user type content  to be automatically completed.
        //OnClientItemSelected: It will call the client side javascript method to set contexKey to another AutoCompleteExtender(acestate).
        function getSelectedMesto(source, eventArgs) {
            $find('autoCompleteUlicaBoravka').set_contextKey(eventArgs.get_value());
            //alert('getSelectedMesto: ' + eventArgs.get_value());
        }
        function getMestoAgain(source, eventArgs) {
            $find('autoCompleteUlicaBoravka').set_contextKey(document.getElementById('<%=this.txtmesto.ClientID%>').value);
        }
        function errorSOAPalert() {
            swal({
                title: 'Greška u slanju zahteva, nije uspelo slanje SOAP poruke.',
                text: 'Popunite zahtev kasnije i pokušajte ponovo.',
                type: 'OK'
            });
        }
        function errorWordToPDFalert() {
            swal({
                title: 'Greška u slanju zahteva, nije uspelo kreiranje dokumenata.',
                text: 'Popunite zahtev kasnije i pokušajte ponovo.',
                type: 'OK'
            });
        }
        </script>
        <style>
        .ui-priority-secondary, .ui-widget-content .ui-priority-secondary, .ui-widget-header .ui-priority-secondary {
            font-weight: bold;
            opacity: 1;
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
                <asp:HyperLink id="pdfhyperlink1" runat="server" NavigateUrl="~/dokumentacija/Uputstvo-izdavanje-fizicko-lice.pdf" target="_blank" style="vertical-align:bottom" TabIndex="-1"><asp:Label id="lblkorisniskouputstvo" runat="server" style="font-size:15px;"></asp:Label></asp:HyperLink>
        </div>
        <div class="row">
                <br />
        </div>
        <!-------------------------------------------OVO SE NE VIDI------------------------------------------------>
        <div class="container-fluid" id="myDiv8" runat="server" style="margin-left: 10px">
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
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">                                 
                        <div class="col-sm-2" style="background-color:white;">
                            <asp:Label id="spanname" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblime" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" TabIndex="1"></asp:TextBox>
                            <asp:Label id="ImePrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"> </asp:Label>
                            <asp:CustomValidator runat="server" id="cvime" controltovalidate="txtime" errormessage="" OnServerValidate="cvime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
            </div>
            <div class="row top10">                                 
                        <div class="col-sm-2" style="background-color:white;">
                            <asp:Label id="spansurname" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblprezime" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtprezime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" TabIndex="0"></asp:TextBox>
                            <asp:Label id="PrezimePrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                            <asp:CustomValidator runat="server" id="cvprezime" controltovalidate="txtprezime" errormessage="" OnServerValidate="cvprezime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
            </div>
            <!--AJAX ToolkitScriptManager-->
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <asp:UpdatePanel id="UpdatePanel5" runat="server">
              <ContentTemplate>
                 <fieldset>
                        <div class="row top10">                                 
                                    <div class="col-sm-2" style="background-color:white;">
                                        <asp:Label id="spanjmbg" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lbljmbg" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                    </div>
                                    <div class="col-sm-10" style="background-color:white;">
                                        <asp:TextBox ID="txtjmbg" runat="server" class="txtbox1" style="font-size:13px;" maxlength="13" ontextchanged="txtjmbg_TextChanged" OnClientClick="return CheckIfChannelHasChanged();" AutoPostBack="true" onkeydown="keydownFunctionJmbg(); return true;" TabIndex="0"></asp:TextBox>
                                        <asp:Label ID="errLabel" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                        <asp:CustomValidator runat="server" id="cvjmbg" controltovalidate="txtjmbg" errormessage="" OnServerValidate="cvjmbg_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                    </div>
                        </div>
                        <div class="row top10">                                 
                                    <div class="col-sm-2" style="background-color:white;">
                                        <asp:Label id="spandatumrodjenja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lbldatumrodjenja" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                    </div>
                                    <div class="col-sm-10" style="background-color:white;">
                                        <asp:TextBox ID="txtdatumrodjenja" runat="server" class="txtbox2" style="font-size:13px;" maxlength="11" tabindex="-1"></asp:TextBox>
                                    </div>
                        </div>
                </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel4" runat="server">
              <ContentTemplate>
                 <fieldset>
                    <div class="row top10">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spansertjmbg" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblsertjmbg" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:DropDownList ID="ddlsertjmbg" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox1" DataSourceID="odsSertJMBG" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlsertjmbg_SelectedIndexChanged" TabIndex="0">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsSertJMBG" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="zahtev-izdavanje-fizicko-lice.aspx" Name="filename" Type="String" />
                                            <asp:Parameter DefaultValue="ddlsertjmbg" Name="controlid" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <br><p class="notification"><asp:Label id="lblnotification" runat="server" style="font-size:13px;"></asp:Label></p>
                                    <asp:CustomValidator runat="server" id="cvsertjmbg" controltovalidate="ddlsertjmbg" OnServerValidate="cvsertjmbg_ServerValidate" errormessage="" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                 </fieldset>
              </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel3" runat="server">
              <ContentTemplate>
                 <fieldset>
                    <div class="row top15">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanvrstadokumenta" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblvrstadokumenta" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:DropDownList ID="ddlvrstadokumenta" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox2" DataSourceID="odsVrstaDokumenta" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlvrstadokumenta_SelectedIndexChanged" TabIndex="0">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsVrstaDokumenta" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="zahtev-izdavanje-fizicko-lice.aspx" Name="filename" Type="String" />
                                            <asp:Parameter DefaultValue="ddlvrstadokumenta" Name="controlid" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:CustomValidator runat="server" id="cvvrstadokumenta" controltovalidate="ddlvrstadokumenta" OnServerValidate="cvvrstadokumenta_ServerValidate" errormessage="" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                    <!-------------------------------------------OVO SE NE VIDI------------------------------------------------>
                    <div class="row top10" id="myDiv1" runat="server">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanbrojiddokumenta" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblbrojiddokumenta" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtbrojiddokumenta" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" Visible="True" TabIndex="0"></asp:TextBox>
                                    <asp:CustomValidator runat="server" id="cvbrojiddokumenta" controltovalidate="txtbrojiddokumenta" errormessage="" OnServerValidate="cvbrojiddokumenta_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                    <div class="row top10" id="myDiv2" runat="server">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanimeinstitucije" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblimeinstitucije" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtimeinstitucije" runat="server" class="txtbox5" style="font-size:13px;" maxlength="50" Visible="True" TabIndex="0"></asp:TextBox>
                                    <asp:CustomValidator runat="server" id="cvimeinstitucije" controltovalidate="txtimeinstitucije" errormessage="" OnServerValidate="cvimeinstitucije_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                    <div class="row top10" id="myDiv3" runat="server">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spandatumizdavanja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lbldatumizdavanja" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtdatumizdavanja" runat="server" class="txtbox2" style="font-size:13px;" maxlength="10" TabIndex="0"></asp:TextBox>
                                    <asp:Label id="IssueExample" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                                    <asp:CustomValidator runat="server" id="cvdatumizdavanja" controltovalidate="txtdatumizdavanja" errormessage="" OnServerValidate="cvdatumizdavanja_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>  
                    <div class="row top10" id="myDiv4" runat="server">                
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spandatumisteka" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lbldatumisteka" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                        <asp:TextBox ID="txtdatumisteka" runat="server" class="txtbox2" style="font-size:13px;" maxlength="10" TabIndex="0"></asp:TextBox>                                                                                                         
                                        <asp:Label id="ExpiryExample" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                                        <asp:CustomValidator runat="server" id="cvdatumisteka" controltovalidate="txtdatumisteka" errormessage="" OnServerValidate="cvdatumisteka_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                  </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
                    <!---------------------------------------------------------------------------------------------------------->
        </div>
        <div class="container-fluid" style="margin-left: 10px">
             <asp:UpdatePanel id="UpdatePanel10" runat="server">
                <ContentTemplate>
                    <fieldset>
                            <div class="row top20">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanadresaeposte" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lbladresaeposte" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtadresaeposte" runat="server" class="txtbox3" style="font-size:13px;" maxlength="50" ontextchanged="txtadresaeposte_TextChanged" OnClientClick="return CheckIfChannelHasChanged2();" AutoPostBack="true" onkeydown="keydownFunctionMail(); return true;" TabIndex="0"></asp:TextBox>
                                    <asp:Label id="errLabelMail" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                    <asp:CustomValidator runat="server" id="cvadresaeposte" controltovalidate="txtadresaeposte" errormessage="" OnServerValidate="cvadresaeposte_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel9" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <div class="row top10">
                                    <div class="col-sm-2" style="background-color:white;">
                                        <asp:Label id="spantelefon" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lbltelefon" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                    </div>
                                    <div class="col-sm-10" style="background-color:white;">
                                        <asp:TextBox ID="txttelefon" runat="server" class="txtbox" style="font-size:13px;" maxlength="11" ontextchanged="txttelefon_TextChanged" OnClientClick="return CheckIfChannelHasChanged1();" AutoPostBack="true" onkeydown="keydownFunctionTelefon(); return true;" TabIndex="0"></asp:TextBox>
                                        <asp:Label id="TelefonPrimer" runat="server" style="font-weight:bold;font-size: 13px; margin-right: 10px;"> </asp:Label>
                                        <asp:Label ID="errLabelNumber" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                        <asp:CustomValidator runat="server" id="cvtelefon" controltovalidate="txttelefon" errormessage="" OnServerValidate="cvtelefon_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                    </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel2" runat="server">
              <ContentTemplate>
                <cc1:AutoCompleteExtender 
                    runat="server" 
                    ID="autoCompleteMestoBoravka" 
                    ServicePath="WebServiceMesta.asmx"
                    TargetControlID="txtmesto"
                    ServiceMethod="GetMesta"
                    MinimumPrefixLength="2"
                    CompletionSetCount="100"
                    CompletionInterval="500"
                    EnableCaching="false"
                    OnClientItemSelected="getSelectedMesto"
                >
                </cc1:AutoCompleteExtender>
                <cc1:AutoCompleteExtender 
                    runat="server" 
                    ID="autoCompleteUlicaBoravka" 
                    ServicePath="WebServiceMesta.asmx"
                    TargetControlID="txtulica"
                    ServiceMethod="GetUlica"
                    MinimumPrefixLength="2"
                    CompletionSetCount="100"
                    CompletionInterval="500"
                    EnableCaching="false"
                    UseContextKey="true"                    
                    OnClientPopulating="getMestoAgain"
                >
                </cc1:AutoCompleteExtender>               
                  <fieldset>
                    <div class="row top10">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanadresasert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lbladresasert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:DropDownList ID="ddlsertadresa" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox4" DataSourceID="odsSertAdresa" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlsertadresa_SelectedIndexChanged" TabIndex="0">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsSertAdresa" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="zahtev-izdavanje-fizicko-lice.aspx" Name="filename" Type="String" />
                                            <asp:Parameter DefaultValue="ddlsertadresa" Name="controlid" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:CustomValidator runat="server" id="cvsertadresa" controltovalidate="ddlsertadresa" errormessage="" OnServerValidate="cvsertadresa_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                    <div class="row top15">                                 
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanmesto" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblmesto" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtmesto" runat="server" readonly="true" class="txtboxgrey" style="font-size:13px;" maxlength="30"></asp:TextBox>
                                    <asp:CustomValidator runat="server" id="cvmesto" controltovalidate="txtmesto" errormessage="" OnServerValidate="cvmesto_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div> 
                    <div class="row top10">                                 
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanulica" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblulica" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtulica" runat="server" readonly="true" class="txtboxgrey5" style="font-size:13px;" maxlength="50"></asp:TextBox>
                                    <asp:CustomValidator runat="server" id="cvulica" controltovalidate="txtulica" errormessage="" OnServerValidate="cvulica_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                    <div class="row top10">                                 
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanbroj" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblbroj" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtbroj" runat="server" readonly="true" class="txtboxgrey" style="font-size:13px;" maxlength="20" ontextchanged="txtbroj_TextChanged" OnClientClick="return CheckIfChannelHasChanged9();" AutoPostBack="true"></asp:TextBox>
                                    <asp:Label ID="errLabelBroj" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                    <asp:CustomValidator runat="server" id="cvbroj" controltovalidate="txtbroj" errormessage="" OnServerValidate="cvbroj_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>                                  
                                </div>
                    </div>
                 </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel id="UpdatePanel11" runat="server">
              <ContentTemplate>
                  <fieldset>
                    <div class="row top10">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanpostanskibroj" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblpostanskibroj" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtpostanskibroj" runat="server" readonly="true" class="txtboxgrey" style="font-size:13px;" maxlength="5"></asp:TextBox>
                                    <asp:CustomValidator runat="server" id="cvpostanskibroj" controltovalidate="txtpostanskibroj" OnServerValidate="cvpostanskibroj_ServerValidate" errormessage="" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                    <div class="row top10">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanpak" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblpak" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtpak" runat="server" readonly="true" class="txtboxgrey9" style="font-size:13px;" maxlength="6"></asp:TextBox>
                                    <asp:CustomValidator runat="server" id="cvpak" controltovalidate="txtpak" errormessage="" OnServerValidate="cvpak_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel> 
        <asp:UpdatePanel id="UpdatePanel1" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="row top10">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanrokkoriscenja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblrokkoriscenjasert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:DropDownList ID="ddlrokkoriscenjasert" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox1" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlrokkoriscenjasert_SelectedIndexChanged" DataSourceID="odsRokKoriscenjaSert" TabIndex="0">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsRokKoriscenjaSert" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="zahtev-izdavanje-fizicko-lice.aspx" Name="filename" Type="String" />
                                            <asp:Parameter DefaultValue="ddlrokkoriscenjasert" Name="controlid" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:CustomValidator runat="server" id="cvrokkoriscenjasert" controltovalidate="ddlrokkoriscenjasert" errormessage="" OnServerValidate="cvrokkoriscenjasert_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel id="UpdatePanel6" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="row top10">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spanmedijsert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblmedijsert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:DropDownList ID="ddlmedijsert" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox1" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlmedijsert_SelectedIndexChanged" DataSourceID="odsMedijSert" TabIndex="0">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsMedijSert" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="zahtev-izdavanje-fizicko-lice.aspx" Name="filename" Type="String" />
                                            <asp:Parameter DefaultValue="ddlmedijsert" Name="controlid" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:CustomValidator runat="server" id="cvmedijsert" controltovalidate="ddlmedijsert" errormessage="" OnServerValidate="cvmedijsert_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel id="UpdatePanel7" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="row top10">
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spannacinplacanja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblnacinplacanja" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:DropDownList ID="ddlnacinplacanja" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox1" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlnacinplacanja_SelectedIndexChanged" DataSourceID="odsNacinPlacanja" TabIndex="0">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsNacinPlacanja" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="zahtev-izdavanje-fizicko-lice.aspx" Name="filename" Type="String" />
                                            <asp:Parameter DefaultValue="ddlnacinplacanja" Name="controlid" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:CustomValidator runat="server" id="cvnacinplacanja" controltovalidate="ddlnacinplacanja" errormessage="" OnServerValidate="cvnacinplacanja_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                    <br />
                            <p class="notification">
                                <asp:Label id="lblnotification1" runat="server" style="font-size:13px;">
                                    
                                </asp:Label>
                            </p>
                                </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel id="UpdatePanel8" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="row top10">                                 
                                <div class="col-sm-2" style="background-color:white;">
                                    <asp:Label id="spancenasaporezom" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblcenasaporezom" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtcenasaporezom" runat="server" class="txtbox1" style="border:solid grey 1px; padding: 3px; text-align: right; height: 26px; width: !important;" tabindex="-1"></asp:TextBox>
                                    <asp:Label id="lbldinara" runat="server" style="font-weight:bold;font-size:13px;"><span style="COLOR: white; font-size:17px;">&nbsp;</span> </asp:Label>
                                </div>
                    </div>
               </fieldset>
           </ContentTemplate>
        </asp:UpdatePanel>     
        <div class="row">
                <br />
        </div>
        <div class="row">
                <br />
        </div>
     </div>
        <!-------------------------------------------OVO SE NE VIDI------------------------------------------------>
        <div class="container-fluid" id="myDiv6" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                        <div class="col-sm-2">
                            <asp:Label id="lbldugme" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                        </div>
                        <div class="col-sm-8">
                            <asp:Button ID="btnEnterRequest" runat="server" class="btn btn-primary save" Text="" style="margin-right: 8px;" OnClick="btnEnterRequest_Click1" onclientclick="unhook()"/>
                            <asp:Button ID="btnReEnterRequest" runat="server" class="btn btn-warning edit" Text="" style="margin-right: 8px;" OnClick="btnReEnterRequest_Click1" onclientclick="unhook()"/>
                        </div>
                        <div class="col-sm-2">
                            <asp:Label id="lbldugme1" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                        </div>
            </div>
        </div>  

        <!---------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                        <div class="col-sm-2">
                            <asp:Label id="lbldugme2" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                        </div>
                        <div class="col-sm-8">
                            <asp:Button ID="btnSubmit" runat="server" class="btn btn-primary save" Text="" onclick="btnSubmit_Click1" onclientclick="unhook()" TabIndex="0"/>                           
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
        <div id="throbber" style="display:none;">
            <p style="font-size:20px; font-weight: bold;"><b>Molimo sačekajte...</b></p>
            <asp:Image ID="imgThrobber" imageurl="~/Content/Gif/throbber.gif" runat="server" style="width:35px;height:35px;"/>
        </div>
    </form>
</body>
</html>
