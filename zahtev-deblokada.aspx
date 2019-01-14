<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="zahtev-deblokada.aspx.cs" Inherits="zahtev_deblokada" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Zahtev za deblokadu smart kartice/USB tokena</title>
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
        function Notification() {
            swal({
                title: 'Obaveštenje.',
                text: 'Za navedeni JIK nema statusa.',
                type: 'OK'
            });
        }
        function keydownFunctionTelefon()
           {
               var telefonpost = document.getElementById('<%=txttelefon.ClientID %>');

            if (telefonpost.value.length == 0 || telefonpost.value.length == 1)
                   document.getElementById('errLabelNumber').style.display = 'none';
        }
        function keydownFunctionAddress()
           {
               var addressepost = document.getElementById('<%=txtadresaeposte.ClientID %>');

               if (addressepost.value.length == 0 || addressepost.value.length == 1)
                   document.getElementById('errLabel').style.display = 'none';
        }
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
                <asp:HyperLink id="pdfhyperlink1" runat="server" NavigateUrl="~/dokumentacija/Uputstvo-zahtev-deblokada.pdf" target="_blank" style="vertical-align:bottom" tabindex="-1"><asp:Label id="lblkorisniskouputstvo" runat="server" style="font-size:15px;"></asp:Label></asp:HyperLink>
        </div>
        <!-------------------------------------------OVO SE NE VIDI------------------------------------------------>
        <div class="container-fluid" id="Container1" runat="server" style="margin-left: 10px">
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
        <div class="row">
                <br />
        </div>
        <div class="container-fluid" id="Div1" runat="server" style="margin-left: 10px">
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
                    <asp:TextBox ID="txtime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" TabIndex="1"></asp:TextBox>
                    <asp:Label id="ImePrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"> </asp:Label>
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
                    <asp:TextBox ID="txtprezime" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" TabIndex="2"></asp:TextBox>
                    <asp:Label id="PrezimePrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                    <asp:CustomValidator runat="server" id="cvprezime" controltovalidate="txtprezime" errormessage="" OnServerValidate="cvprezime_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>
            <!--AJAX ToolkitScriptManager-->
            <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
            <asp:UpdatePanel id="UpdatePanel1" runat="server">
              <ContentTemplate>
                 <fieldset>
                    <div class="row top10">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanjmbgibrojpasosa" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lbljmbgibrojpasosa" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtjmbgibrojpasosa" runat="server" class="txtbox" style="font-size:13px;" maxlength="30" TabIndex="3"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvjmbgibrojpasosa" controltovalidate="txtjmbgibrojpasosa" errormessage="" OnServerValidate="cvjmbgibrojpasosa_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            <br><p class="notification"><asp:Label id="lblnotification1" runat="server" style="font-size:13px;"></asp:Label></p>
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
                                    <asp:Label id="spantelefon" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lbltelefon" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txttelefon" runat="server" class="txtbox" style="font-size:13px;" maxlength="11" ontextchanged="txttelefon_TextChanged" OnClientClick="return CheckIfChannelHasChanged1();" AutoPostBack="true" onkeydown="keydownFunctionTelefon(); return true;" TabIndex="4"></asp:TextBox>
                                <asp:Label id="TelefonPrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                                <asp:Label ID="errLabelNumber" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvtelefon" controltovalidate="txttelefon" errormessage="" OnServerValidate="cvtelefon_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
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
                                    <asp:Label id="spanadresaeposte" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lbladresaeposte" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:TextBox ID="txtadresaeposte" runat="server" class="txtbox3" style="font-size:13px;" maxlength="50" ontextchanged="txtadresaeposte_TextChanged" OnClientClick="return CheckIfChannelHasChanged();" AutoPostBack="true" onkeydown="keydownFunctionAddress(); return true;" TabIndex="5"></asp:TextBox>
                                <asp:Label ID="errLabel" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                                <asp:CustomValidator runat="server" id="cvadresaeposte" controltovalidate="txtadresaeposte" errormessage="" OnServerValidate="cvadresaeposte_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
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
                                <asp:Label id="spannacinplacanja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblnacinplacanja" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:DropDownList ID="ddlnacinplacanja" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox1" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlnacinplacanja_SelectedIndexChanged" DataSourceID="odsNacinPlacanja" TabIndex="6">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsNacinPlacanja" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="zahtev-deblokada.aspx" Name="filename" Type="String" />
                                    <asp:Parameter DefaultValue="ddlnacinplacanja" Name="controlid" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:CustomValidator runat="server" id="cvnacinplacanja" controltovalidate="ddlnacinplacanja" errormessage="" OnServerValidate="cvnacinplacanja_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            <br />
                        </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">
                <div class="col-sm-2" style="background-color:white;">
                </div>
                <div class="col-sm-10">
                            
                </div>
            </div>
        </div>        
        <asp:UpdatePanel id="UpdatePanel5" runat="server">
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
                <div class="container-fluid" id="Container00" runat="server" style="margin-left: 10px">
                    <div class="row top10">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanjik" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lbljik" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label> 
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtjik" runat="server" class="txtbox1" style="font-size:13px;" maxlength="9" ontextchanged="txtjik_TextChanged" OnClientClick="return CheckIfChannelHasChanged2()" AutoPostBack="true" TabIndex="7" ></asp:TextBox>
                            <asp:Label ID="errLabelJik" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                            <asp:CustomValidator runat="server" id="cvjik" controltovalidate="txtjik" errormessage="" OnServerValidate="cvjik_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                            </br><p class="notification"><asp:Label id="lblnotification" runat="server" style="font-size:13px;"></asp:Label></p>
                        </div>
                    </div>
                    <!--////////////////////////////////////////////////////////////////-->
                    <div class="row top10">
                        <div class="col-sm-2 d-flex-forme">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spannacindeblokade" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblnacindeblokade" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>  
                            </div>
                        </div>
                        <div class="col-sm-10">
                            <asp:RadioButton ID="rbIsAttached" runat="server" Text="" OnCheckedChanged="rbIsAttached_CheckedChanged" onclick="unhook()" AutoPostBack="True" GroupName="btnGrp" TabIndex="8"/>
                        </div>                            
                    </div>
                    <div class="row top10">
                        <div class="col-sm-2">
                        </div>
                        <div class="col-sm-10">
                            <asp:RadioButton ID="rbIsNotAttached" runat="server" Text="" OnCheckedChanged="rbIsNotAttached_CheckedChanged" onclick="unhook()" AutoPostBack="True" GroupName="btnGrp" TabIndex="9"/>
                            &nbsp;&nbsp;
                            <asp:CustomValidator runat="server" id="cvnacindeblokade" errormessage="" OnServerValidate="cvnacindeblokade_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div> 
                      </div>          
                </div>
                <div class="container-fluid" id="Container2" runat="server" style="margin-left: 10px">
                    <div class="row top15">
                        <br/><p class="notification" style="margin-left: 20px"><asp:Label id="lblnotification2" runat="server" style="font-size:13px;"></asp:Label> </p>
                    </div>
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
                            <asp:DropDownList ID="ddlsertadresa" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox4" DataSourceID="odsSertAdresa" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlsertadresa_SelectedIndexChanged" TabIndex="10">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsSertAdresa" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="zahtev-deblokada.aspx" Name="filename" Type="String" />
                                    <asp:Parameter DefaultValue="ddlsertadresa" Name="controlid" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:CustomValidator runat="server" id="cvsertadresa" controltovalidate="ddlsertadresa" errormessage="" OnServerValidate="cvsertadresa_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                    <div class="row top15">                                 
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                <asp:Label id="spanmesto" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                                <asp:Label id="lblmesto" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtmesto" runat="server" class="txtbox" style="font-size:13px;" maxlength="30"></asp:TextBox>
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
                            <asp:TextBox ID="txtulica" runat="server" class="txtbox3" style="font-size:13px;" maxlength="50"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvulica" controltovalidate="txtulica" errormessage="" OnServerValidate="cvulica_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
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
                            <asp:TextBox ID="txtbroj" runat="server" class="txtbox" style="font-size:13px;" maxlength="20" ontextchanged="txtbroj_TextChanged" OnClientClick="return CheckIfChannelHasChanged9();" AutoPostBack="true"></asp:TextBox>
                            <asp:Label ID="errLabelBroj" runat="server" style="font-size:13px;" ForeColor="Red"></asp:Label>
                            <asp:CustomValidator runat="server" id="cvbroj" controltovalidate="txtbroj" errormessage="" OnServerValidate="cvbroj_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>                                 
                        </div>
                    </div>
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
                            <asp:TextBox ID="txtpostanskibroj" runat="server" class="txtbox" style="font-size:13px;" maxlength="5"></asp:TextBox>
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
                            <asp:TextBox ID="txtpak" runat="server" class="txtbox" style="font-size:13px;" maxlength="6"></asp:TextBox>
                            <asp:CustomValidator runat="server" id="cvpak" controltovalidate="txtpak" errormessage="" OnServerValidate="cvpak_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
                    </div>
                </div>
                <div class="container-fluid" id="Container3" runat="server" style="margin-left: 10px">
                    <div class="row top15">
                        <br/><p class="notification" style="margin-left: 20px"><asp:Label id="lblnotification3" runat="server" style="font-size:13px;"></asp:Label> </p>
                    </div>
                    <div class="row top10">
                        <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                            <div class="w-2-forme w-8-forme-md">
                                 <asp:Label id="spankarticatoken" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                            </div>
                            <div class="w-98-forme w-92-forme-md">
                               <asp:Label id="lblkarticatoken" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                            </div>
                        </div>
                        <div class="col-sm-10" style="background-color:white;">
                            <asp:TextBox ID="txtkarticatoken" runat="server" class="txtbox3" style="font-size:13px;"></asp:TextBox>
                            <asp:Label id="KarticatokenPrimer" runat="server" style="font-weight:bold; font-size: 13px; margin-right: 10px;"></asp:Label>
                            <asp:CustomValidator runat="server" id="cvkarticatoken" controltovalidate="txtkarticatoken" errormessage="" OnServerValidate="cvkarticatoken_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                        </div>
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
        <div class="container-fluid" style="margin-left: 10px">
          <asp:UpdatePanel id="UpdatePanel7" runat="server">
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
                            <asp:TextBox ID="txtcenasaporezom" runat="server" class="txtbox1" style="border:solid grey 1px; padding: 3px; text-align: right; height: 26px; width: !important;" TabIndex="-1"></asp:TextBox>
                            <asp:Label id="lbldinara" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
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
        <!---------------------------------------------------------------------------------------------------------->
        <div class="container-fluid" id="myDiv5" runat="server" style="margin-left: 10px; background-color:#f5f5f5;">
            <div class="row top20">
                <div class="col-sm-2">
                    <label for="lbldugme" style="font-size:13px;"><span style="COLOR: white; font-size:17px;"></span> </label> 
                </div>
                <div class="col-sm-8">
                    <asp:Button ID="btnSubmit" runat="server" class="btn btn-primary save" Text="" onclick="btnSubmit_Click1" onclientclick="unhook()"/>                           
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
