using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BlueXSOAP;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using log4net;
using System.Threading.Tasks;

public partial class zahtev_izdavanje_fizicko_lice_stranac_bez_JMBG : System.Web.UI.Page
{
    public string SetDarkGray = Constants.SetDarkGray;
    public string SetLightGray = Constants.SetLightGray;
    public string SetWhite = Constants.SetWhite;
    public string SetCss = Constants.SetCss;
    public string SetCss1 = Constants.SetCss1;
    public string SetCss2 = Constants.SetCss2;
    public string SetCss4 = Constants.SetCss4;
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
    //promenljive za validaciju web servisa
    public bool TurnOnAjaxValidation;
    public bool TurnOnJMBGValidation;
    public bool TurnOnEmailValidation;
    public bool TurnOnPhoneValidation;

    public List<PTTVariable> PTTVariables;
    public string CityVariable;
    public string StreetVariable;
    public string HouseNumberVariable;
    public string ZipCodeVariable;
    public string PAKVariable;
    public bool InHouseVariable;
    public bool IsAllowedVariable;

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
            AvoidCashing();
            ShowDatepicker();

            if (!Page.IsPostBack)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzave"] = true;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresa"] = true;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrok"] = true;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlmedij"] = true;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanja"] = true;

                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumrodjenja"] = string.Empty;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumizdavanjapasosa"] = string.Empty;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumistekapasosa"] = string.Empty;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumzahteva"] = string.Empty;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-cenasaporezom"] = string.Empty;

                string IDItem2 = string.Empty;
                string IDItem3 = string.Empty;
                string IDItem4 = string.Empty;
                string IDItem5 = string.Empty;
                SetUpDefaultItem(ddlsertadresa.ClientID, out IDItem2);
                ddlsertadresa.SelectedValue = IDItem2;
                SetUpDefaultItem(ddlrokkoriscenjasert.ClientID, out IDItem3);
                ddlrokkoriscenjasert.SelectedValue = IDItem3;
                SetUpDefaultItem(ddlmedijsert.ClientID, out IDItem4);
                ddlmedijsert.SelectedValue = IDItem4;
                SetUpDefaultItem(ddlnacinplacanja.ClientID, out IDItem5);
                ddlnacinplacanja.SelectedValue = IDItem5;

                txtcenasaporezom.Text = string.Empty;
                myDiv6.Visible = false;
                myDiv8.Visible = false;
                txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
                txtcenasaporezom.ReadOnly = true;
                //txtdatumrodjenja.ReadOnly = true;
                //txtdatumizdavanjapasosa.ReadOnly = true;
                //txtdatumistekapasosa.ReadOnly = true;
                //-------TABINDEX---------------
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle"] = txtime;
                txtmesto.TabIndex = -1;
                txtulica.TabIndex = -1;
                txtbroj.TabIndex = -1;
                txtpostanskibroj.TabIndex = -1;
                txtpak.TabIndex = -1;
                SetFocusOnTextbox();
                //-----------------------------
                GetAllControlOnPage();
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

    private void GetAllControlOnPage()
    {
        try
        {
            //Get Control on all page
            SetUpWSPWrapperService();
            log.Info("successfully set WSPWrapperService Validation!");
            SetUpValidation();
            log.Info("successfully set Validation!");
            SetUpIsRequiredTextBoxes();
            log.Info("successfully set RequiredTextBoxes!");
            SetUpIsRequiredDropDownLists();
            log.Info("successfully set RequiredDropDownLists!...Application Starting, successfully get all controls!");
        }
        catch (Exception ex)
        {
            log.Error("Error in GetAllControlOnPage. " + ex);
        }
    }

    private void AvoidCashing()
    {
        Response.Cache.SetNoStore();
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    private void getUserAgentInformation(out string userAgentBrowser, out string userAgentStringApplicant, out string userAgentOS, out string userAgentIP)
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
        //string IP = string.Empty;
        //Utils.GetIP(out IP);
        //userAgentIP = IP;
        //todo Ovom funkcijom ce se dobiti javna adresa lokalno - kad se publishuje na IIS        
        userAgentIP = Utils.GetIPAddress();
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
        }

        ddlimedrzave.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzave"]);
        ddlimedrzave.CssClass = SetCss;
        ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresa"]);
        ddlsertadresa.CssClass = SetCss4;
        ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrok"]);
        ddlrokkoriscenjasert.CssClass = SetCss1;
        ddlmedijsert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlmedij"]);
        ddlmedijsert.CssClass = SetCss1;
        ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanja"]);
        ddlnacinplacanja.CssClass = SetCss1;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        GetAllControlOnPage();
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

    //-----------------SetUpWSPWrapperService------------------------
    //---------------------------------------------------------------
    protected void SetUpWSPWrapperService()
    {
        Utility utility = new Utility();
        string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_WSPWrapperService);
        TurnOnAjaxValidation = true;

        if (SettingValue == Constants.SETTING_VALUE_TRUE)
        {
            ValidateAjax(TurnOnAjaxValidation);
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-SetUpWSPWrapperService"] = TurnOnAjaxValidation;
        }
        else
        {
            ValidateAjax(!TurnOnAjaxValidation);
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-SetUpWSPWrapperService"] = !TurnOnAjaxValidation;
        }
        log.Debug("SetUpWSPWrapperService parameters. SettingValue is " + SettingValue + " . TurnOnAjaxValidation " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-SetUpWSPWrapperService"].ToString().ToLower());
    }

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
                if (control.Id == txtmesto.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnCityValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtadresaeposte.ClientID)
                {
                    TurnOnEmailValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnEmailValidation"] = TurnOnEmailValidation;
                }
                else if (control.Id == txttelefon.ClientID)
                {
                    TurnOnPhoneValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPhoneValidation"] = TurnOnPhoneValidation;
                }
                else if (control.Id == txtime.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtprezime.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnSurnameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtbrojpasosa.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPassportNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtulica.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnStreetValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtbroj.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnHouseNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpostanskibroj.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPostNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpak.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPAKValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtdatumizdavanjapasosa.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnIssueDateValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtdatumistekapasosa.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnExpiryDateValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            TurnOnEmailValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnEmailValidation"] = TurnOnEmailValidation;
            TurnOnPhoneValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPhoneValidation"] = TurnOnPhoneValidation;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnCityValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnSurnameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPassportNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnStreetValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnHouseNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPostNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPAKValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnIssueDateValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnExpiryDateValidation"] = Constants.VALIDATION_FALSE;
        }

        log.Debug("SetUpValidation parameters. GLOBAL_VALIDATION " + SettingValue + " . All controls: txtmesto " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnCityValidation"].ToString().ToLower() +
                                                                                            " txtadresaeposte " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnEmailValidation"].ToString().ToLower() +
                                                                                            " txttelefon " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPhoneValidation"].ToString().ToLower() +
                                                                                            " txtime " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnNameValidation"].ToString().ToLower() +
                                                                                            " txtprezime " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnSurnameValidation"].ToString().ToLower() +
                                                                                            " txtbrojpasosa " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPassportNumberValidation"].ToString().ToLower() +
                                                                                            " txtulica " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnStreetValidation"].ToString().ToLower() +
                                                                                            " txtbroj " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnHouseNumberValidation"].ToString().ToLower() +
                                                                                            " txtpostanskibroj " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPostNumberValidation"].ToString().ToLower() +
                                                                                            " txtpak " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPAKValidation"].ToString().ToLower() +
                                                                                            " txtdatumizdavanjapasosa " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnIssueDateValidation"].ToString().ToLower() +
                                                                                            " txtdatumistekapasosa " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnExpiryDateValidation"].ToString().ToLower());

    }

    protected void SetUpIsRequiredTextBoxes()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrolePoTipu(page, Constants.CONTROL_TYPE_TEXTBOX);

        foreach (var control in Controls)
        {
            if (control.Id == txtime.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezime.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtprezimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumrodjenja.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumrodjenjaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtadresaeposte.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtadresaeposteIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txttelefon.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txttelefonIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbrojpasosa.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtbrojpasosaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumizdavanjapasosa.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumizdavanjapasosaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumistekapasosa.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumistekapasosaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtmesto.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtmestoIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtulica.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtulicaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbroj.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtbrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpostanskibroj.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtpostanskibrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpak.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtpakIsRequired"] = control.IsRequired;
            }
        }

        log.Debug("SetUpIsRequiredTextBoxes parameters. All controls: txtime " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtimeIsRequired"].ToString().ToLower() +
                                                                    " txtprezime " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtprezimeIsRequired"].ToString().ToLower() +
                                                                    " txtdatumrodjenja " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumrodjenjaIsRequired"].ToString().ToLower() +
                                                                    " txtbrojpasosa " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtbrojpasosaIsRequired"].ToString().ToLower() +
                                                                    " txtdatumizdavanjapasosa " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumizdavanjapasosaIsRequired"].ToString().ToLower() +
                                                                    " txtdatumistekapasosa " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumistekapasosaIsRequired"].ToString().ToLower() +
                                                                    " txtadresaeposte " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtadresaeposteIsRequired"].ToString().ToLower() +
                                                                    " txttelefon " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txttelefonIsRequired"].ToString().ToLower() +
                                                                    " txtulica " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtulicaIsRequired"].ToString().ToLower() +
                                                                    " txtbroj " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtbrojIsRequired"].ToString().ToLower() +
                                                                    " txtpostanskibroj " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtpostanskibrojIsRequired"].ToString().ToLower() +
                                                                    " txtpak " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtpakIsRequired"].ToString().ToLower() + 
                                                                    " txtmesto " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtmestoIsRequired"].ToString().ToLower());
    }

    protected void ValidateAjax(bool TurnOnAjaxValidation)
    {
        autoCompleteMestoBoravka.Enabled = TurnOnAjaxValidation;
        autoCompleteUlicaBoravka.Enabled = TurnOnAjaxValidation;
    }

    protected void SetUpIsRequiredDropDownLists()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrolePoTipu(page, Constants.CONTROL_TYPE_DROPDOWNLIST);

        foreach (var control in Controls)
        {
            if (control.Id == ddlimedrzave.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzaveIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlsertadresa.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlrokkoriscenjasert.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrokkoriscenjasertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlmedijsert.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlmedijsertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlnacinplacanja.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanjaIsRequired"] = control.IsRequired;
            }
        }

        log.Debug("SetUpIsRequiredDropDownLists parameters. All controls: ddlimedrzave " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzaveIsRequired"].ToString().ToLower() +
                                                            " ddlsertadresa " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresaIsRequired"].ToString().ToLower() +
                                                            " ddlrokkoriscenjasert " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrokkoriscenjasertIsRequired"].ToString().ToLower() +
                                                            " ddlmedijsert " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlmedijsertIsRequired"].ToString().ToLower() +
                                                            " ddlnacinplacanja " + Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanjaIsRequired"].ToString().ToLower());

    }

    //---------------------------------------------------------------
    //---------------------------------------------------------------   
    //---------------------------------------------------------------
    //---------------------------------------------------------------

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


    protected void ShowDatepicker()
    {
        //call function pickdate() every time after PostBack in ASP.Net
        ScriptManager.RegisterStartupScript(this, GetType(), "", "pickdate();", true);
        //Avoid: jQuery DatePicker TextBox selected value Lost after PostBack in ASP.Net
        txtdatumrodjenja.Text = Request.Form[txtdatumrodjenja.UniqueID];
        txtdatumizdavanjapasosa.Text = Request.Form[txtdatumizdavanjapasosa.UniqueID];
        txtdatumistekapasosa.Text = Request.Form[txtdatumistekapasosa.UniqueID];
    }

    protected void ddlsertadresa_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlsertadresa.SelectedValue);
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string ddlstring = ddlsertadresa.ClientID;
        //Get PTT Variables from Database
        SetUpPTTVariables(SelectedValue, page, ddlstring, out CityVariable, out StreetVariable, out HouseNumberVariable, out ZipCodeVariable, out PAKVariable, out InHouseVariable, out IsAllowedVariable);

        if (InHouseVariable && IsAllowedVariable)
        {
            ValidateAjax(false);
            Colorgray();
            txtulica.ReadOnly = InHouseVariable;
            txtulica.Text = StreetVariable;
            txtbroj.ReadOnly = InHouseVariable;
            txtbroj.Text = HouseNumberVariable;
            txtpostanskibroj.ReadOnly = InHouseVariable;
            txtpostanskibroj.Text = ZipCodeVariable;
            txtpak.ReadOnly = InHouseVariable;
            txtpak.Text = PAKVariable;
            txtmesto.ReadOnly = InHouseVariable;
            txtmesto.Text = CityVariable;
        }
        else if (!InHouseVariable && IsAllowedVariable)
        {
            ValidateAjax(Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-SetUpWSPWrapperService"]));
            Colorchange();
            txtulica.ReadOnly = false;
            txtulica.Text = StreetVariable;
            txtbroj.ReadOnly = false;
            txtbroj.Text = HouseNumberVariable;
            txtpostanskibroj.ReadOnly = false;
            txtpostanskibroj.Text = ZipCodeVariable;
            txtpak.ReadOnly = false;
            txtpak.Text = PAKVariable;
            txtmesto.ReadOnly = false;
            txtmesto.Text = CityVariable;
            txtmesto.TabIndex = 0;
            txtulica.TabIndex = 0;
            txtbroj.TabIndex = 0;
            txtpostanskibroj.TabIndex = 0;
            txtpak.TabIndex = 0;
        }
        else if (!InHouseVariable && !IsAllowedVariable)
        {
            ValidateAjax(false);
            Colorgray();
            txtulica.ReadOnly = true;
            txtulica.Text = StreetVariable;
            txtbroj.ReadOnly = true;
            txtbroj.Text = HouseNumberVariable;
            txtpostanskibroj.ReadOnly = true;
            txtpostanskibroj.Text = ZipCodeVariable;
            txtpak.ReadOnly = true;
            txtpak.Text = PAKVariable;
            txtmesto.ReadOnly = true;
            txtmesto.Text = CityVariable;
            txtmesto.TabIndex = -1;
            txtulica.TabIndex = -1;
            txtbroj.TabIndex = -1;
            txtpostanskibroj.TabIndex = -1;
            txtpak.TabIndex = -1;
        }
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    //-----------------SetUp PTT VAriables---------------------------
    //---------------------------------------------------------------
    protected void SetUpPTTVariables(int SelectedValue, string page, string ddlstring, out string CityVariable, out string StreetVariable, out string HouseNumberVariable, out string ZipCodeVariable, out string PAKVariable, out bool InHouseVariable, out bool IsAllowedVariable)
    {
        CityVariable = string.Empty;
        StreetVariable = string.Empty;
        HouseNumberVariable = string.Empty;
        ZipCodeVariable = string.Empty;
        PAKVariable = string.Empty;
        InHouseVariable = false;
        IsAllowedVariable = false;

        Utility utilityPTT = new Utility();
        PTTVariables = utilityPTT.pronadjiPromenljivePTT(page, ddlstring);

        foreach (var pttvariables in PTTVariables)
        {
            if (pttvariables.IDItem == SelectedValue)
            {
                CityVariable = pttvariables.City;
                StreetVariable = pttvariables.Street;
                HouseNumberVariable = pttvariables.HouseNumber;
                ZipCodeVariable = pttvariables.ZIPCode;
                PAKVariable = pttvariables.PAK;
                InHouseVariable = pttvariables.InHouse;
                IsAllowedVariable = pttvariables.IsAllowed;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-CityVariable"] = CityVariable;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-StreetVariable"] = StreetVariable;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-HouseNumberVariable"] = HouseNumberVariable;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ZipCodeVariable"] = ZipCodeVariable;
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-PAKVariable"] = PAKVariable;
            }
            else
            { }
        }
    }
    //---------------------------------------------------------------
    //---------------------------------------------------------------

    private void Colorchange()
    {
        /*
        //menja se boja teksta u textbox-u
        txtulica.ForeColor = Color.Gray;
        txtbroj.ForeColor = Color.Gray;
        txtpostanskibroj.ForeColor = Color.Gray;
        txtpak.ForeColor = Color.Gray;
        txtmesto.ForeColor = Color.Gray;
        */
        //menja se boja background textbox-a
        txtulica.BackColor = Color.White;
        txtbroj.BackColor = Color.White;
        txtpostanskibroj.BackColor = Color.White;
        txtpak.BackColor = Color.White;
        txtmesto.BackColor = Color.White;
    }

    private void Colorgray()
    {
        //menja se boja background textbox-a
        txtulica.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtbroj.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtpostanskibroj.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtpak.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtmesto.BackColor = ColorTranslator.FromHtml(SetLightGray);
    }

    private string GetPrice(int ItemMedia, int ItemValidity)
    {
        string Price = string.Empty;
        int CertificateType = 0;

        ItemBLL itemBLL = new ItemBLL();

        try
        {
            CertificateType = itemBLL.GetItemIdForTypeAndValue(Constants.KVALIFIKOVANI_ELEKTRONSKI_SERTIFIKAT, Constants.ITEM_CERTIFICATE_TYPE);
            Price = itemBLL.GetItemPrice(CertificateType, ItemMedia, ItemValidity);
        }
        catch (Exception ex)
        {

            log.Error("Error while getting CertificateType and Price. " + ex.Message);
        }

        return Price;
    }

    protected void SetPrice()
    {
        string price = GetPrice(Convert.ToInt32(ddlmedijsert.SelectedValue), Convert.ToInt32(ddlrokkoriscenjasert.SelectedValue));
        txtcenasaporezom.Text = price;
    }

    protected void ddlrokkoriscenjasert_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetPrice();
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ddlmedijsert_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetPrice();
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ddlnacinplacanja_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlnacinplacanja.SelectedValue);
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ddlimedrzave_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlimedrzave.SelectedValue);
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }


    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Telefonskog BROJA----------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txttelefon_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged1();
        if (errLabelNumber.Text != string.Empty)
        {
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle"] = txtbrojpasosa;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged1()
    {
        string newNumber = txttelefon.Text;
        string errorMessage = string.Empty;
        string numberformat = string.Empty;
        bool LegalEntityPhone = true;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPhoneValidation"]))
        {
            UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txttelefonIsRequired"]), LegalEntityPhone, out errorMessage, out numberformat);
            errLabelNumber.Text = errorMessage;
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
        bool LegalEntityPhone = true;
        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPhoneValidation"]))
        {
            args.IsValid = UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
            txttelefon.Text = numberformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
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
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle"] = txttelefon;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged2()
    {
        string newMail = txtadresaeposte.Text;
        string errorMessage = string.Empty;
        string mailformat = string.Empty;
        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnEmailValidation"]))
        {
            UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
            errLabelMail.Text = errorMessage;
            //Ne moze da se prikaze poruka prilikom promene fokusa
            txtadresaeposte.Text = mailformat;
        }
        else
        {
            newMail = txtadresaeposte.Text;
            errorMessage = string.Empty;
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
            args.IsValid = UtilsValidation.ValidateName(txtime.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtimeIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnNameValidation"]), out ErrorMessage1, out nameformat);
            cvime.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvime.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvprezime_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string surnameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateSurname(txtprezime.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtprezimeIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnSurnameValidation"]), out ErrorMessage1, out surnameformat);
            cvprezime.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvprezime.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvbrojpasosa_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string passportnumberformat = string.Empty;
            args.IsValid = UtilsValidation.ValidatePassportNumber(txtbrojpasosa.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtbrojpasosaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPassportNumberValidation"]), out ErrorMessage1, out passportnumberformat);
            cvbrojpasosa.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvbrojpasosa.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvimederzave_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlimedrzaveString = ddlimedrzave.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlimedrzaveString, out IDItem);

            args.IsValid = UtilsValidation.ValidateImeDrzave(ddlimedrzave.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzaveIsRequired"]) ,IDItem, out ErrorMessage1);
            cvimederzave.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvimederzave.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdatumrodjenja_ServerValidate(object source, ServerValidateEventArgs args)
    {
        Utility utility = new Utility();
        try
        {           
            if (txtdatumrodjenja.Text != string.Empty)
            {
                DateTime datumrodjenja = DateTime.ParseExact(txtdatumrodjenja.Text, "dd.MM.yyyy", null);
                log.Info("datumrodjenja je: " + datumrodjenja);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateDateOfBirth(datumrodjenja, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumrodjenjaIsRequired"]), out ErrorMessage1);
                cvdatumrodjenja.ErrorMessage = ErrorMessage1;
            }
            else
            {
                if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumrodjenjaIsRequired"]))
                {
                    if (txtdatumrodjenja.Text == string.Empty)
                    {
                        cvdatumrodjenja.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
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
            log.Error("Greska prilikom validacije cvdatumrodjenja. " + ex.Message);            
            txtdatumrodjenja.Text = string.Empty;
            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumrodjenjaIsRequired"]))
            {
                cvdatumrodjenja.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
            }
            else
            {
                cvdatumrodjenja.ErrorMessage = string.Empty;
            }
            args.IsValid = false;
        }
    }

    protected void cvadresaeposte_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string mailformat = string.Empty;

            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnEmailValidation"]))
            {
                args.IsValid = UtilsValidation.ValidateMail(txtadresaeposte.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtadresaeposteIsRequired"]), out ErrorMessage1, out mailformat);
                cvadresaeposte.ErrorMessage = ErrorMessage1;
            }
            else
            {
                args.IsValid = UtilsValidation.WithoutValidationMail(txtadresaeposte.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtadresaeposteIsRequired"]), out ErrorMessage1, out mailformat);
                cvadresaeposte.ErrorMessage = ErrorMessage1;
            }
        }
        catch (Exception)
        {
            cvadresaeposte.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdatumizdavanjapasosa_ServerValidate(object source, ServerValidateEventArgs args)
    {
        Utility utility = new Utility();
        try
        {           
            if (txtdatumizdavanjapasosa.Text != string.Empty)
            { 
                DateTime datumizdavanja = DateTime.ParseExact(txtdatumizdavanjapasosa.Text, "dd.MM.yyyy", null);
                log.Info("datumizdavanjaPasosa je: " + datumizdavanja);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateIssuingDate(datumizdavanja, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumizdavanjapasosaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnIssueDateValidation"]), out ErrorMessage1);
                cvdatumizdavanjapasosa.ErrorMessage = ErrorMessage1;
            }       
            else
            {
                if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumizdavanjapasosaIsRequired"]))
                {
                    if (txtdatumizdavanjapasosa.Text == string.Empty)
                    {
                        cvdatumizdavanjapasosa.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
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
            log.Error("Greska prilikom validacije cvdatumizdavanjaPasosa. " + ex.Message);
            txtdatumizdavanjapasosa.Text = string.Empty;
            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumizdavanjapasosaIsRequired"]))
            {
                cvdatumizdavanjapasosa.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
            }
            else
            { 
                cvdatumizdavanjapasosa.ErrorMessage = string.Empty;
            }
            args.IsValid = false;
        }
    }

    protected void cvdatumistekapasosa_ServerValidate(object source, ServerValidateEventArgs args)
    {
        Utility utility = new Utility();
        try
        {            
            if (txtdatumistekapasosa.Text != string.Empty)
            {
                DateTime datumisteka = DateTime.ParseExact(txtdatumistekapasosa.Text, "dd.MM.yyyy", null);
                log.Info("datumistekaPasosa je: " + datumisteka);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateExpiryDate(datumisteka, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumistekapasosaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnExpiryDateValidation"]), out ErrorMessage1);
                cvdatumistekapasosa.ErrorMessage = ErrorMessage1;
            }
            else
            {
                if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumistekapasosaIsRequired"]))
                {
                    if (txtdatumistekapasosa.Text == string.Empty)
                    {
                        cvdatumistekapasosa.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
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
            log.Error("Greska prilikom validacije cvdatumistekaPasosa. " + ex.Message);            
            txtdatumistekapasosa.Text = string.Empty;
            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtdatumistekapasosaIsRequired"]))
            {
                cvdatumistekapasosa.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
            }
            else
            {
                cvdatumistekapasosa.ErrorMessage = string.Empty;
            }
            args.IsValid = false;
        }
    }

    protected void cvsertadresa_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlsertadresaString = ddlsertadresa.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlsertadresaString, out IDItem);

            args.IsValid = UtilsValidation.ValidateSertAdresa(ddlsertadresa.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresaIsRequired"]), IDItem, out ErrorMessage1);
            cvsertadresa.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvsertadresa.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvmesto_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string cityformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateCity(txtmesto.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtmestoIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnCityValidation"]), out ErrorMessage1, out cityformat);
            cvmesto.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvmesto.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvulica_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string streetformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateStreet(txtulica.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtulicaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnStreetValidation"]), out ErrorMessage1, out streetformat);
            cvulica.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvulica.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvbroj_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string housenumberformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateHouseNumber(txtbroj.Text, errLabelBroj.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtbrojIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnHouseNumberValidation"]), out ErrorMessage1, out housenumberformat);
            cvbroj.ErrorMessage = ErrorMessage1;
            errLabelBroj.Text = string.Empty;
        }
        catch (Exception)
        {
            cvbroj.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvpostanskibroj_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string postnumberformat = string.Empty;
            args.IsValid = UtilsValidation.ValidatePostNumber(txtpostanskibroj.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtpostanskibrojIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPostNumberValidation"]), out ErrorMessage1, out postnumberformat);
            cvpostanskibroj.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvpostanskibroj.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvpak_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string pakformat = string.Empty;
            args.IsValid = UtilsValidation.ValidatePAK(txtpak.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-txtpakIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-TurnOnPAKValidation"]), out ErrorMessage1, out pakformat);
            cvpak.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvpak.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvrokkoriscenjasert_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlrokkoriscenjasertString = ddlrokkoriscenjasert.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlrokkoriscenjasertString, out IDItem);

            args.IsValid = UtilsValidation.ValidateRok(ddlrokkoriscenjasert.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrokkoriscenjasertIsRequired"]), IDItem, out ErrorMessage1);
            cvrokkoriscenjasert.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvrokkoriscenjasert.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvmedijsert_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlmedijsertString = ddlmedijsert.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlmedijsertString, out IDItem);

            args.IsValid = UtilsValidation.ValidateMedij(ddlmedijsert.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlmedijsertIsRequired"]), IDItem, out ErrorMessage1);
            cvmedijsert.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvmedijsert.ErrorMessage = string.Empty;
        }
    }

    protected void cvnacinplacanja_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlnacinplacanjaString = ddlnacinplacanja.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlnacinplacanjaString, out IDItem);

            args.IsValid = UtilsValidation.ValidateNacinPlacanja(ddlnacinplacanja.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanjaIsRequired"]), IDItem, out ErrorMessage1);
            cvnacinplacanja.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvnacinplacanja.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }


    protected void btnSubmit_Click1(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "successalert();", true);
                if (true)
                {
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzave"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresa"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrok"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlmedij"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanja"] = false;

                    ValidateAjax(false);

                    txtime.ReadOnly = true;
                    txtime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtprezime.ReadOnly = true;
                    txtprezime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtdatumrodjenja.ReadOnly = true;
                    txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtadresaeposte.ReadOnly = true;
                    txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txttelefon.ReadOnly = true;
                    txttelefon.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtbrojpasosa.ReadOnly = true;
                    txtbrojpasosa.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlimedrzave.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzave"]);
                    ddlimedrzave.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlimedrzave.CssClass = SetCss;
                    txtdatumizdavanjapasosa.ReadOnly = true;
                    txtdatumizdavanjapasosa.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtdatumistekapasosa.ReadOnly = true;
                    txtdatumistekapasosa.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresa"]);
                    ddlsertadresa.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlsertadresa.CssClass = SetCss4;
                    txtulica.ReadOnly = true;
                    txtulica.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtbroj.ReadOnly = true;
                    txtbroj.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtpostanskibroj.ReadOnly = true;
                    txtpostanskibroj.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtpak.ReadOnly = true;
                    txtpak.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtmesto.ReadOnly = true;
                    txtmesto.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrok"]);
                    ddlrokkoriscenjasert.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlrokkoriscenjasert.CssClass = SetCss1;
                    ddlmedijsert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlmedij"]);
                    ddlmedijsert.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlmedijsert.CssClass = SetCss1;
                    ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanja"]);
                    ddlnacinplacanja.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtcenasaporezom.ReadOnly = true;
                    ddlnacinplacanja.CssClass = SetCss1;
                    txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    myDiv6.Visible = true;
                    myDiv5.Visible = false;
                    myDiv8.Visible = true;

                    ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumrodjenja"] = txtdatumrodjenja.Text;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumizdavanjapasosa"] = txtdatumizdavanjapasosa.Text;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumistekapasosa"] = txtdatumistekapasosa.Text;
                    txtdatumzahteva.Text = DateTime.Now.ToString("dd.MM.yyyy");
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumzahteva"] = txtdatumzahteva.Text;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-cenasaporezom"] = txtcenasaporezom.Text;

                    //-----------------GetUserAgent string---------------------------                    
                    Utility utility = new Utility();
                    string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_GEOLOCATIONS);
                    if (SettingValue == Constants.SETTING_VALUE_TRUE)
                    {
                        try
                        {
                            getUserAgentInformation(out userAgentBrowser, out userAgentStringApplicant, out userAgentOS, out userAgentIP);
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
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentBrowser"] = userAgentBrowser;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentStringApplicant"] = userAgentStringApplicant;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentOS"] = userAgentOS;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentIP"] = userAgentIP;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentContinent"] = userAgentContinent;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentCountry"] = userAgentCountry;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentCountryCode"] = userAgentCountryCode;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentCity"] = userAgentCity;
                    Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentISP"] = userAgentISP;                    
                }
            }
            else if (!Page.IsValid)
            {
                errLabelNumber.Text = string.Empty;
                errLabelMail.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
            }
        }
        catch (Exception ex)
        {
            log.Error("Button submit error. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    //--------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------

    //public string BrojZahteva = Utils.Generate15UniqueDigits();
    public string BrojZahteva = string.Empty;
    public string BrojZahtevaPravnoLice = string.Empty;
    public bool isResponseZero = true;

    protected BxSoapEnvelope createSoapEnvelope(Utility utility)
    {
        BxSoapEnvelope envelope = new BxSoapEnvelopeRequestToken();

        envelope.BxData.setValue(@"productionProfile", @"Qualified Electronic Certificate Foreigners");
        envelope.BxData.setValue(@"givenName", txtime.Text);
        envelope.BxData.setValue(@"lastName", txtprezime.Text);
        envelope.BxData.setValue(@"includeUniqueCitizensNumber", ((utility.getItemValueAddedTax(Convert.ToInt32(Constants.INCLUDE_UNIQUE_CITIZENS_NUMBER_NO))).ToString()));
        envelope.BxData.setValue(@"dateOfBirth", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumrodjenja"].ToString());
        envelope.BxData.setValue(@"emailAddress", txtadresaeposte.Text);
        envelope.BxData.setValue(@"phoneNumber", txttelefon.Text);
        envelope.BxData.setValue(@"identificationDocumentType", (utility.getEnglishText(Constants.PASSPORT))); //Fiksirano jer je samo Pasoš, nema lične karte
        envelope.BxData.setValue(@"identificationDocumentNumber", txtbrojpasosa.Text);
        envelope.BxData.setValue(@"identificationIssuer", (utility.getCountryCode(Convert.ToInt32(ddlimedrzave.SelectedValue)))); //send CountryCode
        envelope.BxData.setValue(@"identificationIssuerName", (utility.getItemText(Convert.ToInt32(ddlimedrzave.SelectedValue)))); //send Country Name
        envelope.BxData.setValue(@"paymentMethod", (utility.getEnglishText(Convert.ToInt32(ddlnacinplacanja.SelectedValue))));
        envelope.BxData.setValue(@"totalPrice", txtcenasaporezom.Text);
        envelope.BxData.setValue(@"identificationDocumentValidFrom", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumizdavanjapasosa"].ToString());
        envelope.BxData.setValue(@"identificationDocumentValidUntil", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumistekapasosa"].ToString());
        envelope.BxData.setValue(@"deliveryLocation", (utility.getEnglishText(Convert.ToInt32(ddlsertadresa.SelectedValue))));
        envelope.BxData.setValue(@"distributionCity", txtmesto.Text);
        envelope.BxData.setValue(@"distributionStreet", txtulica.Text);
        envelope.BxData.setValue(@"distributionHouseNumber", txtbroj.Text);
        envelope.BxData.setValue(@"distributionPostalCode", txtpostanskibroj.Text);
        envelope.BxData.setValue(@"distributionPAK", txtpak.Text);
        envelope.BxData.setValue(@"media", (utility.getEnglishText(Convert.ToInt32(ddlmedijsert.SelectedValue))));
        envelope.BxData.setValue(@"validity", (utility.getEnglishText(Convert.ToInt32(ddlrokkoriscenjasert.SelectedValue))));
        //------------------------------------------------------------------------------            
        envelope.BxData.setValue(@"userAgentStringApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentStringApplicant"].ToString());
        envelope.BxData.setValue(@"ipApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentIP"].ToString());
        envelope.BxData.setValue(@"continentApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentContinent"].ToString());
        envelope.BxData.setValue(@"countryApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentCountry"].ToString());
        envelope.BxData.setValue(@"countryCodeApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentCountryCode"].ToString());
        envelope.BxData.setValue(@"cityApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentCity"].ToString());
        envelope.BxData.setValue(@"osApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentOS"].ToString());
        envelope.BxData.setValue(@"ispApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentISP"].ToString());
        envelope.BxData.setValue(@"browserApplicant", Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-userAgentBrowser"].ToString());
        envelope.BxData.setValue(@"ipOperator", string.Empty);
        return envelope;
    }

    protected string CreateDocumentIssuingIndividual(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess)
    {
        string[] parameterNames = {
                @"requestNumber",
                @"firstName",
                @"lastName",
                @"jmbg",
                @"email",
                @"street",
                @"streetNo",
                @"postNo",
                @"city",
                @"phone",
                @"expiry",
                @"hwMedium"
            };
        string[] parameterValues = {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-brojzahteva"].ToString(),
                txtime.Text,
                txtprezime.Text,
                Constants.withoutJMBG,
                txtadresaeposte.Text,
                txtulica.Text,
                txtbroj.Text,
                txtpostanskibroj.Text,
                txtmesto.Text,
                txttelefon.Text,
                (utility.getEnglishText(Convert.ToInt32(ddlrokkoriscenjasert.SelectedValue))),
                (utility.getEnglishText(Convert.ToInt32(ddlmedijsert.SelectedValue)))
            };

        PisMessServiceReference.Parameter[] parameters = new PisMessServiceReference.Parameter[parameterNames.Length];

        for (int i = 0; i < parameterNames.Length; i++)
        {
            parameters[i] = new PisMessServiceReference.Parameter()
            {
                ParameterName = parameterNames[i],
                ParameterValue = parameterValues[i]
            };
        }

        string[] response = new string[2];

        response = ServiceCaller.CallServiceCreateDoc(PisMessServiceReference.TemplateDocumentTypeSerbianPost.IssuingIndividual, parameters);

        string responseMessage = string.Empty;

        if (response[0].Equals("0"))
        {
            //success
            //Putanja do fajla
            //Parsirati putanju zbog naredne stranice, potreban naziv fajla
            responseMessage = response[1];
        }
        else
        {
            //error
            responseMessage = response[0];
            throw new Exception("Response error while creating IssuingIndividual document, response from PissMess: " + responseMessage);
        }

        return responseMessage;
    }

    protected string CreateDocumentPaymentOrder(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess)
    {
        string[] parameterNames = {
                @"requestNumber",
                @"price",
                @"firstName",
                @"lastName",
                @"purposeOfPayment"
            };
        string[] parameterValues = {
                Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-brojzahteva"].ToString(),
                txtcenasaporezom.Text,
                txtime.Text,
                txtprezime.Text,
                Constants.ISSUING
            };

        PisMessServiceReference.Parameter[] parameters = new PisMessServiceReference.Parameter[parameterNames.Length];

        for (int i = 0; i < parameterNames.Length; i++)
        {
            parameters[i] = new PisMessServiceReference.Parameter()
            {
                ParameterName = parameterNames[i],
                ParameterValue = parameterValues[i]
            };
        }

        string[] response = new string[2];

        response = ServiceCaller.CallServiceCreateDoc(PisMessServiceReference.TemplateDocumentTypeSerbianPost.PaymentOrder, parameters);

        string responseMessage = string.Empty;

        if (response[0].Equals("0"))
        {
            //success
            //Putanja do fajla
            //Parsirati putanju zbog naredne stranice, potreban naziv fajla
            responseMessage = response[1];
        }
        else
        {
            //error
            responseMessage = response[0];
            throw new Exception("Response error while creating IssuingIndividualPaymentOrder document, response from PissMess: " + responseMessage);
        }

        return responseMessage;
    }


    protected void btnEnterRequest_Click1(object sender, EventArgs e)
    {
        try
        {
            log.Info("Start sending SOAP message.");

            Utility utility = new Utility();

            BxSoapEnvelope envelope = createSoapEnvelope(utility);
            //envelope.createBxSoapEnvelope();  

            string SOAPresponse = BxSoap.SOAPManual(envelope.createBxSoapEnvelope());
            Utils.ParseSoapEnvelope(SOAPresponse, out BrojZahteva, out BrojZahtevaPravnoLice);

            if (BrojZahteva == string.Empty)
            {
                throw new Exception("RequestNumber is empty!");
            }

            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-brojzahteva"] = BrojZahteva;

            log.Info("Finished sending SOAP message! RequestNumber is: " + BrojZahteva);
            log.Info("Start creating PDF Files.");

            pisMess = new PisMessServiceReference.PisMessServiceClient();

            //string fileName = CreateDocumentIssuingIndividual(utility, pisMess);
            //fileName = CreateDocumentPaymentOrder(utility, pisMess);
            var CreateDocumentIssuingIndividualTask = Task.Run(() => CreateDocumentIssuingIndividual(utility, pisMess));
            var CreateDocumentPaymentOrderTask = Task.Run(() => CreateDocumentPaymentOrder(utility, pisMess));

            Task.WaitAll(new[] { CreateDocumentIssuingIndividualTask, CreateDocumentPaymentOrderTask });

            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-filename"] = CreateDocumentIssuingIndividualTask.Result;
            Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-filenamePaymentOrder"] = CreateDocumentPaymentOrderTask.Result;

            log.Info("Finished creating PDF files!");

            Response.Redirect("zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-podnet.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.

        }
        catch (AggregateException aex)
        {
            log.Error(aex.InnerException.Message, aex);
            throw aex;
        }
        catch (Exception ex)
        {
            log.Error("Error while sending request. " + ex.Message);
            //Disable datepicker
            txtdatumrodjenja.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumrodjenja"].ToString();
            txtdatumizdavanjapasosa.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumizdavanjapasosa"].ToString();
            txtdatumistekapasosa.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumistekapasosa"].ToString();
            ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);
        }                
    }

    protected void btnReEnterRequest_Click1(object sender, EventArgs e)
    {
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzave"] = true;
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresa"] = true;
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrok"] = true;
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlmedij"] = true;
        Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanja"] = true;

        txtime.ReadOnly = false;
        txtime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtprezime.ReadOnly = false;
        txtprezime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtdatumrodjenja.ReadOnly = true;
        txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtadresaeposte.ReadOnly = false;
        txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetWhite);
        txttelefon.ReadOnly = false;
        txttelefon.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtbrojpasosa.ReadOnly = false;
        txtbrojpasosa.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlimedrzave.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlimedrzave"]);
        ddlimedrzave.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlimedrzave.CssClass = SetCss;
        txtdatumizdavanjapasosa.ReadOnly = true;
        txtdatumizdavanjapasosa.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtdatumistekapasosa.ReadOnly = true;
        txtdatumistekapasosa.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlsertadresa"]);
        ddlsertadresa.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlsertadresa.CssClass = SetCss4;
        if (ddlsertadresa.SelectedValue == "7")
        {
            txtulica.ReadOnly = false;
            txtulica.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtbroj.ReadOnly = false;
            txtbroj.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtpostanskibroj.ReadOnly = false;
            txtpostanskibroj.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtpak.ReadOnly = false;
            txtpak.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtmesto.ReadOnly = false;
            txtmesto.BackColor = ColorTranslator.FromHtml(SetWhite);
        }
        else
        {
            txtulica.ReadOnly = true;
            txtulica.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtbroj.ReadOnly = true;
            txtbroj.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtpostanskibroj.ReadOnly = true;
            txtpostanskibroj.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtpak.ReadOnly = true;
            txtpak.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtmesto.ReadOnly = true;
            txtmesto.BackColor = ColorTranslator.FromHtml(SetLightGray);
        }
        ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlrok"]);
        ddlrokkoriscenjasert.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlrokkoriscenjasert.CssClass = SetCss1;
        ddlmedijsert.Enabled = true;
        ddlmedijsert.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlmedijsert.CssClass = SetCss1;
        ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-ddlnacinplacanja"]);
        ddlnacinplacanja.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlnacinplacanja.CssClass = SetCss1;
        txtcenasaporezom.ReadOnly = true;
        txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
        myDiv6.Visible = false;
        myDiv5.Visible = true;
        myDiv8.Visible = false;

        txtdatumrodjenja.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumrodjenja"].ToString();
        txtdatumizdavanjapasosa.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumizdavanjapasosa"].ToString();
        txtdatumistekapasosa.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumistekapasosa"].ToString();

        txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtcenasaporezom.ReadOnly = true;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    //--------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------
    protected void txtbroj_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged9();
    }

    private void CheckIfChannelHasChanged9()
    {
        Utility utility = new Utility();
        string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_VALIDATION);

        if (SettingValue == Constants.SETTING_VALUE_TRUE)
        {
            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-SetUpWSPWrapperService"]))
            {
                string PorukaKorisnik = string.Empty;
                string PostanskiBroj = string.Empty;
                string Pak = string.Empty;
                bool result = UtilsValidation.GetPostNumberAndPAK(txtmesto.Text, txtulica.Text, txtbroj.Text, out PorukaKorisnik, out PostanskiBroj, out Pak);
                txtpostanskibroj.Text = PostanskiBroj;
                txtpak.Text = Pak;
                errLabelBroj.Text = PorukaKorisnik;
                if (PorukaKorisnik == string.Empty)
                {
                    cvbroj.ErrorMessage = string.Empty;
                }
            }
        }
    }

    public void SetFocusOnTextbox()
    {
        try
        {
            if (Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle"] != null)
            {
                TextBox controle = (TextBox)Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle"];
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
            if (Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle-DropDownList"] != null)
            {
                DropDownList padajucalista = (DropDownList)Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-event_controle-DropDownList"];
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