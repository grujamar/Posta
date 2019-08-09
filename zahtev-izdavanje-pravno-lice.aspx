<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="zahtev-izdavanje-pravno-lice.aspx.cs" Inherits="zahtev_izdavanje_pravno_lice" %>
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
        function errorSOAPalert() {
            swal({
                title: 'Greška u slanju zahteva, nije uspelo slanje SOAP poruke.',
                text: 'Popunite zahtev kasnije i pokušajte ponovo.',
                type: 'OK'
            });
        }
        function keydownFunctionJmbg()
           {
               var jmbgpost = document.getElementById('<%=txtjmbg.ClientID %>');

               if (jmbgpost.value.length == 0 || jmbgpost.value.length == 1)
                   document.getElementById('errLabel1').style.display = 'none';
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
        function keydownFunctionURL()
           {
               var urlpost = document.getElementById('<%=txtwebadresa.ClientID %>');

            if (urlpost.value.length == 0 || urlposts.value.length == 1)
                   document.getElementById('errLabelURL').style.display = 'none';
        }
        function DisabledButton() {
            document.getElementById('btnAddAuthorizedPersonalUser').disabled = true;
        }
        function EnabledButton() {
            document.getElementById('btnAddAuthorizedPersonalUser').disabled = false;
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
        }
        function getSelectedMesto1(source, eventArgs) {
            $find('autoCompleteUlicaBoravka1').set_contextKey(eventArgs.get_value());
        }
        function getMestoAgain(source, eventArgs) {
            $find('autoCompleteUlicaBoravka').set_contextKey(document.getElementById('<%=this.txtmesto.ClientID%>').value);
        }
        function getMestoAgain1(source, eventArgs) {
            $find('autoCompleteUlicaBoravka1').set_contextKey(document.getElementById('<%=this.txtmesto1.ClientID%>').value);
        }
        </script>
        <style>
        .ui-priority-secondary, .ui-widget-content .ui-priority-secondary, .ui-widget-header .ui-priority-secondary {
            font-weight: bold;
            opacity: 1;
        }
        h1 {
            border-top: double;
        }
        h2 {
            border-bottom: double;
        }
        .save {
        padding: 15px 30px 15px 43px;
        background: url(Content/Images/save.png) left 12px top 13px no-repeat #4275c9;
        }
        .edit {
            padding: 15px 30px 15px 43px;
            background: url(Content/Images/edit.png) left 12px top 13px no-repeat #FFA500;
        }
        .edit1 {
            padding: 15px 30px 15px 43px;
            background: url(Content/Images/edit.png) left 12px top 13px no-repeat #4275c9;
        }
        .arrowLeft {
            padding: 15px 30px 15px 43px;
            background: url(Content/Images/arrowLeft.png) left 12px top 13px no-repeat #FFA500;
        }
        </style>
        <script>
       <!--Validate All Textbox Fields for btnAddAuthorizedPersonalUser change visible.Use onkeyup for textbox fields. For help link https://www.youtube.com/watch?v=0U8AVjTQJtg-->
           function change(textb,buttont)
           {
                var firstt = document.getElementById('<%=txtmaticnibroj.ClientID %>');                
                var thirdt = document.getElementById('<%=txtpib.ClientID %>');
                var fourtht = document.getElementById('<%=txtsifradel.ClientID %>');
                var fivetht = document.getElementById('<%=txtulica.ClientID %>');
                var sixtht = document.getElementById('<%=txtbroj.ClientID %>');
                var seventht = document.getElementById('<%=txtpostanskibroj.ClientID %>');
                var eightht = document.getElementById('<%=txtmesto.ClientID %>');
                var nintht = document.getElementById('<%=txtkontakttel.ClientID %>');
                var tentht = document.getElementById('<%=txtadresaeposte.ClientID %>');
                var eleventht = document.getElementById('<%=txtimezz.ClientID %>');
                var twelftht = document.getElementById('<%=txtprezimezz.ClientID %>');

               if (firstt.value.length == 8 && thirdt.value.length == 9 && fourtht.value.length == 4 && fivetht.value.length >= 1 && sixtht.value.length >= 1 && seventht.value.length == 5 && eightht.value.length >= 1 && nintht.value.length >= 1 && tentht.value.length >= 1 && eleventht.value.length >= 1 && twelftht.value.length >= 1)
                    document.getElementById(buttont).disabled = false;
                else 
                    document.getElementById(buttont).disabled = true;
           }
           function keydownFunctionAddress()
           {
               var addressepost = document.getElementById('<%=txtadresaeposte.ClientID %>');

               if (addressepost.value.length == 0 || addressepost.value.length == 1)
                   document.getElementById('errLabel').style.display = 'none';
           }
           function keydownFunctionPIB()
           {
               var pib = document.getElementById('<%=txtpib.ClientID %>');

               if (pib.value.length == 0 || pib.value.length == 1)
                   document.getElementById('errLabelPIB').style.display = 'none';

           }
            function keydownFunctionKontaktTel()
            {
               var kontakttel = document.getElementById('<%=txtkontakttel.ClientID %>');

                if (kontakttel.value.length == 0 || kontakttel.value.length == 1)
                   document.getElementById('errLabelKontaktTel').style.display = 'none';
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
        <div class="container-fluid">
            <h4><b style="color:#6b6b6b;"><asp:Label id="lblstranicanaziv" runat="server" style="font-size:18px;"></asp:Label></b></h4>
        </div>
        <div class="container-fluid top10">
            <asp:Image ID="pdfdocument" imageurl="~/Content/Images/pdf_icon.png" runat="server" style="width:20px;" />
            <asp:HyperLink id="pdfhyperlink1" runat="server" NavigateUrl="~/dokumentacija/Uputstvo-izdavanje-pravno-lice.pdf" target="_blank" style="vertical-align:bottom" TabIndex="-1"><asp:Label id="lblkorisniskouputstvo" runat="server" style="font-size:15px;"></asp:Label></asp:HyperLink>
        </div>
        <div class="row">
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
        <div class="container-fluid" id="Container00" runat="server" style="margin-left: 10px; margin-: 10px;">
            <div class="page-header" style="color:darkblue">
                <h4><asp:Label id="lblpodacipravnolice" runat="server" style="font-size:18px;"></asp:Label></h4>
            </div>   
        </div>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!--AJAX ToolkitScriptManager-->
        <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
        <div class="container-fluid" id="Container1" runat="server" style="margin-left: 10px">
          <asp:UpdatePanel id="UpdatePanel5" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="row top10">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanidentificationnumber" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblidentificationnumber" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtmaticnibroj" runat="server" class="txtbox2" style="font-size:13px;" maxlength="8" ontextchanged="txtmaticnibroj_TextChanged" OnClientClick="return CheckIfChannelHasChanged7();" AutoPostBack="true" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" TabIndex="1"></asp:TextBox>
                            <asp:Label ID="errLabelIN" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                            <asp:CustomValidator runat="server" id="cvmaticnibroj" controltovalidate="txtmaticnibroj" errormessage="" OnServerValidate="cvmaticnibroj_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row top10" id="rowLegalEntityName" runat="server">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanlegalname" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lbllegalname" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtnazivpravnoglica" runat="server" class="txtbox5" style="font-size:13px;" maxlength="250" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" TabIndex="0"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvnazivpravnoglica" controltovalidate="txtnazivpravnoglica" errormessage="" OnServerValidate="cvnazivpravnoglica_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                 </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel id="UpdatePanel22" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="row top10" id="rowLegalEntityDDL" runat="server">                                 
                        <div class="col-sm-12" style="background-color:white;">
                            <asp:Label id="spannazivpravnoglicaDDL" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;<asp:Label id="lblnazivpravnoglicaDDL" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                        </div>
                    </div>
                    <div class="row top10" id="rowLegalEntityDDL1" runat="server" >
                        <div class="col-sm-2" style="background-color:white;">
                            <asp:RadioButton ID="rbChooseName" runat="server" Text="" OnCheckedChanged="rbChooseName_CheckedChanged" AutoPostBack="True" onclick="unhook()" style="font-size:12px;" TabIndex="0"/>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:DropDownList ID="ddlLegalEntityName" runat="server" AppendDataBoundItems="False" AutoPostBack="True" class="txtbox5" OnSelectedIndexChanged="ddlLegalEntityName_SelectedIndexChanged" TabIndex="0">
                            </asp:DropDownList>
                            <asp:CustomValidator runat="server" id="cvLegalEntityName" controltovalidate="ddlLegalEntityName" errormessage="" OnServerValidate="cvLegalEntityName_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row top10" id="rowLegalEntityDDL2" runat="server" >
                        <div class="col-sm-2" style="background-color:white;">
                            <asp:RadioButton ID="rbWriteName" runat="server" Text="" OnCheckedChanged="rbWriteName_CheckedChanged" AutoPostBack="True" onclick="unhook()" style="font-size:12px;" TabIndex="0"/>&nbsp;
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtwritename" runat="server" class="txtbox5" style="font-size:13px;" maxlength="250" TabIndex="0"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvwritename" controltovalidate="txtwritename" errormessage="" OnServerValidate="cvwritename_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
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
                                <asp:Label id="spanpib" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblpib" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                            </div>  
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtpib" runat="server" class="txtbox2" style="font-size:13px;" maxlength="9" ontextchanged="txtpib_TextChanged" OnClientClick="return CheckIfChannelHasChanged1();" AutoPostBack="true" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" onkeydown="keydownFunctionPIB(); return true;" TabIndex="0"></asp:TextBox>
                            <asp:Label ID="errLabelPIB" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                            <asp:CustomValidator runat="server" id="cvpib" controltovalidate="txtpib" errormessage="" OnServerValidate="cvpib_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel id="UpdatePanel1" runat="server">
            <ContentTemplate>
            <fieldset>
                <div class="row top10">
                    <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                        <div class="w-2-forme w-8-forme-md">
                            <asp:Label id="spanpdvpayer" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                        </div>
                        <div class="w-98-forme w-92-forme-md">
                            <asp:Label id="lblpdvpayer" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                        </div>   
                    </div>
                    <div class="col-sm-10" style="background-color:white;">
                        <asp:DropDownList ID="ddlobveznikpdv" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox2" DataSourceID="odsObveznikPDV" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlobveznikpdv_SelectedIndexChanged" TabIndex="0">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsObveznikPDV" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                            <SelectParameters>
                                <asp:Parameter DefaultValue="zahtev-izdavanje-pravno-lice.aspx" Name="filename" Type="String" />
                                <asp:Parameter DefaultValue="ddlobveznikpdv" Name="controlid" Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                        <asp:CustomValidator runat="server" id="cvobveznikpdv" controltovalidate="ddlobveznikpdv" errormessage="" OnServerValidate="cvobveznikpdv_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                    </div>
                </div>               
                <div class="row top10">                                 
                    <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                        <div class="w-2-forme w-8-forme-md">
                            <asp:Label id="spanbtc" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                        </div>
                        <div class="w-98-forme w-92-forme-md">
                            <asp:Label id="lblbtc" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                        </div>  
                    </div>
                    <div class="col-sm-10" style="background-color:white;">
                        <asp:TextBox ID="txtsifradel" runat="server" class="txtbox" style="font-size:13px;" maxlength="4" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" TabIndex="0"></asp:TextBox>
                        <asp:CustomValidator runat="server" id="cvsifradel" controltovalidate="txtsifradel" errormessage="" OnServerValidate="cvsifradel_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                    </div>
                </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel id="UpdatePanel18" runat="server">
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
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanmesto" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblmesto" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div> 
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtmesto" runat="server" class="txtbox" style="font-size:13px;" maxlength="20" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" TabIndex="0"></asp:TextBox>                                        
                                <asp:CustomValidator runat="server" id="cvmesto" controltovalidate="txtmesto" errormessage="" OnServerValidate="cvmesto_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanulica" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblulica" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>  
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtulica" runat="server" class="txtbox5" style="font-size:13px;" maxlength="50" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" TabIndex="0"></asp:TextBox>
                                <asp:CustomValidator runat="server" id="cvulica" controltovalidate="txtulica" errormessage="" OnServerValidate="cvulica_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel6" runat="server">
              <ContentTemplate>
                    <fieldset>
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanbroj" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblbroj" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtbroj" runat="server" class="txtbox" style="font-size:13px;" maxlength="20" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" ontextchanged="txtbroj_TextChanged" OnClientClick="return CheckIfChannelHasChanged9();" AutoPostBack="true" TabIndex="0"></asp:TextBox>
                                <asp:Label ID="errLabelBroj" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvbroj" controltovalidate="txtbroj" errormessage="" OnServerValidate="cvbroj_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel19" runat="server">
              <ContentTemplate>
                  <fieldset>
                    <div class="row top10">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanpostanskibroj" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;  
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblpostanskibroj" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>   
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtpostanskibroj" runat="server" class="txtbox" style="font-size:13px;" maxlength="5" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" TabIndex="0"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvpostanskibroj" controltovalidate="txtpostanskibroj" OnServerValidate="cvpostanskibroj_ServerValidate" errormessage="" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row top10">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanpak" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;   
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblpak" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>    
                            </div> 
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtpak" runat="server" class="txtbox2" style="font-size:13px;" maxlength="6" TabIndex="0"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvpak" controltovalidate="txtpak" errormessage="" OnServerValidate="cvpak_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                 </fieldset>
               </ContentTemplate>
            </asp:UpdatePanel> 
            <asp:UpdatePanel id="UpdatePanel15" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                   <asp:Label id="spankontakttel" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp; 
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblkontakttel" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtkontakttel" runat="server" class="txtbox1" style="font-size:13px;" maxlength="11" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" ontextchanged="txtkontakttel_TextChanged" OnClientClick="return CheckIfChannelHasChanged5();" AutoPostBack="true" onkeydown="keydownFunctionKontaktTel()" TabIndex="0"></asp:TextBox>
								<asp:Label ID="TelefonPrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                                <asp:Label ID="errLabelKontaktTel" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvkontakttel" controltovalidate="txtkontakttel" errormessage="" OnServerValidate="cvkontakttel_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel4" runat="server">
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
                                <asp:TextBox ID="txtadresaeposte" runat="server" class="txtbox3" style="font-size:13px;" maxlength="50" ontextchanged="txtadresaeposte_TextChanged" OnClientClick="return CheckIfChannelHasChanged();" AutoPostBack="true" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" onkeydown="keydownFunctionAddress(); return true;" TabIndex="0"></asp:TextBox>
                                <asp:Label ID="errLabel" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvadresaeposte" controltovalidate="txtadresaeposte" errormessage="" OnServerValidate="cvadresaeposte_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                <br><p class="notification" style="margin-bottom: 3px;"><asp:Label id="lblnotification2" runat="server" style="font-size:13px;"></asp:Label></p>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel id="UpdatePanel17" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <div class="row top10">                                 
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spanwebadresa" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblwebadresa" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtwebadresa" runat="server" class="txtbox5" style="font-size:13px;" maxlength="50" ontextchanged="txtwebadresa_TextChanged" OnClientClick="return CheckIfChannelHasChanged6();" AutoPostBack="true" onkeydown="keydownFunctionURL(); return true;" TabIndex="0"></asp:TextBox>
                                <asp:Label ID="errLabelURL" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvwebadresa" controltovalidate="txtwebadresa" errormessage="" OnServerValidate="cvwebadresa_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                     </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
            <asp:UpdatePanel id="UpdatePanel23" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <div class="page-header" style="color:darkblue">
                            <h4><asp:Label id="lblnotification4" runat="server" style="font-size:18px;"></asp:Label></h4>
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
                                <asp:TextBox ID="txtimezz" runat="server" class="txtbox5" style="font-size:13px;" maxlength="64" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" TabIndex="0"></asp:TextBox>
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
                                <asp:TextBox ID="txtprezimezz" runat="server" class="txtbox5" style="font-size:13px;" maxlength="64" onkeyup="change(this,'btnAddAuthorizedPersonalUser');" TabIndex="0"></asp:TextBox>
                                <asp:CustomValidator runat="server" id="cvprezimezz" controltovalidate="txtprezimezz" errormessage="" OnServerValidate="cvprezimezz_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="Container3" runat="server" style="margin-left: 10px">
                <div class="page-header" style="color:darkblue">
                    <h4><asp:Label id="lblnotification5" runat="server" style="font-size:18px;"></asp:Label></h4>
                </div>
            
                            <div class="row" style="margin-left: 0px; margin-right: 10px;">
                                <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" HorizontalAlign="Center" ShowHeaderWhenEmpty="True" 
                                Width="100%" BackColor="#F4F7F6" BorderColor="#146B93" BorderStyle="None" BorderWidth="10px" CellPadding="3" Font-Size="13px" CssClass="table table-bordered" DataKeyNames="Jmbg,SertJmbg,VrstaDokumenta,BrojIDDokumenta,ImeInstitucije,DatumIzdavanja,DatumIsteka,SertAdresa,Mesto,Ulica,Broj,PostanskiBroj,CenaSaPorezom,PAK" OnRowCommand="GridView1_RowCommand" OnPageIndexChanging="GridView1_PageIndexChanging" TabIndex="0" PageSize="20">
                        <Columns>
                            <asp:TemplateField HeaderText="R. br."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="Ime" HeaderText="Ime" SortExpression="Ime" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Prezime" HeaderText="Prezime" SortExpression="Prezime" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Jmbg" HeaderText="JMBG" SortExpression="Jmbg" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="AdresaEPoste" HeaderText="Adresa E-Pošte" SortExpression="Adresa E-Pošte" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Telefon" HeaderText="Telefon" SortExpression="Telefon" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Medij" HeaderText="Medij" SortExpression="Medij" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Rok" HeaderText="Rok" SortExpression="Rok" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <ItemTemplate >
                                    <asp:HiddenField ID="SertJmbg" runat="server" Value='<%# Eval("SertJmbg") %>' />
                                    <asp:HiddenField ID="VrstaDokumenta" runat="server" Value='<%# Eval("VrstaDokumenta") %>' />
                                    <asp:HiddenField ID="BrojIDDokumenta" runat="server" Value='<%# Eval("BrojIDDokumenta") %>' />
                                    <asp:HiddenField ID="ImeInstitucije" runat="server" Value='<%# Eval("ImeInstitucije") %>' />
                                    <asp:HiddenField ID="DatumIzdavanja" runat="server" Value='<%# Eval("DatumIzdavanja") %>' />
                                    <asp:HiddenField ID="DatumIsteka" runat="server" Value='<%# Eval("DatumIsteka") %>' />
                                    <asp:HiddenField ID="SertAdresa" runat="server" Value='<%# Eval("SertAdresa") %>' />
                                    <asp:HiddenField ID="Mesto" runat="server" Value='<%# Eval("Mesto") %>'  />
                                    <asp:HiddenField ID="Ulica" runat="server" Value='<%# Eval("Ulica") %>' />
                                    <asp:HiddenField ID="Broj" runat="server" Value='<%# Eval("Broj") %>' />
                                    <asp:HiddenField ID="PostanskiBroj" runat="server" Value='<%# Eval("PostanskiBroj") %>'  />
                                    <asp:HiddenField ID="CenaSaPorezom" runat="server" Value='<%# Eval("CenaSaPorezom") %>' />
                                </ItemTemplate>                         
                            </asp:TemplateField>
                            <asp:TemplateField>                                
                                <ItemTemplate>                                    
                                    <asp:Button ID="btnDetail" runat="server"
                                        CommandName="ViewProfile" ToolTip="Prikaži detalje" Text="Detalji" CommandArgument='<%# Container.DisplayIndex %>' onclientclick="unhook()"/>
                                    <asp:Button ID="btnEdit" runat="server"
                                        CommandName="EditProfile" ToolTip="Izmeni" Text="Izmeni" CommandArgument='<%# Container.DisplayIndex %>' onclientclick="unhook()"/>
                                    <asp:Button ID="btnDelete" runat="server"
                                        CommandName="DeleteProfile" ToolTip="Obriši" Text="Obriši" CommandArgument='<%# Container.DisplayIndex %>' onclientclick="unhook()"/>                                      
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            </Columns>
                                    <HeaderStyle Font-Bold="True" Font-Size="15px" ForeColor="#146B93" />
                                </asp:GridView>
                            </div>                                               

          </div>
          <div class="row">
                <br />
          </div>
          <div class="container-fluid" id="Container33" runat="server" style="margin-left: 10px;">
            <div class="row top20">
                <div class="col-sm-2">
                    <!--#379e34 when button true--->
                    <asp:Button ID="btnAddAuthorizedPersonalUser" runat="server" class="btn btn-success buttonborder2" Text="" onclick="btnAddAuthorizedPersonalUser_Click1" onclientclick="unhook()" ForeColor="#FFFFCC" BackColor="#4CB74C" TabIndex="0"/>                            
                </div>
                <div class="col-sm-10">
                    <label for="lblAddAuthorizedPersonalUser" style="font-size: 13px; margin-right: 10px; COLOR: gray;"><asp:Label id="lblnotification6" runat="server" style="font-size:13px;"></asp:Label></label>
                    <asp:CustomValidator runat="server" id="cvGridView1" errormessage="" ControlToValidate="" OnServerValidate="cvGridView1_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;"/>
                </div>
            </div>  
          </div>
            <div class="row">
                <br />
                <br />
            </div>
          <div class="container-fluid" id="Container34" runat="server" style="margin-left: 10px;">
                <asp:UpdatePanel id="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <fieldset>
                            <div class="row top10">                                 
                                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                    <div class="w-2-forme w-8-forme-md">
                                        <asp:Label id="spancenasaporezom" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                    </div>
                                    <div class="w-98-forme w-92-forme-md">
                                        <asp:Label id="lblcenasaporezom" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                    </div>   
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtcenasaporezom" runat="server" text="0,00" readonly="true" class="txtbox1" style="border:solid grey 1px; padding: 3px; text-align: right; height: 26px; width: !important;" TabIndex="-1"></asp:TextBox>
                                    <asp:Label id="lbldinara" runat="server" style="font-weight:bold;font-size:13px;"><span style="COLOR: white; font-size:17px;">&nbsp;</span> </asp:Label> 
                                </div>
                            </div>
                       </fieldset>
                   </ContentTemplate>
                </asp:UpdatePanel>
                <div class="row">
                 <br />
                </div>
        </div>
        
        <!---------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------OVO SE NE VIDI------------------------------------------------->
        <div class="container-fluid" id="myDiv6" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <asp:Label id="lbldugme" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                </div>
                <div class="col-sm-10">
                    <asp:Button ID="btnEnterRequest" runat="server" class="btn btn-primary save" Text="" style="margin-right: 8px;" OnClick="btnEnterRequest_Click1" onclientclick="unhook()" />
                    <asp:Button ID="btnReEnterRequest" runat="server" class="btn btn-warning edit" Text="" style="margin-right: 8px;" OnClick="btnReEnterRequest_Click1" onclientclick="unhook()" />
                </div>
                <div class="col-sm-2">
                    <asp:Label id="lbldugme1" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                </div>
            </div>
        </div>  

        <!---------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <asp:Label id="lbldugme2" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                </div>
                <div class="col-sm-10">
                    <asp:Button ID="btnSubmit" runat="server" class="btn btn-primary save" Text="" onclick="btnSubmit_Click1" onclientclick="unhook()"/>                            
                </div>
                <div class="col-sm-2">
                    <asp:Label id="lbldugme3" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label>  
                </div>
            </div>  
        </div>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="Container4" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row">
                <br />
            </div>
        </div>  
        <div class="row" id="Container5" runat="server">
            <br />
            <br />
        </div>
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------IFRAME------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!--<div class="container-fluid" style="margin-left: 10px">-->
            <!--<iframe id="ifrm" runat="server" width="100%" height="auto" scrolling="no" frameborder="0" style="border:double">-->
                        <div class="container-fluid" id="Container6" runat="server" style="margin-left: 10px; margin-top:-50px">
                            <h2></h2>
                            <div class="row top10"> 
                            <h3 style="color:#6b6b6b;">&nbsp;&nbsp;&nbsp;<asp:Label id="lblnotification7" runat="server" style="font-size:18px;"></asp:Label></h3>
                            </div>
                        </div>
                        <div class="container-fluid" id="Container7" runat="server" style="margin-left: 10px">
                            <div class="row top20">                                 
                                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                    <div class="w-2-forme w-8-forme-md">
                                        <asp:Label id="spanname" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                    </div>
                                    <div class="w-98-forme w-92-forme-md">
                                        <asp:Label id="lblime" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                    </div>
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" TabIndex="1"></asp:TextBox>
                                    <asp:Label id="ImePrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"> </asp:Label>
                                    <asp:CustomValidator runat="server" id="cvime" controltovalidate="txtime" errormessage="" OnServerValidate="cvime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                            </div>
                            <div class="row top10">                                 
                                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                    <div class="w-2-forme w-8-forme-md">
                                        <asp:Label id="spansurname" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                    </div>
                                    <div class="w-98-forme w-92-forme-md">
                                        <asp:Label id="lblprezime" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                    </div>   
                                </div>
                                <div class="col-sm-10" style="background-color:white;">
                                    <asp:TextBox ID="txtprezime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" TabIndex="0"></asp:TextBox>
                                    <asp:Label id="PrezimePrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                                    <asp:CustomValidator runat="server" id="cvprezime" controltovalidate="txtprezime" errormessage="" OnServerValidate="cvprezime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                </div>
                            </div>
                            <asp:UpdatePanel id="UpdatePanel7" runat="server">
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
                                            <asp:TextBox ID="txtjmbg" runat="server" class="txtbox1" style="font-size:13px;" maxlength="13" ontextchanged="txtjmbg_TextChanged" onclientclick="return CheckIfChannelHasChanged2();" AutoPostBack="true" onkeydown="keydownFunctionJmbg(); return true;" TabIndex="0"></asp:TextBox>
                                            <asp:Label ID="errLabel1" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                                            <asp:CustomValidator runat="server" id="cvjmbg" controltovalidate="txtjmbg" errormessage="" OnServerValidate="cvjmbg_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                        </div>
                                    </div>
                                    <div class="row top10">                                 
                                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                            <div class="w-2-forme w-8-forme-md">
                                                <asp:Label id="spandatumrodjenja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                            </div>
                                            <div class="w-98-forme w-92-forme-md">
                                                <asp:Label id="lbldatumrodjenja" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-sm-10" style="background-color:white;">
                                            <asp:TextBox ID="txtdatumrodjenja" runat="server" class="txtbox2" style="font-size:13px;" maxlength="11" tabindex="-1"></asp:TextBox>
                                        </div>
                                    </div>
                                </fieldset>
                              </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel id="UpdatePanel8" runat="server">
                              <ContentTemplate>
                                 <fieldset>
                                    <div class="row top10">
                                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                            <div class="w-2-forme w-8-forme-md">
                                                <asp:Label id="spansertjmbg" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                            </div>
                                            <div class="w-98-forme w-92-forme-md">
                                                <asp:Label id="lblsertjmbg" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                            </div>
                                        </div>
                                        <div class="col-sm-10" style="background-color:white;">
                                            <asp:DropDownList ID="ddlsertjmbg" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox1" DataSourceID="odsSertJMBG" DataTextField="ItemText" DataValueField="IDItem" TabIndex="0" OnSelectedIndexChanged="ddlsertjmbg_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <asp:ObjectDataSource ID="odsSertJMBG" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                                <SelectParameters>
                                                    <asp:Parameter DefaultValue="zahtev-izdavanje-pravno-lice.aspx" Name="filename" Type="String" />
                                                    <asp:Parameter DefaultValue="ddlsertjmbg" Name="controlid" Type="String" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                            <br><p class="notification"><asp:Label id="lblnotification8" runat="server" style="font-size:13px;"></asp:Label></p>
                                            <asp:CustomValidator runat="server" id="cvsertjmbg" controltovalidate="ddlsertjmbg" OnServerValidate="cvsertjmbg_ServerValidate" errormessage="" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                        </div>
                                    </div>
                                 </fieldset>
                              </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="container-fluid" id="Container77" runat="server" style="margin-left: 10px">
                            <asp:UpdatePanel id="UpdatePanel9" runat="server">
                              <ContentTemplate>
                                 <fieldset>
                                    <div class="row top15">
                                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                            <div class="w-2-forme w-8-forme-md">
                                                <asp:Label id="spanvrstadokumenta" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                            </div>
                                            <div class="w-98-forme w-92-forme-md">
                                                <asp:Label id="lblvrstadokumenta" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-sm-10" style="background-color:white;">
                                            <asp:DropDownList ID="ddlvrstadokumenta" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox2" DataSourceID="odsVrstaDokumenta" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlvrstadokumenta_SelectedIndexChanged" TabIndex="0">
                                            </asp:DropDownList>
                                            <asp:ObjectDataSource ID="odsVrstaDokumenta" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                                <SelectParameters>
                                                    <asp:Parameter DefaultValue="zahtev-izdavanje-pravno-lice.aspx" Name="filename" Type="String" />
                                                    <asp:Parameter DefaultValue="ddlvrstadokumenta" Name="controlid" Type="String" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                            <asp:CustomValidator runat="server" id="cvvrstadokumenta" controltovalidate="ddlvrstadokumenta" OnServerValidate="cvvrstadokumenta_ServerValidate" errormessage="" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                        </div>
                                    </div>
                                    <!-------------------------------------------OVO SE NE VIDI------------------------------------------------>
                                    <div class="row top10" id="myDiv1" runat="server">
                                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                            <div class="w-2-forme w-8-forme-md">
                                                <asp:Label id="spanbrojiddokumenta" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                            </div>
                                            <div class="w-98-forme w-92-forme-md">
                                                <asp:Label id="lblbrojiddokumenta" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-sm-10" style="background-color:white;">
                                            <asp:TextBox ID="txtbrojiddokumenta" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" Visible="True" TabIndex="0"></asp:TextBox>
                                            <asp:CustomValidator runat="server" id="cvbrojiddokumenta" controltovalidate="txtbrojiddokumenta" errormessage="" OnServerValidate="cvbrojiddokumenta_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                        </div>
                                    </div>
                                    <div class="row top10" id="myDiv2" runat="server">
                                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                            <div class="w-2-forme w-8-forme-md">
                                                <asp:Label id="spanimeinstitucije" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                            </div>
                                            <div class="w-98-forme w-92-forme-md">
                                                <asp:Label id="lblimeinstitucije" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                                            </div> 
                                        </div>
                                        <div class="col-sm-10" style="background-color:white;">
                                            <asp:TextBox ID="txtimeinstitucije" runat="server" class="txtbox5" style="font-size:13px;" maxlength="50" Visible="True" TabIndex="0"></asp:TextBox>
                                            <asp:CustomValidator runat="server" id="cvimeinstitucije" controltovalidate="txtimeinstitucije" errormessage="" OnServerValidate="cvimeinstitucije_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                        </div>
                                    </div>
                                    <div class="row top10" id="myDiv3" runat="server">
                                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                            <div class="w-2-forme w-8-forme-md">
                                                <asp:Label id="spandatumizdavanja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                            </div>
                                            <div class="w-98-forme w-92-forme-md">
                                                <asp:Label id="lbldatumizdavanja" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-sm-10" style="background-color:white;">
                                            <asp:TextBox ID="txtdatumizdavanja" runat="server" class="txtbox2" style="font-size:13px;" maxlength="10" TabIndex="0"></asp:TextBox>
                                            <asp:Label id="IssueExample" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                                            <asp:CustomValidator runat="server" id="cvdatumizdavanja" controltovalidate="txtdatumizdavanja" errormessage="" OnServerValidate="cvdatumizdavanja_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                        </div>
                                    </div>  
                                    <div class="row top10" id="myDiv4" runat="server">                
                                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                            <div class="w-2-forme w-8-forme-md">
                                                <asp:Label id="spandatumisteka" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                            </div>
                                            <div class="w-98-forme w-92-forme-md">
                                                <asp:Label id="lbldatumisteka" runat="server" style="font-weight:bold;font-size:13px;" Visible="True"> </asp:Label>
                                            </div> 
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
                         </div>                       
        <!-------------------------------------------------------------------------------------------------------------------------------------->
        <!-------------------------------------------------------------------------------------------------------------------------------------->
                        <div class="container-fluid" id="Container8" runat="server" style="margin-left: 10px">
                            <asp:UpdatePanel id="UpdatePanel16" runat="server">
                                <ContentTemplate>
                                    <fieldset>
                                        <div class="row top20">
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanadresaeposte1" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lbladresaeposte1" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                                </div>
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:TextBox ID="txtadresaeposte1" runat="server" class="txtbox3" style="font-size:13px;" maxlength="50" ontextchanged="txtadresaeposte1_TextChanged" OnClientClick="return CheckIfChannelHasChanged4();" AutoPostBack="true" onkeydown="keydownFunctionMail(); return true;" TabIndex="0"></asp:TextBox>
                                                <asp:Label ID="errLabelMail" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                                <asp:CustomValidator runat="server" id="cvadresaeposte1" controltovalidate="txtadresaeposte1" errormessage="" OnServerValidate="cvadresaeposte1_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                    </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel id="UpdatePanel10" runat="server">
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
                                                <asp:TextBox ID="txttelefon" runat="server" class="txtbox" style="font-size:13px;" maxlength="11" ontextchanged="txttelefon_TextChanged" OnClientClick="return CheckIfChannelHasChanged3();" AutoPostBack="true" onkeydown="keydownFunctionTelefon()" TabIndex="0"></asp:TextBox>
                                                <asp:Label id="PrimerTelefon" runat="server" style="font-weight:bold;font-size: 13px; margin-right: 10px;"> </asp:Label>
                                                <asp:Label ID="errLabelNumber" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                                                <asp:CustomValidator runat="server" id="cvtelefon" controltovalidate="txttelefon" errormessage="" OnServerValidate="cvtelefon_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                    </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel> 
                            <asp:UpdatePanel id="UpdatePanel11" runat="server">
                                <ContentTemplate>
                                    <cc1:AutoCompleteExtender 
                                        runat="server" 
                                        ID="autoCompleteMestoBoravka1" 
                                        ServicePath="WebServiceMesta.asmx"
                                        TargetControlID="txtmesto1"
                                        ServiceMethod="GetMesta"
                                        MinimumPrefixLength="2"
                                        CompletionSetCount="100"
                                        CompletionInterval="500"
                                        EnableCaching="false"
                                        OnClientItemSelected="getSelectedMesto1"
                                    >
                                    </cc1:AutoCompleteExtender>
                                    <cc1:AutoCompleteExtender 
                                        runat="server" 
                                        ID="autoCompleteUlicaBoravka1" 
                                        ServicePath="WebServiceMesta.asmx"
                                        TargetControlID="txtulica1"
                                        ServiceMethod="GetUlica"
                                        MinimumPrefixLength="2"
                                        CompletionSetCount="100"
                                        CompletionInterval="500"
                                        EnableCaching="false"
                                        UseContextKey="true"
                                        OnClientPopulating="getMestoAgain1"
                                    >
                                    </cc1:AutoCompleteExtender>
                                    <fieldset>
                                        <div class="row top10">
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanadresasert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lbladresasert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:DropDownList ID="ddlsertadresa" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox4" DataSourceID="odsSertAdresa" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlsertadresa_SelectedIndexChanged" TabIndex="0">
                                                </asp:DropDownList>
                                                <asp:ObjectDataSource ID="odsSertAdresa" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                                    <SelectParameters>
                                                        <asp:Parameter DefaultValue="zahtev-izdavanje-pravno-lice.aspx" Name="filename" Type="String" />
                                                        <asp:Parameter DefaultValue="ddlsertadresa" Name="controlid" Type="String" />
                                                    </SelectParameters>
                                                </asp:ObjectDataSource>
                                                <asp:CustomValidator runat="server" id="cvsertadresa" controltovalidate="ddlsertadresa" errormessage="" OnServerValidate="cvsertadresa_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                                <br><p class="notification"><asp:Label id="lblnotification9" runat="server" style="font-size:13px;"></asp:Label></p>
                                            </div>
                                        </div>
                                        <div class="row top15">                                 
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanmesto1" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lblmesto1" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                                                </div>
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:TextBox ID="txtmesto1" runat="server" readonly="true" class="txtboxgrey" style="font-size:13px;" maxlength="20"></asp:TextBox>
                                                <asp:CustomValidator runat="server" id="cvmesto1" controltovalidate="txtmesto1" errormessage="" OnServerValidate="cvmesto1_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                        <div class="row top10">                                 
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanulica1" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lblulica1" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                                </div>   
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:TextBox ID="txtulica1" runat="server" readonly="true" class="txtboxgrey5" style="font-size:13px;" maxlength="50"></asp:TextBox>
                                                <asp:CustomValidator runat="server" id="cvulica1" controltovalidate="txtulica1" errormessage="" OnServerValidate="cvulica1_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                    </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel id="UpdatePanel20" runat="server">
                                <ContentTemplate>
                                    <fieldset>
                                        <div class="row top10">                                 
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanbroj1" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lblbroj1" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                                </div> 
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:TextBox ID="txtbroj1" runat="server" readonly="true" class="txtboxgrey" style="font-size:13px;" maxlength="20" ontextchanged="txtbroj1_TextChanged" OnClientClick="return CheckIfChannelHasChanged10();" AutoPostBack="true"></asp:TextBox>
                                                <asp:Label ID="errLabelBroj1" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                                <asp:CustomValidator runat="server" id="cvbroj1" controltovalidate="txtbroj1" errormessage="" OnServerValidate="cvbroj1_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                    </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel id="UpdatePanel21" runat="server">
                                <ContentTemplate> 
                                    <fieldset>
                                        <div class="row top10">
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanpostanskibroj1" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lblpostanskibroj1" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:TextBox ID="txtpostanskibroj1" runat="server" readonly="true" class="txtboxgrey" style="font-size:13px;" maxlength="5"></asp:TextBox>
                                                <asp:CustomValidator runat="server" id="cvpostanskibroj1" controltovalidate="txtpostanskibroj1" errormessage="" OnServerValidate="cvpostanskibroj1_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                        <div class="row top10">
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanpak1" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                   <asp:Label id="lblpak1" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:TextBox ID="txtpak1" runat="server" readonly="true" class="txtboxgrey9" style="font-size:13px;" maxlength="6"></asp:TextBox>
                                                <asp:CustomValidator runat="server" id="cvpak1" controltovalidate="txtpak1" errormessage="" OnServerValidate="cvpak1_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                    </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel> 
                            <asp:UpdatePanel id="UpdatePanel12" runat="server">
                                <ContentTemplate>
                                    <fieldset>
                                        <div class="row top10">
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanrokkoriscenja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lblrokkoriscenjasert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                                </div> 
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:DropDownList ID="ddlrokkoriscenjasert" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox1" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlrokkoriscenjasert_SelectedIndexChanged" DataSourceID="odsRokKoriscenjaSert" TabIndex="0">
                                                </asp:DropDownList>
                                                <asp:ObjectDataSource ID="odsRokKoriscenjaSert" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                                    <SelectParameters>
                                                        <asp:Parameter DefaultValue="zahtev-izdavanje-pravno-lice.aspx" Name="filename" Type="String" />
                                                        <asp:Parameter DefaultValue="ddlrokkoriscenjasert" Name="controlid" Type="String" />
                                                    </SelectParameters>
                                                </asp:ObjectDataSource>
                                                <asp:CustomValidator runat="server" id="cvrokkoriscenjasert" controltovalidate="ddlrokkoriscenjasert" errormessage="" OnServerValidate="cvrokkoriscenjasert_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                    </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel id="UpdatePanel13" runat="server">
                                <ContentTemplate>
                                    <fieldset>
                                        <div class="row top10">
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spanmedijsert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lblmedijsert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:DropDownList ID="ddlmedijsert" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox1" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlmedijsert_SelectedIndexChanged" DataSourceID="odsMedijSert" TabIndex="0">
                                                </asp:DropDownList>
                                                <asp:ObjectDataSource ID="odsMedijSert" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                                    <SelectParameters>
                                                        <asp:Parameter DefaultValue="zahtev-izdavanje-pravno-lice.aspx" Name="filename" Type="String" />
                                                        <asp:Parameter DefaultValue="ddlmedijsert" Name="controlid" Type="String" />
                                                    </SelectParameters>
                                                </asp:ObjectDataSource>
                                                <asp:CustomValidator runat="server" id="cvmedijsert" controltovalidate="ddlmedijsert" errormessage="" OnServerValidate="cvmedijsert_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                                            </div>
                                        </div>
                                    </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel id="UpdatePanel14" runat="server">
                                <ContentTemplate>
                                    <fieldset>
                                        <div class="row top10">                                 
                                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                                <div class="w-2-forme w-8-forme-md">
                                                    <asp:Label id="spancenasaporezom1" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                                </div>
                                                <div class="w-98-forme w-92-forme-md">
                                                    <asp:Label id="lblcenasaporezom1" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                                </div>    
                                            </div>
                                            <div class="col-sm-10" style="background-color:white;">
                                                <asp:TextBox ID="txtcenasaporezom1" runat="server" class="txtbox1" style="border:solid grey 1px; padding: 3px; text-align: right; height: 26px; width: !important;" TabIndex="-1"></asp:TextBox>
                                                <asp:Label id="lbldinara1" runat="server" style="font-weight:bold;font-size:13px;"><span style="COLOR: white; font-size:17px;">&nbsp;</span> </asp:Label> 
                                            </div>
                                        </div>
                                   </fieldset>
                               </ContentTemplate>
                            </asp:UpdatePanel>
                       </div>
                    <!-------------------------------------------------------------------------------------------------------------------------------------->
                    <!-------------------------------------------------------------------------------------------------------------------------------------->    
                        <div class="row" id="Container9" runat="server">
                            <br />
                        </div>
                        <div class="row" id="Container10" runat="server">
                            <br />
                        </div>
                        <div class="container-fluid" id="Div1" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
                            <div class="row top20">
                                <div class="col-sm-2">
                                    <asp:Label id="lbldugme5" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label>  
                                </div>
                                <div class="col-sm-8">
                                    <asp:Button ID="btnSubmit2" runat="server" class="btn btn-primary save" Text="" onclick="btnSubmit2_Click2" onclientclick="unhook()"/>
                                    <asp:Button ID="btnQuit" runat="server" class="btn btn-warning arrowLeft" Text="" onclick="btnQuit_Click2" onclientclick="unhook()"/>                            
                                </div>
                                <div class="col-sm-2">
                                    <asp:Label id="lbldugme6" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label>  
                                </div>
                            </div> 
                        </div>
                        <div class="container-fluid" id="Div2" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
                            <div class="row top20">
                                <div class="col-sm-2">
                                    <asp:Label id="lbldugme7" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label>
                                </div>
                                <div class="col-sm-8">
                                    <asp:Button ID="btnBack" runat="server" class="btn btn-warning arrowLeft" Text="" onclick="btnQuit_Click3" onclientclick="unhook()" Width="150px"/>                            
                                </div>
                                <div class="col-sm-2">
                                    <asp:Label id="lbldugme8" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label> 
                                </div>
                            </div> 
                        </div>
                        <div class="container-fluid" id="Div3" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
                            <div class="row top20">
                                <div class="col-sm-2">
                                    <asp:Label id="lbldugme9" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label>  
                                </div>
                                <div class="col-sm-8">
                                    <asp:Button ID="btnEdit1" runat="server" class="btn btn-primary edit1" Text="" onclick="btnEdit1_Click2" onclientclick="unhook()"/>
                                    <asp:Button ID="btnBack1" runat="server" class="btn btn-warning arrowLeft" Text="" onclick="btnBack1_Click2" onclientclick="unhook()" Width="150px"/>                            
                                </div>
                                <div class="col-sm-2">
                                    <asp:Label id="lbldugme10" runat="server" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </asp:Label>  
                                </div>
                            </div> 
                        </div>
                        <div class="container-fluid" id="Container11" runat="server" style="margin-left: -10px; background-color:#f5f5f5;">                          
                            <div class="row">
                                <br />
                            </div>
                        </div>  
                        <div class="container-fluid" id="Container12" runat="server">
                            <h2></h2>
                            <br />
                            <br />
                        </div>
              <div id="throbber" style="display:none;">
                <p style="font-size:20px; font-weight: bold;"><b>Molimo sačekajte...</b></p>
                <asp:Image ID="imgThrobber" imageurl="~/Content/Gif/throbber.gif" runat="server" style="width:35px;height:35px;"/>
            </div>
            <!--</iframe>-->
        <!--</div>-->
    </form>
</body>
</html>
