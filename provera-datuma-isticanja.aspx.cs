using BlueXSOAP;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class provera_datuma_isticanja : System.Web.UI.Page
{
    public string SetDarkGray = Constants.SetDarkGray;
    public string SetLightGray = Constants.SetLightGray;
    public string SetWhite = Constants.SetWhite;

    public string userAgentStringApplicant = string.Empty;
    public string userAgentBrowser = string.Empty;
    public string userAgentOS = string.Empty;
    public string userAgentIP = string.Empty;
    public string userAgentContinent = string.Empty;
    public string userAgentCountry = string.Empty;
    public string userAgentCountryCode = string.Empty;
    public string userAgentCity = string.Empty;
    public string userAgentISP = string.Empty;
    public PisMessServiceReference.PisMessServiceClient pisMess;

    public List<WebControl> Controls;
    public List<ItemVariable> Items;

    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            Session["zahtev-provera-datuma-isticanja-sertifikata-JIK-string.empty"] = false;

            // replace encoded plus sign "%2b" with real plus sign +
            encryptedParameters = encryptedParameters.Replace("%2b", "+");
            string decryptedParameters = AuthenticatedEncryption.AuthenticatedEncryption.Decrypt(encryptedParameters, Constants.CryptKey, Constants.AuthKey);

            HttpRequest req = new HttpRequest("", "http://www.pis.rs", decryptedParameters);

            string Checked = req.QueryString["checked"];
            string error = req.QueryString["error"];
            string data = req.QueryString["data"];

            if ((data != string.Empty) && (data != null))
            {
                GetCertificateData(data, error, Checked);
            }
            else
            {
                Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"] = string.Empty;
                Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateFirstName"] = string.Empty;
                Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateLastName"] = string.Empty;
            }

            AvoidCashing();

            if (!Page.IsPostBack)
            {
                if (Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"] != null)
                {
                    string jik = Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"].ToString();
                    if (jik != string.Empty)
                    {
                        FieldToDisplay();
                        hideImeSertifikata.Visible = true;
                        txtjik.Text = jik;
                        txtjik.BackColor = ColorTranslator.FromHtml(SetLightGray);
                        txtjik.ReadOnly = true;
                        txtserijskibroj02.Text = Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateCN"].ToString();
                        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
                        txtserijskibroj02.ReadOnly = true;
                        txtime.Text = Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateFirstName"].ToString();
                        txtprezime.Text = Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateLastName"].ToString();
                        ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCkeckAutomatik", "RadioButtonCkeckAutomatik();", true);
                    }
                    else
                    {
                        FieldToDisplay();
                        hideImeSertifikata.Visible = false;
                        if (error != null)
                        { 
                            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorNotification", "ErrorNotification();", true);
                            log.Error("error is not null. error: " + error);
                        }
                        ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCkeck", "RadioButtonCkeck();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "EnableSubmitButton", "EnableSubmitButton();", true);
                    }
                }
                else
                { 
                    FieldToDisplay();
                    hideImeSertifikata.Visible = false;
                    ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCkeck", "RadioButtonCkeck();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "EnableSubmitButton", "EnableSubmitButton();", true);
                }
                //-------TABINDEX---------------
                if (Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"] != null)
                {
                    string jik = Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"].ToString();
                    if (jik != string.Empty)
                    {
                        Session["zahtev-provera-datuma-isticanja-sertifikata-event_controle-RadioButton"] = rbAutomatikJik;
                    }
                }
                else
                {
                    Session["zahtev-provera-datuma-isticanja-sertifikata-event_controle-RadioButton"] = rbManualJik;
                }
                txtime.TabIndex = -1;
                txtserijskibroj02.TabIndex = -1;
                txtprezime.TabIndex = -1;
                txtdatumizdavanja.TabIndex = -1;
                txtdatumsiteka.TabIndex = -1;
                SetFocusOnRadioButton();
                //------------------------------
                //Get Control on all page
                SetUpValidation();
                log.Debug("successfully set Validation!");
                SetUpIsRequiredTextBoxes();
                log.Debug("successfully set RequiredTextBoxes!");
                //SetUpIsRequiredDropDownLists();
                log.Debug("Application Starting, successfully get all controls!");
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

    protected void GetCertificateData(string data, string error, string Checked)
    {
        try
        {
            //string encryptedParameters = Request.QueryString["d"];

            //if ((encryptedParameters != string.Empty) && (encryptedParameters != null))
            //{
            //    // replace encoded plus sign "%2b" with real plus sign +
            //    encryptedParameters = encryptedParameters.Replace("%2b", "+");

            //    string decryptedParameters = AuthenticatedEncryption.AuthenticatedEncryption.Decrypt(encryptedParameters, Constants.CryptKey, Constants.AuthKey);

            //    HttpRequest req = new HttpRequest("", "https://www.pis.rs", decryptedParameters);

            //    string Checked = req.QueryString["checked"];
            //    string error = req.QueryString["error"];
            //    string data = req.QueryString["data"];

                if (!error.Equals("0"))
                {
                    throw new Exception("Error reading certificate data. " + error);
                }

                if (!Checked.Equals("1"))
                {
                    throw new Exception("Error reading certificate data. " + Checked);
                }

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters = ParseData(data);

                foreach (var par in parameters)
                {
                    if (par.Key.Equals("jik", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"] = par.Value;
                    }
                    else if (par.Key.Equals("cn", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateCN"] = par.Value;
                    }
                    else if (par.Key.Equals("firstName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateFirstName"] = par.Value;
                    }
                    else if (par.Key.Equals("lastName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateLastName"] = par.Value;
                    }
                }
            //}
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetCertificateData. " + ex.Message);
            //string ErrorPage = System.Configuration.ConfigurationManager.AppSettings["ErrorPage"].ToString();
            //Response.Redirect(@ErrorPage);
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorNotification", "ErrorNotification();", true);
        }
    }

    protected Dictionary<string, string> ParseData(string data)
    {
        try
        {
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            //List<ReturnParameter> parameterList = new List<ReturnParameter>();

            int temp1 = 0;
            int temp2 = 0;

            int start = 0;
            do
            {
                temp1 = data.IndexOf("|||", start);
                start = temp1 + 3;
                temp2 = data.IndexOf("|||", start);

                if (temp2 > 0)
                {
                    string paramString = data.Substring(start, temp2 - temp1 - 3);
                    string[] parameter = ParseParameter(paramString);
                    parameterList.Add(parameter[0], parameter[1]);
                }
            }
            while (temp2 > 0);

            return parameterList;
        }
        catch (Exception ex)
        {
            throw new Exception("Parameters format is not correct. " + ex.Message);
        }
    }

    protected string[] ParseParameter(string param)
    {
        string[] parameter = new string[2];
        int temp1 = param.IndexOf("=");
        parameter[0] = param.Substring(0, temp1);
        parameter[1] = param.Substring(temp1 + 1);

        return parameter;
    }

    private void AvoidCashing()
    {
        Response.Cache.SetNoStore();
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    protected void FieldToDisplay()
    {
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtserijskibroj02.ReadOnly = true;
        txtime.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtime.ReadOnly = true;
        txtprezime.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtprezime.ReadOnly = true;
        txtdatumizdavanja.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtdatumizdavanja.ReadOnly = true;
        txtdatumsiteka.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtdatumsiteka.ReadOnly = true;
        errLabel.Text = string.Empty;
    }

    protected void rbAutomatikJik_CheckedChanged(object sender, EventArgs e)
    {
        Session["zahtev-provera-datuma-isticanja-sertifikata-JIK-string.empty"] = true;
        rbAutomatikJik.Checked = true;
        ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
        ScriptManager.RegisterStartupScript(this, GetType(), "DisableSubmitButton", "DisableSubmitButton();", true);
        rbManualJik.Checked = false;
        txtjik.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtjik.ReadOnly = true;
        txtjik.Text = string.Empty;
        txtserijskibroj02.ReadOnly = true;
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtime.Text = string.Empty;
        txtprezime.Text = string.Empty;
        txtdatumizdavanja.Text= string.Empty;
        txtdatumsiteka.Text = string.Empty;
        cvjik.Enabled = false;
        FieldToDisplay();
        hideImeSertifikata.Visible = true;
        Session["zahtev-provera-datuma-isticanja-sertifikata-event_controle-RadioButton"] = rbAutomatikJik;
        txtjik.TabIndex = -1;
        txtime.TabIndex = -1;
        txtserijskibroj02.TabIndex = -1;
        txtprezime.TabIndex = -1;
        txtdatumizdavanja.TabIndex = -1;
        txtdatumsiteka.TabIndex = -1;
        SetFocusOnRadioButton();
    }

    protected void rbManualJik_CheckedChanged(object sender, EventArgs e)
    {
        rbManualJik.Checked = true;
        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
        ScriptManager.RegisterStartupScript(this, GetType(), "EnableSubmitButton", "EnableSubmitButton();", true);
        rbAutomatikJik.Checked = false;
        if (Convert.ToBoolean(Session["zahtev-provera-datuma-isticanja-sertifikata-JIK-string.empty"]))
        {
            txtjik.Text = string.Empty;
        }
        else if (txtime.Text != string.Empty || txtprezime.Text != string.Empty)
        {
            txtjik.Text = string.Empty;
        }
        txtime.Text = string.Empty;
        txtprezime.Text = string.Empty;
        txtdatumizdavanja.Text = string.Empty;
        txtdatumsiteka.Text = string.Empty;
        txtserijskibroj02.ReadOnly = true;
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtserijskibroj02.Text = string.Empty;

        txtjik.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtjik.ReadOnly = false;
        cvjik.Enabled = true;
        FieldToDisplay();
        hideImeSertifikata.Visible = false;
        Session["zahtev-provera-datuma-isticanja-sertifikata-event_controle-RadioButton"] = rbManualJik;
        txtjik.TabIndex = 0;
        txtime.TabIndex = -1;
        txtserijskibroj02.TabIndex = -1;
        txtprezime.TabIndex = -1;
        txtdatumizdavanja.TabIndex = -1;
        txtdatumsiteka.TabIndex = -1;
        SetFocusOnRadioButton();
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
                    log.Debug("Error while setting control's " + control.Controlid + " text: " + ex.Message);
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
                    log.Debug("Error while setting control's " + control.Controlid + " text: " + ex.Message);
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
                log.Debug("Error while setting control's " + control.Controlid + " visibility: " + ex.Message);
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
                    log.Debug("Error while setting control's " + control.Controlid + "  " + ex.Message);
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
                    log.Debug("Error while setting control's " + control.Controlid + "  " + ex.Message);
                }
            }

            if (Constants.CONTROL_TYPE_RADIOBUTTON.ToLower() == control.ControlType.ToLower())
            {
                try
                {
                    RadioButton radiodugme = (RadioButton)FindControlRecursive(Page, control.Controlid);
                    //tekstualnopolje.Text = control.ControlTittle;
                    radiodugme.Text = control.ControlTittle;
                    radiodugme.Enabled = control.IsEnabled;
                    radiodugme.Visible = control.IsVisible;
                }
                catch (Exception ex)
                {
                    log.Debug("Error while setting control's " + control.Controlid + "  " + ex.Message);
                }
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetUpAllFields();
        log.Debug("Successfully set all Fields on page!");
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

    //-----------------SetUpValidation-------------------------------
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
                if (control.Id == txtjik.ClientID)
                {
                    Session["zahtev-provera-datuma-isticanja-sertifikata-TurnOnJIKValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            Session["zahtev-provera-datuma-isticanja-sertifikata-TurnOnJIKValidation"] = Constants.VALIDATION_FALSE;
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
            if (control.Id == txtjik.ClientID)
            {
                Session["zahtev-provera-datuma-isticanja-sertifikata-txtjikIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtserijskibroj02.ClientID)
            {
                Session["zahtev-provera-datuma-isticanja-sertifikata-txtserijskibroj02IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtime.ClientID)
            {
                Session["zahtev-provera-datuma-isticanja-sertifikata-txtimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezime.ClientID)
            {
                Session["zahtev-provera-datuma-isticanja-sertifikata-txtprezimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumizdavanja.ClientID)
            {
                Session["zahtev-provera-datuma-isticanja-sertifikata-txtdatumizdavanjaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumsiteka.ClientID)
            {
                Session["zahtev-provera-datuma-isticanja-sertifikata-txtdatumsitekaIsRequired"] = control.IsRequired;
            }
        }
    }

    /*
    protected void SetUpIsRequiredDropDownLists()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrole(page);

        foreach (var control in Controls)
        {
            if (control.Id == ddlListaSertifikata.ClientID)
            {
                Session["provera-statusa-zahteva-ddlListaSertifikataIsRequired"] = control.IsRequired;
            }
        }
    }
    */
    //---------------------------------------------------------------
    //---------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    protected void cvjik_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateJIK(txtjik.Text, Convert.ToBoolean(Session["zahtev-provera-datuma-isticanja-sertifikata-txtjikIsRequired"]), Convert.ToBoolean(Session["zahtev-provera-datuma-isticanja-sertifikata-TurnOnJIKValidation"]), out ErrorMessage1);
            cvjik.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvjik.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvime_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtime.Text, Convert.ToBoolean(Session["zahtev-provera-datuma-isticanja-sertifikata-txtimeIsRequired"]), out ErrorMessage1);
            cvime.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvime.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvserijskibroj02_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtserijskibroj02.Text, Convert.ToBoolean(Session["zahtev-provera-datuma-isticanja-sertifikata-txtserijskibroj02IsRequired"]), out ErrorMessage1);
            cvserijskibroj02.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvserijskibroj02.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvprezime_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtprezime.Text, Convert.ToBoolean(Session["zahtev-provera-datuma-isticanja-sertifikata-txtprezimeIsRequired"]), out ErrorMessage1);
            cvprezime.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvprezime.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdatumizdavanja_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtdatumizdavanja.Text, Convert.ToBoolean(Session["zahtev-provera-datuma-isticanja-sertifikata-txtdatumizdavanjaIsRequired"]), out ErrorMessage1);
            cvdatumizdavanja.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvdatumizdavanja.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdatumisteka_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtdatumsiteka.Text, Convert.ToBoolean(Session["zahtev-provera-datuma-isticanja-sertifikata-txtdatumsitekaIsRequired"]), out ErrorMessage1);
            cvdatumisteka.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvdatumisteka.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txtjik_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged();
    }

    private void CheckIfChannelHasChanged()
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
    }

    public List<CertificateStatusCheck> CertificateStatuses;

    protected void btnSubmit_Click1(object sender, EventArgs e)
    {
        Utility utility = new Utility();
        try
        {
            if (Page.IsValid)
            {
                txtime.Text = string.Empty;
                txtprezime.Text = string.Empty;
                txtdatumizdavanja.Text = string.Empty;
                txtdatumsiteka.Text = string.Empty;
                try
                {
                    /*
                    //-----------------GetUserAgent string---------------------------
                    Utility utility = new Utility();
                    string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_GEOLOCATIONS);
                    if (SettingValue == Constants.SETTING_VALUE_TRUE)
                    {
                        GetUserAgentInformation(out userAgentBrowser, out userAgentStringApplicant, out userAgentOS, out userAgentIP);
                        log.Debug("GetUserAgentInformation function. userAgentBrowser is " + userAgentBrowser + ". userAgentStringApplicant is " + userAgentStringApplicant + ". userAgentOS is " + userAgentOS + ". userAgentIP is " + userAgentIP);
                        PisMessServiceReference.IpGeolocationData ipGeolocationData = new PisMessServiceReference.IpGeolocationData();
                        try
                        {
                            ipGeolocationData = new PisMessServiceReference.PisMessServiceClient().SendIpGeolocationRequest(userAgentIP);
                        }
                        catch (Exception)
                        {
                            log.Error("IP address is not in correct format or it is empty. IP is: " + userAgentIP);
                        }
                        Utils.CheckIPGeolocationData(ipGeolocationData.Status, userAgentIP, ipGeolocationData.Country, ipGeolocationData.CountryCode, ipGeolocationData.City, ipGeolocationData.Isp, ipGeolocationData.Continent, out userAgentCountry, out userAgentCountryCode, out userAgentCity, out userAgentISP, out userAgentContinent);
                    }
                    else
                    {
                        log.Debug("Geolocation is not active!");
                    }
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentBrowser"] = userAgentBrowser;
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentStringApplicant"] = userAgentStringApplicant;
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentOS"] = userAgentOS;
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentIP"] = userAgentIP;
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentContinent"] = userAgentContinent;
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentCountry"] = userAgentCountry;
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentCountryCode"] = userAgentCountryCode;
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentCity"] = userAgentCity;
                    Session["zahtev-provera-datuma-isticanja-sertifikata-userAgentISP"] = userAgentISP;
                    //todo Poslati SOAP poruku sa JIK-om i dobiti ime, prezime, datum izdavanja korisnika i datum isteka vaznosti sertifikata
                    */
                    log.Debug("Start sending SOAP message with USI.");

                    BxSoapEnvelope envelope = new BxSoapEnvelopeCertificateStatusCheck();
                    //todo samo zameni
                    if (Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"] != null)
                    {
                        string jik = Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"].ToString();
                        if (jik != string.Empty)
                        {
                            envelope.BxData.setValue(@"USI", jik);
                        }
                        else
                        {
                            envelope.BxData.setValue(@"USI", txtjik.Text);
                        }
                    }
                    else
                    {
                        envelope.BxData.setValue(@"USI", txtjik.Text);
                    }
                    //envelope.BxData.setValue(@"USI", "PNORS-2603978934645");

                    string SOAPresponse = string.Empty;
                    try
                    {
                        SOAPresponse = BxSoap.SOAPManual(envelope.createBxSoapEnvelope());
                        
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error. Response from BlueX is: " + SOAPresponse + ex.Message);
                    }

                    CertificateStatuses = Utils.ParseSoapEnvelopeCertificateStatusCheck(SOAPresponse);

                    if (CertificateStatuses == null)
                    {
                        throw new Exception("Error. CertificateStatuses List is null. " + SOAPresponse);
                    }

                    Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateStatuses"] = CertificateStatuses;

                    log.Debug("Successfully send SOAP message with USI.");

                    if (CertificateStatuses.Count > 0)
                    {
                        foreach (var certificatestatus in CertificateStatuses)
                        {
                            if (Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"] != null)
                            {
                                string jik = Session["zahtev-provera-datuma-isticanja-sertifikata-CertificateJIK"].ToString();
                                if (jik != string.Empty)
                                {
                                    txtjik.Text = jik;
                                }
                            }
                            else
                            {
                                txtjik.Text = txtjik.Text;
                            }
                            errLabel.Text = string.Empty;
                            txtime.Text = certificatestatus.GivenName;
                            txtprezime.Text = certificatestatus.LastName;
                            txtdatumizdavanja.Text = certificatestatus.ValidFrom;
                            txtdatumsiteka.Text = certificatestatus.ValidTo;
                            ScriptManager.RegisterStartupScript(this, GetType(), "successalert", "successalert();", true);
                        }
                    }
                    else
                    {
                        log.Debug("Za navedeni USI(JIK) " + txtjik.Text + "nema statusa.");
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Notification();", true);
                    }
                }
                catch (Exception ex)
                {
                    log.Debug("Za navedeni USI(JIK) " + txtjik.Text + "nema statusa. " + ex.Message);
                    ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Notification();", true);
                    errLabel.Text = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3371);
                    //log.Error("Error while sending request. " + ex.Message);
                    //ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);
                }
            }
            else if (!Page.IsValid)
            {
                txtime.Text = string.Empty;
                txtprezime.Text = string.Empty;
                txtdatumizdavanja.Text = string.Empty;
                txtdatumsiteka.Text = string.Empty;
                errLabel.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
            }

            if (rbAutomatikJik.Checked)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "EnableSubmitButton", "EnableSubmitButton();", true);
        }
        catch (Exception ex)
        {
            log.Error("Error while sending SOAP message. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "errorSOAPalert", "errorSOAPalert();", true);
        }
    }

    protected void btnReadCardInfo_Click(object sender, EventArgs e)
    {
        cvjik.Enabled = false;

        ScriptManager.RegisterStartupScript(this, GetType(), "ExplorerLogout", "ExplorerLogout();", true);
        ScriptManager.RegisterStartupScript(this, GetType(), "ChromeLogout", "ChromeLogout();", true);

        string ReturnURL = System.Configuration.ConfigurationManager.AppSettings["ReturnURL"].ToString();
        string parameters = @"returnUrl=" + ReturnURL; //type moze da bude jik ili serial
        string encodedString = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt(parameters, Constants.CryptKey, Constants.AuthKey);
        // znak plus pravi problem kada se posalje u url-u, pa mora da se svuda zameni sa "%252b"
        encodedString = encodedString.Replace("+", "%252b");
        string ClientSslAuthenticationURL = System.Configuration.ConfigurationManager.AppSettings["ClientSslAuthenticationURL"].ToString();
        log.Debug("URL kojim se poziva aplikacija za očitavanje sertifikata: " + @ClientSslAuthenticationURL + encodedString);
        Response.Redirect(@ClientSslAuthenticationURL + encodedString);
    }

    private void GetUserAgentInformation(out string userAgentBrowser, out string userAgentStringApplicant, out string userAgentOS, out string userAgentIP)
    {
        userAgentStringApplicant = Request.UserAgent.ToString();
        //-----Browser-----
        HttpBrowserCapabilities bc = Request.Browser;
        userAgentBrowser = bc.Type;
        //-----Operating System-----
        string OS = string.Empty;
        Utils.getOS(userAgentStringApplicant, out OS);
        userAgentOS = OS;
        //-------IP-----------------
        //todo Ovom funkcijom ce se dobiti javna adresa pomocu javnog servisa - koristimo u test fazi
        //string IP = string.Empty;
        //Utils.GetIP(out IP);
        //userAgentIP = IP;
        //todo Ovom funkcijom ce se dobiti javna adresa lokalno - kad se publishuje na IIS        
        userAgentIP = Utils.GetIPAddress();
        ////todo Ovom funkcijom ce se dobiti javna adresa lokalno - kad se publishuje na IIS        
        ////userAgentIP = Utils.GetIPAddress();
        ////-------------------------
        ////-------Geolocation-------
        //string Continent = string.Empty;
        //string Country = string.Empty;
        //string CountryCode = string.Empty;
        //string City = string.Empty;
        //string ISP = string.Empty;
        //Utils.GetLocation(userAgentIP, out Continent, out Country, out CountryCode, out City, out ISP);
        //userAgentContinent = Continent;
        //userAgentCountry = Country;
        //userAgentCountryCode = CountryCode;
        //userAgentCity = City;
        //userAgentISP = ISP;
        //-------------------------
    }

    public void SetFocusOnTextbox()
    {
        try
        {
            if (Session["zahtev-provera-datuma-isticanja-sertifikata-event_controle"] != null)
            {
                TextBox controle = (TextBox)Session["zahtev-provera-datuma-isticanja-sertifikata-event_controle"];
                //controle.Focus();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "foc", "document.getElementById('" + controle.ClientID + "').focus();", true);
            }
        }
        catch (InvalidCastException inEx)
        {
            log.Error("Problem with setting focus on control. Error: " + inEx);
        }
    }

    public void SetFocusOnRadioButton()
    {
        try
        {
            if (Session["zahtev-provera-datuma-isticanja-sertifikata-event_controle-RadioButton"] != null)
            {
                RadioButton radiodugme = (RadioButton)Session["zahtev-provera-datuma-isticanja-sertifikata-event_controle-RadioButton"];
                //radiodugme.Focus();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "foc", "document.getElementById('" + radiodugme.ClientID + "').focus();", true);
            }
        }
        catch (InvalidCastException inEx)
        {
            log.Error("Problem with setting focus on control. Error: " + inEx);
        }
    }
}