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
using System.Data;
using System.Threading.Tasks;

public partial class zahtev_promena_statusa : System.Web.UI.Page
{
    public string SetDarkGray = Constants.SetDarkGray;
    public string SetLightGray = Constants.SetLightGray;
    public string SetWhite = Constants.SetWhite;
    public string SetCss1 = Constants.SetCss1;
    public string SetCss2 = Constants.SetCss2;
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
    public string isLegalEntity = string.Empty;
    public string legalEntityName = string.Empty;
    public string radioButtonStatus = string.Empty;
    public string imeStatus = string.Empty;
    public string prezimeStatus = string.Empty;
    public string usiStatus = string.Empty;
    public PisMessServiceReference.PisMessServiceClient pisMess;

    public List<ItemVariable> Items;

    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public List<WebControl> Controls;
    //promenljive za validaciju 
    public bool TurnOnJMBGValidation;
    public bool TurnOnEmailValidation;
    public bool TurnOnPhoneValidation;
    public bool TurnOnPIBValidation;
    public bool TurnOnRegistrationNumberValidation;

    public List<StatusChangeVariable> StatusChangeVariables;
    public bool IsAllowedVariable;
    public bool IsDefaultVariable;
    public int ItemValue;
    public int IDItem;

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
            Session["zahtev-promena-statusa-sertifikata-JIK01-string.empty"] = false;
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
                Session["zahtev-promena-statusa-sertifikata-CertificateJIK"] = string.Empty;
                Session["zahtev-promena-statusa-sertifikata-CertificateFirstName"] = string.Empty;
                Session["zahtev-promena-statusa-sertifikata-CertificateLastName"] = string.Empty;
            }
            AvoidCashing();
            ShowDatepicker();

            if (!Page.IsPostBack)
            {
                RadioButtonsDropdownListsTrue();

                if (Session["zahtev-promena-statusa-sertifikata-CertificateJIK"] != null)
                {
                    string jik = Session["zahtev-promena-statusa-sertifikata-CertificateJIK"].ToString();
                    if (jik != string.Empty)
                    {
                        txtjik01.Text = jik;
                        txtjik01.BackColor = ColorTranslator.FromHtml(SetLightGray);
                        txtjik01.ReadOnly = true;
                        txtime02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateFirstName"].ToString();
                        txtprezime02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateLastName"].ToString();
                        txtserijskibroj02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateCN"].ToString();
                        FieldsToDisplay();
                        ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCkeckAutomatik", "RadioButtonCkeckAutomatik();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCheck2", "RadioButtonCheck2();", true);
                    }
                    else
                    {
                        if (error != null)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorNotification", "ErrorNotification();", true);
                            log.Error("error is not null. error: " + error);
                        }
                        FieldsToDisplay();
                        ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCheck1", "RadioButtonCheck1();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCheck2", "RadioButtonCheck2();", true);
                    }
                }
                else
                {
                    FieldsToDisplay();
                    ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCheck1", "RadioButtonCheck1();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "RadioButtonCheck2", "RadioButtonCheck2();", true);
                }
                //-------TABINDEX---------------
                if (Session["zahtev-promena-statusa-sertifikata-CertificateJIK"] != null)
                {
                    string jik = Session["zahtev-promena-statusa-sertifikata-CertificateJIK"].ToString();
                    if (jik != string.Empty)
                    {
                        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbAutomatikJik;
                    }
                    else
                    {
                        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbUnknownJik;
                    }
                }
                else
                {
                    Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbUnknownJik;
                }
                SetFocusOnRadioButton();
                txtjik01.TabIndex = 0;
                txtime02.TabIndex = -1;
                txtserijskibroj02.TabIndex = -1;
                txtprezime02.TabIndex = -1;               
                //------------------------------
                //Get Control on all page
                SetUpValidation();
                log.Debug("successfully set Validation!");
                SetUpIsRequiredTextBoxes();
                log.Debug("successfully set RequiredTextBoxes!");
                SetUpIsRequiredDropDownLists();
                log.Debug("successfully set RequiredDropDownLists!...Application Starting, successfully get all controls!");
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

    protected void RadioButtonsDropdownListsTrue()
    {
        Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbManualJIK"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbAutomatikJIK"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbUnknownJIK"] = true;
        Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbIndividual"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbLegal"] = true;

        Session["zahtev-promena-statusa-sertifikata-rblosstoken"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbcompromise"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbchangedata"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbcessation"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbcessationofneed"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbother"] = true;
    }

    protected void RadioButtonsDropdownListsFalse()
    {
        Session["zahtev-promena-statusa-sertifikata-rbManualJIK"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbAutomatikJIK"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbUnknownJIK"] = false;
        Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"] = false;
        Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbIndividual"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbLegal"] = false;

        Session["zahtev-promena-statusa-sertifikata-rblosstoken"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbcompromise"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbchangedata"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbcessation"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbcessationofneed"] = false;
        Session["zahtev-promena-statusa-sertifikata-rbother"] = false;
    }

    protected void FieldsToDisplay()
    {
        Container01.Visible = false;
        Container04.Visible = true;
        txtime02.ReadOnly = true;
        txtime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtserijskibroj02.ReadOnly = true;
        txtprezime02.ReadOnly = true;
        txtprezime02.BackColor = ColorTranslator.FromHtml(SetLightGray);        
        myDiv1.Visible = false;
        myDiv5.Visible = true;
        myDiv6.Visible = false;
        Container07.Visible = false;
        Container10.Visible = true;
        Container08.Visible = false;
        Container09.Visible = false;
        Container11.Visible = false;
        Container12.Visible = false;
        Container13.Visible = false;
        rowLegalEntityDDL.Visible = false;
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
            //if(!data.Equals(string.Empty))
            //{ 
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
                        Session["zahtev-promena-statusa-sertifikata-CertificateJIK"] = par.Value;
                    }
                    else if (par.Key.Equals("cn", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["zahtev-promena-statusa-sertifikata-CertificateCN"] = par.Value;
                    }
                    else if (par.Key.Equals("firstName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["zahtev-promena-statusa-sertifikata-CertificateFirstName"] = par.Value;
                    }
                    else if (par.Key.Equals("lastName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Session["zahtev-promena-statusa-sertifikata-CertificateLastName"] = par.Value;
                    }
                }
            //}
            
        }
        catch (Exception ex)
        {
            log.Debug("Error in function GetCertificateData. " + ex.Message);
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
        rbUnknownJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbUnknownJIK"]);
        rbIndividual.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbIndividual"]);
        rbLegal.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbLegal"]);               
        ddlnacinpromene.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"]);
        ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"]);
        rblosstoken.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rblosstoken"]);
        rbcompromise.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcompromise"]);
        rbchangedata.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbchangedata"]);
        rbcessation.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcessation"]);
        rbcessationofneed.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcessationofneed"]);
        rbother.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbother"]);
        rbManualJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbManualJIK"]);
        rbAutomatikJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbAutomatikJIK"]);
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
                if (control.Id == txtjmbg.ClientID)
                {
                    TurnOnJMBGValidation = control.ControlStatus;
                    Session["zahtev-promena-statusa-sertifikata-TurnOnJMBGValidation"] = TurnOnJMBGValidation;
                }
                else if (control.Id == txtadresaeposte.ClientID)
                {
                    TurnOnEmailValidation = control.ControlStatus;
                    Session["zahtev-promena-statusa-sertifikata-TurnOnEmailValidation"] = TurnOnEmailValidation;
                }
                else if (control.Id == txttelefon.ClientID)
                {
                    TurnOnPhoneValidation = control.ControlStatus;
                    Session["zahtev-promena-statusa-sertifikata-TurnOnPhoneValidation"] = TurnOnPhoneValidation;
                }
                else if (control.Id == txtpib.ClientID)
                {
                    TurnOnPIBValidation = control.ControlStatus;
                    Session["zahtev-promena-statusa-sertifikata-TurnOnPIBValidation"] = TurnOnPIBValidation;
                }
                else if (control.Id == txtmaticnibroj.ClientID)
                {
                    TurnOnRegistrationNumberValidation = control.ControlStatus;
                    Session["zahtev-promena-statusa-sertifikata-TurnOnRegistrationNumberValidation"] = TurnOnRegistrationNumberValidation;
                }
                else if (control.Id == txtime.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtprezime.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnSurnameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtimezz.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnNameZZValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtprezimezz.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnSurnameZZValidation"] = control.ControlStatus;
                }               
                else if (control.Id == txtostalo.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnTheRestValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtdrugo.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnOtherValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtnazivpravnoglica.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnLegalEntityNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtjik01.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnJIKValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtjik.ClientID)
                {
                    Session["zahtev-promena-statusa-sertifikata-TurnOnSecondJIKValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            TurnOnJMBGValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnJMBGValidation"] = TurnOnJMBGValidation;
            TurnOnEmailValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnEmailValidation"] = TurnOnEmailValidation;
            TurnOnPhoneValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnPhoneValidation"] = TurnOnPhoneValidation;
            TurnOnPIBValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnPIBValidation"] = TurnOnPIBValidation;        
            TurnOnRegistrationNumberValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnRegistrationNumberValidation"] = TurnOnRegistrationNumberValidation;
            //-------------------------------------------------------
            Session["zahtev-promena-statusa-sertifikata-TurnOnNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnSurnameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnNameZZValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnSurnameZZValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnWriteNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnTheRestValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnOtherValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnLegalEntityNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnJIKValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-promena-statusa-sertifikata-TurnOnSecondJIKValidation"] = Constants.VALIDATION_FALSE;
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
            if (control.Id == txtjik01.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtjik01IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtserijskibroj02.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtserijskibroj02IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtime02.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtime02IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezime02.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtprezime02IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtjik.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtjikIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtime.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezime.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtprezimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtnazivpravnoglica.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtnazivpravnoglicaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtimezz.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtimezzIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezimezz.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtprezimezzIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumgubitka.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtdatumgubitkaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumcompromise.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtdatumcompromiseIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdrugo.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtdrugoIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtostalo.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtostaloIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtjmbg.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtjmbgIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtadresaeposte.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtadresaeposteIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txttelefon.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txttelefonIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpib.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtpibIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtmaticnibroj.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-txtmaticnibrojIsRequired"] = control.IsRequired;
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
            if (control.Id == ddlLegalEntityName.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityNameIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlnacinpromene.ClientID)
            {
                Session["zahtev-promena-statusa-sertifikata-ddlnacinpromeneIsRequired"] = control.IsRequired;
            }
        }
    }

    //---------------------------------------------------------------
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    //---------------------------------------------------------------

    protected void ShowDatepicker()
    {
        //call function pickdate() every time after PostBack in ASP.Net
        ScriptManager.RegisterStartupScript(this, GetType(), "pickdate", "pickdate();", true);
        //Avoid: jQuery DatePicker TextBox selected value Lost after PostBack in ASP.Net
        txtdatumgubitka.Text = Request.Form[txtdatumgubitka.UniqueID];
        txtdatumcompromise.Text = Request.Form[txtdatumcompromise.UniqueID];
    }

    protected void btnReadCardInfo_Click(object sender, EventArgs e)
    {
        cvjik01.Enabled = false;

        ScriptManager.RegisterStartupScript(this, GetType(), "ExplorerLogout", "ExplorerLogout();", true);
        ScriptManager.RegisterStartupScript(this, GetType(), "ChromeLogout", "ChromeLogout();", true);

        string ReturnURLChangeStatus = System.Configuration.ConfigurationManager.AppSettings["ReturnURLChangeStatus"].ToString();
        string parameters = @"returnUrl=" + ReturnURLChangeStatus; //type moze da bude jik ili serial
        string encodedString = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt(parameters, Constants.CryptKey, Constants.AuthKey);
        // znak plus pravi problem kada se posalje u url-u, pa mora da se svuda zameni sa "%252b"
        encodedString = encodedString.Replace("+", "%252b");
        string ClientSslAuthenticationURL = System.Configuration.ConfigurationManager.AppSettings["ClientSslAuthenticationURL"].ToString();
        log.Debug("URL kojim se poziva aplikacija za očitavanje sertifikata: " + @ClientSslAuthenticationURL + encodedString);
        Response.Redirect(@ClientSslAuthenticationURL + encodedString);
    }

    
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                RadioButtonsDropdownListsFalse();

                rbUnknownJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbUnknownJIK"]);
                rbIndividual.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbIndividual"]);
                rbLegal.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbLegal"]);
                ddlnacinpromene.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"]);
                ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"]);
                rblosstoken.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rblosstoken"]);
                rbcompromise.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcompromise"]);
                rbchangedata.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbchangedata"]);
                rbcessation.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcessation"]);
                rbcessationofneed.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcessationofneed"]);
                rbother.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbother"]);
                rbManualJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbManualJIK"]);
                rbAutomatikJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbAutomatikJIK"]);
                
                if (rbUnknownJik.Checked == true)
                {                   
                    ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "successalert();", true);
                    if (true)
                    {
                        Container00.Visible = true;
                        Container01.Visible = false;
                        txtjik.ReadOnly = true;
                        txtjik.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txtime.ReadOnly = true;
                        txtime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txtprezime.ReadOnly = true;
                        txtprezime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txtjmbg.ReadOnly = true;
                        txtjmbg.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txtadresaeposte.ReadOnly = true;
                        txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txttelefon.ReadOnly = true;
                        txttelefon.BackColor = ColorTranslator.FromHtml(SetDarkGray);

                        SetDarkAndReadonlyTextBoxes();

                        Session["zahtev-promena-statusa-sertifikata-datumgubitka"] = txtdatumgubitka.Text;
                        Session["zahtev-promena-statusa-sertifikata-datumcompromise"] = txtdatumcompromise.Text;
                        //Disable datepicker
                        ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);

                        myDiv1.Visible = true;
                        myDiv6.Visible = true;
                        myDiv5.Visible = false;
                        //
                        cvjik01.Enabled = true;
                        Session["zahtev-promena-statusa-sertifikata-brojzahteva"] = BrojZahteva;
                        txtdatumzahteva.Text = DateTime.Now.ToString("dd.MM.yyyy");
                        Session["zahtev-promena-statusa-sertifikata-datumzahteva"] = txtdatumzahteva.Text;
                    }
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                }
                else if (rbAutomatikJik.Checked == true)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "successalert();", true);
                    if (true)
                    {            
                        
                        Container00.Visible = true;
                        Container01.Visible = true;
                        txtjik01.ReadOnly = true;
                        txtjik01.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txtime02.ReadOnly = true;
                        txtime02.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txtprezime02.ReadOnly = true;
                        txtprezime02.BackColor = ColorTranslator.FromHtml(SetDarkGray);

                        SetDarkAndReadonlyTextBoxes();

                        myDiv1.Visible = true;
                        myDiv6.Visible = true;
                        myDiv5.Visible = false;
                        //Disable datepicker
                        ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);

                        cvjik01.Enabled = false;
                        txtdatumzahteva.Text = DateTime.Now.ToString("dd.MM.yyyy");
                        Session["zahtev-promena-statusa-sertifikata-datumzahteva"] = txtdatumzahteva.Text;
                    }
                    Session["zahtev-promena-statusa-sertifikata-datumgubitka"] = txtdatumgubitka.Text;
                    Session["zahtev-promena-statusa-sertifikata-datumcompromise"] = txtdatumcompromise.Text;
                }
                else if (rbManualJik.Checked == true)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "successalert();", true);
                    if (true)
                    {
                        Container00.Visible = true;
                        Container01.Visible = true;
                        txtjik01.ReadOnly = true;
                        txtjik01.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txtime02.ReadOnly = true;
                        txtime02.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                        txtprezime02.ReadOnly = true;
                        txtprezime02.BackColor = ColorTranslator.FromHtml(SetDarkGray);

                        SetDarkAndReadonlyTextBoxes();

                        myDiv1.Visible = true;
                        myDiv6.Visible = true;
                        myDiv5.Visible = false;

                        //Disable datepicker
                        ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);

                        cvjik01.Enabled = true;
                        txtdatumzahteva.Text = DateTime.Now.ToString("dd.MM.yyyy");
                        Session["zahtev-promena-statusa-sertifikata-datumzahteva"] = txtdatumzahteva.Text;
                        Session["zahtev-promena-statusa-sertifikata-datumgubitka"] = txtdatumgubitka.Text;
                        Session["zahtev-promena-statusa-sertifikata-datumcompromise"] = txtdatumcompromise.Text;
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
                    catch (Exception ex)
                    {
                        log.Error("Error while getting ipGeolocationData. " + ex.Message);
                        userAgentContinent = string.Empty;
                        userAgentCountry = string.Empty;
                        userAgentCountryCode = string.Empty;
                        userAgentCity = string.Empty;
                        userAgentISP = string.Empty;
                    }
                }
                else
                {
                    log.Debug("Geolocation is not active!");
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
                errLabel01.Text = string.Empty;
                errLabelNumber.Text = string.Empty;
                errLabelMail.Text = string.Empty;
                errLabelPIB.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
                if (rbAutomatikJik.Checked == true)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "EnabledButton", "EnabledButton();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Button submit error. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
        }

    }

    protected void SetDarkAndReadonlyTextBoxes()
    {
        txtmaticnibroj.ReadOnly = true;
        txtmaticnibroj.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        txtnazivpravnoglica.ReadOnly = true;
        txtnazivpravnoglica.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        ddlLegalEntityName.CssClass = SetCss5;
        txtpib.ReadOnly = true;
        txtpib.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        txtimezz.ReadOnly = true;
        txtimezz.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        txtprezimezz.ReadOnly = true;
        txtprezimezz.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        ddlnacinpromene.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        ddlnacinpromene.CssClass = SetCss4;
        txtdatumgubitka.ReadOnly = true;
        txtdatumgubitka.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        txtdatumcompromise.ReadOnly = true;
        txtdatumcompromise.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        txtostalo.ReadOnly = true;
        txtostalo.BackColor = ColorTranslator.FromHtml(SetDarkGray);
        txtdrugo.ReadOnly = true;
        txtdrugo.BackColor = ColorTranslator.FromHtml(SetDarkGray);
    }

    protected void rbAutomatikJik_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        Session["zahtev-promena-statusa-sertifikata-JIK01-string.empty"] = true;
        rbAutomatikJik.Checked = true;
        ScriptManager.RegisterStartupScript(this, GetType(), "EnableButton", "EnableButton();", true);
        rbManualJik.Checked = false;
        rbUnknownJik.Checked = false;
        txtjik01.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtjik01.ReadOnly = true;
        txtjik01.Text = string.Empty;
        txtime02.Text = string.Empty;
        txtserijskibroj02.Text = string.Empty;
        txtprezime02.Text = string.Empty;
        errLabel01.Text = string.Empty;


        txtserijskibroj02.ReadOnly = true;
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        Container01.Visible = true;
        txtime02.ReadOnly = true;
        txtime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtprezime02.ReadOnly = true;
        txtprezime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        Container04.Visible = false;
        myDiv1.Visible = false;
        myDiv5.Visible = true;

        cvjik01.Enabled = true;

        Container06.Visible = true;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbAutomatikJik;
        txtjik01.TabIndex = -1;
        txtime02.TabIndex = -1;
        txtserijskibroj02.TabIndex = -1;
        txtprezime02.TabIndex = -1;
        SetFocusOnRadioButton();
    }

    protected void rbManualJik_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        rbManualJik.Checked = true;
        rbAutomatikJik.Checked = false;
        rbUnknownJik.Checked = false;
        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
        txtjik01.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtjik01.ReadOnly = false;
        if (txtime02.Text != string.Empty || txtprezime02.Text != string.Empty)
        {
            //if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-JIK01-string.empty"]))
            //{
            if (Session["zahtev-promena-statusa-sertifikata-CertificateJIK"] != null)
            {
                string jik = Session["zahtev-promena-statusa-sertifikata-CertificateJIK"].ToString();
                if (jik != string.Empty)
                {
                    txtjik01.Text = string.Empty;
                    txtime02.Text = string.Empty;
                    txtprezime02.Text = string.Empty;
                }
            }
        }
        txtime02.ReadOnly = true;   
        txtime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtserijskibroj02.ReadOnly = true;
        txtserijskibroj02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtserijskibroj02.Text = string.Empty;
        txtprezime02.ReadOnly = true;        
        //errLabel01.Text = string.Empty;
        Container01.Visible = true;
        txtprezime02.BackColor = ColorTranslator.FromHtml(SetLightGray);
        Container04.Visible = false;
        myDiv1.Visible = false;
        myDiv5.Visible = true;

        cvjik01.Enabled = true;
        Container06.Visible = true;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbManualJik;
        txtjik01.TabIndex = 0;
        txtime02.TabIndex = -1;
        txtserijskibroj02.TabIndex = -1;
        txtprezime02.TabIndex = -1;
        SetFocusOnRadioButton();
    }

    protected void rbUnknownJik_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        Session["zahtev-promena-statusa-sertifikata-JIK01-string.empty"] = true;
        rbUnknownJik.Checked = true;
        rbAutomatikJik.Checked = false;
        rbManualJik.Checked = false;
        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);

        Container01.Visible = false;
        Container04.Visible = true;
        myDiv1.Visible = false;
        myDiv5.Visible = true;
        /*
        txtime.Text = string.Empty;
        txtprezime.Text = string.Empty;
        txtjmbg.Text = string.Empty;
        txtadresaeposte.Text = string.Empty;
        txttelefon.Text = string.Empty;
        cvime.ErrorMessage = string.Empty;
        cvprezime.ErrorMessage = string.Empty;
        cvjmbg.ErrorMessage = string.Empty;
        errLabelNumber.Text = string.Empty;
        errLabelMail.Text = string.Empty;
        errLabelPIB.Text = string.Empty;
        */
        cvjik01.Enabled = true;
        Container06.Visible = true;

        //Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbUnknownJik;
        txtime.TabIndex = 0;
        txtprezime.TabIndex = 0;
        txtjmbg.TabIndex = 0;
        txtadresaeposte.TabIndex = 0;
        txttelefon.TabIndex = 0;
        //SetFocusOnRadioButton();
    }

    protected void rbIndividual_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        if (rbAutomatikJik.Checked)
        {
            if (Session["zahtev-promena-statusa-sertifikata-CertificateJIK"] != null)
            {
                txtjik01.Text = Session["zahtev-promena-statusa-sertifikata-CertificateJIK"].ToString();
                txtime02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateFirstName"].ToString();
                txtprezime02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateLastName"].ToString();
            }
        }
        rbIndividual.Checked = true;
        rbLegal.Checked = false;
        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"]))
        { }
        else
        {
            Container08.Visible = false;
            Container09.Visible = false;
        }
        Container07.Visible = false;
        Container10.Visible = true;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbIndividual;
        SetFocusOnRadioButton();
    }

    protected void rbLegal_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        if (rbAutomatikJik.Checked)
        {
            if (Session["zahtev-promena-statusa-sertifikata-CertificateJIK"] != null)
            { 
                txtjik01.Text = Session["zahtev-promena-statusa-sertifikata-CertificateJIK"].ToString();
                txtime02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateFirstName"].ToString();
                txtprezime02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateLastName"].ToString();
            }
        }
        rbLegal.Checked =true;
        rbIndividual.Checked = false;
        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"]))
        { }
        else
        {
            Container08.Visible = false;
            Container09.Visible = false;
        }
        Container07.Visible = true;
        Container10.Visible = true;
        txtmaticnibroj.Text = string.Empty;
        txtnazivpravnoglica.Text = string.Empty;
        txtnazivpravnoglica.ReadOnly = false;
        txtnazivpravnoglica.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtpib.Text = string.Empty;
        txtpib.ReadOnly = true;
        txtpib.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtimezz.Text = string.Empty;
        txtprezimezz.Text = string.Empty;
        
        //string IDItem1 = string.Empty;
        //SetUpDefaultItem(ddlnacinpromene.ClientID, out IDItem1);
        //ddlnacinpromene.SelectedValue = IDItem1;
        
        cvmaticnibroj.ErrorMessage = string.Empty;
        cvnazivpravnoglica.ErrorMessage = string.Empty;
        cvpib.ErrorMessage = string.Empty;
        cvimezz.ErrorMessage = string.Empty;
        cvprezimezz.ErrorMessage = string.Empty;
        cvnacinpromene.ErrorMessage = string.Empty;
        errLabelPIB.Text = string.Empty;
        errLabelIN.Text = string.Empty;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbLegal;
        txtpib.TabIndex = -1;
        SetFocusOnRadioButton();
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


    protected bool SendJIKtoBlueX(string JIK01, out string ErrorLabel, out string GivenName, out string LastName)
    {
        bool returnValue = false;
        ErrorLabel = string.Empty;
        GivenName = string.Empty;
        LastName = string.Empty;

        Utility utility = new Utility();
        try
        {
            //Salje se SOAP poruka sa JIK-om, a kao Response se dobija ime i prezime..isto kao u Formi 6 samo bez datuma isticanja i izdavanja 
            log.Debug("Start sending SOAP message with USI.");

            BxSoapEnvelope envelope = new BxSoapEnvelopeCertificateStatusCheck();
            //todo samo zameni
            envelope.BxData.setValue(@"USI", txtjik01.Text);
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
                throw new Exception("Error while sending SOAP message with USI " + ". List CertificateStatuses is " + CertificateStatuses + ". SOAPresponse is " + SOAPresponse);
            }

            Session["zahtev-promena-statusa-sertifikata-CertificateStatuses"] = CertificateStatuses;

            log.Debug("Successfully send SOAP message with USI.");

            if (CertificateStatuses.Count > 0)
            {
                foreach (var certificatestatus in CertificateStatuses)
                {
                    GivenName = certificatestatus.GivenName;
                    LastName = certificatestatus.LastName;
                    log.Debug("givenName is: " + GivenName + ", lastNAme is: " + LastName);
                    ErrorLabel = string.Empty;
                    returnValue = true;
                }
                log.Debug("Successfully send first SOAP message with JIK!");
            }
            else
            {
                log.Debug("Za navedeni USI(JIK) " + txtjik01.Text + "nema statusa.");
                ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Notification();", true);
            }
            
        }
        catch (Exception ex)
        {
            log.Debug("Za navedeni USI(JIK) " + txtjik01.Text + "nema statusa. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Notification();", true);
            ErrorLabel = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3371);
            //ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);
            //log.Error("Error while sending first SOAP message with USI. " + ex.Message);
            //Disable datepicker
            ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);

            if (txtdatumgubitka.Text == string.Empty)
            {

            }
            else if (txtdatumcompromise.Text == string.Empty)
            {

            }
            else
            {
                txtdatumgubitka.Text = Session["zahtev-promena-statusa-sertifikata-datumgubitka"].ToString();
                txtdatumcompromise.Text = Session["zahtev-promena-statusa-sertifikata-datumcompromise"].ToString();
            }
            returnValue = false;
        }

        return returnValue;
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    protected void cvjik01_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string GivenName = string.Empty;
            string LastName = string.Empty;
            string ErrorMessage1 = string.Empty;           
            args.IsValid = UtilsValidation.ValidateJIK(txtjik01.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtjik01IsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnJIKValidation"]), out ErrorMessage1);
            cvjik01.ErrorMessage = ErrorMessage1;

            if (ErrorMessage1 == string.Empty)
            { 
                args.IsValid = SendJIKtoBlueX(txtjik01.Text, out ErrorMessage1, out GivenName, out LastName);
                cvjik01.ErrorMessage = ErrorMessage1;
                txtime02.Text = GivenName;
                txtprezime02.Text = LastName;
            }
        }
        catch (Exception)
        {
            cvjik01.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvjik_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateJIK(txtjik.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtjikIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnSecondJIKValidation"]), out ErrorMessage1);
            cvjik.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvjik.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    public List<CertificateStatusCheck> CertificateStatuses;
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txtjik01_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged1();
    }

    private void CheckIfChannelHasChanged1()
    {
        string GivenName = string.Empty;
        string LastName = string.Empty;
        //txtime02.Text = string.Empty;
        //txtprezime02.Text = string.Empty;
        string ErrorMessage1 = string.Empty;
        string JIK = txtjik01.Text;
        UtilsValidation.ValidateJIK(JIK, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtjik01IsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnJIKValidation"]), out ErrorMessage1);
        errLabel01.Text = ErrorMessage1;
        ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);

        if (errLabel01.Text == string.Empty)
        { 
            SendJIKtoBlueX(txtjik01.Text, out ErrorMessage1, out GivenName, out LastName);
            errLabel01.Text = ErrorMessage1;
            txtime02.Text = GivenName;
            txtprezime02.Text = LastName;
        }
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------  


    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA MATIČNOG BROJA-------------------------------------
    //------------------------------------------------------------------------------------------------
    protected void cvjmbg_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateJmbgiBrojPasosa(txtjmbg.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtjmbgIsRequired"]), out ErrorMessage1);
            cvjmbg.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvjmbg.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Telefonskog BROJA----------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txttelefon_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged3();
        if (errLabelNumber.Text != string.Empty)
        {
            Session["zahtev-promena-statusa-sertifikata-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbIndividual;
            SetFocusOnRadioButton();
        }
    }

    private void CheckIfChannelHasChanged3()
    {
        string newNumber = txttelefon.Text;
        string errorMessage = string.Empty;
        string numberformat = string.Empty;
        bool LegalEntityPhone = false;

        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnPhoneValidation"]))
        {
            UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txttelefonIsRequired"]), LegalEntityPhone, out errorMessage, out numberformat);
            errLabelNumber.Text = errorMessage;
            newNumber = numberformat;
        }
        else
        {
            newNumber = txttelefon.Text;
            errorMessage = string.Empty;
        }
    }

    protected void cvtelefon_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newNumber = txttelefon.Text;
        string errMessage = string.Empty;
        string numberformat = string.Empty;
        bool LegalEntityPhone = false;

        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnPhoneValidation"]))
        {
            args.IsValid = UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
            txttelefon.Text = numberformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationNumber(newNumber, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
        }
    }
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Adrese E Pošte-------------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txtadresaeposte_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged2();
        if (errLabelMail.Text != string.Empty)
        {
            Session["zahtev-promena-statusa-sertifikata-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-promena-statusa-sertifikata-event_controle"] = txttelefon;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged2()
    {
        string newMail = txtadresaeposte.Text;
        string errorMessage = string.Empty;
        string mailformat = string.Empty;
        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnEmailValidation"]))
        {
            UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
            errLabelMail.Text = errorMessage;
            newMail = mailformat;
        }
        else
        {
            UtilsValidation.WithoutValidationMail(newMail, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
            errLabelMail.Text = errorMessage;
            newMail = mailformat;
        }
    }


    protected void cvadresaeposte_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string mailformat = string.Empty;

            if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnEmailValidation"]))
            {
                args.IsValid = UtilsValidation.ValidateMail(txtadresaeposte.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtadresaeposteIsRequired"]), out ErrorMessage1, out mailformat);
                cvadresaeposte.ErrorMessage = ErrorMessage1;
            }
            else
            {
                args.IsValid = UtilsValidation.WithoutValidationMail(txtadresaeposte.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtadresaeposteIsRequired"]), out ErrorMessage1, out mailformat);
                cvadresaeposte.ErrorMessage = ErrorMessage1;
            }
        }
        catch (Exception)
        {
            cvadresaeposte.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    protected void cvime_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateName(txtime.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtimeIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnNameValidation"]), out ErrorMessage1, out nameformat);
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
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtserijskibroj02.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtserijskibroj02IsRequired"]), out ErrorMessage1);
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
            string surnameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateSurname(txtprezime.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtprezimeIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnSurnameValidation"]), out ErrorMessage1, out surnameformat);
            cvprezime.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvprezime.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    protected void cvnazivpravnoglica_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string newLegalNameformat = string.Empty;
            string errMessage = string.Empty;
            string newLegalName = txtnazivpravnoglica.Text;
            args.IsValid = UtilsValidation.ValidateNazivPravnogLica(newLegalName, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtnazivpravnoglicaIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnLegalEntityNameValidation"]), out errMessage, out newLegalNameformat);
            cvnazivpravnoglica.ErrorMessage = errMessage;
        }
        catch (Exception)
        {
            cvnazivpravnoglica.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    //-------------------------------------------------------------------------------------
    //------------------------------------PROVERA PIB-a Prva Forma-------------------------
    //-------------------------------------------------------------------------------------
    protected void txtpib_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged5();
    }

    private void CheckIfChannelHasChanged5()
    {
        string newPIB = txtpib.Text;
        string errorMessage = string.Empty;
        string pibformat = string.Empty;
        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnPIBValidation"]))
        {
            UtilsValidation.ValidatePIB(newPIB, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtpibIsRequired"]), out errorMessage, out pibformat); //proveri da li je PIB ispravno napisan
            errLabelPIB.Text = errorMessage;
            newPIB = pibformat;
        }
        else
        {
            newPIB = txtpib.Text;
            errorMessage = string.Empty;
        }
    }

    protected void cvpib_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newPIB = txtpib.Text;
        string errorMessage = string.Empty;
        string pibformat = string.Empty;

        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnPIBValidation"]))
        {
            args.IsValid = UtilsValidation.ValidatePIB(newPIB, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtpibIsRequired"]), out errorMessage, out pibformat); //proveri da li je PIB ispravno napisan
            cvpib.ErrorMessage = errorMessage;
            newPIB = pibformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationPIB(newPIB, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtpibIsRequired"]), out errorMessage, out pibformat);
            cvpib.ErrorMessage = errorMessage;
        }
    }

    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------
    protected void cvimezz_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameZZformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateImeZZ(txtimezz.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtimezzIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnNameZZValidation"]), out ErrorMessage1, out nameZZformat);
            cvimezz.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvimezz.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvprezimezz_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string surnameZZformat = string.Empty;
            args.IsValid = UtilsValidation.ValidatePrezimeZZ(txtprezimezz.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtprezimezzIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnSurnameZZValidation"]), out ErrorMessage1, out surnameZZformat);
            cvprezimezz.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvprezimezz.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    //-----------------SetUp StatusChanges VAriables---------------------------
    //---------------------------------------------------------------
    protected void SetUpStatusChangeVariables(int SelectedValue, string page, string ddlstring, out int IDItem, out bool IsDefaultVariable, out bool IsAllowedVariable, out int ItemValue)
    {
        IDItem = 0;
        IsDefaultVariable = false;
        IsAllowedVariable = false;
        ItemValue = 0;

        Utility utilityStatusChange = new Utility();
        StatusChangeVariables = utilityStatusChange.pronadjiPromenljiveStatusChange(page, ddlstring);

        foreach (var statusvariable in StatusChangeVariables)
        {
            if (statusvariable.IDItem == SelectedValue)
            {
                IsDefaultVariable = statusvariable.IsDefault;
                IsAllowedVariable = statusvariable.IsAllowed;
                ItemValue = statusvariable.ItemValue;
                Session["zahtev-promena-statusa-sertifikata-IsDefaultVariable"] = IsDefaultVariable;
                Session["zahtev-promena-statusa-sertifikata-IsAllowedVariable"] = IsAllowedVariable;
                Session["zahtev-promena-statusa-sertifikata-ItemValue"] = ItemValue;
            }
            else
            { }
        }
    }
    //---------------------------------------------------------------
    //---------------------------------------------------------------

    protected void ddlnacinpromene_SelectedIndexChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        if (rbAutomatikJik.Checked)
        {
            if (Session["zahtev-promena-statusa-sertifikata-CertificateJIK"] != null)
            { 
                txtjik01.Text = Session["zahtev-promena-statusa-sertifikata-CertificateJIK"].ToString();
                txtime02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateFirstName"].ToString();
                txtprezime02.Text = Session["zahtev-promena-statusa-sertifikata-CertificateLastName"].ToString();
            }
        }
        Session["zahtev-promena-statusa-sertifikata-NE-OTVARA-PRVI-PUT"] = true;
        Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"] = true;
        ddlnacinpromene.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"]);

        int SelectedValue = Convert.ToInt32(ddlnacinpromene.SelectedValue);
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string ddlstring = ddlnacinpromene.ClientID;

        //Get PTT Variables from Database
        SetUpStatusChangeVariables(SelectedValue, page, ddlstring, out IDItem, out IsDefaultVariable, out IsAllowedVariable, out ItemValue);

        if (!IsAllowedVariable && IsDefaultVariable)
        {
            Container08.Visible = false;
            Container09.Visible = false;
        }
        else if (IsAllowedVariable && !IsDefaultVariable && ItemValue == Constants.ItemValue_NACINPROMENE_REVOCATION)
        {
            Container08.Visible = true;
            Container09.Visible = false;
        }
        else if (IsAllowedVariable && !IsDefaultVariable && ItemValue == Constants.ItemValue_NACINPROMENE_SUSPENSION)
        {
            Container08.Visible = false;
            Container09.Visible = false;
        }
        else if (IsAllowedVariable && !IsDefaultVariable && ItemValue == Constants.ItemValue_NACINPROMENE_SUSPENSION_REVOCATION)
        {
            Container08.Visible = false;
            Container09.Visible = false;
        }
        else if (IsAllowedVariable && !IsDefaultVariable)
        {
            Container08.Visible = false;
            Container09.Visible = true;
            txtostalo.Text = string.Empty;
            txtostalo.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtostalo.ReadOnly = false;
        }

        Session["zahtev-promena-statusa-sertifikata-event_controle-DropDownList"] = ddlnacinpromene;
        SetFocusOnDropDownLists();
    }

    protected void cvnacinpromene_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlnacinpromeneString = ddlnacinpromene.ClientID;
            string IDItem1 = string.Empty;
            SetUpDefaultItem(ddlnacinpromeneString, out IDItem1);

            args.IsValid = UtilsValidation.ValidateNacinPromene(ddlnacinpromene.SelectedValue, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlnacinpromeneIsRequired"]), IDItem1, out ErrorMessage1);
            cvnacinpromene.ErrorMessage = ErrorMessage1;           
        }
        catch (Exception)
        {
            cvnacinpromene.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvostalo_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateOstalo(txtostalo.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtdrugoIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnTheRestValidation"]), out ErrorMessage1);
            cvostalo.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvostalo.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdrugo_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateDrugo(txtdrugo.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtostaloIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnOtherValidation"]), out ErrorMessage1);
            cvdrugo.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvdrugo.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdatumgubitka_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            Utility utility = new Utility();
            if (txtdatumgubitka.Text != string.Empty)
            {
                DateTime datumgubitka = DateTime.ParseExact(txtdatumgubitka.Text, "dd.MM.yyyy", null);
                log.Debug("datumgubitka je: " + datumgubitka);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateDateOfBirth(datumgubitka, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtdatumgubitkaIsRequired"]), out ErrorMessage1);
                cvdatumgubitka.ErrorMessage = ErrorMessage1;
            }
            else
            {
                if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtdatumgubitkaIsRequired"]))
                {
                    if (txtdatumgubitka.Text == string.Empty)
                    {
                        cvdatumgubitka.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                        args.IsValid = false;
                    }
                    else
                    {
                        args.IsValid = true;
                    }
                }
                else
                {
                    args.IsValid = true;
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Greska prilikom validacije cvdatumgubitka. " + ex.Message);
            cvdatumgubitka.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdatumcompromise_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            Utility utility = new Utility();
            if (txtdatumcompromise.Text != string.Empty)
            {
                DateTime datumcompromise = DateTime.ParseExact(txtdatumcompromise.Text, "dd.MM.yyyy", null);
                log.Debug("datumcompromise je: " + datumcompromise);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateDateOfBirth(datumcompromise, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtdatumcompromiseIsRequired"]), out ErrorMessage1);
                cvdatumcompromise.ErrorMessage = ErrorMessage1;
            }
            else
            {
                if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtdatumcompromiseIsRequired"]))
                {
                    if (txtdatumcompromise.Text == string.Empty)
                    {
                        cvdatumcompromise.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                        args.IsValid = false;
                    }
                    else
                    {
                        args.IsValid = true;
                    }
                }
                else
                {
                    args.IsValid = true;
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Greska prilikom validacije cvdatumcompromise. " + ex.Message);
            cvdatumcompromise.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void rblosstoken_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        rblosstoken.Checked = true;
        rbcompromise.Checked = false;
        rbchangedata.Checked = false;
        rbcessation.Checked = false;
        rbcessationofneed.Checked = false;
        rbother.Checked = false;
        Container11.Visible = true;
        Container12.Visible = false;
        Container13.Visible = false;
        txtdrugo.Text = string.Empty;
        txtdatumgubitka.ReadOnly = true;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rblosstoken;
        SetFocusOnRadioButton();
    }

    protected void rbcompromise_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        rblosstoken.Checked = false;
        rbcompromise.Checked = true;
        rbchangedata.Checked = false;
        rbcessation.Checked = false;
        rbcessationofneed.Checked = false;
        rbother.Checked = false;
        Container11.Visible = false;
        Container12.Visible = true;
        Container13.Visible = false;
        txtdrugo.Text = string.Empty;
        txtdatumcompromise.Enabled = true;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbcompromise;
        SetFocusOnRadioButton();
    }

    protected void rbother_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        rblosstoken.Checked = false;
        rbcompromise.Checked = false;
        rbother.Checked = true;
        rbchangedata.Checked = false;
        rbcessation.Checked = false;
        rbcessationofneed.Checked = false;
        Container11.Visible = false;
        Container12.Visible = false;
        Container13.Visible = true;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbother;
        SetFocusOnRadioButton();
    }

    protected void rbchangedata_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        rblosstoken.Checked = false;
        rbcompromise.Checked = false;
        rbchangedata.Checked = true;
        rbcessation.Checked = false;
        rbcessationofneed.Checked = false;
        rbother.Checked = false;
        Container11.Visible = false;
        Container12.Visible = false;
        Container13.Visible = false;
        txtdrugo.Text = string.Empty;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbchangedata;
        SetFocusOnRadioButton();
    }

    protected void rbcessation_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        rblosstoken.Checked = false;
        rbcompromise.Checked = false;
        rbchangedata.Checked = false;
        rbcessation.Checked = true;
        rbcessationofneed.Checked = false;
        rbother.Checked = false;
        Container11.Visible = false;
        Container12.Visible = false;
        Container13.Visible = false;
        txtdrugo.Text = string.Empty;

        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbcessation;
        SetFocusOnRadioButton();
    }

    protected void rbcessationofneed_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();
        rblosstoken.Checked = false;
        rbcompromise.Checked = false;
        rbchangedata.Checked = false;
        rbcessation.Checked = false;
        rbcessationofneed.Checked = true;
        rbother.Checked = false;
        Container11.Visible = false;
        Container12.Visible = false;
        Container13.Visible = false;
        txtdrugo.Text = string.Empty;
        Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] = rbcessationofneed;
        SetFocusOnRadioButton();

    }

    //public string BrojZahteva = Utils.Generate15UniqueDigits();
    public string BrojZahteva = string.Empty;
    public string USI = string.Empty;
    public string ResponseStatus = string.Empty;
    public bool isResponseZero = true;
    public string responseMessage = string.Empty;
    public int RevokeReason = 0;

    protected BxSoapEnvelope createSoapEnvelope(Utility utility, string isLegalEntity, string radioButtonStatus, int RevokeReason, string imeStatus, string prezimeStatus, string usistatus)
    {
        BxSoapEnvelope envelope = new BxSoapEnvelopeStatusChange();

        envelope.BxData.setValue(@"USI", usistatus);
        envelope.BxData.setValue(@"givenName", imeStatus);
        envelope.BxData.setValue(@"lastName", prezimeStatus);
        envelope.BxData.setValue(@"uniqueCitizensNumber", txtjmbg.Text);
        envelope.BxData.setValue(@"emailAddress", txtadresaeposte.Text);
        envelope.BxData.setValue(@"phoneNumber", txttelefon.Text);
        envelope.BxData.setValue(@"isLegalEntity", isLegalEntity);
        envelope.BxData.setValue(@"legalEntityName", legalEntityName);
        envelope.BxData.setValue(@"legalEntityGivenName", txtimezz.Text);
        envelope.BxData.setValue(@"legalEntityLastName", txtprezimezz.Text);
        envelope.BxData.setValue(@"businessRegistrationNumber", txtmaticnibroj.Text);
        envelope.BxData.setValue(@"taxNumber", txtpib.Text);
        envelope.BxData.setValue(@"statusChange", (utility.getEnglishText(Convert.ToInt32(ddlnacinpromene.SelectedValue))));
        envelope.BxData.setValue(@"statusChangeReason", radioButtonStatus);
        envelope.BxData.setValue(@"dateTokenCompromise", Session["zahtev-promena-statusa-sertifikata-datumcompromise"].ToString());
        envelope.BxData.setValue(@"dateTokenLost", Session["zahtev-promena-statusa-sertifikata-datumgubitka"].ToString());
        //------------------------------------------------------------------------------            
        envelope.BxData.setValue(@"userAgentStringApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentStringApplicant"].ToString());
        envelope.BxData.setValue(@"ipApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentIP"].ToString());
        envelope.BxData.setValue(@"continentApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentContinent"].ToString());
        envelope.BxData.setValue(@"countryApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentCountry"].ToString());
        envelope.BxData.setValue(@"countryCodeApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentCountryCode"].ToString());
        envelope.BxData.setValue(@"cityApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentCity"].ToString());
        envelope.BxData.setValue(@"osApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentOS"].ToString());
        envelope.BxData.setValue(@"ispApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentISP"].ToString());
        envelope.BxData.setValue(@"browserApplicant", Session["zahtev-promena-statusa-sertifikata-userAgentBrowser"].ToString());
        envelope.BxData.setValue(@"ipOperator", string.Empty);
        return envelope;
    }

    protected List<PisMessServiceReference.Parameter> getDocumentParametersList(Utility utility, string legalEntityName, string radioButtonStatus, int RevokeReason, string imeStatus, string prezimeStatus, string usistatus)
    {
        List<PisMessServiceReference.Parameter> documentParameters = new List<PisMessServiceReference.Parameter>();

        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "requestNumber",
                //todo samo zameni
                ParameterValue = Session["zahtev-promena-statusa-sertifikata-brojzahteva"].ToString()
                //ParameterValue = "200000002"
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "firstName",
                ParameterValue = imeStatus
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "lastName",
                ParameterValue = prezimeStatus
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "jmbg",
                ParameterValue = txtjmbg.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "email",
                ParameterValue = txtadresaeposte.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "phone",
                ParameterValue = txttelefon.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "jik",
                ParameterValue = usistatus
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "invalidityDate1",
                ParameterValue = Session["zahtev-promena-statusa-sertifikata-datumgubitka"].ToString()
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "invalidityDate2",
                ParameterValue = Session["zahtev-promena-statusa-sertifikata-datumcompromise"].ToString()
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "drugo",
                ParameterValue = txtdrugo.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "ostalo",
                ParameterValue = txtostalo.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "legalEntityName",
                ParameterValue = legalEntityName
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "reason",
                ParameterValue = RevokeReason.ToString()
            });

        return documentParameters;
    }

    protected string CreateDocumentCertificateStatusChange(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess, List<PisMessServiceReference.Parameter> documentParameters)
    {
        //templateDocumentType: CertificateStatusChange
        //-----------------------------------------
        string responseMessage = string.Empty;
        List<string> response = new List<string>(ServiceCaller.CallServiceCreateDoc(PisMessServiceReference.TemplateDocumentTypeSerbianPost.CertificateStatusChange, documentParameters.ToArray()));
        if (response[0].Equals("0"))
        {
            //success
            responseMessage = response[1]; //write file path to some text box
        }
        else
        {
            //error
            responseMessage = response[0]; //write error description to some text box
            throw new Exception("Response error while creating CertificateStatusChange document, response from PissMess: " + responseMessage);
        }
        return responseMessage;
    }

    protected void btnEnterRequest_Click1(object sender, EventArgs e)
    {
        try
        {
            log.Debug("Start sending SOAP message.");

            if (rbIndividual.Checked == true)
            {
                Session["zahtev-promena-statusa-sertifikata-isLegalEntity"] = Constants.isLegalEntityFalse;
                isLegalEntity = Session["zahtev-promena-statusa-sertifikata-isLegalEntity"].ToString();
            }
            else
            {
                Session["zahtev-promena-statusa-sertifikata-isLegalEntity"] = Constants.isLegalEntityTrue;
                isLegalEntity = Session["zahtev-promena-statusa-sertifikata-isLegalEntity"].ToString();

                if (txtnazivpravnoglica.Text == string.Empty)
                {
                    legalEntityName = ddlLegalEntityName.SelectedItem.Text;
                    /*
                    if (txtwritename.Text == string.Empty)
                    {
                        legalEntityName = ddlLegalEntityName.SelectedItem.Text;
                    }
                    else
                    {
                        legalEntityName = txtwritename.Text;
                    }
                    */
                }
                else
                {
                    legalEntityName = txtnazivpravnoglica.Text;
                }
            }

            if (rblosstoken.Checked)
            {
                radioButtonStatus = rblosstoken.Text;
                RevokeReason = 1;
            }
            else if (rbcompromise.Checked)
            {
                radioButtonStatus = rbcompromise.Text;
                RevokeReason = 2;
            }
            else if (rbchangedata.Checked)
            {
                radioButtonStatus = rbchangedata.Text;
                RevokeReason = 3;
            }
            else if (rbcessation.Checked)
            {
                radioButtonStatus = rbcessation.Text;
                RevokeReason = 4;
            }
            else if (rbcessationofneed.Checked)
            {
                radioButtonStatus = rbcessationofneed.Text;
                RevokeReason = 5;
            }
            else if (rbother.Checked)
            {
                radioButtonStatus = rbother.Text;
                RevokeReason = 6;
            }
            else if (Convert.ToInt32(ddlnacinpromene.SelectedValue) == Constants.IDITEM_UNSUSPENSION)
            {
                RevokeReason = 7;
            }
            else if (Convert.ToInt32(ddlnacinpromene.SelectedValue) == Constants.IDITEM_SUSPEND)
            {
                RevokeReason = 8;
            }
            else if (Convert.ToInt32(ddlnacinpromene.SelectedValue) == Constants.IDITEM_OTHER_REASON)
            {
                radioButtonStatus = txtostalo.Text;
                RevokeReason = 9;
            }

            if (txtime02.Text == string.Empty)
            {
                imeStatus = txtime.Text;
                prezimeStatus = txtprezime.Text;
            }
            else
            {
                imeStatus = txtime02.Text;
                prezimeStatus = txtprezime02.Text;
            }

            if (txtjik.Text != string.Empty)
            {
                usiStatus = txtjik.Text;
            }
            else
            {
                usiStatus = txtjik01.Text;
            }

            pisMess = new PisMessServiceReference.PisMessServiceClient();
            List<PisMessServiceReference.Parameter> documentParameters = new List<PisMessServiceReference.Parameter>();

            Utility utility = new Utility();

            BxSoapEnvelope envelope = createSoapEnvelope(utility, isLegalEntity, radioButtonStatus, RevokeReason, imeStatus, prezimeStatus, usiStatus);

            //envelope.createBxSoapEnvelope();   //create SOAP.xml
            string SOAPresponse = string.Empty;
            try
            {
                SOAPresponse = BxSoap.SOAPManual(envelope.createBxSoapEnvelope());
            }
            catch (Exception ex)
            {
                throw new Exception("Error. Response from BlueX is: " + SOAPresponse + ex.Message);
            }

            Utils.ParseSoapEnvelopeStatusChange(SOAPresponse, out USI, out BrojZahteva, out ResponseStatus);

            if (ResponseStatus != Constants.RESPONSE_STATUS_CHANGE_SUCCESS)
            {
                throw new Exception("ResponseStatus is not success!");
            }

            Session["zahtev-promena-statusa-sertifikata-brojzahteva"] = BrojZahteva;

            log.Debug("Successfully send SOAP message! RequestNumber for StatusChange is: " + BrojZahteva);

            log.Debug("Start creating PDF Files.");

            documentParameters = getDocumentParametersList(utility, legalEntityName, radioButtonStatus, RevokeReason, imeStatus, prezimeStatus, usiStatus);
            var CreateDocumentCertificateStatusChangeTask = Task.Run(() => CreateDocumentCertificateStatusChange(utility, pisMess, documentParameters));
            CreateDocumentCertificateStatusChangeTask.Wait();

            //string fileName = CreateDocumentCertificateStatusChange(utility, pisMess, documentParameters);
            Session["zahtev-promena-statusa-sertifikata-filename"] = CreateDocumentCertificateStatusChangeTask.Result;

            log.Debug("Finished creating PDF files!");

            Response.Redirect("zahtev-promena-statusa-podnet.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
        }
        catch (AggregateException aex)
        {
            log.Error(aex.InnerException.Message, aex);
            throw aex;
        }
        catch (Exception ex)
        {
            log.Error("Error while sending request. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);
            if (txtdatumgubitka.Text == string.Empty)
            {

            }
            else if (txtdatumcompromise.Text == string.Empty)
            {

            }
            else
            {
                txtdatumgubitka.Text = Session["zahtev-promena-statusa-sertifikata-datumgubitka"].ToString();
                txtdatumcompromise.Text = Session["zahtev-promena-statusa-sertifikata-datumcompromise"].ToString();
            }
        }       
    }

    protected void btnReEnterRequest_Click1(object sender, EventArgs e)
    {
        RadioButtonsDropdownListsTrue();

        Session["zahtev-promena-statusa-sertifikata-rbChooseName"] = true;
        Session["zahtev-promena-statusa-sertifikata-rbWriteName"] = true;

        //rbChooseName.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbChooseName"]);
        //rbWriteName.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbWriteName"]);
        rbManualJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbManualJIK"]);
        rbAutomatikJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbAutomatikJIK"]);
        rbUnknownJik.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbUnknownJIK"]);
        rbIndividual.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbIndividual"]);
        rbLegal.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbLegal"]);
        ddlnacinpromene.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlnacinpromene"]);
        ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"]);
        rblosstoken.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rblosstoken"]);
        rbcompromise.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcompromise"]);
        rbchangedata.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbchangedata"]);
        rbcessation.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcessation"]);
        rbcessationofneed.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbcessationofneed"]);
        rbother.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-rbother"]);

        myDiv1.Visible = false;
        myDiv6.Visible = false;
        myDiv5.Visible = true;

        if (rbUnknownJik.Checked == true)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);
            Container00.Visible = true;
            Container01.Visible = false;
            ForAllThreeRadioButtons();
            //Enable datepicker
            ScriptManager.RegisterStartupScript(this, GetType(), "Enable", "EnableCalendar();", true);
        }
        else if (rbAutomatikJik.Checked == true)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "EnabledButton", "EnabledButton();", true);
            Container00.Visible = true;
            Container01.Visible = true;
            txtjik01.ReadOnly = false;
            txtjik01.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtime02.ReadOnly = true;
            txtime02.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtprezime02.ReadOnly = true;
            txtprezime02.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            //Enable datepicker
            ScriptManager.RegisterStartupScript(this, GetType(), "Enable", "EnableCalendar();", true);
            ForAllThreeRadioButtons();
        }
        else if (rbManualJik.Checked == true)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "DisableButton", "DisableButton();", true);

            Container00.Visible = true;
            Container01.Visible = true;
            txtjik01.ReadOnly = false;
            txtjik01.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtime02.ReadOnly = true;
            txtime02.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtprezime02.ReadOnly = true;
            txtprezime02.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            //Enable datepicker
            ScriptManager.RegisterStartupScript(this, GetType(), "Enable", "EnableCalendar();", true);
            ForAllThreeRadioButtons();
        }
    }

    protected void ForAllThreeRadioButtons()
    {
        txtjik.ReadOnly = false;
        txtjik.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtime.ReadOnly = false;
        txtime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtprezime.ReadOnly = false;
        txtprezime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtjmbg.ReadOnly = false;
        txtjmbg.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtadresaeposte.ReadOnly = false;
        txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetWhite);
        txttelefon.ReadOnly = false;
        txttelefon.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtmaticnibroj.ReadOnly = false;
        txtmaticnibroj.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtnazivpravnoglica.ReadOnly = false;
        txtnazivpravnoglica.BackColor = ColorTranslator.FromHtml(SetWhite);        
        ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlLegalEntityName.CssClass = SetCss5;
        txtpib.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtimezz.ReadOnly = false;
        txtimezz.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtprezimezz.ReadOnly = false;
        txtprezimezz.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlnacinpromene.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlnacinpromene.CssClass = SetCss4;
        txtdatumgubitka.ReadOnly = true;
        txtdatumgubitka.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtdatumgubitka.Text = Session["zahtev-promena-statusa-sertifikata-datumgubitka"].ToString();
        txtdatumcompromise.ReadOnly = true;
        txtdatumcompromise.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtdatumcompromise.Text = Session["zahtev-promena-statusa-sertifikata-datumcompromise"].ToString();
        txtdrugo.ReadOnly = false;
        txtdrugo.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtostalo.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtostalo.ReadOnly = false;
    }
    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA MATIČNOG BROJA FIRME-------------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txtmaticnibroj_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged7();
    }

    public List<LegalEntityVariable> LegalEntities;

    private void CheckIfChannelHasChanged7()
    {
        string newIN = txtmaticnibroj.Text;
        string errorMessage = string.Empty;
        LegalEntities = new List<LegalEntityVariable>();
        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnRegistrationNumberValidation"]))
        {
            getIdentificationNumber(newIN, out errorMessage, out LegalEntities);
            errLabelIN.Text = errorMessage;
            ClearTextboxesAndDropDownLists();
            
            if (LegalEntities.Count > 1)
            {
                List<String> entityFullNames = new List<String>();
                Utility utility = new Utility();
                entityFullNames.Add(utility.getItemText(Constants.DefaultIdItemLegal));
                foreach (var entity in LegalEntities)
                {
                    entityFullNames.Add(entity.FullName);
                }
                rowLegalEntityName.Visible = false;
                rowLegalEntityDDL.Visible = true;
                //rowLegalEntityDDL1.Visible = true;
                //rowLegalEntityDDL2.Visible = true;

                rbChooseNameCheckedChanged();

                //ddlLegalEntityName.Items.Insert(0, utility.getItemText(Constants.DefaultIdItemLegal));
                ddlLegalEntityName.DataSource = entityFullNames;
                ddlLegalEntityName.DataBind();

                Session["zahtev-promena-statusa-sertifikata-event_controle-DropDownList"] = ddlLegalEntityName;
                SetFocusOnDropDownLists();
            }
            else
            {              
                rowLegalEntityName.Visible = true;
                rowLegalEntityDDL.Visible = false;
                //rowLegalEntityDDL1.Visible = false;
                //rowLegalEntityDDL2.Visible = false;
                foreach (var entity in LegalEntities)
                {
                    txtnazivpravnoglica.Text = entity.FullName;
                    txtpib.Text = entity.PIB;
                }


                if (errLabelIN.Text != string.Empty)
                {
                    Session["zahtev-promena-statusa-sertifikata-event_controle"] = txtmaticnibroj;
                    SetFocusOnTextbox();
                }
                else
                {
                    Session["zahtev-promena-statusa-sertifikata-event_controle"] = txtimezz;
                    SetFocusOnTextbox();
                }
            }
        }
        else
        {
            newIN = txtmaticnibroj.Text;
            errorMessage = string.Empty;
            Session["zahtev-promena-statusa-sertifikata-event_controle"] = txtnazivpravnoglica;
            SetFocusOnTextbox();
        }
    }

    protected void ClearTextboxesAndDropDownLists()
    {
        txtnazivpravnoglica.Text = string.Empty;
        txtpib.Text = string.Empty;

    }

    protected void cvmaticnibroj_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newIN = txtmaticnibroj.Text;
        string errMessage = string.Empty;
        List<LegalEntityVariable> LegalEntities = new List<LegalEntityVariable>();
        if (Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnRegistrationNumberValidation"]))
        {
            args.IsValid = getIdentificationNumber(newIN, out errMessage, out LegalEntities);
            errLabelIN.Text = string.Empty;
            cvmaticnibroj.ErrorMessage = errMessage;
        }
        else
        {
            args.IsValid = UtilsValidation.ValidateIdentificationNumber(newIN, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtmaticnibrojIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnRegistrationNumberValidation"]), out errMessage);
            cvmaticnibroj.ErrorMessage = errMessage;
        }
    }

    protected bool getIdentificationNumber(string newIN, out string ErrorMessage, out List<LegalEntityVariable> LegalEntities)
    {
        bool returnValue = true;
        ErrorMessage = string.Empty;
        LegalEntities = new List<LegalEntityVariable>();

        try
        {
            /*
            if (newIN == string.Empty)
            {
                ErrorMessage = "Matični broj je obavezno polјe.";
                returnValue = false;
            }
            else if (!Utils.allowNumbers(newIN))
            {
                ErrorMessage = "Moguće je uneti samo cifre!";
                returnValue = false;
            }
            else if (newIN.Length < Constants.LEGAL_ENTITY_MATICNI_BROJ)
            {
                ErrorMessage = "Matični broj mora sadržati "+ Constants.LEGAL_ENTITY_MATICNI_BROJ + " cifara.";
                returnValue = false;
            }
            else if (newIN.Length == 8)
            {
                Utility utility = new Utility();
                LegalEntities = utility.pronadjiPromenljiveLegalEntity(newIN);
                returnValue = true;
                ErrorMessage = string.Empty;
            }
            else
            {
                ErrorMessage = "Neispravna dužina matičnog broja!";
                returnValue = false;
            }
            */
            returnValue = UtilsValidation.ValidateIdentificationNumber(newIN, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtmaticnibrojIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnRegistrationNumberValidation"]), out ErrorMessage);
            if (returnValue)
            {
                Utility utility = new Utility();
                LegalEntities = utility.pronadjiPromenljiveLegalEntity(newIN);
                if (LegalEntities.Count == 0)
                {
                    ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3380);
                    returnValue = false;
                }
                else
                {                     
                    returnValue = true;
                    ErrorMessage = string.Empty;
                }
            }
            else
            {
                cvmaticnibroj.ErrorMessage = ErrorMessage;
            }
        }
        catch (Exception)
        {
            ErrorMessage = string.Empty;
            returnValue = false;
        }

        return returnValue;
    }
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------


    protected void ddlLegalEntityName_SelectedIndexChanged(object sender, EventArgs e)
    {
        string SelectedValue = ddlLegalEntityName.SelectedItem.Text;
        Utility utility = new Utility();
        LegalEntities = utility.pronadjiPromenljiveLegalEntity(txtmaticnibroj.Text);
        string FullNameDefault = utility.getItemText(Constants.DefaultIdItemLegal);

        foreach (var entity in LegalEntities)
        {
            if (FullNameDefault == SelectedValue)
            {
                txtnazivpravnoglica.Text = string.Empty;
                txtpib.Text = string.Empty;
                Session["zahtev-promena-statusa-sertifikata-event_controle"] = txtimezz;
                SetFocusOnTextbox();
            }
            else if (entity.FullName == SelectedValue)
            {
                txtnazivpravnoglica.Text = entity.FullName;
                txtpib.Text = entity.PIB;
                Session["zahtev-promena-statusa-sertifikata-event_controle"] = txtimezz;
                SetFocusOnTextbox();
            }
        }
    }


    protected void cvLegalEntityName_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string errMessage = string.Empty;
            string SelectedValue = ddlLegalEntityName.SelectedItem.Text;
            Utility utility = new Utility();
            string FullNameDefault = utility.getItemText(Constants.DefaultIdItemLegal);

            args.IsValid = UtilsValidation.ValidateLegalEntityName(FullNameDefault, SelectedValue, out errMessage);
            cvLegalEntityName.ErrorMessage = errMessage;

        }
        catch (Exception)
        {
            cvLegalEntityName.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void rbChooseName_CheckedChanged(object sender, EventArgs e)
    {
        rbChooseNameCheckedChanged();
    }
    /*
    protected void rbWriteName_CheckedChanged(object sender, EventArgs e)
    {
        //rbWriteNameCheckedChanged();
    }

    protected void cvwritename_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string newLegalNameformat = string.Empty;
            string errMessage = string.Empty;
            string newLegalName = txtwritename.Text;
            args.IsValid = UtilsValidation.ValidateNazivPravnogLica(newLegalName, Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-txtwritenameIsRequired"]), Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-TurnOnWriteNameValidation"]), out errMessage, out newLegalNameformat);
            cvwritename.ErrorMessage = errMessage;
        }
        catch (Exception)
        {
            cvwritename.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }
    */
    protected void rbChooseNameCheckedChanged()
    {
        Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"] = true;

        //rbChooseName.Checked = true;
        //rbWriteName.Checked = false;
        Utility utility = new Utility();
        ddlLegalEntityName.SelectedIndex = ddlLegalEntityName.Items.IndexOf(ddlLegalEntityName.Items.FindByText(utility.getItemText(Constants.DefaultIdItemLegal)));
        ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"]);

        ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlLegalEntityName.CssClass = SetCss5;
        //txtwritename.ReadOnly = true;
        //txtwritename.BackColor = ColorTranslator.FromHtml(SetLightGray);
        //txtwritename.CssClass = SetCss5;
        cvLegalEntityName.Enabled = true;
        //cvwritename.Enabled = false;
    }
    /*
    protected void rbWriteNameCheckedChanged()
    {
        Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"] = false;

        //rbWriteName.Checked = true;
        rbChooseName.Checked = false;
        Utility utility = new Utility();
        ddlLegalEntityName.SelectedIndex = ddlLegalEntityName.Items.IndexOf(ddlLegalEntityName.Items.FindByText(utility.getItemText(Constants.DefaultIdItemLegal)));
        ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-promena-statusa-sertifikata-ddlLegalEntityName"]);

        ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetLightGray);
        ddlLegalEntityName.CssClass = SetCss5;
        txtwritename.ReadOnly = false;
        txtwritename.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtwritename.CssClass = SetCss5;
        cvLegalEntityName.Enabled = false;
        cvwritename.Enabled = true;

        ClearTextboxesAndDropDownLists();
    }
    */

    public void SetFocusOnTextbox()
    {
        try
        {
            if (Session["zahtev-promena-statusa-sertifikata-event_controle"] != null)
            {
                TextBox controle = (TextBox)Session["zahtev-promena-statusa-sertifikata-event_controle"];
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
            if (Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"] != null)
            {
                RadioButton radiodugme = (RadioButton)Session["zahtev-promena-statusa-sertifikata-event_controle-RadioButton"];
                //radiodugme.Focus();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "foc", "document.getElementById('" + radiodugme.ClientID + "').focus();", true);
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
            if (Session["zahtev-promena-statusa-sertifikata-event_controle-DropDownList"] != null)
            {
                DropDownList padajucalista = (DropDownList)Session["zahtev-promena-statusa-sertifikata-event_controle-DropDownList"];
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