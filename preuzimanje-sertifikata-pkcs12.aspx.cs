using BlueXSOAP;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class preuzimanje_sertifikata_pkcs12 : System.Web.UI.Page
{
    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
    public List<WebControl> Controls;
    public List<ItemVariable> Items;

    protected void Page_Load(object sender, EventArgs e)
    {
        Utility utility = new Utility();
        bool ConnectionActive = utility.IsAvailableConnection();
        if (!ConnectionActive)
        {
            Response.Redirect("GreskaBaza.aspx"); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
        }

        string pageName = Path.GetFileName(Page.AppRelativeVirtualPath);
        bool PageActive = Utils.IsActivePage(pageName);
        if (!PageActive)
        {
            Response.Redirect("Obavestenje.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
        }

        string encryptedParameters = Request.QueryString["d"];

        if ((encryptedParameters != string.Empty) && (encryptedParameters != null))
        {
            AvoidCashing();

            if (!Page.IsPostBack)
            {
                Session["Preuzimanje-softverskog-sertifikata-expiredtime"] = false;
                string page = Path.GetFileName(Page.AppRelativeVirtualPath);
                string Secоnds = utility.getpkcs12timeout(Constants.PKCS12TIMEOUT);
                int TimerSeconds = Convert.ToInt32(Secоnds);
                Session["Preuzimanje-softverskog-sertifikata-time"] = TimerSeconds;
                string P12ErrorPage = System.Configuration.ConfigurationManager.AppSettings["P12ErrorPage"].ToString();
                Session["Preuzimanje-softverskog-sertifikata-P12ErrorPage"] = P12ErrorPage;

                string IDItem1 = string.Empty;
                SetUpDefaultItem(ddlnacinslanja.ClientID, out IDItem1);
                ddlnacinslanja.SelectedValue = IDItem1;

                txtbrojzahteva.Text = string.Empty;
                txtkodovipreuzimanje.Text = string.Empty;
                txtdatotekasert.Text = string.Empty;
                errLabel.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                //-------TABINDEX---------------
                Session["Preuzimanje-softverskog-sertifikata-pkcs12-event_controle"] = txtbrojzahteva;
                SetFocusOnTextbox();
                //------------------------------
                //Get Control on all page
                SetUpValidation();
                log.Info("successfully set Validation!");
                SetUpIsRequiredTextBoxes();
                log.Info("successfully set RequiredTextBoxes!");
                SetUpIsRequiredDropDownLists();
                log.Info("successfully set RequiredDropDownLists...Application Starting, successfully get all controls!");
            }
        }
        else
        {
            bool PageAgreemeActive = Utils.IsActiveAgreemePage(pageName);
            if (PageAgreemeActive)
            {
                string page = @"returnUrl=" + pageName;
                string page1 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt(page, Constants.CryptKey, Constants.AuthKey);
                page1 = page1.Replace("+", "%252b");
                Response.Redirect(string.Format("~/Uputstvo.aspx?d={0}", page1));
            }
            else
            {
                string Checked = @"checked=1";
                string checkedParameters = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt(Checked, Constants.CryptKey, Constants.AuthKey);
                checkedParameters = checkedParameters.Replace("+", "%252b");
                Response.Redirect(string.Format("~/" + pageName + "?d={0}", checkedParameters), false);
            }
        }
    }

    private void AvoidCashing()
    {
        Response.Cache.SetNoStore();
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    //-----------------SetUpAllFields-------------------------------
    //---------------------------------------------------------------
    public List<WebControlLanguage> WebControls;

    protected void SetUpAllFields()
    {
        WebControls = new List<WebControlLanguage>();

        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Utility utility = new Utility();
        WebControls = utility.pronadjiSvaPoljaNaStranici(page);

        foreach (var control in WebControls)
        {
            if (Constants.CONTROL_ТYPE_LABEL.ToLower() == control.ControlType.ToLower())
            {
                try
                {
                    Label labela = (Label)FindControlRecursive(Page, control.Controlid);
                    labela.Text = control.ControlTittle;
                    labela.Visible = control.IsVisible;
                }
                catch (Exception ex)
                {
                    log.Info("Error while setting control's " + control.Controlid + " text: " + ex.Message);
                }
            }

            if (Constants.CONTROL_ТYPE_BUTTON.ToLower() == control.ControlType.ToLower())
            {
                try
                {
                    Button dugme = (Button)FindControlRecursive(Page, control.Controlid);
                    dugme.Text = control.ControlTittle;

                    dugme.Enabled = control.IsEnabled;
                    dugme.Visible = control.IsVisible;
                }
                catch (Exception ex)
                {
                    log.Info("Error while setting control's " + control.Controlid + " text: " + ex.Message);
                }
            }

            try
            {
                if (control.ControlTittle.Equals("*"))
                {
                    if (!control.IsVisible)
                    {
                        Label labela = (Label)FindControlRecursive(Page, control.Controlid);
                        labela.Text = "&nbsp;";
                        labela.Visible = true;
                    }
                }
                else
                {
                    FindControlRecursive(Page, control.Controlid).Visible = control.IsVisible;
                }
            }
            catch (Exception ex)
            {
                log.Info("Error while setting control's " + control.Controlid + " visibility: " + ex.Message);
            }

            if (Constants.CONTROL_TYPE_TEXTBOX.ToLower() == control.ControlType.ToLower())
            {
                try
                {
                    TextBox tekstualnopolje = (TextBox)FindControlRecursive(Page, control.Controlid);
                    //tekstualnopolje.Text = control.ControlTittle;

                    tekstualnopolje.Enabled = control.IsEnabled;
                    tekstualnopolje.Visible = control.IsVisible;
                }
                catch (Exception ex)
                {
                    log.Info("Error while setting control's " + control.Controlid + " text: " + ex.Message);
                }
            }

            if (Constants.CONTROL_TYPE_DROPDOWNLIST.ToLower() == control.ControlType.ToLower())
            {
                try
                {
                    DropDownList padajucalista = (DropDownList)FindControlRecursive(Page, control.Controlid);
                    //tekstualnopolje.Text = control.ControlTittle;
                    padajucalista.Enabled = control.IsEnabled;
                    padajucalista.Visible = control.IsVisible;

                }
                catch (Exception ex)
                {
                    log.Info("Error while setting control's " + control.Controlid + " text: " + ex.Message);
                }
            }

        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetUpAllFields();
        log.Info("Successfully set all Fields on page!");
    }

    public static Control FindControlRecursive(Control Root, string Id)
    {
        if (Root.ID == Id)
            return Root;

        foreach (Control Ctl in Root.Controls)
        {
            Control FoundCtl = FindControlRecursive(Ctl, Id);
            if (FoundCtl != null)
                return FoundCtl;
        }

        return null;
    }

    //---------------------------------------------------------------
    //---------------------------------------------------------------
    protected void SetUpValidation()
    {
        Utility utility = new Utility();
        string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_VALIDATION);

        if (SettingValue == Constants.SETTING_VALUE_TRUE)
        {
            string page = Path.GetFileName(Page.AppRelativeVirtualPath);
            Utility utility1 = new Utility();
            Controls = new List<WebControl>();
            Controls = utility1.pronadjiKontrole(page);

            foreach (var control in Controls)
            {
                if (control.Id == txtbrojzahteva.ClientID)
                {
                    Session["Preuzimanje-softverskog-sertifikata-pkcs12-TurnOnRequestNumberValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            Session["Preuzimanje-softverskog-sertifikata-pkcs12-TurnOnRequestNumberValidation"] = Constants.VALIDATION_FALSE;
        }
    }



    protected void SetUpIsRequiredTextBoxes()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrolePoTipu(page, Constants.CONTROL_TYPE_TEXTBOX);

        foreach (var control in Controls)
        {
            if (control.Id == txtbrojzahteva.ClientID)
            {
                Session["Preuzimanje-softverskog-sertifikata-pkcs12-txtbrojzahtevaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtkodovipreuzimanje.ClientID)
            {
                Session["Preuzimanje-softverskog-sertifikata-pkcs12-txtkodovipreuzimanjeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatotekasert.ClientID)
            {
                Session["Preuzimanje-softverskog-sertifikata-pkcs12-txtdatotekasertIsRequired"] = control.IsRequired;
            }
        }
    }

 
    protected void SetUpIsRequiredDropDownLists()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrolePoTipu(page, Constants.CONTROL_TYPE_DROPDOWNLIST);

        foreach (var control in Controls)
        {
            if (control.Id == ddlnacinslanja.ClientID)
            {
                Session["Preuzimanje-softverskog-sertifikata-pkcs12-ddlnacinslanjaIsRequired"] = control.IsRequired;
            }
        }
    }

    //---------------------------------------------------------------
    //---------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    protected void cvkodovipreuzimanje_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage = string.Empty;
            args.IsValid = UtilsValidation.ValidateKodoviZaPreuzimanje(txtkodovipreuzimanje.Text, Convert.ToBoolean(Session["Preuzimanje-softverskog-sertifikata-pkcs12-txtkodovipreuzimanjeIsRequired"]), out ErrorMessage);
            cvkodovipreuzimanje.ErrorMessage = ErrorMessage;
        }
        catch (Exception)
        {
            cvkodovipreuzimanje.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvbrojzahteva_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage = string.Empty;
            args.IsValid = UtilsValidation.ValidateBrojZahteva(txtbrojzahteva.Text, Convert.ToBoolean(Session["Preuzimanje-softverskog-sertifikata-pkcs12-txtbrojzahtevaIsRequired"]), Convert.ToBoolean(Session["Preuzimanje-softverskog-sertifikata-pkcs12-TurnOnRequestNumberValidation"]), out ErrorMessage);
            cvbrojzahteva.ErrorMessage = ErrorMessage;
        }
        catch (Exception)
        {
            cvbrojzahteva.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdatotekasert_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtdatotekasert.Text, Convert.ToBoolean(Session["Preuzimanje-softverskog-sertifikata-pkcs12-txtdatotekasertIsRequired"]), out ErrorMessage1);
            cvdatotekasert.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvdatotekasert.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    //-----------------SetUpDefaultItem-------------------------------
    //---------------------------------------------------------------
    protected string SetUpDefaultItem(string controlid, out string IDItem)
    {
        IDItem = string.Empty;
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Utility utility = new Utility();
        Items = utility.getIdItemDefault(page, controlid);

        foreach (var itemdefault in Items)
        {
            if (itemdefault.IsDefault == true)
            {
                IDItem = (itemdefault.IDItem).ToString();
            }
            else
            {

            }
        }

        return IDItem;
    }

    //---------------------------------------------------------------
    //---------------------------------------------------------------  

    protected void cvnacinslanja_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ddlnacinslanjaString = ddlnacinslanja.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlnacinslanjaString, out IDItem);

            string ErrorMessage = string.Empty;
            args.IsValid = UtilsValidation.ValidateNacinSlanja(ddlnacinslanja.SelectedValue, Convert.ToBoolean(Session["Preuzimanje-softverskog-sertifikata-pkcs12-ddlnacinslanjaIsRequired"]), IDItem, out ErrorMessage);
            cvnacinslanja.ErrorMessage = ErrorMessage;
        }
        catch (Exception)
        {
            cvnacinslanja.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void ddlnacinslanja_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["Preuzimanje-softverskog-sertifikata-pkcs12-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected BxSoapEnvelope createSoapEnvelope(Utility utility)
    {
        BxSoapEnvelope envelope = new BxSoapEnvelopePKCS12();

        envelope.BxData.setValue(@"requestNumber", txtbrojzahteva.Text);
        envelope.BxData.setValue(@"downloadAuthorizationCode", txtkodovipreuzimanje.Text);
        envelope.BxData.setValue(@"sendingMethod", utility.getEnglishText(Convert.ToInt32(ddlnacinslanja.SelectedValue)));
        return envelope;
    }

    public string USI = string.Empty;
    public string pkcs12 = string.Empty;
    public string errorPKCS12 = string.Empty;

    protected void btnSubmit_Click1(object sender, EventArgs e)
    {
        Session["Preuzimanje-softverskog-sertifikata-parseErrorPKCS12"] = string.Empty;
        Utility utility = new Utility();

        if (Page.IsValid)
        {
            try
            {
                log.Info("Start sending SOAP message.");

                BxSoapEnvelope envelope = createSoapEnvelope(utility);

                //envelope.createBxSoapEnvelope();   //create SOAP.xml 
                string SOAPresponse = BxSoap.SOAPManual(envelope.createBxSoapEnvelope());
                //string SOAPresponse = "<?xml version='1.0' encoding='UTF-8'?><env:Envelope xmlns:env='http://schemas.xmlsoap.org/soap/envelope/' xmlns:bx='http://namespaces.bluex.com/bluex/bluexml'><env:Header/><env:Body><bx:BlueXMLRequestMessage xmlns:bx='http://namespaces.bluex.com/bluex/bluexml'><bx:data><error>[10DB032A] Error getting REQID/UID [Component_ElementaryAction_Workflow_SetUID::DoAction()]</error><error>Authorization Code does not match</error></bx:data></bx:BlueXMLRequestMessage></env:Body></env:Envelope>";
                /*
                if (Convert.ToBoolean(Session["Preuzimanje-softverskog-sertifikata-expiredtime"]))
                {
                    throw new Exception("Time is expired! ");
                }
                */
                log.Info("Response SOAP message from BlueX for pkcs12 is: " + SOAPresponse);
                Utils.ParseSoapEnvelopePKCS12(SOAPresponse, out USI, out pkcs12, out errorPKCS12);

                if (errorPKCS12 != string.Empty)
                {
                    string parseErrorPKCS12 = Utils.Between(errorPKCS12, Constants.rightBrackets, Constants.leftBrackets);
                    Session["Preuzimanje-softverskog-sertifikata-parseErrorPKCS12"] = parseErrorPKCS12;
                    log.Info("parseErrorPKCS12: " + parseErrorPKCS12);
                }
                
                if (pkcs12 == string.Empty)
                {
                    throw new Exception("pkcs12 is empty! ");
                }

                Session["Preuzimanje-softverskog-sertifikata-pkcs12"] = pkcs12;

                string pkcs12filename = Path.GetFileName(pkcs12);                

                ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
                txtdatotekasert.Text = pkcs12filename;

                ScriptManager.RegisterStartupScript(this, GetType(), "successalert", "successalert();", true);

                log.Info("Successfully send SOAP message! pkcs12 is: " + pkcs12);
            }
            catch (Exception ex)
            {
                log.Error("Error while sending request. " + ex.Message);
                //errLabel.Text = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3374);
                if (Session["Preuzimanje-softverskog-sertifikata-parseErrorPKCS12"].ToString().Equals(Constants.AuthorizationCodeDoesNotMatch))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorGettingAuthorizationCodePKCS12", "errorGettingAuthorizationCodePKCS12();", true);
                }
                else if (Session["Preuzimanje-softverskog-sertifikata-parseErrorPKCS12"].ToString().Equals(Constants.ErrorGettingRequestNumber))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorGettingRequestNumberPKCS12", "errorGettingRequestNumberPKCS12();", true);
                }
                else if (Session["Preuzimanje-softverskog-sertifikata-parseErrorPKCS12"].ToString().Equals(Constants.TransferServiceFailed))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorTransferServiceFailedPKCS12", "errorTransferServiceFailedPKCS12();", true);
                }
                else if (Session["Preuzimanje-softverskog-sertifikata-parseErrorPKCS12"].ToString().Equals(Constants.RrequestIsNotInRequiredState))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorRrequestIsNotInRequiredStatePKCS12", "errorRrequestIsNotInRequiredStatePKCS12();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
            }
        }
        else if (!Page.IsValid)
        {
            errLabel.Text = string.Empty;
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
        }
    }

    protected void btnDownloadPKCS12Certificate_Click(object sender, EventArgs e)
    {
        try
        {
            //***TESTIRANJE***Session["Preuzimanje-softverskog-sertifikata-pkcs12"] = @"\\ca-sajt.ca.posta.rs\c$\inetpub\wwwroot\Posta\Dokumentacija\P12_FOLDER\Sertifikat-PKCS12-4e624421b7dcb6dbdf-05112018.p12";
            //String FileName = "Sertifikat-PKCS12-338151188815333c-20180605.p12";
            string FileName = @"attachment; filename=""" + Path.GetFileName(Session["Preuzimanje-softverskog-sertifikata-pkcs12"].ToString()) + "";
            //String FilePath = @"C:\inetpub\wwwroot\Posta\Dokumentacija\P12_FOLDER\Sertifikat-PKCS12-338151188815333c-20180605.p12";
            string FilePath = Utils.ConvertToLocalPath(Session["Preuzimanje-softverskog-sertifikata-pkcs12"].ToString());            
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.Buffer = true;
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition", FileName); //forse save as dialog in Mozzila an Explorer but no in Chrome
            response.TransmitFile(FilePath);
            response.Flush();
        }
        catch (Exception ex)
        {
            log.Error("Error while saving .p12 file. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);
        }
        finally
        {
            HttpContext.Current.Response.End();
        }
    }

    public void SetFocusOnTextbox()
    {
        try
        {
            if (Session["Preuzimanje-softverskog-sertifikata-pkcs12-event_controle"] != null)
            {
                TextBox controle = (TextBox)Session["Preuzimanje-softverskog-sertifikata-pkcs12-event_controle"];
                //controle.Focus();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "foc", "document.getElementById('" + controle.ClientID + "').focus();", true);
            }
        }
        catch (InvalidCastException inEx)
        {
            log.Error("Problem with setting focus on control. Error: " + inEx);
        }
    }

    public void SetFocusOnDropDownLists()
    {
        try
        {
            if (Session["Preuzimanje-softverskog-sertifikata-pkcs12-event_controle-DropDownList"] != null)
            {
                DropDownList padajucalista = (DropDownList)Session["Preuzimanje-softverskog-sertifikata-pkcs12-event_controle-DropDownList"];
                //padajucalista.Focus();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "foc", "document.getElementById('" + padajucalista.ClientID + "').focus();", true);
            }
        }
        catch (InvalidCastException inEx)
        {
            log.Error("Problem with setting focus on control. Error: " + inEx);
        }
    }
}