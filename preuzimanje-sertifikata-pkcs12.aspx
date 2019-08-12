<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="preuzimanje-sertifikata-pkcs12.aspx.cs" Inherits="preuzimanje_sertifikata_pkcs12" %>
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Preuzimanje sertifikata kao PKCS#12 datoteke</title>
    <!--#include virtual="~/Content/elements/head.inc"-->
    <script type="text/javascript">
        $(document).ready(function () {
            $('#btnSubmit').click(function () {
                $.blockUI({
                    message: $('#throbber'),
                    css: {
                        border: 'none',
                        padding: '15px',
                        backgroundColor: '#000',
                        '-webkit-border-radius': '10px',
                        '-moz-border-radius': '10px',
                        opacity: .5,
                        color: '#fff'
                    }
                });
            });
        });
        function countupTimeStart() {
            var time = '<%= Session["Preuzimanje-softverskog-sertifikata-time"] %>';
            var secondsLabel = document.getElementById("seconds");
            var totalSeconds = 0;
            setInterval(setTime, 1000);

            function setTime() {
                ++totalSeconds;
                if (totalSeconds == time) {
                    '<%Session["Preuzimanje-softverskog-sertifikata-expiredtime"] = "true"; %>';
                    window.location = '<%= Session["Preuzimanje-softverskog-sertifikata-P12ErrorPage"] %>';                   
                    clearInterval(setInterval);                                      
                }    
                secondsLabel.innerHTML = pad(totalSeconds);                          
            }

            function pad(val) {
                  var valString = val + "";
                  if (valString.length < 2) {
                    return "0" + valString;
                  } else {
                    return valString;
                  }
                }
        }
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
                title:'Uspešno kreiran sertifikat.',
                text: 'Pritisnite odgovarajuće dugme za preuzimanje sertifikata.',
                type: 'OK'
            });
        }
        function erroralertTime() {
            swal({
                title: 'Greška prilikom kreiranja sertifikata.',
                text: 'Isteklo je vreme za kreiranje.',
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
                title: 'Greška prilikom preuzimanja sertifikata.',
                text: 'Isteklo je vreme za kreiranje sertifikata ili nije uspelo preuzimanje. Pokušajte ponovo kasnije.',
                type: 'OK'
            });
        }
        function errorDownloadingPKCS12() {
            swal({
                title: 'Greška.',
                text: 'Nemate ovlašćenja za snimanje PKCS#12 datoteke sertifikata u izabranom direktorijumu. Izaberite drugi direktorijum.',
                type: 'OK'
            });
        }
        function errorGettingRequestNumberPKCS12() {
            swal({
                title: 'Greška.',
                text: 'Nemoguće je kreirati PKCS#12 datoteku sertifikata, zato što je broj zahteva neispravan.',
                type: 'OK'
            });
        }
        function errorGettingAuthorizationCodePKCS12() {
            swal({
                title: 'Greška.',
                text: 'Nemoguće je kreirati PKCS#12 datoteku sertifikata, zato što je autorizacioni kod neispravan.',
                type: 'OK'
            });
        }
        function errorTransferServiceFailedPKCS12() {
            swal({
                title: 'Greška.',
                text: 'Nemoguće je kreirati PKCS#12 datoteku sertifikata, zato što postoji problem u komunikaciji sa serverom za izdavanje sertifikata. Pokušajte ponovo kasnije.',
                type: 'OK'
            });
        }
        function errorRrequestIsNotInRequiredStatePKCS12() {
            swal({
                title: 'Greška.',
                text: 'Nemoguće je kreirati PKCS#12 datoteku sertifikata, zato što je sertifikat već preuzet.',
                type: 'OK'
            });
        }
        function DisableButton() {
            document.getElementById('btnDownloadPKCS12Certificate').disabled = true;
        }
        function EnableButton() {
            document.getElementById('btnDownloadPKCS12Certificate').disabled = false;
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
                <asp:HyperLink id="pdfhyperlink1" runat="server" NavigateUrl="~/dokumentacija/Uputstvo-preuzimanje-sertifikata-pkcs12.pdf" target="_blank" style="vertical-align:bottom" TabIndex="-1"><asp:Label id="lblkorisniskouputstvo" runat="server" style="font-size:15px;"></asp:Label></asp:HyperLink>
        </div>
        <div class="row">
                <br />
        </div>
        <!---------------------------------------------------------------------------------------------------------->
        <!--AJAX ToolkitScriptManager-->
        <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">
                <div class="col-sm-2 d-flex-forme">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spanbrojzahteva" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblbrojzahteva" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>  
                </div>
                <div class="col-sm-10">
                    <asp:TextBox ID="txtbrojzahteva" runat="server" class="txtbox" style="font-size:13px;" maxlength="8" TabIndex="1"></asp:TextBox>
                    <asp:CustomValidator runat="server" id="cvbrojzahteva" controltovalidate="txtbrojzahteva" errormessage="" OnServerValidate="cvbrojzahteva_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <div class="row top10">                                 
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spankodovizapreuzimanje" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lblkodovizapreuzimanje" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-10" style="background-color:white;">
                    <asp:TextBox ID="txtkodovipreuzimanje" runat="server" class="txtbox5" style="font-size:13px;" maxlength="14" TabIndex="0"></asp:TextBox>
                    <asp:Label ID="errLabel" runat="server" ForeColor="Red" style="font-size:13px;"></asp:Label>
                    <asp:CustomValidator runat="server" id="cvkodovipreuzimanje" controltovalidate="txtkodovipreuzimanje" errormessage="" OnServerValidate="cvkodovipreuzimanje_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                </div>
            </div>
        </div>
        <div class="container-fluid" style="margin-left: 10px">
            <asp:UpdatePanel id="UpdatePanel9" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <div class="row top10">
                            <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                                <div class="w-2-forme w-8-forme-md">
                                    <asp:Label id="spannacinslanja" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                                </div>
                                <div class="w-98-forme w-92-forme-md">
                                    <asp:Label id="lblnacinslanja" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                                </div>
                            </div>
                            <div class="col-sm-10" style="background-color:white;">
                                <asp:DropDownList ID="ddlnacinslanja" runat="server" AppendDataBoundItems="True" AutoPostBack="True" class="txtbox4" DataSourceID="odsNacinSlanja" DataTextField="ItemText" DataValueField="IDItem" OnSelectedIndexChanged="ddlnacinslanja_SelectedIndexChanged" TabIndex="0">
                                </asp:DropDownList>
                                <asp:ObjectDataSource ID="odsNacinSlanja" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetItemByPageAndControl" TypeName="RABackTableAdapters.ItemTableAdapter">
                                    <SelectParameters>
                                        <asp:Parameter DefaultValue="preuzimanje-sertifikata-pkcs12.aspx" Name="filename" Type="String" />
                                        <asp:Parameter DefaultValue="ddlnacinslanja" Name="controlid" Type="String" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                <asp:CustomValidator runat="server" id="cvnacinslanja" controltovalidate="ddlnacinslanja" errormessage="" OnServerValidate="cvnacinslanja_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
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
                    <asp:Button ID="btnSubmit" runat="server" class="btn-lg btn-primary buttonborder" Text="" onclick="btnSubmit_Click1" OnClientClick="countupTimeStart();unhook()" TabIndex="0"/>                           
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
        <div class="container-fluid" style="margin-left: 10px;">
            <div class="row top10">
                <div class="col-sm-2 d-flex-forme" style="background-color:white;">
                    <div class="w-2-forme w-8-forme-md">
                        <asp:Label id="spandatotekasert" runat="server" style="COLOR: red; font-weight:bold; font-size:17px;"></asp:Label>&nbsp;
                    </div>
                    <div class="w-98-forme w-92-forme-md">
                        <asp:Label id="lbldatotekasert" runat="server" style="font-weight:bold;font-size:13px;"> </asp:Label>
                    </div>
                </div>
                <div class="col-sm-10">
                    <asp:TextBox ID="txtdatotekasert" runat="server" ReadOnly="true" class="txtbox4" style="font-size:13px; background-color: #e2e2e2; margin-bottom: 5px;" maxlength="50" TabIndex="-1"></asp:TextBox>
                    <asp:CustomValidator runat="server" id="cvdatotekasert" controltovalidate="txtdatotekasert" errormessage="" OnServerValidate="cvdatotekasert_ServerValidate" Display="Dynamic" ForeColor="Red" style="font-size:13px;" ValidateEmptyText="true"/>
                    <asp:Button ID="btnDownloadPKCS12Certificate" runat="server" class="btn btn-warning btn-sm submit" Text="" style="border: 4px solid #FFA500;" OnClick="btnDownloadPKCS12Certificate_Click" OnClientClick="unhook()" TabIndex="0"/>
                </div>
            </div>
        </div>
        <div class="row">
            <br />
        </div>
        <div class="col-sm-10" style="background-color:white;">                                      
            <p class="notification" style="margin-bottom: 3px;"><asp:Label id="lblnotification" runat="server" style="font-size:13px;"></asp:Label></p>
        </div>
        <br />
            <div id="throbber" style="display:none;">
                <p style="font-size:20px; font-weight: bold;"><b>Kreiranje sertifikata je u toku. Molimo sačekajte. Proteklo vreme je:                                 
                        <span id="seconds" style="font-size:20px;margin-right:2px;font-weight:bold;">00</span>
                    sekunde.</b></p>
                <asp:Image ID="imgThrobber" imageurl="~/Content/Gif/throbber.gif" runat="server" style="width:35px;height:35px;"/>                               
            </div>
        <br />
        <div class="row">
            <br />
            <br />
        </div>
    </form>
</body>
</html>
