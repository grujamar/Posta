using log4net;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class provera_opozvanosti_sertifikata : System.Web.UI.Page
{
    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public string SetDarkGray = Constants.SetDarkGray;
    public string SetLightGray = Constants.SetLightGray;
    public string SetWhite = Constants.SetWhite;
    public string SetCss1 = Constants.SetCss1;
    public string SetCss2 = Constants.SetCss2;
    public string SetCss3 = Constants.SetCss3;
    public string SetCss4 = Constants.SetCss4;
    public string SetCss5 = Constants.SetCss5;
    public string userAgentStringApplicant = string.Empty;
    public string userAgentBrowser = string.Empty;
    public string userAgentOS = string.Empty;
    public string userAgentIP = string.Empty;
    public string userAgentContinent = string.Empty;
    public string userAgentCountry = string.Empty;
    public string userAgentCountryCode = string.Empty;
    public string userAgentCity = string.Empty;
    public string userAgentISP = string.Empty;
    public PisMessServiceReference.PisMessServiceClient pisMess1;

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
            Session["zahtev-promena-statusa-sertifikata-SerialNo-string.empty"] = false;
            Session["zahtev-promena-statusa-sertifikata-SerialNo-string.empty1"] = true;
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
                Session["provera-opozvanosti-sertifikata-CertificateJIK"] = string.Empty;
            }
            AvoidCashing();

            if (!Page.IsPostBack)
            {
                Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"] = true;

                if (Session["provera-opozvanosti-sertifikata-CertificateJIK"] != null)
                {
                    string jik = Session["provera-opozvanosti-sertifikata-CertificateJIK"].ToString();
                    if (jik != string.Empty)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCkeckAutomatik", "RadioButtonCkeckAutomatik();", true);

                        txtserialno.Text = Session["provera-opozvanosti-sertifikata-CertificateSerial"].ToString();
                        txtserialno.BackColor = ColorTranslator.FromHtml(SetLightGray);
                        txtserialno.ReadOnly = true;
                        txtserijskibroj02.Text = Session["provera-opozvanosti-sertifikata-CertificateCN"].ToString();
                        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
                        txtserijskibroj02.ReadOnly = true;
                        txtime02.Text = Session["provera-opozvanosti-sertifikata-CertificateFirstName"].ToString();
                        txtime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
                        txtime02.ReadOnly = true;
                        txtprezime02.Text = Session["provera-opozvanosti-sertifikata-CertificateLastName"].ToString();
                        txtprezime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
                        txtprezime02.ReadOnly = true;
                        txtjik01.Text = jik;
                        txtjik01.BackColor = ColorTranslator.FromHtml(SetLightGray);
                        txtjik01.ReadOnly = true;
                        
                        SetPageOnBeginning();
                    }
                    else
                    {
                        if (error != null)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorNotification", "ErrorNotification();", true);
                            log.Error("error is not null. error: " + error);
                        }
                        ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCkeck", "RadioButtonCkeck();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "EnableSubmitButton", "EnableSubmitButton();", true);
                        txtserialno.Text = string.Empty;
                        SetPageOnBeginning();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCkeck", "RadioButtonCkeck();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "EnableSubmitButton", "EnableSubmitButton();", true);
                    txtserialno.Text = string.Empty;
                    SetPageOnBeginning();
                }
                //-------TABINDEX---------------
                if (Session["provera-opozvanosti-sertifikata-CertificateJIK"] != null)
                {
                    string jik = Session["provera-opozvanosti-sertifikata-CertificateJIK"].ToString();
                    if (jik != string.Empty)
                    {
                        Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbAutomatikSerialNo;
                    }
                }
                else
                {
                    Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbManualSerialNo;
                }
                txtserialno.TabIndex = 0;
                txtserijskibroj02.TabIndex = -1;
                txtime02.TabIndex = -1;
                txtprezime02.TabIndex = -1;
                txtjik01.TabIndex = -1;
                SetFocusOnRadioButton();
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


        SetObjectDataSourceParameterRevocationCheckTypeOne();
    }

    protected void SetPageOnBeginning()
    {
        Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"] = true;

        txtserijskibroj02.ReadOnly = true;
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtime02.ReadOnly = true;
        txtime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtprezime02.ReadOnly = true;
        txtprezime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtjik01.ReadOnly = true;
        txtjik01.BackColor = ColorTranslator.FromHtml(SetLightGray);

        txtrevocationaddress.ReadOnly = true;
        txtrevocationaddress.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtrevocationaddress.Text = string.Empty;

        UnhideFirstPartPage();
        
        string IDItem1 = string.Empty;
        SetUpDefaultItem(ddlimeizdavaoca.ClientID, out IDItem1);
        ddlimeizdavaoca.SelectedValue = IDItem1;
        
        rbOCSPRevocation.Checked = true;
        rbCRLRevocation.Checked = false;       
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

            //    HttpRequest req = new HttpRequest("", "http://www.pis.rs", decryptedParameters);

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
                        Session["provera-opozvanosti-sertifikata-CertificateJIK"] = par.Value;
                    }
                    else if (par.Key.Equals("cn", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["provera-opozvanosti-sertifikata-CertificateCN"] = par.Value;
                    }
                    else if (par.Key.Equals("firstName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["provera-opozvanosti-sertifikata-CertificateFirstName"] = par.Value;
                    }
                    else if (par.Key.Equals("lastName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["provera-opozvanosti-sertifikata-CertificateLastName"] = par.Value;
                    }
                    else if (par.Key.Equals("serial", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["provera-opozvanosti-sertifikata-CertificateSerial"] = par.Value;
                    }
                }
            //}
        }
        catch (Exception ex)
        {
            log.Info("Error in function GetCertificateData. " + ex.Message);
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

                    //dugme.Enabled = control.IsEnabled;
                    //dugme.Visible = control.IsVisible;
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
                    log.Info("Error while setting control's " + control.Controlid + "  " + ex.Message);
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
                    log.Info("Error while setting control's " + control.Controlid + "  " + ex.Message);
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
                    log.Info("Error while setting control's " + control.Controlid + "  " + ex.Message);
                }
            }

            if (Constants.CONTROL_TYPE_HYPERLINK.ToLower() == control.ControlType.ToLower())
            {
                try
                {
                    HyperLink hiperlink = (HyperLink)FindControlRecursive(Page, control.Controlid);
                    //tekstualnopolje.Text = control.ControlTittle;
                    hiperlink.Text = control.ControlTittle;
                    hiperlink.Enabled = control.IsEnabled;
                    hiperlink.Visible = control.IsVisible;
                }
                catch (Exception ex)
                {
                    log.Info("Error while setting control's " + control.Controlid + " text: " + ex.Message);
                }
            }

        }
        ddlimeizdavaoca.Enabled = Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"]);
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
                if (control.Id == txtserialno.ClientID)
                {
                    Session["provera-opozvanosti-sertifikata-TurnOnSerialNumberValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            Session["provera-opozvanosti-sertifikata-TurnOnSerialNumberValidation"] = Constants.VALIDATION_FALSE;
        }
    }
    //---------------------------------------------------------------
    //---------------------------------------------------------------

    protected void SetUpIsRequiredTextBoxes()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrolePoTipu(page, Constants.CONTROL_TYPE_TEXTBOX);

        foreach (var control in Controls)
        {
            if (control.Id == txtserialno.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtserialnoIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtserijskibroj02.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtserijskibroj02IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtime02.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtime02IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezime02.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtprezime02IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtjik01.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtjik01IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtrevocationaddress.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtrevocationaddressIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtserijskibrojsert.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtserijskibrojsertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtstatussert.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtstatussertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtrazlogopozivasert.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtrazlogopozivasertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumvremeopozivasert.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtdatumvremeopozivasertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumvremekompromitovanjasert.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtdatumvremekompromitovanjasertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtimeizdavaoca.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtimeizdavaocaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtimeservera.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtimeserveraIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtnacinprovere.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtnacinprovereIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumvremesprovedeneprovere.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtdatumvremesprovedeneprovereIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtcheckingurl.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-txtcheckingurlIsRequired"] = control.IsRequired;
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
            if (control.Id == ddlimeizdavaoca.ClientID)
            {
                Session["provera-opozvanosti-sertifikata-ddlimeizdavaocaIsRequired"] = control.IsRequired;
            }
        }
    }

    //---------------------------------------------------------------
    //---------------------------------------------------------------

    protected void rbManualSerialNo_CheckedChanged(object sender, EventArgs e)
    {
        Session["zahtev-promena-statusa-sertifikata-SerialNo-string.empty1"] = true;
        rbManualSerialNo.Checked = true;
        rbAutomatikSerialNo.Checked = false;
        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
        
        txtserialno.ReadOnly = false;
        txtserialno.BackColor = ColorTranslator.FromHtml(SetWhite);
        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-SerialNo-string.empty"]))
        {
            txtserialno.Text = string.Empty;
        }
        else if (txtime02.Text != string.Empty || txtprezime02.Text != string.Empty || txtjik01.Text != string.Empty)
        {
            txtserialno.Text = string.Empty;
        }
        txtserijskibroj02.ReadOnly = true;
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtserijskibroj02.Text = string.Empty;
        txtime02.ReadOnly = true;
        txtime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtime02.Text = string.Empty;
        txtprezime02.ReadOnly = true;
        txtprezime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtjik01.ReadOnly = true;
        txtprezime02.Text = string.Empty;
        txtjik01.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtjik01.Text = string.Empty;

        cvserialno.Enabled = true;

        Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbManualSerialNo;
        SetFocusOnRadioButton();
        txtserialno.TabIndex = 0;
        txtserijskibroj02.TabIndex = -1;
        txtime02.TabIndex = -1;
        txtprezime02.TabIndex = -1;
        txtjik01.TabIndex = -1;
    }

    protected void rbAutomatikSerialNo_CheckedChanged(object sender, EventArgs e)
    {
        Session["zahtev-promena-statusa-sertifikata-SerialNo-string.empty"] = true;
        rbAutomatikSerialNo.Checked = true;
        rbManualSerialNo.Checked = false;
        ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);

        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-SerialNo-string.empty1"]))
        {
            txtserialno.Text = string.Empty;
        }
        txtserialno.ReadOnly = true;
        txtserialno.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtserijskibroj02.ReadOnly = true;
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtime02.ReadOnly = true;
        txtime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtprezime02.ReadOnly = true;
        txtprezime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtjik01.ReadOnly = true;
        txtjik01.BackColor = ColorTranslator.FromHtml(SetLightGray);

        cvserialno.Enabled = false;

        Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbAutomatikSerialNo;
        txtserialno.TabIndex = -1;
        txtserijskibroj02.TabIndex = -1;
        txtime02.TabIndex = -1;
        txtprezime02.TabIndex = -1;
        txtjik01.TabIndex = -1;
        SetFocusOnRadioButton();
    }

    protected void btnReadCardInfo_Click(object sender, EventArgs e)
    {
        cvserialno.Enabled = false;

        ScriptManager.RegisterStartupScript(this, GetType(), "ExplorerLogout", "ExplorerLogout();", true);
        ScriptManager.RegisterStartupScript(this, GetType(), "ChromeLogout", "ChromeLogout();", true);

        string ReturnURLRevocation = System.Configuration.ConfigurationManager.AppSettings["ReturnURLRevocation"].ToString();
        string parameters = @"returnUrl=" + ReturnURLRevocation; //type moze da bude jik ili serial
        string encodedString = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt(parameters, Constants.CryptKey, Constants.AuthKey);
        // znak plus pravi problem kada se posalje u url-u, pa mora da se svuda zameni sa "%252b"
        encodedString = encodedString.Replace("+", "%252b");
        string ClientSslAuthenticationURL = System.Configuration.ConfigurationManager.AppSettings["ClientSslAuthenticationURL"].ToString();
        log.Info("URL kojim se poziva aplikacija za očitavanje sertifikata: " + @ClientSslAuthenticationURL + encodedString);
        Response.Redirect(@ClientSslAuthenticationURL + encodedString);
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    protected void cvserialno_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string txtserialnostring = txtserialno.ClientID;
        int PropertyValueMin = 0;
        int PropertyValueMax = 0;

        SetUpSerialNoVariables(page, txtserialnostring, out PropertyValueMin, out PropertyValueMax);

        try
        {
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateSerialNo(txtserialno.Text, Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-txtserialnoIsRequired"]), Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-TurnOnSerialNumberValidation"]), PropertyValueMin, PropertyValueMax, out ErrorMessage1);
            cvserialno.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvserialno.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvimeizdavaoca_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ddlimeizdavaocaString = ddlimeizdavaoca.ClientID;
            string IDItem1 = string.Empty;
            SetUpDefaultItem(ddlimeizdavaocaString, out IDItem1);

            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateImeIzdavaoca(ddlimeizdavaoca.SelectedValue, Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-ddlimeizdavaocaIsRequired"]), IDItem1, out ErrorMessage1);
            cvimeizdavaoca.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvimeizdavaoca.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvserijskibroj02_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtserijskibroj02.Text, Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-txtserijskibroj02IsRequired"]), out ErrorMessage1);
            cvserijskibroj02.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvserijskibroj02.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvime02_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtime02.Text, Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-txtime02IsRequired"]), out ErrorMessage1);
            cvime02.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvime02.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvprezime02_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtprezime02.Text, Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-txtprezime02IsRequired"]), out ErrorMessage1);
            cvprezime02.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvprezime02.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvjik01_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtjik01.Text, Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-txtjik01IsRequired"]), out ErrorMessage1);
            cvjik01.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvjik01.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvrevocationaddress_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtrevocationaddress.Text, Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-txtrevocationaddressIsRequired"]), out ErrorMessage1);
            cvrevocationaddress.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvrevocationaddress.ErrorMessage = string.Empty;
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

    protected void ddlimeizdavaoca_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["zahtev-promena-statusa-sertifikata-SerialNo-string.empty"] = false;
        Session["zahtev-promena-statusa-sertifikata-SerialNo-string.empty1"] = false;
        Utility utility = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string ddlstring = ddlimeizdavaoca.ClientID;
        int SelectedValue = Convert.ToInt32(ddlimeizdavaoca.SelectedValue);

        try
        {
            if (rbOCSPRevocation.Checked)
            {
                string OCSPurl = utility.getURLocsp(page, ddlstring, Constants.REVOCATION_OCSP_CHECK_TYPE, SelectedValue);
                txtrevocationaddress.Text = OCSPurl;
                SetObjectDataSourceParameterRevocationCheckTypeOne();
            }
            else if (rbLDAP.Checked)
            {
                string LDAPurl = utility.getURLocsp(page, ddlstring, Constants.REVOCATION_CRL_LDAP_CHECK_TYPE, SelectedValue);
                txtrevocationaddress.Text = LDAPurl;
                SetObjectDataSourceParameterRevocationCheckTypeTwo();
            }
            else if (rbHTTP.Checked)
            {
                string HTTPurl = utility.getURLocsp(page, ddlstring, Constants.REVOCATION_CRL_HTTP_CHECK_TYPE, SelectedValue);
                txtrevocationaddress.Text = HTTPurl;
                SetObjectDataSourceParameterRevocationCheckTypeThree();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txtserialno_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged1();
        if (errLabelSerialNo.Text != string.Empty)
        {
            Session["provera-opozvanosti-sertifikata-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbOCSPRevocation;
            SetFocusOnRadioButton();
        }
    }

    private void CheckIfChannelHasChanged1()
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);

        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string txtserialnostring = txtserialno.ClientID;
        int PropertyValueMin = 0;
        int PropertyValueMax = 0;

        SetUpSerialNoVariables(page, txtserialnostring, out PropertyValueMin, out PropertyValueMax);

        try
        {
            string ErrorMessage1 = string.Empty;
            UtilsValidation.ValidateSerialNo(txtserialno.Text, Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-txtserialnoIsRequired"]), Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-TurnOnSerialNumberValidation"]), PropertyValueMin, PropertyValueMax, out ErrorMessage1);
            errLabelSerialNo.Text = ErrorMessage1;
        }
        catch (Exception ex)
        {
            log.Error("Error while controling Serial Number. " + ex.Message);
        }
    }

    public List<SerialNoVariable> SerialNoVariables;
    //-----------------SetUp SERIAL NO VAriables---------------------------
    //---------------------------------------------------------------
    protected void SetUpSerialNoVariables(string page, string txtserialnostring, out int PropertyValueMin, out int PropertyValueMax)
    {
        PropertyValueMin = 0;
        PropertyValueMax = 0;

        Utility utility = new Utility();

        string PropertyValueMinString = utility.getMinAndMaxSerialLength(Constants.MINLENSERIAL);
        string PropertyValueMaxString = utility.getMinAndMaxSerialLength(Constants.MAXLENSERIAL);
        //SerialNoVariables = utility.pronadjiPromenljiveSerialNo(page, txtserialnostring);

        PropertyValueMin = Convert.ToInt32(PropertyValueMinString);
        PropertyValueMax = Convert.ToInt32(PropertyValueMaxString);
        /*
        foreach (var serialnovariable in SerialNoVariables)
        {
            if (serialnovariable.PropertyName == Constants.REVOCATION_SERIAL_NO_PROPERTY_NAME_MIN)
            {
                PropertyValueMin = serialnovariable.PropertyValue;
            }
            else if (serialnovariable.PropertyName == Constants.REVOCATION_SERIAL_NO_PROPERTY_NAME_MAX)
            {
                PropertyValueMax = serialnovariable.PropertyValue;
            }
        }
        */
    }
    //---------------------------------------------------------------
    //---------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------


    protected void rbOCSPRevocation_CheckedChanged(object sender, EventArgs e)
    {
        if (rbAutomatikSerialNo.Checked)
        {
            if (Session["provera-opozvanosti-sertifikata-CertificateSerial"] != null)
            { 
                txtserialno.Text = Session["provera-opozvanosti-sertifikata-CertificateSerial"].ToString();
            }
        }
        Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"] = true;

        rbOCSPRevocation.Checked = true;
        rbCRLRevocation.Checked = false;
        Container04.Visible = false;
        Container05.Visible = false;

        string pageName = Path.GetFileName(Page.AppRelativeVirtualPath);
        //Insert parameters to objectdatasource
        ddlimeizdavaoca.Items.Clear();
        SetObjectDataSourceParameterRevocationCheckTypeOne();

        ddlimeizdavaoca.Enabled = Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"]);
        ddlimeizdavaoca.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlimeizdavaoca.CssClass = SetCss3;

        txtrevocationaddress.Text = string.Empty;

        Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbOCSPRevocation;
        SetFocusOnRadioButton();
    }

    protected void rbCRLRevocation_CheckedChanged(object sender, EventArgs e)
    {
        if (rbAutomatikSerialNo.Checked)
        {
            if (Session["provera-opozvanosti-sertifikata-CertificateSerial"] != null)
            {
                txtserialno.Text = Session["provera-opozvanosti-sertifikata-CertificateSerial"].ToString();
            }
        }
        Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"] = true;

        odsImeIzdavaoca.DataBind();

        rbOCSPRevocation.Checked = false;
        rbCRLRevocation.Checked = true;
        Container04.Visible = true;
        Container05.Visible = true;
        rbLDAP.Checked = true;
        try
        {
            //Insert parameters to objectdatasource
            ddlimeizdavaoca.Items.Clear();
            SetObjectDataSourceParameterRevocationCheckTypeTwo();
            /*
            Utility utility = new Utility();
            string page = Path.GetFileName(Page.AppRelativeVirtualPath);
            string ddlstring = ddlimeizdavaoca.ClientID;

            string LDAPurl = utility.getURLcrl(page, ddlstring, Constants.REVOCATION_CRL_LDAP_CHECK_TYPE);
            txtrevocationaddress.Text = LDAPurl;
            */
            txtrevocationaddress.Text = string.Empty;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        rbHTTP.Checked = false;

        ddlimeizdavaoca.Enabled = Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"]);
        ddlimeizdavaoca.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlimeizdavaoca.CssClass = SetCss3;
        
        string IDItem1 = string.Empty;
        SetUpDefaultItem(ddlimeizdavaoca.ClientID, out IDItem1);
        ddlimeizdavaoca.SelectedValue = IDItem1;
        
        cvimeizdavaoca.Enabled = true;

        Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbCRLRevocation;
        SetFocusOnRadioButton();
    }

    protected void rbLDAP_CheckedChanged(object sender, EventArgs e)
    {
        rbLDAP.Checked = true;
        rbHTTP.Checked = false;
        Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"] = true;
        ddlimeizdavaoca.Enabled = Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"]);
        ddlimeizdavaoca.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlimeizdavaoca.CssClass = SetCss3;
       
        try
        {
            //Insert parameters to objectdatasource
            ddlimeizdavaoca.Items.Clear();
            SetObjectDataSourceParameterRevocationCheckTypeTwo();            
            /*
            Utility utility = new Utility();
            string page = Path.GetFileName(Page.AppRelativeVirtualPath);
            string ddlstring = ddlimeizdavaoca.ClientID;

            string LDAPurl = utility.getURLcrl(page, ddlstring, Constants.REVOCATION_CRL_LDAP_CHECK_TYPE);
            txtrevocationaddress.Text = LDAPurl;
            */
            txtrevocationaddress.Text = string.Empty;
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbLDAP;
        SetFocusOnRadioButton();
    }

    protected void rbHTTP_CheckedChanged(object sender, EventArgs e)
    {
        rbLDAP.Checked = false;
        rbHTTP.Checked = true;
        Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"] = true;
        ddlimeizdavaoca.Enabled = Convert.ToBoolean(Session["provera-opozvanosti-sertifikata-ddlimeizdavaoca"]);
        ddlimeizdavaoca.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlimeizdavaoca.CssClass = SetCss3;
        try
        {
            //Insert parameters to objectdatasource
            ddlimeizdavaoca.Items.Clear();
            SetObjectDataSourceParameterRevocationCheckTypeThree();
            /*
            Utility utility = new Utility();
            string page = Path.GetFileName(Page.AppRelativeVirtualPath);
            string ddlstring = ddlimeizdavaoca.ClientID;

            string HTTPurl = utility.getURLcrl(page, ddlstring, Constants.REVOCATION_CRL_HTTP_CHECK_TYPE);
            txtrevocationaddress.Text = HTTPurl;
            */
            txtrevocationaddress.Text = string.Empty;
        }
        catch (Exception ex)
        {
            throw ex;
        }

        Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] = rbHTTP;
        SetFocusOnRadioButton();
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                if (rbAutomatikSerialNo.Checked == true)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "successalertOCSP", "successalertOCSP();", true);
                    if (true)
                    {
                        HideFirstPartPage();
                        CheckRevocationSubmit();
                    }
                }
                else if (rbManualSerialNo.Checked == true)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "successalertOCSP", "successalertOCSP();", true);
                    if (true)
                    {
                        HideFirstPartPage();
                        CheckRevocationSubmit();
                    }
                }

                //-----------------GetUserAgent string---------------------------
                Utility utility = new Utility();
                string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_GEOLOCATIONS);
                if (SettingValue == Constants.SETTING_VALUE_TRUE)
                {
                    try
                    {
                        GetUserAgentInformation(out userAgentBrowser, out userAgentStringApplicant, out userAgentOS, out userAgentIP);
                        log.Debug("GetUserAgentInformation function. userAgentBrowser is " + userAgentBrowser + ". userAgentStringApplicant is " + userAgentStringApplicant + ". userAgentOS is " + userAgentOS + ". userAgentIP is " + userAgentIP);
                        PisMessServiceReference.IpGeolocationData ipGeolocationData = null;
                        try
                        {
                            log.Info("Start getting ipGeolocationData. ");
                            PisMessServiceReference.PisMessServiceClient client = new PisMessServiceReference.PisMessServiceClient();
                            ipGeolocationData = client.SendIpGeolocationRequest(userAgentIP);
                            log.Info("End getting ipGeolocationData. ");
                        }
                        catch (Exception ex)
                        {
                            log.Error("IP address is not in correct format or it is empty. IP is: " + userAgentIP + " - " + ex.Message);
                        }
                        log.Debug("ipGeolocationData is: Status - " + ipGeolocationData.Status + " ,Country - " + ipGeolocationData.Country + " ,City - " + ipGeolocationData.City + " ,Isp - " + ipGeolocationData.Isp + " ,Continent - " + ipGeolocationData.Continent + " ,CountryCode - " + ipGeolocationData.CountryCode);
                        Utils.CheckIPGeolocationData(ipGeolocationData.Status, userAgentIP, ipGeolocationData.Country, ipGeolocationData.CountryCode, ipGeolocationData.City, ipGeolocationData.Isp, ipGeolocationData.Continent, out userAgentCountry, out userAgentCountryCode, out userAgentCity, out userAgentISP, out userAgentContinent);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error while getting ipGeolocationData. " + ex.Message);
                        Utils.userDataEmpty(userAgentCountry, userAgentCountryCode, userAgentCity, userAgentISP, userAgentContinent);
                    }
                }
                else
                {
                    log.Info("Geolocation is not active!");
                }
                Session["zahtev-promena-statusa-sertifikata-userAgentBrowser"] = userAgentBrowser;
                Session["zahtev-promena-statusa-sertifikata-userAgentStringApplicant"] = userAgentStringApplicant;
                Session["zahtev-promena-statusa-sertifikata-userAgentOS"] = userAgentOS;
                Session["zahtev-promena-statusa-sertifikata-userAgentIP"] = userAgentIP;
                Session["zahtev-promena-statusa-sertifikata-userAgentContinent"] = userAgentContinent;
                Session["zahtev-promena-statusa-sertifikata-userAgentCountry"] = userAgentCountry;
                Session["zahtev-promena-statusa-sertifikata-userAgentCountryCode"] = userAgentCountryCode;
                Session["zahtev-promena-statusa-sertifikata-userAgentCity"] = userAgentCity;
                Session["zahtev-promena-statusa-sertifikata-userAgentISP"] = userAgentISP;
            }
            else if (!Page.IsValid)
            {
                if (rbAutomatikSerialNo.Checked)
                {
                    if (Session["provera-opozvanosti-sertifikata-CertificateJIK"] != null)
                    {
                        string jik = Session["provera-opozvanosti-sertifikata-CertificateJIK"].ToString();
                        if (jik != string.Empty)
                        {
                            txtserialno.Text = Session["provera-opozvanosti-sertifikata-CertificateSerial"].ToString();
                        }
                    }
                    else
                    {
                        errLabelSerialNo.Text = string.Empty;
                    }
                }
                else
                {
                    errLabelSerialNo.Text = string.Empty;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
                if (rbAutomatikSerialNo.Checked == true)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralertOCSP", "erroralertOCSP();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "DisableButtonRevocation", "DisableButtonRevocation();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "DisableButtonOCSP", "DisableButtonOCSP();", true);
            ClearTextBoxes();
            log.Error("Error while trying to communicate with server. " + ex.Message + "; " + ex.InnerException);           
        }        
    }

    protected void CheckRevocationSubmit()
    {
        try
        {
            log.Info("Start checking revocation status.");
            PisMessServiceReference.PisMessServiceClient pisMess = new PisMessServiceReference.PisMessServiceClient();
            PisMessServiceReference.RevocationResponse response = new PisMessServiceReference.RevocationResponse();
            Utility utility = new Utility();
            
            int SelectedValue = Convert.ToInt32(ddlimeizdavaoca.SelectedValue);
            string loadCertificateName = utility.getCertificateName(SelectedValue);                       
            string loadCertificateRoot = utility.getCertificateRoot(Constants.CERTFOLDER);
            string FinalCertificatePath = loadCertificateRoot + loadCertificateName;
            log.Info("FinalCertificatePath for sending to PisMess is: " + FinalCertificatePath);

            response = GetResponse(response, pisMess, FinalCertificatePath);

            if (response.Result != "0")
            {
                throw new Exception("response.Result from PisMess is: " + response.Result);
            }

            string returnCertificateRoot = response.ServerCertificate;
            Session["provera-opozvanosti-sertifikata-filenamePathCER"] = returnCertificateRoot;

            txtserijskibrojsert.Text = response.CertificateSerialNumberHexaDecimal + " (heksadecimalno) ili " + response.CertificateSerialNumberDecimal + " (dekadno) ";

            txtstatussert.Text = GetStatusSrb((response.CertificateStatus).ToString());
            StatusSertColor();

            txtrazlogopozivasert.Text = response.RevocationReason;

            //date.ToString("HH:mm:ss"); // for 24hr format
            //date.ToString("hh:mm:ss"); // for 12hr format, it shows AM/PM
            string FormatDateTime = "dd.MM.yyyy HH:mm:ss";

            //Datum i Vreme Opoziva Sert
            txtdatumvremeopozivasert.Text = GetRevocationDate(response, FormatDateTime);

            //Datum i Vreme Kompromitovanja Sert
            txtdatumvremekompromitovanjasert.Text = GetCompromiseDate(response, FormatDateTime);

            txtimeizdavaoca.Text = response.IssuerName;
            txtimeservera.Text = response.ServerName;

            txtnacinprovere.Text = GetRevocationMethod(response);

            txtcheckingurl.Text = response.UrlForChecking;
            //Datum i Vreme Sprovedene Provere
            txtdatumvremesprovedeneprovere.Text = GetConductedChecksDate(response, FormatDateTime);

            //putanja do fajla koji treba da se download-uje na dugme
            Session["provera-opozvanosti-sertifikata-filenamePath"] = response.ResponseSource;

            string compromiseDateString = "";
            if (response.CompromiseDate != null)
            {
                compromiseDateString = ((DateTime)(response.CompromiseDate)).ToString(FormatDateTime);
            }

            string revocationDateString = "";
            if (response.RevocationDate != null)
            {
                revocationDateString = ((DateTime)(response.RevocationDate)).ToString(FormatDateTime);
            }

            log.Info("RESPONSE RESULT: HEX - " + response.CertificateSerialNumberHexaDecimal + "; " + " DEC - " + response.CertificateSerialNumberDecimal + "; " + " STATUS - " + response.CertificateStatus + "; " + " REASON - " + response.RevocationReason + "; " + " CheckingDate - " + response.CheckingDate.ToString(FormatDateTime) + "; " + " CompromiseDate - " + compromiseDateString + "; " + " RevocationDate - " + revocationDateString + "; " + " ISSUER NAME - " + response.IssuerName + "; " + " SERVER - " + response.ServerName + "; " + " RevocationMethod - " + response.RevocationMethod + "; " + " RESPONSE SOURCE - " + response.ResponseSource + "; " + " RESPONSE URL - " + response.UrlForChecking + "; ");

            log.Info("Finished checking revocation status. ");
        }
        catch (Exception ex)
        {
            throw new Exception("Error in CheckRevocationSubmit function. " + ex.Message);
        }
    }

    public static Org.BouncyCastle.X509.X509Certificate LoadCertificate(string filename)
    {
        X509CertificateParser certParser = new X509CertificateParser();
        FileStream fs = new FileStream(filename, FileMode.Open);
        Org.BouncyCastle.X509.X509Certificate cert = certParser.ReadCertificate(fs);
        fs.Close();

        return cert;
    }

    public string GetConductedChecksDate(PisMessServiceReference.RevocationResponse response, string FormatDateTime)
    {
        string datumvremesprovedeneprovere = string.Empty;

        try
        {
            //Datum i Vreme Sprovedene Provere
            string datumvremeopozivasertUTC = (response.CheckingDate).ToString(FormatDateTime);
            DateTime invalidityDateSerbianCheckingDate = getLocalTime(response.CheckingDate);
            string datumvremeopozivasertSRB = invalidityDateSerbianCheckingDate.ToString(FormatDateTime);
            datumvremesprovedeneprovere = datumvremeopozivasertSRB + " (" + datumvremeopozivasertUTC + " UTC) ";
            log.Info("Function GetConductedChecksDate. 24h time is: UTC - " + datumvremeopozivasertUTC + " Local - "+ datumvremeopozivasertSRB + " . Format datetime is: " + FormatDateTime);
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetConductedChecksDate. " + ex.Message);
            throw new Exception("Error in function GetConductedChecksDate. " + ex.Message);
        }

        return datumvremesprovedeneprovere;
    }

    public string GetRevocationDate(PisMessServiceReference.RevocationResponse response, string FormatDateTime)
    {
        string datumvremeopozivasert = string.Empty;

        try
        {
            //Datum i Vreme Opoziva Sert
            if (response.RevocationDate == null)
            {
                datumvremeopozivasert = string.Empty;
            }
            else
            {
                DateTime invalidityDateSerbianRD = (DateTime)response.RevocationDate;
                string datumvremesprovedeneprovereUTC = (invalidityDateSerbianRD).ToString(FormatDateTime);
                DateTime invalidityDateSerbianRevocationDate = getLocalTime(invalidityDateSerbianRD);
                string datumvremesprovedeneprovereSRB = invalidityDateSerbianRevocationDate.ToString(FormatDateTime);
                datumvremeopozivasert = datumvremesprovedeneprovereSRB + " (" + datumvremesprovedeneprovereUTC + " UTC) ";
                log.Info("Function GetRevocationDate. 24h time is: UTC - " + datumvremesprovedeneprovereUTC + " Local - " + datumvremesprovedeneprovereSRB + " . Format datetime is: " + FormatDateTime);
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetRevocationDate. " + ex.Message);
            throw new Exception("Error in function GetRevocationDate. " + ex.Message);
        }

        return datumvremeopozivasert;
    }

    public string GetCompromiseDate(PisMessServiceReference.RevocationResponse response, string FormatDateTime)
    {
        string datumvremekompromitovanjasert = string.Empty;

        try
        {
            //Datum i Vreme Opoziva Sert
            if (response.CompromiseDate == null)
            {
                datumvremekompromitovanjasert = string.Empty;
            }
            else
            {
                DateTime invalidityDateSerbianC = (DateTime)response.CompromiseDate;
                string datumvremekompromitovanjasertUTC = (invalidityDateSerbianC).ToString(FormatDateTime);
                DateTime invalidityDateSerbianCompromise = getLocalTime(invalidityDateSerbianC);
                string datumvremekompromitovanjasertSRB = invalidityDateSerbianCompromise.ToString(FormatDateTime);
                datumvremekompromitovanjasert = datumvremekompromitovanjasertSRB + " (" + datumvremekompromitovanjasertUTC + " UTC) ";
                log.Info("Function GetCompromiseDate. 24h time is: UTC - " + datumvremekompromitovanjasertUTC + " Local - " + datumvremekompromitovanjasertSRB + " . Format datetime is: " + FormatDateTime);
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetCompromiseDate. " + ex.Message);
            throw new Exception("Error in function GetCompromiseDate. " + ex.Message);
        }

        return datumvremekompromitovanjasert;
    }

    public string GetRevocationMethod(PisMessServiceReference.RevocationResponse response)
    {
        string revocationmethod = string.Empty;

        try
        {
            Utility utility = new Utility();
            revocationmethod = utility.getRevocationMethod((response.RevocationMethod).ToString());
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetRevocationMethod. " + ex.Message);
            throw new Exception("Error in function GetRevocationMethod. " + ex.Message);
        }

        return revocationmethod;
    }

    public PisMessServiceReference.RevocationResponse GetResponse(PisMessServiceReference.RevocationResponse response, PisMessServiceReference.PisMessServiceClient pisMess, string loadCertificateRoot)
    {
        try
        {
            if (rbOCSPRevocation.Checked == true)
            {
                response = pisMess.CheckRevocationStatus(txtserialno.Text, PisMessServiceReference.RevocationMethodEnum.Ocsp, txtrevocationaddress.Text, loadCertificateRoot);
            }
            else if (rbHTTP.Checked == true)
            {
                response = pisMess.CheckRevocationStatus(txtserialno.Text, PisMessServiceReference.RevocationMethodEnum.CrlHttp, txtrevocationaddress.Text, loadCertificateRoot);
            }
            else if (rbLDAP.Checked == true)
            {
                response = pisMess.CheckRevocationStatus(txtserialno.Text, PisMessServiceReference.RevocationMethodEnum.CrlLdap, txtrevocationaddress.Text, loadCertificateRoot);
            }            
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetResponse. " + ex.Message);
            throw new Exception("Error in function GetResponse. " + ex.Message);
        }

        return response;
    }

    public void StatusSertColor()
    {
        if (txtstatussert.Text == Constants.REVOCATION_CERITIFATE_STATUS_REVOKE_MESSAGE)
        {
            txtstatussert.ForeColor = Color.Red;
            txtstatussert.Font.Bold = true;
        }
        else if (txtstatussert.Text == Constants.REVOCATION_CERITIFATE_STATUS_UNREVOKE_MESSAGE)
        {
            txtstatussert.ForeColor = Color.Green;
            txtstatussert.Font.Bold = true;
        }
        else if (txtstatussert.Text == Constants.REVOCATION_CERITIFATE_STATUS_OTHER_MESSAGE)
        {
            txtstatussert.ForeColor = Color.DeepPink;
            txtstatussert.Font.Bold = true;
        }
    }

    public DateTime getLocalTime(DateTime responseDateTime)
    {
        DateTime invalidityDateSerbian = responseDateTime.ToLocalTime();
        return invalidityDateSerbian;
    }    

    public string GetStatusSrb(string responsestatus)
    {
        string getstatussrb = string.Empty;

        if (responsestatus == Constants.REVOCATION_CERITIFATE_STATUS_REVOKE)
        {
            getstatussrb = Constants.REVOCATION_CERITIFATE_STATUS_REVOKE_MESSAGE;
        }
        else if (responsestatus == Constants.REVOCATION_CERITIFATE_STATUS_UNREVOKE)
        {
            getstatussrb = Constants.REVOCATION_CERITIFATE_STATUS_UNREVOKE_MESSAGE;
        }
        else if (responsestatus == Constants.REVOCATION_CERITIFATE_STATUS_OTHER)
        {
            getstatussrb = Constants.REVOCATION_CERITIFATE_STATUS_OTHER_MESSAGE;
        }

        return getstatussrb;
    }

    protected void HideFirstPartPage()
    {
        Container00.Visible = false;
        Container01.Visible = false;
        Container02.Visible = false;
        Container06.Visible = false;
        Container03.Visible = false;
        Container08.Visible = false;
        Container09.Visible = false;
        Container10.Visible = false;
        Container11.Visible = false;
        Container12.Visible = false;
        myDiv5.Visible = false;
        Container04.Visible = false;
        Container05.Visible = false;

        Container07.Visible = true;
        txtserijskibrojsert.ReadOnly = true;
        txtserijskibrojsert.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtstatussert.ReadOnly = true;
        txtstatussert.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtrazlogopozivasert.ReadOnly = true;
        txtrazlogopozivasert.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtdatumvremeopozivasert.ReadOnly = true;
        txtdatumvremeopozivasert.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtdatumvremekompromitovanjasert.ReadOnly = true;
        txtdatumvremekompromitovanjasert.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtimeizdavaoca.ReadOnly = true;
        txtimeizdavaoca.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtimeservera.ReadOnly = true;
        txtimeizdavaoca.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtcheckingurl.ReadOnly = true;
        txtnacinprovere.ReadOnly = true;
        txtnacinprovere.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtdatumvremesprovedeneprovere.ReadOnly = true;
        txtdatumvremesprovedeneprovere.BackColor = ColorTranslator.FromHtml(SetLightGray);
    }

    protected void UnhideFirstPartPage()
    {
        Container00.Visible = true;
        Container01.Visible = true;
        Container02.Visible = true;
        Container06.Visible = true;
        Container03.Visible = true;
        Container08.Visible = true;
        Container09.Visible = true;
        Container10.Visible = true;
        Container11.Visible = true;
        Container12.Visible = true;

        myDiv5.Visible = true;
        Container04.Visible = false;
        Container05.Visible = false;

        Container07.Visible = false;

        if (rbAutomatikSerialNo.Checked)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
        }
        else
        {
            if (Session["provera-opozvanosti-sertifikata-CertificateJIK"] != null)
            {
                string jik = Session["provera-opozvanosti-sertifikata-CertificateJIK"].ToString();
                if (jik != string.Empty)
                {
                    if (rbManualSerialNo.Checked)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                    }
                    else
                    { 
                        ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                }
            }
            else
            { 
                ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
            }
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "EnableSubmitButton", "EnableSubmitButton();", true);
        ScriptManager.RegisterStartupScript(this, GetType(), "EnableButtonRevocation", "DisableButtonRevocation();", true);
        ScriptManager.RegisterStartupScript(this, GetType(), "EnableButtonOCSP", "DisableButtonOCSP();", true);
    }


    protected void btnQuit_Click3(object sender, EventArgs e)
    {
        string pageName = Path.GetFileName(Page.AppRelativeVirtualPath);
        Response.Redirect(pageName, false);
    }

    protected void btnGetRevocationCertificate_Click(object sender, EventArgs e)
    {
        try
        {
            //String FileName = "CARoot.cer";
            string FileName = @"attachment; filename=""" + Path.GetFileName(Session["provera-opozvanosti-sertifikata-filenamePathCER"].ToString()) + "";
            //String FilePath = @"C:\CertificateUpload\"; 
            string FilePath = Session["provera-opozvanosti-sertifikata-filenamePathCER"].ToString();
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.Buffer = true;
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition", FileName); //forse save as dialog in Mozzila an Explorer but no in Chrome
            //response.TransmitFile(Server.MapPath(@"~\Dokumentacija\CertificateUpload\" + FileName + ""));
            response.TransmitFile(FilePath);
            response.Flush();
        }
        catch (Exception ex)
        {
            log.Error("Error while saving .cer revocation certificate file. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "errorDownloadingCER", "errorDownloadingCER;", true);
        }
        finally
        {
            HttpContext.Current.Response.End();
        }
    }

    protected void btnGetRevocationOCSP_Click(object sender, EventArgs e)
    {
        try
        {
            //String FileName = "CARoot.cer";
            string FileName = @"attachment; filename=""" + Path.GetFileName(Session["provera-opozvanosti-sertifikata-filenamePath"].ToString()) + "";
            //String FilePath = @"D:\Cert"; 
            string FilePath = Session["provera-opozvanosti-sertifikata-filenamePath"].ToString();
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.Buffer = true;
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition", FileName); //forse save as dialog in Mozzila an Explorer but no in Chrome
            //response.TransmitFile(Server.MapPath(@"~\Dokumentacija\RevocationResponseFiles\" + FileName + ""));
            response.TransmitFile(FilePath);
            response.Flush();
            response.End();
        }
        catch (Exception ex)
        {
            log.Error("Error while saving .ocr or .crl file. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "errorDownloadingOCR", "errorDownloadingOCR;", true);
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
            if (Session["provera-opozvanosti-sertifikata-event_controle"] != null)
            {
                TextBox controle = (TextBox)Session["provera-opozvanosti-sertifikata-event_controle"];
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
            if (Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"] != null)
            {
                RadioButton radiodugme = (RadioButton)Session["provera-opozvanosti-sertifikata-event_controle-RadioButton"];
                //radiodugme.Focus();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "foc", "document.getElementById('" + radiodugme.ClientID + "').focus();", true);
            }
        }
        catch (InvalidCastException inEx)
        {
            log.Error("Problem with setting focus on control. Error: " + inEx);
        }
    }

    protected void SetObjectDataSourceParameterRevocationCheckTypeOne()
    {
        //odsImeIzdavaoca.InsertParameters["filename"]. = pageName;
        //odsImeIzdavaoca.InsertParameters["controlid"].DefaultValue = ddlimeizdavaoca.ClientID;
        //odsImeIzdavaoca.InsertParameters["RevocationCheckType"].DefaultValue = "1";
        //odsImeIzdavaoca.Insert();
        string pageName = Path.GetFileName(Page.AppRelativeVirtualPath);
        Session["provera-opozvanosti-sertifikata-filename"] = pageName;
        Session["provera-opozvanosti-sertifikata-controlid"] = ddlimeizdavaoca.ClientID;
        Session["provera-opozvanosti-sertifikata-revocationchecktype"] = 1;
        odsImeIzdavaoca.DataBind();
    }

    protected void SetObjectDataSourceParameterRevocationCheckTypeTwo()
    {
        //odsImeIzdavaoca.InsertParameters["filename"]. = pageName;
        //odsImeIzdavaoca.InsertParameters["controlid"].DefaultValue = ddlimeizdavaoca.ClientID;
        //odsImeIzdavaoca.InsertParameters["RevocationCheckType"].DefaultValue = "2";
        //odsImeIzdavaoca.Insert();
        string pageName = Path.GetFileName(Page.AppRelativeVirtualPath);
        Session["provera-opozvanosti-sertifikata-filename"] = pageName;
        Session["provera-opozvanosti-sertifikata-controlid"] = ddlimeizdavaoca.ClientID;
        Session["provera-opozvanosti-sertifikata-revocationchecktype"] = 2;
        odsImeIzdavaoca.DataBind();
    }

    protected void SetObjectDataSourceParameterRevocationCheckTypeThree()
    {
        //odsImeIzdavaoca.InsertParameters["filename"]. = pageName;
        //odsImeIzdavaoca.InsertParameters["controlid"].DefaultValue = ddlimeizdavaoca.ClientID;
        //odsImeIzdavaoca.InsertParameters["RevocationCheckType"].DefaultValue = "3";
        //odsImeIzdavaoca.Insert();
        string pageName = Path.GetFileName(Page.AppRelativeVirtualPath);
        Session["provera-opozvanosti-sertifikata-filename"] = pageName;
        Session["provera-opozvanosti-sertifikata-controlid"] = ddlimeizdavaoca.ClientID;
        Session["provera-opozvanosti-sertifikata-revocationchecktype"] = 3;
        odsImeIzdavaoca.DataBind();
    }

    protected void ClearTextBoxes()
    {
        txtserijskibrojsert.Text = string.Empty;
        txtstatussert.Text = string.Empty;
        txtrazlogopozivasert.Text = string.Empty;
        txtdatumvremeopozivasert.Text = string.Empty;
        txtdatumvremekompromitovanjasert.Text = string.Empty;
        txtimeizdavaoca.Text = string.Empty;
        txtimeservera.Text = string.Empty;
        txtcheckingurl.Text = string.Empty;
        txtnacinprovere.Text = string.Empty;
        txtdatumvremesprovedeneprovere.Text = string.Empty;
    }
}