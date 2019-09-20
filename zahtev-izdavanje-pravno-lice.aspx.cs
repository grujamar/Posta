using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using BlueXSOAP;
using log4net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public partial class zahtev_izdavanje_pravno_lice : System.Web.UI.Page
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
    public PisMessServiceReference.PisMessServiceClient pisMess;

    public List<WebControl> Controls;
    //promenljive za validaciju 
    public bool TurnOnRegistrationNumberValidation;
    public bool TurnOnPIBValidation;
    public bool TurnOnAjax1Validation;
    public bool TurnOnPhone1Validaiton;
    public bool TurnOnEmail1Validation;
    public bool TurnOnURLValidation;
    public bool TurnOnJMBGValidation;
    public bool TurnOnAjax2Validation;
    public bool TurnOnPhone2Validation;
    public bool TurnOnEmail2Validation;

    public List<PTTVariable> PTTVariables;
    public string CityVariable;
    public string StreetVariable;
    public string HouseNumberVariable;
    public string ZipCodeVariable;
    public string PAKVariable;
    public bool InHouseVariable;
    public bool IsAllowedVariable;
    public int IsLegalEntity;

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
                Session["zahtev-izdavanje-pravno-lice-ddlobveznikpdv"] = true;
                Session["zahtev-izdavanje-pravno-lice-ddlsertjmbg"] = true;
                Session["zahtev-izdavanje-pravno-lice-sertjmbg1"] = true;
                Session["zahtev-izdavanje-pravno-lice-vrstadokumenta"] = true;
                Session["zahtev-izdavanje-pravno-lice-sertadresa"] = true;
                Session["zahtev-izdavanje-pravno-lice-rokkoriscenja"] = true;
                Session["zahtev-izdavanje-pravno-lice-medijsert"] = true;

                Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"] = true;

                Session["zahtev-izdavanje-pravno-lice-rbChooseName"] = true;
                Session["zahtev-izdavanje-pravno-lice-rbWriteName"] = true;

                myDiv6.Visible = false;
                myDiv8.Visible = false;
                Container6.Visible = false;
                Container7.Visible = false;
                Container77.Visible = false;
                Container8.Visible = false;
                Container9.Visible = false;
                Div1.Visible = false;
                Div2.Visible = false;
                Div3.Visible = false;
                Container10.Visible = false;
                Container11.Visible = false;
                Container12.Visible = false;
                rowLegalEntityDDL.Visible = false;
                rowLegalEntityDDL1.Visible = false;
                rowLegalEntityDDL2.Visible = false;
                txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
                txtcenasaporezom.ReadOnly = true;
                txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetLightGray);
                txtcenasaporezom1.ReadOnly = true;
                txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetLightGray);
                txtdatumrodjenja.ReadOnly = true;
                //-------TABINDEX---------------
                Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtmaticnibroj;
                txtmesto1.TabIndex = -1;
                txtulica1.TabIndex = -1;
                txtbroj1.TabIndex = -1;
                txtpostanskibroj1.TabIndex = -1;
                txtpak1.TabIndex = -1;
                SetFocusOnTextbox();
                //------------------------------
                ScriptManager.RegisterStartupScript(this, GetType(), "DisabledButton", "DisabledButton();", true);

                SetInitialRow();
                GridView1.Columns[8].Visible = false;
                //Get Control on all page
                SetUpWSPWrapperService();
                log.Info("successfully set WSPWrapperService Validation!");
                SetUpValidation();
                log.Info("successfully set Validation!");
                SetUpIsRequiredTextBoxesSecondPart();
                log.Info("successfully set RequiredTextBoxes!");
                SetUpIsRequiredDropDownListsSecondPart();
                log.Info("successfully set RequiredDropDownLists!...Application Starting, successfully get all controls!");
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
        }

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlsertjmbg"]) == false)
        {
            ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlsertjmbg"]);
            ddlsertjmbg.CssClass = SetCss1;
        }
        else
        {
            ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-sertjmbg1"]);
            ddlsertjmbg.CssClass = SetCss1;
        }
        ddlvrstadokumenta.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-vrstadokumenta"]);
        ddlvrstadokumenta.CssClass = SetCss1;
        ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-sertadresa"]);
        ddlsertadresa.CssClass = SetCss4;
        ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rokkoriscenja"]);
        ddlrokkoriscenjasert.CssClass = SetCss1;
        ddlmedijsert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-medijsert"]);
        ddlmedijsert.CssClass = SetCss1;
        ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"]);
        ddlLegalEntityName.CssClass = SetCss5;
        ddlobveznikpdv.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlobveznikpdv"]);
        ddlobveznikpdv.CssClass = SetCss2;
        rbChooseName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rbChooseName"]);
        rbWriteName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rbWriteName"]);
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
        TurnOnAjax1Validation = true;
        TurnOnAjax2Validation = true;

        if (SettingValue == Constants.SETTING_VALUE_TRUE)
        {
            ValidateAjax1(TurnOnAjax1Validation);
            ValidateAjax2(TurnOnAjax2Validation);
            Session["zahtev-izdavanje-pravno-lice-SetUpWSPWrapperService"] = TurnOnAjax1Validation;
        }
        else
        {
            ValidateAjax1(!TurnOnAjax1Validation);
            ValidateAjax2(!TurnOnAjax2Validation);
            Session["zahtev-izdavanje-pravno-lice-SetUpWSPWrapperService"] = !TurnOnAjax1Validation;
        }
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
                if (control.Id == txtmaticnibroj.ClientID)
                {
                    TurnOnRegistrationNumberValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-pravno-lice-TurnOnRegistrationNumberValidation"] = TurnOnRegistrationNumberValidation;
                }
                else if (control.Id == txtwritename.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnWriteNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpib.ClientID)
                {
                    TurnOnPIBValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-pravno-lice-TurnOnPIBValidation"] = TurnOnPIBValidation;
                }
                else if (control.Id == txtmesto.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnCityValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtkontakttel.ClientID)
                {
                    TurnOnPhone1Validaiton = control.ControlStatus;
                    Session["zahtev-izdavanje-pravno-lice-TurnOnPhone1Validaiton"] = TurnOnPhone1Validaiton;
                }
                else if (control.Id == txtadresaeposte.ClientID)
                {
                    TurnOnEmail1Validation = control.ControlStatus;
                    Session["zahtev-izdavanje-pravno-lice-TurnOnEmail1Validation"] = TurnOnEmail1Validation;
                }
                else if (control.Id == txtwebadresa.ClientID)
                {
                    TurnOnURLValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-pravno-lice-TurnOnURLValidation"] = TurnOnURLValidation;
                }
                else if (control.Id == txtnazivpravnoglica.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnLegalEntityNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtsifradel.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnActivityCodeValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtulica.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnStreetValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtbroj.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnHouseNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpostanskibroj.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnPostNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpak.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnPAKValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtimezz.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnNameZZValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtprezimezz.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnSurnameZZValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtjmbg.ClientID)
                {
                    TurnOnJMBGValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-pravno-lice-TurnOnJMBGValidation"] = TurnOnJMBGValidation;
                }
                else if (control.Id == txtmesto1.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnCityValidation1"] = control.ControlStatus;
                }
                else if (control.Id == txtadresaeposte1.ClientID)
                {
                    TurnOnEmail2Validation = control.ControlStatus;
                    Session["zahtev-izdavanje-pravno-lice-TurnOnEmail2Validation"] = TurnOnEmail2Validation;
                }
                else if (control.Id == txttelefon.ClientID)
                {
                    TurnOnPhone2Validation = control.ControlStatus;
                    Session["zahtev-izdavanje-pravno-lice-TurnOnPhone2Validation"] = TurnOnPhone2Validation;
                }
                else if (control.Id == txtime.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtprezime.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnSurnameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtbrojiddokumenta.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnIDDocumentNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtimeinstitucije.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnInstitutionNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtulica1.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnStreetValidation1"] = control.ControlStatus;
                }
                else if (control.Id == txtbroj1.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnHouseNumberValidation1"] = control.ControlStatus;
                }
                else if (control.Id == txtpostanskibroj1.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnPostNumberValidation1"] = control.ControlStatus;
                }
                else if (control.Id == txtpak1.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnPAKValidation1"] = control.ControlStatus;
                }
                else if (control.Id == txtdatumizdavanja.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnIssueDateValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtdatumisteka.ClientID)
                {
                    Session["zahtev-izdavanje-pravno-lice-TurnOnExpiryDateValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            TurnOnRegistrationNumberValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnRegistrationNumberValidation"] = TurnOnRegistrationNumberValidation;
            TurnOnPIBValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnPIBValidation"] = TurnOnPIBValidation;
            TurnOnPhone1Validaiton = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnPhone1Validaiton"] = TurnOnPhone1Validaiton;
            TurnOnEmail1Validation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnEmail1Validation"] = TurnOnEmail1Validation;
            TurnOnURLValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnURLValidation"] = TurnOnURLValidation;
            TurnOnJMBGValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnJMBGValidation"] = TurnOnJMBGValidation;
            TurnOnEmail2Validation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnEmail2Validation"] = TurnOnEmail2Validation;
            TurnOnPhone2Validation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnPhone2Validation"] = TurnOnPhone2Validation;
            //-------------------------------------------------------------------------
            Session["zahtev-izdavanje-pravno-lice-TurnOnLegalEntityNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnActivityCodeValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnCityValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnStreetValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnHouseNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnPostNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnPAKValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnNameZZValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnSurnameZZValidation"] = Constants.VALIDATION_FALSE;
            //-------------------------------------------------------------------------
            Session["zahtev-izdavanje-pravno-lice-TurnOnCityValidation1"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnSurnameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnIDDocumentNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnInstitutionNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnStreetValidation1"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnHouseNumberValidation1"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnPostNumberValidation1"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnPAKValidation1"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnIssueDateValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-pravno-lice-TurnOnExpiryDateValidation"] = Constants.VALIDATION_FALSE;
        }
    }

    protected void ValidateAjax1(bool TurnOnAjax1Validation)
    {
        autoCompleteMestoBoravka.Enabled = TurnOnAjax1Validation;
        autoCompleteUlicaBoravka.Enabled = TurnOnAjax1Validation;
    }

    protected void ValidateAjax2(bool TurnOnAjax2Validation)
    {
        autoCompleteMestoBoravka1.Enabled = TurnOnAjax2Validation;
        autoCompleteUlicaBoravka1.Enabled = TurnOnAjax2Validation;
    }

    protected void SetUpIsRequiredTextBoxesSecondPart()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrolePoTipu(page, Constants.CONTROL_TYPE_TEXTBOX);

        foreach (var control in Controls)
        {
            if (control.Id == txtime.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezime.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtprezimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtjmbg.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtjmbgIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbrojiddokumenta.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtbrojiddokumentaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtimeinstitucije.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtimeinstitucijeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumizdavanja.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtdatumizdavanjaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumisteka.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtdatumistekaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtadresaeposte1.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtadresaeposte1IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txttelefon.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txttelefonIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtmesto1.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtmesto1IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtulica1.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtulica1IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbroj1.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtbroj1IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpostanskibroj1.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtpostanskibroj1IsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpak1.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtpak1IsRequired"] = control.IsRequired;
            }
        }

        SetUpIsRequiredTextBoxesFirstPart(Controls);
    }

    protected void SetUpIsRequiredTextBoxesFirstPart(List<WebControl> Controls)
    {
        foreach (var control in Controls)
        {
            if (control.Id == txtmaticnibroj.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtmaticnibrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtnazivpravnoglica.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtnazivpravnoglicaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtwritename.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtwritenameIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpib.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtpibIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtsifradel.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtsifradelIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtmesto.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtmestoIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtulica.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtulicaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbroj.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtbrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpostanskibroj.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtpostanskibrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpak.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtpakIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtkontakttel.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtkontakttelIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtadresaeposte.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtadresaeposteIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtwebadresa.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtwebadresaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtimezz.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtimezzIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezimezz.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-txtprezimezzIsRequired"] = control.IsRequired;
            }
        }
    }

    protected void SetUpIsRequiredDropDownListsSecondPart()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrolePoTipu(page, Constants.CONTROL_TYPE_DROPDOWNLIST);

        foreach (var control in Controls)
        {
            if (control.Id == ddlsertjmbg.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-ddlsertjmbgIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlvrstadokumenta.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-ddlvrstadokumentaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlsertadresa.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-ddlsertadresaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlrokkoriscenjasert.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-ddlrokkoriscenjasertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlmedijsert.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-ddlmedijsertIsRequired"] = control.IsRequired;
            }
        }

        SetUpIsRequiredDropDownListsFirstPart(Controls);
    }

    protected void SetUpIsRequiredDropDownListsFirstPart(List<WebControl> Controls)
    {
        foreach (var control in Controls)
        {
            if (control.Id == ddlLegalEntityName.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityNameIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlobveznikpdv.ClientID)
            {
                Session["zahtev-izdavanje-pravno-lice-ddlobveznikpdvIsRequired"] = control.IsRequired;
            }
        }
    }
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
        txtdatumisteka.Text = Request.Form[txtdatumisteka.UniqueID];
        txtdatumizdavanja.Text = Request.Form[txtdatumizdavanja.UniqueID];
    }

    protected void ddlsertjmbg_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    private void SetInitialRow()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add(new DataColumn("RedniBroj", typeof(string)));
        dt.Columns.Add(new DataColumn("Ime", typeof(string)));
        dt.Columns.Add(new DataColumn("Prezime", typeof(string)));
        dt.Columns.Add(new DataColumn("Jmbg", typeof(string)));
        dt.Columns.Add(new DataColumn("Adresaeposte", typeof(string)));
        dt.Columns.Add(new DataColumn("Telefon", typeof(string)));
        dt.Columns.Add(new DataColumn("Medij", typeof(string)));
        dt.Columns.Add(new DataColumn("Rok", typeof(string)));
        //-------------------------------------------------------------
        dt.Columns.Add(new DataColumn("DatumRodjenja", typeof(string)));
        dt.Columns.Add(new DataColumn("SertJmbg", typeof(string)));
        dt.Columns.Add(new DataColumn("VrstaDokumenta", typeof(string)));
        dt.Columns.Add(new DataColumn("BrojIDDokumenta", typeof(string)));
        dt.Columns.Add(new DataColumn("ImeInstitucije", typeof(string)));
        dt.Columns.Add(new DataColumn("DatumIzdavanja", typeof(string)));
        dt.Columns.Add(new DataColumn("DatumIsteka", typeof(string)));
        dt.Columns.Add(new DataColumn("SertAdresa", typeof(string)));
        dt.Columns.Add(new DataColumn("Mesto", typeof(string)));
        dt.Columns.Add(new DataColumn("Ulica", typeof(string)));
        dt.Columns.Add(new DataColumn("Broj", typeof(string)));
        dt.Columns.Add(new DataColumn("PostanskiBroj", typeof(string)));
        dt.Columns.Add(new DataColumn("PAK", typeof(string)));
        dt.Columns.Add(new DataColumn("CenaSaPorezom", typeof(string)));
        //Store the DataTable in ViewState
        ViewState["pravno-lice-CurrentTable"] = dt;

        GridView1.DataSource = dt;
        GridView1.DataBind();
    }

    private void AddNewRowToGrid()
    {
        try
        {           
            int rowIndex = 0;
            if (ViewState["pravno-lice-CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["pravno-lice-CurrentTable"];
                DataRow drCurrentRow = null;

                if (dtCurrentTable.Rows.Count >= 0)
                {
                    drCurrentRow = dtCurrentTable.NewRow();

                    drCurrentRow["Ime"] = txtime.Text;
                    drCurrentRow["Prezime"] = txtprezime.Text;
                    drCurrentRow["Jmbg"] = txtjmbg.Text;
                    drCurrentRow["AdresaEPoste"] = txtadresaeposte1.Text;
                    drCurrentRow["Telefon"] = txttelefon.Text;
                    drCurrentRow["Medij"] = ddlmedijsert.SelectedItem.Text;
                    drCurrentRow["Rok"] = ddlrokkoriscenjasert.SelectedItem.Text;
                    //Ove kolone se ne vide, uvedene su zbog templatefield button-a u GridView-u
                    drCurrentRow["DatumRodjenja"] = txtdatumrodjenja.Text;
                    drCurrentRow["SertJmbg"] = ddlsertjmbg.SelectedItem.Text;
                    drCurrentRow["VrstaDokumenta"] = ddlvrstadokumenta.SelectedItem.Text;
                    drCurrentRow["BrojIDDokumenta"] = txtbrojiddokumenta.Text;
                    drCurrentRow["ImeInstitucije"] = txtimeinstitucije.Text;
                    drCurrentRow["DatumIzdavanja"] = txtdatumizdavanja.Text;
                    drCurrentRow["DatumIsteka"] = txtdatumisteka.Text;
                    drCurrentRow["SertAdresa"] = ddlsertadresa.SelectedItem.Text;
                    drCurrentRow["Mesto"] = txtmesto1.Text;
                    drCurrentRow["Ulica"] = txtulica1.Text;
                    drCurrentRow["Broj"] = txtbroj1.Text;
                    drCurrentRow["PostanskiBroj"] = txtpostanskibroj1.Text;
                    drCurrentRow["PAK"] = txtpak1.Text;
                    drCurrentRow["CenaSaPorezom"] = txtcenasaporezom1.Text;

                    rowIndex++;

                    dtCurrentTable.Rows.Add(drCurrentRow);
                    ViewState["pravno-lice-CurrentTable"] = dtCurrentTable;

                    GridView1.DataSource = dtCurrentTable;
                    GridView1.DataBind();
                    }
                }            
        }
        catch (Exception ex)
        {
            log.Error("Greška prilikom dodavanja novog reda u GridView tabeli. " + ex.Message);
        }
    }

    protected void btnAddAuthorizedPersonalUser_Click1(object sender, EventArgs e)
    {
        Session["zahtev-izdavanje-pravno-lice-ddlsertjmbg"] = true;
        Session["zahtev-izdavanje-pravno-lice-sertjmbg1"] = true;
        Session["zahtev-izdavanje-pravno-lice-vrstadokumenta"] = true;
        Session["zahtev-izdavanje-pravno-lice-sertadresa"] = true;
        Session["zahtev-izdavanje-pravno-lice-rokkoriscenja"] = true;
        Session["zahtev-izdavanje-pravno-lice-medijsert"] = true;

        myDiv6.Visible = false;
        myDiv8.Visible = false;
        Container00.Visible = false;
        Container1.Visible = false;
        //Container2.Visible = false;
        Container3.Visible = false;
        Container33.Visible = false;
        Container34.Visible = false;
        myDiv5.Visible = false;
        Container4.Visible = false;
        Container5.Visible = false;

        Container6.Visible = true;
        Container7.Visible = true;
        Container77.Visible = true;
        Container8.Visible = true;
        txtadresaeposte1.Text = string.Empty;
        Container9.Visible = true;
        Div1.Visible = true;
        Div2.Visible = false;
        Div3.Visible = false;
        Container10.Visible = true;
        Container11.Visible = true;
        Container12.Visible = true;

        bool k = false;
        if (Container77.Visible == true)
        k = true;

        if (k)
        {
            myDiv1.Visible = false;
            myDiv2.Visible = false;
            myDiv3.Visible = false;
            myDiv4.Visible = false;                
        }
        string IDItem1 = string.Empty;
        string IDItem2 = string.Empty;
        string IDItem3 = string.Empty;
        string IDItem4 = string.Empty;
        SetUpDefaultItem(ddlvrstadokumenta.ClientID, out IDItem1);
        ddlvrstadokumenta.SelectedValue = IDItem1;
        SetUpDefaultItem(ddlsertadresa.ClientID, out IDItem2);
        ddlsertadresa.SelectedValue = IDItem2;
        SetUpDefaultItem(ddlrokkoriscenjasert.ClientID, out IDItem3);
        ddlrokkoriscenjasert.SelectedValue = IDItem3;
        SetUpDefaultItem(ddlmedijsert.ClientID, out IDItem4);
        ddlmedijsert.SelectedValue = IDItem4;

        FormClearToAddNewPerson();

        Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtime;
        SetFocusOnTextbox();
    }

    protected void FormClearToAddNewPerson()
    {
        txtime.Text = string.Empty;
        txtime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtime.ReadOnly = false;
        txtprezime.Text = string.Empty;
        txtprezime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtprezime.ReadOnly = false;
        txtjmbg.Text = string.Empty;
        txtjmbg.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtjmbg.ReadOnly = false;
        txtdatumrodjenja.Text = string.Empty;
        txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtdatumrodjenja.ReadOnly = true;
        ddlsertjmbg.Enabled = true;
        ddlsertjmbg.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlsertjmbg.CssClass = SetCss1;        
        ddlvrstadokumenta.Enabled = true;
        ddlvrstadokumenta.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlvrstadokumenta.CssClass = SetCss2;
        txtbrojiddokumenta.Text = string.Empty;
        txtbrojiddokumenta.ReadOnly = false;
        txtbrojiddokumenta.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtimeinstitucije.Text = string.Empty;
        txtimeinstitucije.ReadOnly = false;
        txtimeinstitucije.BackColor = ColorTranslator.FromHtml(SetWhite);
        //txtdatumizdavanja.ReadOnly = true;
        txtdatumizdavanja.BackColor = ColorTranslator.FromHtml(SetWhite);
        //txtdatumisteka.ReadOnly = true;
        txtdatumisteka.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtadresaeposte1.Text = string.Empty;
        txtadresaeposte1.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtadresaeposte1.ReadOnly = false;
        txttelefon.Text = string.Empty;
        txttelefon.BackColor = ColorTranslator.FromHtml(SetWhite);
        txttelefon.ReadOnly = false;       
        ddlsertadresa.Enabled = true;
        ddlsertadresa.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlsertadresa.CssClass = SetCss4;        
        ddlrokkoriscenjasert.Enabled = true;
        ddlrokkoriscenjasert.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlrokkoriscenjasert.CssClass = SetCss1;       
        ddlmedijsert.Enabled = true;
        ddlmedijsert.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlrokkoriscenjasert.CssClass = SetCss1;
        txtulica1.Text = string.Empty;
        txtulica1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtbroj1.Text = string.Empty;
        txtbroj1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtpostanskibroj1.Text = string.Empty;
        txtpostanskibroj1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtpak1.Text = string.Empty;
        txtpak1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtmesto1.Text = string.Empty;
        txtmesto1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtcenasaporezom1.Text = string.Empty;
        txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtcenasaporezom1.ReadOnly = true;
        txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtdatumrodjenja.ReadOnly = true;
        errLabel1.Text = string.Empty;
        errLabelMail.Text = string.Empty;
        errLabelNumber.Text = string.Empty;
        errLabelBroj1.Text = string.Empty;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------PRVA FORMA-------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------
    //------------------------------------PROVERA E ADRESE Prva Forma----------------------------------
    //--------------------------------------------------------------------------------------
    protected void txtadresaeposte_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged();
        if (errLabel.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtwebadresa;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged()
    {
        string newMail = txtadresaeposte.Text;
        string errorMessage = string.Empty;
        string mailformat = string.Empty;
        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnEmail1Validation"]))
        {
            UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
            errLabel.Text = errorMessage;
            newMail = mailformat;
        }
        else
        {
            newMail = txtadresaeposte.Text;
            errorMessage = string.Empty;
        }
    }

    protected void cvadresaeposte_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newMail = txtadresaeposte.Text;
        string errorMessage = string.Empty;
        string mailformat = string.Empty;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnEmail1Validation"]))
        {
            args.IsValid = UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
            cvadresaeposte.ErrorMessage = errorMessage;
            newMail = mailformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationMail(newMail, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
            cvadresaeposte.ErrorMessage = errorMessage;
        }
    }

    protected void cvadresaeposte1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newMail = txtadresaeposte1.Text;
        string errorMessage = string.Empty;
        string mailformat = string.Empty;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnEmail2Validation"]))
        {
            args.IsValid = UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtadresaeposte1IsRequired"]), out errorMessage, out mailformat);
            cvadresaeposte1.ErrorMessage = errorMessage;
            newMail = mailformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationMail(newMail, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtadresaeposte1IsRequired"]), out errorMessage, out mailformat);
            cvadresaeposte1.ErrorMessage = errorMessage;
        }
    }

    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------
    //------------------------------------PROVERA PIB-a Prva Forma-------------------------
    //-------------------------------------------------------------------------------------
    protected void txtpib_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged1();
        if (errLabelPIB.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ddlobveznikpdv;
            SetFocusOnDropDownLists();
        }
    }

    private void CheckIfChannelHasChanged1()
    {
        string newPIB = txtpib.Text;
        string errorMessage = string.Empty;
        string pibformat = string.Empty;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPIBValidation"]))
        {
            UtilsValidation.ValidatePIB(newPIB, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtpibIsRequired"]), out errorMessage, out pibformat); //proveri da li je PIB ispravno napisan
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

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPIBValidation"]))
        {
            args.IsValid = UtilsValidation.ValidatePIB(newPIB, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtpibIsRequired"]), out errorMessage, out pibformat); //proveri da li je PIB ispravno napisan
            cvpib.ErrorMessage = errorMessage;
            newPIB = pibformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationPIB(newPIB, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtpibIsRequired"]), out errorMessage, out pibformat);
            cvpib.ErrorMessage = errorMessage;
        }
    }      

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Kontakt telefonskog broja Prva FORMA---------------
    //------------------------------------------------------------------------------------------------
    protected void txtkontakttel_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged5();
        if (errLabelKontaktTel.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtadresaeposte;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged5()
    {
        string newNumber = txtkontakttel.Text;
        string errorMessage = string.Empty;
        string numberformat = string.Empty;
        bool LegalEntityPhone = true;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPhone1Validaiton"]))
        {
            UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtkontakttelIsRequired"]), LegalEntityPhone, out errorMessage, out numberformat);
            errLabelKontaktTel.Text = errorMessage;
            newNumber = numberformat;
        }
        else
        {
            newNumber = txtkontakttel.Text;
            errorMessage = string.Empty;
        }
    }

    protected void cvkontakttel_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newNumber = txtkontakttel.Text;
        string errMessage = string.Empty;
        string numberformat = string.Empty;
        bool LegalEntityPhone = true;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPhone1Validaiton"]))
        {
            args.IsValid = UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtkontakttelIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvkontakttel.ErrorMessage = errMessage;
            newNumber = numberformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationKontaktNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtkontakttelIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvkontakttel.ErrorMessage = errMessage;
        }
    }

    //--------------------------------------------------------------------------------------
    //------------------------------------PROVERA WEB ADRESE URL-a Prva Forma----------------------------------
    //--------------------------------------------------------------------------------------
    protected void txtwebadresa_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged6();
        if (errLabelURL.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtimezz;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged6()
    {
        string urlformat = string.Empty;
        string errMessage = string.Empty;
        string newURL = txtwebadresa.Text;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnURLValidation"]))
        {
            UtilsValidation.ValidateWEBurl(newURL, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtwebadresaIsRequired"]), out errMessage, out urlformat);
            errLabelURL.Text = errMessage;
        }
        else
        {
            newURL = txtwebadresa.Text;
            errLabelURL.Text = string.Empty;
        }
    }

    protected void cvwebadresa_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string urlformat = string.Empty;
            string errMessage = string.Empty;
            string newURL = txtwebadresa.Text;

            if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnURLValidation"]))
            {
                args.IsValid = UtilsValidation.ValidateWEBurl(newURL, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtwebadresaIsRequired"]), out errMessage, out urlformat);
                cvwebadresa.ErrorMessage = errMessage;
                txtwebadresa.Text = urlformat;
            }
            else
            {
                args.IsValid = UtilsValidation.WithoutValidateWEBurl(newURL, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtwebadresaIsRequired"]), out errMessage, out urlformat);
                cvwebadresa.ErrorMessage = errMessage;
            }
        }
        catch (Exception)
        {
            cvwebadresa.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    protected void cvnazivpravnoglica_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string newLegalNameformat = string.Empty;
            string errMessage = string.Empty;
            string newLegalName = txtnazivpravnoglica.Text;
            args.IsValid = UtilsValidation.ValidateNazivPravnogLica(newLegalName, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtnazivpravnoglicaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnLegalEntityNameValidation"]), out errMessage, out newLegalNameformat);
            cvnazivpravnoglica.ErrorMessage = errMessage;
        }
        catch (Exception)
        {
            cvnazivpravnoglica.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvobveznikpdv_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string errMessage = string.Empty;
            string ddlobveznikpdvString = ddlobveznikpdv.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlobveznikpdvString, out IDItem);

            args.IsValid = UtilsValidation.ValidateObveznikPDV(ddlobveznikpdv.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlobveznikpdvIsRequired"]), IDItem, out errMessage);
            cvobveznikpdv.ErrorMessage = errMessage;
        }
        catch (Exception)
        {
            cvobveznikpdv.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvsifradel_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string newSifraDelformat = string.Empty;
            string errMessage = string.Empty;
            string newSifraDel = txtsifradel.Text;
            args.IsValid = UtilsValidation.ValidateSifraDel(newSifraDel, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtsifradelIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnActivityCodeValidation"]), out errMessage, out newSifraDelformat);
            cvsifradel.ErrorMessage = errMessage;
        }
        catch (Exception)
        {
            cvsifradel.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvimezz_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameZZformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateImeZZ(txtimezz.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtimezzIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnNameZZValidation"]), out ErrorMessage1, out nameZZformat);
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
            args.IsValid = UtilsValidation.ValidatePrezimeZZ(txtprezimezz.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtprezimezzIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnSurnnameZZValidation"]), out ErrorMessage1, out surnameZZformat);
            cvprezimezz.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvprezimezz.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvmesto_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string cityformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateCity(txtmesto.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtmestoIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnCityValidation"]), out ErrorMessage1, out cityformat);
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
            args.IsValid = UtilsValidation.ValidateStreet(txtulica.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtulicaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnStreetValidation"]), out ErrorMessage1, out streetformat);
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
            args.IsValid = UtilsValidation.ValidateHouseNumber(txtbroj.Text, errLabelBroj.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtbrojIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnHouseNumberValidation"]), out ErrorMessage1, out housenumberformat);
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
            args.IsValid = UtilsValidation.ValidatePostNumber(txtpostanskibroj.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtpostanskibrojIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPostNumberValidation"]), out ErrorMessage1, out postnumberformat);
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
            args.IsValid = UtilsValidation.ValidatePAK(txtpak.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtpakIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPAKValidation"]), out ErrorMessage1, out pakformat);
            cvpak.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvpak.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvGridView1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string gridformat = string.Empty;

            if (ViewState["pravno-lice-CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["pravno-lice-CurrentTable"];

                args.IsValid = UtilsValidation.ValidateGridView(dtCurrentTable, out ErrorMessage1, out gridformat);
                cvGridView1.ErrorMessage = ErrorMessage1;
            }
        }
        catch (Exception)
        {
            cvGridView1.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //-----------------SetUp PTT VAriables---------------------------
    //---------------------------------------------------------------
    protected void SetUpPTTVariables(int SelectedValue, string page, string ddlstring, out string CityVariable, out string StreetVariable, out string HouseNumberVariable, out string ZipCodeVariable, out string PAKVariable, out bool InHouseVariable, out bool IsAllowedVariable, out int IsLegalEntity)
    {
        CityVariable = string.Empty;
        StreetVariable = string.Empty;
        HouseNumberVariable = string.Empty;
        ZipCodeVariable = string.Empty;
        PAKVariable = string.Empty;
        InHouseVariable = false;
        IsAllowedVariable = false;
        IsLegalEntity = 0;

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
                IsLegalEntity = pttvariables.IsLegalEntity;
                Session["zahtev-izdavanje-pravno-lice-CityVariable"] = CityVariable;
                Session["zahtev-izdavanje-pravno-lice-StreetVariable"] = StreetVariable;
                Session["zahtev-izdavanje-pravno-lice-HouseNumberVariable"] = HouseNumberVariable;
                Session["zahtev-izdavanje-pravno-lice-ZipCodeVariable"] = ZipCodeVariable;
                Session["zahtev-izdavanje-pravno-lice-PAKVariable"] = PAKVariable;
            }
            else
            {}
        }
    }
    //---------------------------------------------------------------
    //---------------------------------------------------------------

    protected void ddlsertadresa_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlsertadresa.SelectedValue);
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string ddlstring = ddlsertadresa.ClientID;
        //Get PTT Variables from Database
        SetUpPTTVariables(SelectedValue, page, ddlstring, out CityVariable, out StreetVariable, out HouseNumberVariable, out ZipCodeVariable, out PAKVariable, out InHouseVariable, out IsAllowedVariable, out IsLegalEntity);

        if (InHouseVariable && IsAllowedVariable)
        {
            ValidateAjax2(false);
            Colorgray();
            txtulica1.ReadOnly = InHouseVariable;
            txtulica1.Text = StreetVariable;
            txtbroj1.ReadOnly = InHouseVariable;
            txtbroj1.Text = HouseNumberVariable;
            txtpostanskibroj1.ReadOnly = InHouseVariable;
            txtpostanskibroj1.Text = ZipCodeVariable;
            txtpak1.ReadOnly = InHouseVariable;
            txtpak1.Text = PAKVariable;
            txtmesto1.ReadOnly = InHouseVariable;
            txtmesto1.Text = CityVariable;
        }
        else if (!InHouseVariable && IsAllowedVariable && IsLegalEntity == 0)
        {
            ValidateAjax2(Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-SetUpWSPWrapperService"]));
            Colorchange();
            txtulica1.ReadOnly = false;
            txtulica1.Text = StreetVariable;
            txtbroj1.ReadOnly = false;
            txtbroj1.Text = HouseNumberVariable;
            txtpostanskibroj1.ReadOnly = false;
            txtpostanskibroj1.Text = ZipCodeVariable;
            txtpak1.ReadOnly = false;
            txtpak1.Text = PAKVariable;
            txtmesto1.ReadOnly = false;
            txtmesto1.Text = CityVariable;
            txtmesto1.TabIndex = 0;
            txtulica1.TabIndex = 0;
            txtbroj1.TabIndex = 0;
            txtpostanskibroj1.TabIndex = 0;
            txtpak1.TabIndex = 0;
        }
        else if (!InHouseVariable && !IsAllowedVariable)
        {
            ValidateAjax2(false);
            Colorgray();
            txtulica1.ReadOnly = true;
            txtulica1.Text = StreetVariable;
            txtbroj1.ReadOnly = true;
            txtbroj1.Text = HouseNumberVariable;
            txtpostanskibroj1.ReadOnly = true;
            txtpostanskibroj1.Text = ZipCodeVariable;
            txtpak1.ReadOnly = true;
            txtpak1.Text = PAKVariable;
            txtmesto1.ReadOnly = true;
            txtmesto1.Text = CityVariable;
            txtmesto1.TabIndex = -1;
            txtulica1.TabIndex = -1;
            txtbroj1.TabIndex = -1;
            txtpostanskibroj1.TabIndex = -1;
            txtpak1.TabIndex = -1;
        }
        else if (!InHouseVariable && IsAllowedVariable && IsLegalEntity == 1)
        {
            ValidateAjax2(false);
            Colorgray();
            txtulica1.ReadOnly = true;
            txtulica1.Text = txtulica.Text;
            txtbroj1.ReadOnly = true;
            txtbroj1.Text = txtbroj.Text;
            txtpostanskibroj1.ReadOnly = true;
            txtpostanskibroj1.Text = txtpostanskibroj.Text;
            txtpak1.ReadOnly = true;
            txtpak1.Text = txtpak.Text;
            txtmesto1.ReadOnly = true;
            txtmesto1.Text = txtmesto.Text;
            txtmesto1.TabIndex = -1;
            txtulica1.TabIndex = -1;
            txtbroj1.TabIndex = -1;
            txtpostanskibroj1.TabIndex = -1;
            txtpak1.TabIndex = -1;
        }
        Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ddlobveznikpdv_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    private void Colorchange()
    {
        /*
        //menja se boja teksta u textbox-u
        txtulica1.ForeColor = Color.Gray;
        txtbroj1.ForeColor = Color.Gray;
        txtpostanskibroj1.ForeColor = Color.Gray;
        txtpak1.ForeColor = Color.Gray;
        txtmesto1.ForeColor = Color.Gray;
        */
        //menja se boja background textbox-a
        txtulica1.BackColor = Color.White;
        txtbroj1.BackColor = Color.White;
        txtpostanskibroj1.BackColor = Color.White;
        txtpak1.BackColor = Color.White;
        txtmesto1.BackColor = Color.White;
    }

    private void Colorgray()
    {
        //menja se boja background textbox-a
        txtulica1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtbroj1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtpostanskibroj1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtpak1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtmesto1.BackColor = ColorTranslator.FromHtml(SetLightGray);
    }

    protected void ddlvrstadokumenta_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlvrstadokumenta.SelectedValue);
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string ddlstring = ddlvrstadokumenta.ClientID;
        //Get PTT Variables from Database
        SetUpPTTVariables(SelectedValue, page, ddlstring, out CityVariable, out StreetVariable, out HouseNumberVariable, out ZipCodeVariable, out PAKVariable, out InHouseVariable, out IsAllowedVariable, out IsLegalEntity);

        if (IsAllowedVariable)
        {
            myDiv1.Visible = IsAllowedVariable;
            myDiv2.Visible = IsAllowedVariable;
            myDiv3.Visible = IsAllowedVariable;
            myDiv4.Visible = IsAllowedVariable;
        }
        else
        {
            myDiv1.Visible = IsAllowedVariable;
            myDiv2.Visible = IsAllowedVariable;
            myDiv3.Visible = IsAllowedVariable;
            myDiv4.Visible = IsAllowedVariable;
        }
        Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
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
        txtcenasaporezom1.Text = price;
    }

    protected void ddlrokkoriscenjasert_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetPrice();
        Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ddlmedijsert_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetPrice();
        Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    //-----------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------

    protected void btnSubmit2_Click2(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            ClientScript.RegisterStartupScript(GetType(), "unhook", "unhook();", true);
            if (true)
            {
                FormBeforeEditOrSubmit();

                AddNewRowToGrid();

                CalculatePrices();
                //----------------------------------------------------------------------------------------------------------------------
            }
        }
        else if (!Page.IsValid)
        {
            errLabel1.Text = string.Empty;
            errLabelNumber.Text = string.Empty;
            errLabelMail.Text = string.Empty;
            errLabelURL.Text = string.Empty;
            txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtcenasaporezom1.ReadOnly = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
      }
      
    }

    protected void FormBeforeEditOrSubmit()
    {
        myDiv6.Visible = false;
        myDiv8.Visible = false;
        Container00.Visible = true;
        Container1.Visible = true;
        //Container2.Visible = true;
        Container3.Visible = true;
        Container33.Visible = true;
        Container34.Visible = true;
        myDiv5.Visible = true;
        Container4.Visible = true;
        Container5.Visible = true;

        Container6.Visible = false;
        Container7.Visible = false;
        Container77.Visible = false;
        Container8.Visible = false;
        Container9.Visible = false;
        Div1.Visible = false;
        Div2.Visible = false;
        Div3.Visible = false;
        Container10.Visible = false;
        Container11.Visible = false;
        Container12.Visible = false;
        txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtcenasaporezom1.ReadOnly = true;

        btnAddAuthorizedPersonalUser.Enabled = true;

    }

    protected void btnQuit_Click2(object sender, EventArgs e)
    {
        Back();
    }

    protected void btnQuit_Click3(object sender, EventArgs e)
    {
        Back();
    }

    protected void btnBack1_Click2(object sender, EventArgs e)
    {
        Back();
    }

    protected void Back()
    {
        Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"] = true;
        Session["zahtev-izdavanje-pravno-lice-rbChooseName"] = true;
        Session["zahtev-izdavanje-pravno-lice-rbWriteName"] = true;
        myDiv6.Visible = false;
        myDiv8.Visible = false;
        Container00.Visible = true;
        Container1.Visible = true;
        //Container2.Visible = true;
        Container3.Visible = true;
        Container33.Visible = true;
        Container34.Visible = true;
        myDiv5.Visible = true;
        Container4.Visible = true;
        Container5.Visible = true;

        Container6.Visible = false;
        Container7.Visible = false;
        Container77.Visible = false;
        Container8.Visible = false;
        Container9.Visible = false;
        Div1.Visible = false;
        Div2.Visible = false;
        Div3.Visible = false;
        Container10.Visible = false;
        Container11.Visible = false;
        Container12.Visible = false;
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
                    Session["zahtev-izdavanje-pravno-lice-rbChooseName"] = false;
                    Session["zahtev-izdavanje-pravno-lice-rbWriteName"] = false;
                    Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"] = false;

                    ValidateAjax1(false);

                    myDiv6.Visible = true;
                    myDiv8.Visible = true;
                    myDiv5.Visible = false;
                    Container33.Visible = false;
                    Container34.Visible = true;
                    txtmaticnibroj.ReadOnly = true;
                    txtmaticnibroj.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtnazivpravnoglica.ReadOnly = true;
                    txtnazivpravnoglica.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlLegalEntityName.Enabled = false;
                    ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlLegalEntityName.CssClass = SetCss5;
                    txtwritename.ReadOnly = true;
                    txtwritename.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtwritename.CssClass = SetCss5;
                    txtpib.ReadOnly = true;
                    txtpib.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    Session["zahtev-izdavanje-pravno-lice-ddlobveznikpdv"] = false;
                    ddlobveznikpdv.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlobveznikpdv"]);
                    ddlobveznikpdv.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlobveznikpdv.CssClass = SetCss2;
                    txtsifradel.ReadOnly = true;
                    txtsifradel.BackColor = ColorTranslator.FromHtml(SetDarkGray);
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
                    txtkontakttel.ReadOnly = true;
                    txtkontakttel.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtadresaeposte.ReadOnly = true;
                    txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtwebadresa.ReadOnly = true;
                    txtwebadresa.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtimezz.ReadOnly = true;
                    txtimezz.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtprezimezz.ReadOnly = true;
                    txtprezimezz.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtcenasaporezom.ReadOnly = true;
                    txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtcenasaporezom1.ReadOnly = true;
                    txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    rbChooseName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rbChooseName"]);
                    rbWriteName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rbWriteName"]);
                    ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"]);
                    ddlLegalEntityName.CssClass = SetCss5;

                    Container3.Visible = true;
                    GridView1.Enabled = false;

                    GridView1.Columns[9].Visible = false;

                    txtdatumzahteva.Text = DateTime.Now.ToString("dd.MM.yyyy");
                    Session["zahtev-izdavanje-pravno-lice-datumzahteva"] = txtdatumzahteva.Text;
                    Session["zahtev-izdavanje-pravno-lice-cenasaporezom"] = txtcenasaporezom.Text;

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
                    Session["zahtev-izdavanje-pravno-lice-userAgentBrowser"] = userAgentBrowser;
                    Session["zahtev-izdavanje-pravno-lice-userAgentStringApplicant"] = userAgentStringApplicant;
                    Session["zahtev-izdavanje-pravno-lice-userAgentOS"] = userAgentOS;
                    Session["zahtev-izdavanje-pravno-lice-userAgentIP"] = userAgentIP;
                    Session["zahtev-izdavanje-pravno-lice-userAgentContinent"] = userAgentContinent;
                    Session["zahtev-izdavanje-pravno-lice-userAgentCountry"] = userAgentCountry;
                    Session["zahtev-izdavanje-pravno-lice-userAgentCountryCode"] = userAgentCountryCode;
                    Session["zahtev-izdavanje-pravno-lice-userAgentCity"] = userAgentCity;
                    Session["zahtev-izdavanje-pravno-lice-userAgentISP"] = userAgentISP;
                }
            }
            //----------------------------------------------------------------------------------------------------------------------
            else if (!Page.IsValid)
            {
                errLabelPIB.Text = string.Empty;
                errLabelKontaktTel.Text = string.Empty;
                errLabel.Text = string.Empty;
                errLabel1.Text = string.Empty;
                errLabelNumber.Text = string.Empty;
                errLabelMail.Text = string.Empty;
                errLabelURL.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
                if (txtmaticnibroj.Text.Length == Constants.LEGAL_ENTITY_MATICNI_BROJ && txtpib.Text.Length == Constants.LEGAL_ENTITY_PIB && txtsifradel.Text.Length == Constants.LEGAL_ENTITY_SIFRA_DELATNOSTI && txtulica.Text != string.Empty && txtbroj.Text != string.Empty && txtpostanskibroj.Text.Length == Constants.LEGAL_ENTITY_POSTANSKI_BROJ && txtmesto.Text != string.Empty && txtkontakttel.Text != string.Empty && txtadresaeposte.Text != string.Empty && txtimezz.Text != string.Empty && txtprezimezz.Text != string.Empty)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "EnabledButton", "EnabledButton();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "DisabledButton", "DisabledButton();", true);
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Button submit error. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
        }
    }

    protected void btnReEnterRequest_Click1(object sender, EventArgs e)
    {
        Session["zahtev-izdavanje-pravno-lice-rbChooseName"] = true;
        Session["zahtev-izdavanje-pravno-lice-rbWriteName"] = true;
        Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"] = true;

        ValidateAjax1(Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-SetUpWSPWrapperService"]));

        myDiv6.Visible = false;
        myDiv8.Visible = false;
        myDiv5.Visible = true;
        Container33.Visible = true;
        Container34.Visible = true;
        txtmaticnibroj.ReadOnly = false;
        txtmaticnibroj.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtnazivpravnoglica.ReadOnly = false;
        txtnazivpravnoglica.BackColor = ColorTranslator.FromHtml(SetWhite);
        if (rbChooseName.Checked == true)
        {
            ddlLegalEntityName.Enabled = true;
            ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetWhite);
            ddlLegalEntityName.CssClass = SetCss5;
            txtwritename.ReadOnly = true;
            txtwritename.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtwritename.CssClass = SetCss5;
        }
        else
        {
            ddlLegalEntityName.Enabled = false;
            ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetLightGray);
            ddlLegalEntityName.CssClass = SetCss5;
            txtwritename.ReadOnly = false;
            txtwritename.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtwritename.CssClass = SetCss5;
        }
        txtpib.ReadOnly = false;
        txtpib.BackColor = ColorTranslator.FromHtml(SetWhite);
        Session["zahtev-izdavanje-pravno-lice-ddlobveznikpdv"] = true;
        ddlobveznikpdv.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlobveznikpdv"]);
        ddlobveznikpdv.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlobveznikpdv.CssClass = SetCss2;
        txtsifradel.ReadOnly = false;
        txtsifradel.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtulica.ReadOnly = false;
        txtulica.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtbroj.ReadOnly = false;
        txtbroj.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtpostanskibroj.ReadOnly = true;
        txtpostanskibroj.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtpak.ReadOnly = false;
        txtpak.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtmesto.ReadOnly = false;
        txtmesto.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtkontakttel.ReadOnly = false;
        txtkontakttel.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtadresaeposte.ReadOnly = false;
        txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtwebadresa.ReadOnly = false;
        txtwebadresa.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtimezz.ReadOnly = false;
        txtimezz.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtprezimezz.ReadOnly = false;
        txtprezimezz.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtcenasaporezom.ReadOnly = true;
        txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
        rbChooseName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rbChooseName"]);
        rbWriteName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rbWriteName"]);
        ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"]);
        ddlLegalEntityName.CssClass = SetCss5;

        Container3.Visible = true;
        GridView1.Enabled = true;

        GridView1.Columns[8].Visible = false;
        GridView1.Columns[9].Visible = true;
    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------DONJA FORMA-------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    protected void cvime_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateName(txtime.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtimeIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnNameValidation"]), out ErrorMessage1, out nameformat);
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
            args.IsValid = UtilsValidation.ValidateSurname(txtprezime.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtprezimeIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnSurnameValidation"]), out ErrorMessage1, out surnameformat);
            cvprezime.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvprezime.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvbrojiddokumenta_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string documentidformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateIDDocument(txtbrojiddokumenta.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtbrojiddokumentaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnIDDocumentNumberValidation"]), out ErrorMessage1, out documentidformat);
            cvbrojiddokumenta.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvbrojiddokumenta.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvimeinstitucije_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string InstitutionNameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateInstitutionName(txtimeinstitucije.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtimeinstitucijeIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnInstitutionNameValidation"]), out ErrorMessage1, out InstitutionNameformat);
            cvimeinstitucije.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvimeinstitucije.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvdatumizdavanja_ServerValidate(object source, ServerValidateEventArgs args)
    {
        Utility utility = new Utility();
        try
        {
            if (txtdatumizdavanja.Text != string.Empty)
            {
                DateTime datumizdavanja = DateTime.ParseExact(txtdatumizdavanja.Text, "dd.MM.yyyy", null);
                log.Info("datumizdavanja je: " + datumizdavanja);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateIssuingDate(datumizdavanja, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtdatumizdavanjaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnIssueDateValidation"]), out ErrorMessage1);
                cvdatumizdavanja.ErrorMessage = ErrorMessage1;
            }
            else
            {
                if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtdatumizdavanjaIsRequired"]))
                {
                    if (txtdatumizdavanja.Text == string.Empty)
                    {
                        cvdatumizdavanja.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
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
            log.Error("Greska prilikom validacije cvdatumizdavanja. " + ex.Message);
            txtdatumizdavanja.Text = string.Empty;
            if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtdatumizdavanjaIsRequired"]))
            {
                cvdatumizdavanja.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
            }
            else
            {
                cvdatumizdavanja.ErrorMessage = string.Empty;
            }
            args.IsValid = false;
        }
    }

    protected void cvdatumisteka_ServerValidate(object source, ServerValidateEventArgs args)
    {
        Utility utility = new Utility();
        try
        {
            if (txtdatumisteka.Text != string.Empty)
            {
                DateTime datumisteka = DateTime.ParseExact(txtdatumisteka.Text, "dd.MM.yyyy", null);
                log.Info("datumisteka je: " + datumisteka);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateExpiryDate(datumisteka, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtdatumistekaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnExpiryDateValidation"]), out ErrorMessage1);
                cvdatumisteka.ErrorMessage = ErrorMessage1;
            }
            else
            {
                if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtdatumistekaIsRequired"]))
                {
                    if (txtdatumisteka.Text == string.Empty)
                    {
                        cvdatumisteka.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
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
            log.Error("Greska prilikom validacije cvdatumisteka. " + ex.Message);
            txtdatumisteka.Text = string.Empty;
            if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtdatumistekaIsRequired"]))
            {
                cvdatumisteka.ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
            }
            else
            {
                cvdatumisteka.ErrorMessage = string.Empty;
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

            args.IsValid = UtilsValidation.ValidateSertAdresa(ddlsertadresa.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlsertadresaIsRequired"]), IDItem, out ErrorMessage1);
            cvsertadresa.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvsertadresa.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvmesto1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string cityformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateCity(txtmesto1.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtmesto1IsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnCityValidation1"]), out ErrorMessage1, out cityformat);
            cvmesto1.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvmesto1.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvulica1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string streetformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateStreet(txtulica1.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtulica1IsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnStreetValidation1"]), out ErrorMessage1, out streetformat);
            cvulica1.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvulica1.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvbroj1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string housenumberformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateHouseNumber(txtbroj1.Text, errLabelBroj1.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtbroj1IsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnHouseNumberValidation1"]), out ErrorMessage1, out housenumberformat);
            cvbroj1.ErrorMessage = ErrorMessage1;
            errLabelBroj1.Text = string.Empty;
        }
        catch (Exception)
        {
            cvbroj1.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvpostanskibroj1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string postnumberformat = string.Empty;
            args.IsValid = UtilsValidation.ValidatePostNumber(txtpostanskibroj1.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtpostanskibroj1IsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPostNumberValidation1"]), out ErrorMessage1, out postnumberformat);
            cvpostanskibroj1.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvpostanskibroj1.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvpak1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string pakformat = string.Empty;
            args.IsValid = UtilsValidation.ValidatePAK(txtpak1.Text, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtpak1IsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPAKValidation1"]), out ErrorMessage1, out pakformat);
            cvpak1.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvpak1.ErrorMessage = string.Empty;
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

            args.IsValid = UtilsValidation.ValidateRok(ddlrokkoriscenjasert.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlrokkoriscenjasertIsRequired"]), IDItem, out ErrorMessage1);
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

            args.IsValid = UtilsValidation.ValidateMedij(ddlmedijsert.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlmedijsertIsRequired"]), IDItem, out ErrorMessage1);
            cvmedijsert.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvmedijsert.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvsertjmbg_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlsertjmbgString = ddlsertjmbg.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlsertjmbgString, out IDItem);

            args.IsValid = UtilsValidation.ValidateSertJMBG(ddlsertjmbg.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlsertjmbgIsRequired"]), IDItem, out ErrorMessage1);
            cvsertjmbg.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvsertjmbg.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvvrstadokumenta_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlvrstadokumentaString = ddlvrstadokumenta.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlvrstadokumentaString, out IDItem);

            args.IsValid = UtilsValidation.ValidateSertAdresa(ddlvrstadokumenta.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlvrstadokumentaIsRequired"]), IDItem, out ErrorMessage1);
            cvvrstadokumenta.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvvrstadokumenta.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA JMBG-a DRUGA FORMA---------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txtjmbg_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged2();
        if (errLabel1.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ddlsertjmbg;
            SetFocusOnDropDownLists();
        }
    }

    private void CheckIfChannelHasChanged2()
    {
        bool RRforeigner = false;
        string newJMBG = txtjmbg.Text;
        string errorMessage = string.Empty;
        string jmbgformat = string.Empty;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnJMBGValidation"]))
        {
            UtilsValidation.validateJMBG(newJMBG, Constants.ID_ITEM_DDLSERTJMBG, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtjmbgIsRequired"]), out errorMessage, out jmbgformat, out RRforeigner);
            errLabel1.Text = errorMessage;
            txtdatumrodjenja.Text = jmbgformat;

            if (RRforeigner)
            {
                ddlsertjmbg.SelectedValue = GetItemID(Constants.YES, Constants.ITEM_YES_NO).ToString();
                Session["zahtev-izdavanje-pravno-lice-ddlsertjmbg"] = false;
            }
            else
            {
                Session["zahtev-izdavanje-pravno-lice-ddlsertjmbg"] = true;
            }
        }
        else
        {
            newJMBG = txtjmbg.Text;
            errorMessage = string.Empty;
        }
    }

    private int GetItemID(int ItemValue, int IDTypeOfItem)
    {
        int ItemID = 0;

        ItemBLL itemBLL = new ItemBLL();

        try
        {
            ItemID = itemBLL.GetItemIDByValueAndType(ItemValue, IDTypeOfItem);
        }
        catch (Exception ex)
        {
            log.Error("Error while getting ItemID. " + ex.Message);
        }

        return ItemID;
    }

    protected void cvjmbg_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool RRforeigner = false;
        string newJMBG = txtjmbg.Text;
        string errMessage = string.Empty;
        string jmbgformat = string.Empty;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnJMBGValidation"]))
        {
            args.IsValid = UtilsValidation.validateJMBG(newJMBG, Constants.ID_ITEM_DDLSERTJMBG, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtjmbgIsRequired"]), out errMessage, out jmbgformat, out RRforeigner);
            cvjmbg.ErrorMessage = errMessage;
            txtdatumrodjenja.Text = jmbgformat;

            if (RRforeigner)
            {
                ddlsertjmbg.SelectedValue = GetItemID(Constants.YES, Constants.ITEM_YES_NO).ToString();
                Session["zahtev-izdavanje-pravno-lice-ddlsertjmbg"] = false;
            }
            else
            {
                Session["zahtev-izdavanje-pravno-lice-ddlsertjmbg"] = true;
            }
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationJMBG(newJMBG, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtjmbgIsRequired"]), out errMessage, out jmbgformat);
            cvjmbg.ErrorMessage = errMessage;
        }
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Telefonskog broja DRUGA FORMA----------------------
    //------------------------------------------------------------------------------------------------
    protected void txttelefon_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged3();
        if (errLabelNumber.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ddlsertadresa;
            SetFocusOnDropDownLists();
        }
    }

    private void CheckIfChannelHasChanged3()
    {
        string newNumber = txttelefon.Text;
        string errorMessage = string.Empty;
        string numberformat = string.Empty;
        bool LegalEntityPhone = false;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPhone2Validation"]))
        {
            UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txttelefonIsRequired"]), LegalEntityPhone, out errorMessage, out numberformat);
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

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnPhone2Validation"]))
        {
            args.IsValid = UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
            newNumber = numberformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
        }
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Adresa E Pošte DRUGA FORMA-------------------------
    //------------------------------------------------------------------------------------------------

    protected void txtadresaeposte1_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged4();
        if (errLabelMail.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = txttelefon;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged4()
    {
        string newMail = txtadresaeposte1.Text;
        string errorMessage = string.Empty;
        string mailformat = string.Empty;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnEmail2Validation"]))
        {
            UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtadresaeposte1IsRequired"]), out errorMessage, out mailformat);
            errLabelMail.Text = errorMessage;
            newMail = mailformat;
        }
        else
        {
            newMail = txtadresaeposte1.Text;
            errorMessage = string.Empty;
        }
    }

    
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------GRID VIEW ACTION--------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------   

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string DatumRodjenja = string.Empty;
        string SertJmbg = string.Empty;
        string VrstaDokumenata = string.Empty;
        string BrojIDDokumenta = string.Empty;
        string ImeInstitucije = string.Empty;
        string DatumIzdavanja = string.Empty;
        string DatumIsteka = string.Empty;
        string SertAdresa = string.Empty;
        string Mesto = string.Empty;
        string Ulica = string.Empty;
        string Broj = string.Empty;
        string PostanskiBroj = string.Empty;
        string CenaSaPorezom = string.Empty;
        string PAK = string.Empty;

        if (e.CommandName.Equals("ViewProfile"))
        {
            int rowno = Convert.ToInt32(e.CommandArgument);

            string JMBG = GridView1.DataKeys[rowno]["Jmbg"].ToString();

            //postavljeno zbog greške "Cannot have multiple items selected in a DropDownList".
            //Upiše se drugi korisnik pa se klikne na preged profila prvog korisnika i baca ovu grešku gore
            ClearDropDownLists();
            bool Delete = false;

            GridViewAction(rowno, JMBG, Delete, out DatumRodjenja, out SertJmbg, out VrstaDokumenata, out BrojIDDokumenta, out ImeInstitucije, out DatumIzdavanja, out DatumIsteka, out SertAdresa, out Mesto, out Ulica, out Broj, out PostanskiBroj, out PAK, out CenaSaPorezom);

            //Ove kolone se ne vide, nalaze se u HidenField-u u GridView-u
            txtdatumrodjenja.Text = DatumRodjenja;
            ddlsertjmbg.Items.FindByText(SertJmbg).Selected = true;
            ddlvrstadokumenta.Items.FindByText(VrstaDokumenata).Selected = true;
            txtbrojiddokumenta.Text = BrojIDDokumenta;
            txtimeinstitucije.Text = ImeInstitucije;
            txtdatumizdavanja.Text = DatumIzdavanja;
            txtdatumisteka.Text = DatumIsteka;
            ddlsertadresa.Items.FindByText(SertAdresa).Selected = true;
            txtmesto1.Text = Mesto;
            txtulica1.Text = Ulica;
            txtbroj1.Text = Broj;
            txtpostanskibroj1.Text = PostanskiBroj;
            txtpak1.Text = PAK;
            txtcenasaporezom1.Text = CenaSaPorezom;
            txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtcenasaporezom1.ReadOnly = true;
            txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtdatumrodjenja.ReadOnly = true;

            Session["zahtev-izdavanje-pravno-lice-sertjmbg1"] = false;
            Session["zahtev-izdavanje-pravno-lice-vrstadokumenta"] = false;
            Session["zahtev-izdavanje-pravno-lice-sertadresa"] = false;
            Session["zahtev-izdavanje-pravno-lice-rokkoriscenja"] = false;
            Session["zahtev-izdavanje-pravno-lice-medijsert"] = false;

            txtime.ReadOnly = true;
            txtime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtprezime.ReadOnly = true;
            txtprezime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtjmbg.ReadOnly = true;
            txtjmbg.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtdatumrodjenja.ReadOnly = true;
            txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-sertjmbg1"]);
            ddlsertjmbg.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            ddlsertjmbg.CssClass = SetCss1;
            ddlvrstadokumenta.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-vrstadokumenta"]);
            ddlvrstadokumenta.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            ddlvrstadokumenta.CssClass = SetCss2;
            txtbrojiddokumenta.ReadOnly = true;
            txtbrojiddokumenta.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtimeinstitucije.ReadOnly = true;
            txtimeinstitucije.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtdatumizdavanja.ReadOnly = true;
            txtdatumizdavanja.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtdatumisteka.ReadOnly = true;
            txtdatumisteka.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtadresaeposte1.ReadOnly = true;
            txtadresaeposte1.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txttelefon.ReadOnly = true;
            txttelefon.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-sertadresa"]);
            ddlsertadresa.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            ddlsertadresa.CssClass = SetCss4;
            txtulica1.ReadOnly = true;
            txtulica1.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtbroj1.ReadOnly = true;
            txtbroj1.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtpostanskibroj1.ReadOnly = true;
            txtpostanskibroj1.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtpak1.ReadOnly = true;
            txtpak1.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            txtmesto1.ReadOnly = true;
            txtmesto1.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rokkoriscenja"]);
            ddlrokkoriscenjasert.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            ddlrokkoriscenjasert.CssClass = SetCss1;
            ddlmedijsert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-medijsert"]);
            ddlmedijsert.BackColor = ColorTranslator.FromHtml(SetDarkGray);
            ddlmedijsert.CssClass = SetCss1;
            txtcenasaporezom1.ReadOnly = true;
            txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetDarkGray);

            Div2.Visible = true;
            Div3.Visible = false;

            ScriptManager.RegisterStartupScript(this, GetType(), "DisableCalendar", "DisableCalendar();", true);
        }
        else if (e.CommandName.Equals("EditProfile"))
        {
            int rowno = Convert.ToInt32(e.CommandArgument);
            Session["zahtev-izdavanje-pravno-lice-rowno"] = rowno;

            string JMBG = GridView1.DataKeys[rowno]["Jmbg"].ToString();
            Session["zahtev-izdavanje-pravno-lice-jmbgedit"] = JMBG;

            //postavljeno zbog greške "Cannot have multiple items selected in a DropDownList".
            //Upiše se drugi korisnik pa se klikne na preged profila prvog korisnika i baca ovu grešku gore
            ClearDropDownLists();
            bool Delete = false;

            GridViewAction(rowno, JMBG, Delete, out DatumRodjenja, out SertJmbg, out VrstaDokumenata, out BrojIDDokumenta, out ImeInstitucije, out DatumIzdavanja, out DatumIsteka, out SertAdresa, out Mesto, out Ulica, out Broj, out PostanskiBroj, out PAK, out CenaSaPorezom);

            //Ove kolone se ne vide, nalaze se u HidenField-u u GridView-u
            txtdatumrodjenja.Text = DatumRodjenja;
            ddlsertjmbg.Items.FindByText(SertJmbg).Selected = true;
            ddlvrstadokumenta.Items.FindByText(VrstaDokumenata).Selected = true;
            txtbrojiddokumenta.Text = BrojIDDokumenta;
            txtimeinstitucije.Text = ImeInstitucije;
            txtdatumizdavanja.Text = DatumIzdavanja;
            txtdatumisteka.Text = DatumIsteka;
            ddlsertadresa.Items.FindByText(SertAdresa).Selected = true;
            txtmesto1.Text = Mesto;
            txtulica1.Text = Ulica;
            txtbroj1.Text = Broj;
            txtpostanskibroj1.Text = PostanskiBroj;
            txtcenasaporezom1.Text = CenaSaPorezom;
            txtpak1.Text = PAK;
            Session["zahtev-izdavanje-pravno-lice-CenaSaPorezom"] = CenaSaPorezom;   //dodato zog izracunavanje cena
            txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtcenasaporezom1.ReadOnly = true;
            txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtdatumrodjenja.ReadOnly = true;

            Session["zahtev-izdavanje-pravno-lice-sertjmbg1"] = true;
            Session["zahtev-izdavanje-pravno-lice-vrstadokumenta"] = true;
            Session["zahtev-izdavanje-pravno-lice-sertadresa"] = true;
            Session["zahtev-izdavanje-pravno-lice-rokkoriscenja"] = true;
            Session["zahtev-izdavanje-pravno-lice-medijsert"] = true;

            txtime.ReadOnly = false;
            txtime.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtprezime.ReadOnly = false;
            txtprezime.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtjmbg.ReadOnly = false;
            txtjmbg.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtdatumrodjenja.ReadOnly = false;
            txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetLightGray);
            ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-sertjmbg1"]);
            ddlsertjmbg.BackColor = ColorTranslator.FromHtml(SetWhite);
            ddlsertjmbg.CssClass = SetCss1;
            ddlvrstadokumenta.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-vrstadokumenta"]);
            ddlvrstadokumenta.BackColor = ColorTranslator.FromHtml(SetWhite);
            ddlvrstadokumenta.CssClass = SetCss2;
            txtbrojiddokumenta.ReadOnly = false;
            txtbrojiddokumenta.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtimeinstitucije.ReadOnly = false;
            txtimeinstitucije.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtdatumizdavanja.ReadOnly = false;
            txtdatumizdavanja.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtdatumisteka.ReadOnly = false;
            txtdatumisteka.BackColor = ColorTranslator.FromHtml(SetWhite);
            txtadresaeposte1.ReadOnly = false;
            txtadresaeposte1.BackColor = ColorTranslator.FromHtml(SetWhite);
            txttelefon.ReadOnly = false;
            txttelefon.BackColor = ColorTranslator.FromHtml(SetWhite);
            ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-sertadresa"]);
            ddlsertadresa.BackColor = ColorTranslator.FromHtml(SetWhite);
            ddlsertadresa.CssClass = SetCss4;
            txtulica1.ReadOnly = false;
            txtulica1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtbroj1.ReadOnly = false;
            txtbroj1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtpostanskibroj1.ReadOnly = false;
            txtpostanskibroj1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtpak1.ReadOnly = false;
            txtpak1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtmesto1.ReadOnly = false;
            txtmesto1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-rokkoriscenja"]);
            ddlrokkoriscenjasert.BackColor = ColorTranslator.FromHtml(SetWhite);
            ddlrokkoriscenjasert.CssClass = SetCss1;
            ddlmedijsert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-medijsert"]);
            ddlmedijsert.BackColor = ColorTranslator.FromHtml(SetWhite);
            ddlmedijsert.CssClass = SetCss1;
            txtcenasaporezom1.ReadOnly = false;
            txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetLightGray);

            Div2.Visible = false;
            Div3.Visible = true;

            ScriptManager.RegisterStartupScript(this, GetType(), "EnableCalendar", "EnableCalendar();", true);
        }
        else if (e.CommandName.Equals("DeleteProfile"))
        {           
            int rowno = Convert.ToInt32(e.CommandArgument);
            string JMBG = GridView1.DataKeys[rowno]["Jmbg"].ToString();

            bool Delete = true;

            GridViewAction(rowno, JMBG, Delete, out DatumRodjenja, out SertJmbg, out VrstaDokumenata, out BrojIDDokumenta, out ImeInstitucije, out DatumIzdavanja, out DatumIsteka, out SertAdresa, out Mesto, out Ulica, out Broj, out PostanskiBroj, out PAK, out CenaSaPorezom);

            DataTable dtCurrentTable = (DataTable)ViewState["pravno-lice-CurrentTable"];

            string Ime = Session["zahtev-izdavanje-pravno-lice-ImeEdit"].ToString();
            string Prezime = Session["zahtev-izdavanje-pravno-lice-PrezimeEdit"].ToString();
            string Telefon = Session["zahtev-izdavanje-pravno-lice-TelefonEdit"].ToString();
            string Medij = Session["zahtev-izdavanje-pravno-lice-MedijEdit"].ToString();
            string Rok = Session["zahtev-izdavanje-pravno-lice-RokEdit"].ToString();

            DataRow[] rows = dtCurrentTable.Select("Jmbg ='" + JMBG + "' AND Ime ='" + Ime + "' AND Prezime ='" + Prezime + "' AND Telefon ='" + Telefon + "' AND Medij ='" + Medij + "' AND Rok ='" + Rok + "'");

            foreach (DataRow row in rows)
            {
                row.Delete();

                dtCurrentTable.AcceptChanges();
            }

            ViewState["pravno-lice-CurrentTable"] = dtCurrentTable;

            GridView1.DataSource = dtCurrentTable;
            GridView1.DataBind();

            FormBeforeEditOrSubmit();

            //-------------------------------------
            string NovaCena = string.Empty;
            CalculatePriceAfterDeleting(CenaSaPorezom, txtcenasaporezom.Text, out NovaCena);
            txtcenasaporezom.Text = NovaCena;
            //-------------------------------------
        }
    }

    protected void GridViewAction(int RowNumber, string jmbg, bool Delete, out string DatumRodjenja, out string SertJmbg, out string VrstaDokumenta, out string BrojIDDokumenta, out string ImeInstitucije, out string DatumIzdavanja, out string DatumIsteka, out string SertAdresa, out string Mesto, out string Ulica, out string Broj, out string PostanskiBroj, out string PAK, out string CenaSaPorezom)
    {
        GridViewRow row = GridView1.Rows[RowNumber];

        string Ime = row.Cells[1].Text;
        Session["zahtev-izdavanje-pravno-lice-ImeEdit"] = Ime;
        string Prezime = row.Cells[2].Text;
        Session["zahtev-izdavanje-pravno-lice-PrezimeEdit"] = Prezime;
        string Adresaeposte = row.Cells[4].Text;
        string Telefon = row.Cells[5].Text;
        Session["zahtev-izdavanje-pravno-lice-TelefonEdit"] = Telefon;
        string Medij = row.Cells[6].Text;
        Session["zahtev-izdavanje-pravno-lice-MedijEdit"] = Medij;
        string Rok = row.Cells[7].Text;
        Session["zahtev-izdavanje-pravno-lice-RokEdit"] = Rok;

        txtime.Text = Ime;
        txtprezime.Text = Prezime;
        txtjmbg.Text = jmbg;
        txttelefon.Text = Telefon;
        if (Adresaeposte == "&nbsp;" || Adresaeposte == string.Empty)
        {
            txtadresaeposte1.Text = string.Empty;
        }
        else
        {
            txtadresaeposte1.Text = Adresaeposte;
        }
        ddlrokkoriscenjasert.Items.FindByText(Rok).Selected = true;
        ddlmedijsert.Items.FindByText(Medij).Selected = true;
        /*
        HiddenField hd = row.FindControl("SertJmbg") as HiddenField;
        SertJmbg = hd.Value;
        HiddenField hd1 = row.FindControl("VrstaDokumenta") as HiddenField;
        VrstaDokumenta = hd1.Value;
        HiddenField hd2 = row.FindControl("BrojIDDokumenta") as HiddenField;
        BrojIDDokumenta = hd2.Value;
        HiddenField hd3 = row.FindControl("ImeInstitucije") as HiddenField;
        ImeInstitucije = hd3.Value;
        HiddenField hd4 = row.FindControl("DatumIzdavanja") as HiddenField;
        DatumIzdavanja = hd4.Value;
        HiddenField hd5 = row.FindControl("DatumIsteka") as HiddenField;
        DatumIsteka = hd5.Value;
        HiddenField hd6 = row.FindControl("SertAdresa") as HiddenField;
        SertAdresa = hd6.Value;
        HiddenField hd7 = row.FindControl("Mesto") as HiddenField;
        Mesto = hd7.Value;
        HiddenField hd8 = row.FindControl("Ulica") as HiddenField;
        Ulica = hd8.Value;
        HiddenField hd9 = row.FindControl("Broj") as HiddenField;
        Broj = hd9.Value;
        HiddenField hd10 = row.FindControl("PostanskiBroj") as HiddenField;
        PostanskiBroj = hd10.Value;
        HiddenField hd11 = row.FindControl("CenaSaPorezom") as HiddenField;
        CenaSaPorezom = hd11.Value;
        */
        DatumRodjenja = GridView1.DataKeys[RowNumber]["DatumRodjenja"].ToString();
        SertJmbg = GridView1.DataKeys[RowNumber]["SertJmbg"].ToString();
        VrstaDokumenta = GridView1.DataKeys[RowNumber]["VrstaDokumenta"].ToString();
        BrojIDDokumenta = GridView1.DataKeys[RowNumber]["BrojIDDokumenta"].ToString();
        ImeInstitucije = GridView1.DataKeys[RowNumber]["ImeInstitucije"].ToString();
        DatumIzdavanja = GridView1.DataKeys[RowNumber]["DatumIzdavanja"].ToString();
        DatumIsteka = GridView1.DataKeys[RowNumber]["DatumIsteka"].ToString();
        SertAdresa = GridView1.DataKeys[RowNumber]["SertAdresa"].ToString();
        Mesto = GridView1.DataKeys[RowNumber]["Mesto"].ToString();
        Ulica = GridView1.DataKeys[RowNumber]["Ulica"].ToString();
        Broj = GridView1.DataKeys[RowNumber]["Broj"].ToString();
        PostanskiBroj = GridView1.DataKeys[RowNumber]["PostanskiBroj"].ToString();
        PAK = GridView1.DataKeys[RowNumber]["PAK"].ToString();
        CenaSaPorezom = GridView1.DataKeys[RowNumber]["CenaSaPorezom"].ToString();

        if (!Delete)
        { 
            myDiv6.Visible = false;
            myDiv8.Visible = false;
            Container00.Visible = false;
            Container1.Visible = false;
            //Container2.Visible = false;
            Container3.Visible = false;
            Container33.Visible = false;
            Container34.Visible = false;
            myDiv5.Visible = false;
            Container4.Visible = false;
            Container5.Visible = false;

            Container6.Visible = true;
            Container7.Visible = true;
            Container77.Visible = true;
            Container8.Visible = true;
            Container9.Visible = true;
        
            Container10.Visible = true;
            Container11.Visible = true;
            Container12.Visible = true;

            Container77.Visible = true;
        }
    }

    protected void ClearDropDownLists()
    {
        ddlsertjmbg.ClearSelection();
        ddlvrstadokumenta.ClearSelection();
        ddlsertadresa.ClearSelection();
        ddlrokkoriscenjasert.ClearSelection();
        ddlmedijsert.ClearSelection();
    }

    protected void btnEdit1_Click2(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            ClientScript.RegisterStartupScript(GetType(), "unhook", "unhook();", true);
            if (true)
            {
                string staracenasaporezom = string.Empty;
                string cenasaporezom1 = Session["zahtev-izdavanje-pravno-lice-CenaSaPorezom"].ToString();
                CalculatePricesBeforeEditing(txtcenasaporezom.Text, cenasaporezom1, out staracenasaporezom);
                string cenasaporezom3 = staracenasaporezom;

                FormBeforeEditOrSubmit();

                EditExistingRowToGrid();

                string novacenasaporezom = string.Empty;
                CalculatePricesAfterEditing(cenasaporezom3, txtcenasaporezom1.Text, out novacenasaporezom);
                txtcenasaporezom.Text = novacenasaporezom;
                //----------------------------------------------------------------------------------------------------------------------
            }
        }
        else if (!Page.IsValid)
        {
            errLabel1.Text = string.Empty;
            errLabelNumber.Text = string.Empty;
            errLabelMail.Text = string.Empty;
            errLabelURL.Text = string.Empty;
            txtcenasaporezom1.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtcenasaporezom1.ReadOnly = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
        }
    }

    protected void EditExistingRowToGrid()
    {
        if (ViewState["pravno-lice-CurrentTable"] != null)
        {
            DataTable dtCurrentTable = (DataTable)ViewState["pravno-lice-CurrentTable"];
            string rownumber = Session["zahtev-izdavanje-pravno-lice-rowno"].ToString();
            string jmbg = Session["zahtev-izdavanje-pravno-lice-jmbgedit"].ToString();

            string Ime = Session["zahtev-izdavanje-pravno-lice-ImeEdit"].ToString();
            string Prezime = Session["zahtev-izdavanje-pravno-lice-PrezimeEdit"].ToString();
            string Telefon = Session["zahtev-izdavanje-pravno-lice-TelefonEdit"].ToString();
            string Medij = Session["zahtev-izdavanje-pravno-lice-MedijEdit"].ToString();
            string Rok = Session["zahtev-izdavanje-pravno-lice-RokEdit"].ToString();

            DataRow[] rows = dtCurrentTable.Select("Jmbg ='"+ jmbg + "' AND Ime ='" + Ime + "' AND Prezime ='" + Prezime + "' AND Telefon ='" + Telefon + "' AND Medij ='" + Medij + "' AND Rok ='" + Rok + "'");

            foreach (DataRow row in rows)
            {
                    row["Ime"] = txtime.Text;
                    row["Prezime"] = txtprezime.Text;
                    row["Jmbg"] = txtjmbg.Text;
                    row["AdresaEPoste"] = txtadresaeposte1.Text;
                    row["Telefon"] = txttelefon.Text;
                    row["Medij"] = ddlmedijsert.SelectedItem.Text;
                    row["Rok"] = ddlrokkoriscenjasert.SelectedItem.Text;
                    //Ove kolone se ne vide, uvedene su zbog templatefield button-a u GridView-u
                    row["SertJmbg"] = ddlsertjmbg.SelectedItem.Text;
                    row["VrstaDokumenta"] = ddlvrstadokumenta.SelectedItem.Text;
                    row["BrojIDDokumenta"] = txtbrojiddokumenta.Text;
                    row["ImeInstitucije"] = txtimeinstitucije.Text;
                    row["DatumIzdavanja"] = txtdatumizdavanja.Text;
                    row["DatumIsteka"] = txtdatumisteka.Text;
                    row["SertAdresa"] = ddlsertadresa.SelectedItem.Text;
                    row["Mesto"] = txtmesto1.Text;
                    row["Ulica"] = txtulica1.Text;
                    row["Broj"] = txtbroj1.Text;
                    row["PostanskiBroj"] = txtpostanskibroj1.Text;
                    row["PAK"] = txtpak1.Text;
                    row["CenaSaPorezom"] = txtcenasaporezom1.Text;

                    dtCurrentTable.AcceptChanges();
                    row.SetModified();
            }

            ViewState["pravno-lice-CurrentTable"] = dtCurrentTable;

            GridView1.DataSource = dtCurrentTable;
            GridView1.DataBind();
        }

    }

    private void CalculatePricesBeforeEditing(string cenasaporezom, string cenasaporezom1, out string cenasaporezom2)
    {
       string number1 = cenasaporezom;
       string number2 = cenasaporezom1;
       string number3 = string.Empty;
       string number4 = string.Empty;
       cenasaporezom2 = string.Empty;

        if (number1.Length == 8)
        {
            number3 = number1.Substring(0, 1) + number1.Substring(2, 1) + number1.Substring(3, 1) + number1.Substring(4, 1);
        }
        else
        {
            number3 = number1.Substring(0, 1) + number1.Substring(1, 1) + number1.Substring(3, 1) + number1.Substring(4, 1) + number1.Substring(5, 1);
        }
        number4 = number2.Substring(0, 1) + number2.Substring(2, 1) + number2.Substring(3, 1) + number2.Substring(4, 1);
        int result = Convert.ToInt32(number3) - Convert.ToInt32(number4);

        if (result < 0)
        {
            cenasaporezom2 = "00.000,00";
        }
        else if (result > 0 || result == 0)
        {
            string cenasaporezomprepared = result.ToString();
            string cenasaporezomprepared1 = Utils.getPreparedEditablePrice(cenasaporezomprepared);
            cenasaporezom2 = cenasaporezomprepared1.Substring(0, 1) + cenasaporezomprepared1.Substring(1, 1) + "." + cenasaporezomprepared1.Substring(2, 1) + cenasaporezomprepared1.Substring(3, 1) + cenasaporezomprepared1.Substring(4, 1) + ",00";
        }

    }

    private void CalculatePricesAfterEditing(string cenasaporezom3, string cenasaporezom4, out string novacenasaporezom)
    {
        novacenasaporezom = string.Empty;
        string number1 = cenasaporezom3;
        string number2 = cenasaporezom4;
        string number3 = number1.Substring(0, 1) + number1.Substring(1, 1) + number1.Substring(3, 1) + number1.Substring(4, 1) + number1.Substring(5, 1);
        string number4 = number2.Substring(0, 1) + number2.Substring(2, 1) + number2.Substring(3, 1) + number2.Substring(4, 1);
        string number5 = Utils.getPreparedEditablePrice(number4);
        int result = Convert.ToInt32(number3) + Convert.ToInt32(number5);

        string novacenasaporezom1 = result.ToString();

        if (novacenasaporezom1.Length <= 4)
        {
            novacenasaporezom = novacenasaporezom1.Substring(0, 1) + "." + novacenasaporezom1.Substring(1, 1) + novacenasaporezom1.Substring(2, 1) + novacenasaporezom1.Substring(3, 1) + ",00";            
        }
        else
        {
            novacenasaporezom = novacenasaporezom1.Substring(0, 1) + novacenasaporezom1.Substring(1, 1) + "." + novacenasaporezom1.Substring(2, 1) + novacenasaporezom1.Substring(3, 1) + novacenasaporezom1.Substring(4, 1) + ",00";
        }
    }

    private void CalculatePriceAfterDeleting(string deletingPrice, string formPrice, out string NewPrice)
    {
        string number1 = formPrice;
        string number2 = deletingPrice;
        string number3 = string.Empty;
        string number4 = string.Empty;
        string cenasaporezomprepared1;
        if (number1.Length == 8)
        {
            number3 = number1.Substring(0, 1) + number1.Substring(2, 1) + number1.Substring(3, 1) + number1.Substring(4, 1);
        }
        else
        {
            number3 = number1.Substring(0, 1) + number1.Substring(1, 1) + number1.Substring(3, 1) + number1.Substring(4, 1) + number1.Substring(5, 1);
        }
        number4 = number2.Substring(0, 1) + number2.Substring(2, 1) + number2.Substring(3, 1) + number2.Substring(4, 1);
        int result = Convert.ToInt32(number3) - Convert.ToInt32(number4);

        cenasaporezomprepared1 = result.ToString();
        if (cenasaporezomprepared1 == "0")
        {
            NewPrice = "0,00";
        }
        else if (cenasaporezomprepared1.Length == 4)
        {
            NewPrice = cenasaporezomprepared1.Substring(0, 1) + "." + cenasaporezomprepared1.Substring(1, 1) + cenasaporezomprepared1.Substring(2, 1) + cenasaporezomprepared1.Substring(3, 1) + ",00";
        }
        else
        {
            NewPrice = cenasaporezomprepared1.Substring(0, 1) + cenasaporezomprepared1.Substring(1, 1) + "." + cenasaporezomprepared1.Substring(2, 1) + cenasaporezomprepared1.Substring(3, 1) + cenasaporezomprepared1.Substring(4, 1) + ",00";
        }
    }

    private void CalculatePrices()
    {
        string number1 = string.Empty;
        string number2 = string.Empty;
        string number3 = string.Empty;
        string number4 = string.Empty;
        try
        {
            if (txtcenasaporezom.Text == "0,00")
            {
                number1 = "00.000,00";
                number2 = txtcenasaporezom1.Text;
                number3 = number1.Substring(0, 1) + number1.Substring(1, 1) + number1.Substring(3, 1) + number1.Substring(4, 1) + number1.Substring(5, 1);
                number4 = number2.Substring(0, 1) + number2.Substring(2, 1) + number2.Substring(3, 1) + number2.Substring(4, 1);
                int result = Convert.ToInt32(number3) + Convert.ToInt32(number4);
                string cenasaporezom = result.ToString();

                if (cenasaporezom.Length <= 4)
                {
                    txtcenasaporezom.Text = cenasaporezom.Substring(0, 1) + "." + cenasaporezom.Substring(1, 1) + cenasaporezom.Substring(2, 1) + cenasaporezom.Substring(3, 1) + ",00";
                }
                else
                {
                    txtcenasaporezom.Text = cenasaporezom.Substring(0, 1) + cenasaporezom.Substring(1, 1) + "." + cenasaporezom.Substring(2, 1) + cenasaporezom.Substring(3, 1) + cenasaporezom.Substring(4, 1) + ",00";
                }
            }
            else
            {
                number1 = txtcenasaporezom.Text;
                if (number1.Length == 8)
                {
                    number3 = number1.Substring(0, 1) + number1.Substring(2, 1) + number1.Substring(3, 1) + number1.Substring(4, 1);
                }
                else
                {
                    number3 = number1.Substring(0, 1) + number1.Substring(1, 1) + number1.Substring(3, 1) + number1.Substring(4, 1) + number1.Substring(5, 1);
                }

                number2 = txtcenasaporezom1.Text;
                number4 = number2.Substring(0, 1) + number2.Substring(2, 1) + number2.Substring(3, 1) + number2.Substring(4, 1);
                int result = Convert.ToInt32(number3) + Convert.ToInt32(number4);
                string cenasaporezom = result.ToString();
                if (cenasaporezom.Length <= 4)
                {
                    txtcenasaporezom.Text = cenasaporezom.Substring(0, 1) + "." + cenasaporezom.Substring(1, 1) + cenasaporezom.Substring(2, 1) + cenasaporezom.Substring(3, 1) + ",00";
                }
                else
                {
                    txtcenasaporezom.Text = cenasaporezom.Substring(0, 1) + cenasaporezom.Substring(1, 1) + "." + cenasaporezom.Substring(2, 1) + cenasaporezom.Substring(3, 1) + cenasaporezom.Substring(4, 1) + ",00";
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Greška prilikom kalkulacije cene. " + ex.Message);
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public string BrojZahteva = string.Empty;
    public string BrojZahtevaPravnoLice = string.Empty;
    public string legalEntityName = string.Empty;

    protected BxSoapEnvelope createSoapEnvelope(Utility utility, int i, List<PisMessServiceReference.CertificatesAuthorizedUser> certificatesAuthorizedUsers, string legalEntityName)
    {
        string Ime = string.Empty;
        string Prezime = string.Empty;
        string JMBG = string.Empty;
        string AdresaEPoste = string.Empty;
        string AdresaEPoste1 = string.Empty;
        string Telefon = string.Empty;
        string Medij = string.Empty;
        string Rok = string.Empty;
        string DatumRodjenja = string.Empty;
        string SertJMBG = string.Empty;
        string VrstaDokumenta = string.Empty;
        string BrojIDDokumenta = string.Empty;
        string ImeInstitucije = string.Empty;
        string DatumIzdavanja = string.Empty;
        string DatumIsteka = string.Empty;
        string SertAdresa = string.Empty;
        string Mesto = string.Empty;
        string Ulica = string.Empty;
        string Broj = string.Empty;
        string PostanskiBroj = string.Empty;
        string PAK = string.Empty;
        string CenaSaPorezom = string.Empty;
        
        BxSoapEnvelope envelope = new BxSoapEnvelopeRequestToken();

        envelope.BxData.setValue(@"productionProfile", @"Qualified Electronic Certificate Legal Entity");
        envelope.BxData.setValue(@"businessRegistrationNumber", txtmaticnibroj.Text);
        envelope.BxData.setValue(@"legalEntityName", legalEntityName);
        envelope.BxData.setValue(@"taxNumber", txtpib.Text);
        envelope.BxData.setValue(@"addedTax", (utility.getItemValueAddedTax(Convert.ToInt32(ddlobveznikpdv.SelectedValue)).ToString()));
        envelope.BxData.setValue(@"legalEntityActivityCode", txtsifradel.Text);
        envelope.BxData.setValue(@"legalEntityCity", txtmesto.Text);
        envelope.BxData.setValue(@"legalEntityStreet", txtulica.Text);
        envelope.BxData.setValue(@"legalEntityHouseNumber", txtbroj.Text);
        envelope.BxData.setValue(@"legalEntityPostalCode", txtpostanskibroj.Text);
        envelope.BxData.setValue(@"legalEntityPAK", txtpak.Text);
        envelope.BxData.setValue(@"legalEntityPhoneNumber", txtkontakttel.Text);
        envelope.BxData.setValue(@"legalEntityEmail", txtadresaeposte.Text);
        envelope.BxData.setValue(@"legalEntityURL", txtwebadresa.Text);
        envelope.BxData.setValue(@"legalEntityGivenName", txtimezz.Text);
        envelope.BxData.setValue(@"legalEntityLastName", txtprezimezz.Text);
        envelope.BxData.setValue(@"legalEntityTotalPrice", txtcenasaporezom.Text);
        envelope.BxData.setValue(@"legalEntityOrderNumber", Session["zahtev-izdavanje-pravno-lice-brojzahteva"].ToString());
        //------------------------------------------------------------------------------
        Ime = GridView1.Rows[i].Cells[1].Text;
        Prezime = GridView1.Rows[i].Cells[2].Text;
        JMBG = GridView1.Rows[i].Cells[3].Text;
        AdresaEPoste = GridView1.Rows[i].Cells[4].Text;
        if (AdresaEPoste == "&nbsp;" || AdresaEPoste == string.Empty)
        {
            AdresaEPoste1 = string.Empty;
        }
        else
        {
            AdresaEPoste1 = AdresaEPoste;
        }
        Telefon = GridView1.Rows[i].Cells[5].Text;
        Medij = GridView1.Rows[i].Cells[6].Text;
        Rok = GridView1.Rows[i].Cells[7].Text;

        //todo
        //SertJMBG = ((HiddenField)GridView1.Rows[i].FindControl("SertJmbg")).Value;
        //VrstaDokumenta = ((HiddenField)GridView1.Rows[i].FindControl("VrstaDokumenta")).Value;
        //BrojIDDokumenta = ((HiddenField)GridView1.Rows[i].FindControl("BrojIDDokumenta")).Value;
        //ImeInstitucije = ((HiddenField)GridView1.Rows[i].FindControl("ImeInstitucije")).Value;
        //DatumIzdavanja = ((HiddenField)GridView1.Rows[i].FindControl("DatumIzdavanja")).Value;
        //DatumIsteka = ((HiddenField)GridView1.Rows[i].FindControl("DatumIsteka")).Value;
        //SertAdresa = ((HiddenField)GridView1.Rows[i].FindControl("SertAdresa")).Value;
        //Mesto = ((HiddenField)GridView1.Rows[i].FindControl("Mesto")).Value;
        //Ulica = ((HiddenField)GridView1.Rows[i].FindControl("Ulica")).Value;
        //Broj = ((HiddenField)GridView1.Rows[i].FindControl("Broj")).Value;
        //PostanskiBroj = ((HiddenField)GridView1.Rows[i].FindControl("PostanskiBroj")).Value;
        //CenaSaPorezom = ((HiddenField)GridView1.Rows[i].FindControl("CenaSaPorezom")).Value;
        DatumRodjenja = GridView1.DataKeys[i]["DatumRodjenja"].ToString();
        SertJMBG = GridView1.DataKeys[i]["SertJmbg"].ToString();
        VrstaDokumenta = GridView1.DataKeys[i]["VrstaDokumenta"].ToString();
        BrojIDDokumenta = GridView1.DataKeys[i]["BrojIDDokumenta"].ToString();
        ImeInstitucije = GridView1.DataKeys[i]["ImeInstitucije"].ToString();
        DatumIzdavanja = GridView1.DataKeys[i]["DatumIzdavanja"].ToString();
        DatumIsteka = GridView1.DataKeys[i]["DatumIsteka"].ToString();
        SertAdresa = GridView1.DataKeys[i]["SertAdresa"].ToString();
        Mesto = GridView1.DataKeys[i]["Mesto"].ToString();
        Ulica = GridView1.DataKeys[i]["Ulica"].ToString();
        Broj = GridView1.DataKeys[i]["Broj"].ToString();
        PostanskiBroj = GridView1.DataKeys[i]["PostanskiBroj"].ToString();
        PAK = GridView1.DataKeys[i]["PAK"].ToString();
        CenaSaPorezom = GridView1.DataKeys[i]["CenaSaPorezom"].ToString();

        envelope.BxData.setValue(@"givenName", Ime);
        envelope.BxData.setValue(@"lastName", Prezime);
        envelope.BxData.setValue(@"uniqueCitizensNumber", JMBG);
        envelope.BxData.setValue(@"dateOfBirth", DatumRodjenja);
        envelope.BxData.setValue(@"includeUniqueCitizensNumber", (utility.getEnglishTextInputString(SertJMBG)));
        envelope.BxData.setValue(@"identificationDocumentType", (utility.getEnglishTextInputString(VrstaDokumenta)));
        envelope.BxData.setValue(@"identificationDocumentNumber", BrojIDDokumenta);
        //envelope.BxData.setValue(@"identificationIssuer", ImeInstitucije);
        envelope.BxData.setValue(@"identificationIssuerName", ImeInstitucije);
        envelope.BxData.setValue(@"identificationDocumentValidFrom", DatumIzdavanja);
        envelope.BxData.setValue(@"identificationDocumentValidUntil", DatumIsteka);
        envelope.BxData.setValue(@"emailAddress", AdresaEPoste1);
        envelope.BxData.setValue(@"phoneNumber", Telefon);
        envelope.BxData.setValue(@"deliveryLocation", (utility.getEnglishTextInputString(SertAdresa)));
        envelope.BxData.setValue(@"distributionCity", Mesto);
        envelope.BxData.setValue(@"distributionStreet", Ulica);
        envelope.BxData.setValue(@"distributionHouseNumber", Broj);
        envelope.BxData.setValue(@"distributionPostalCode", PostanskiBroj);
        envelope.BxData.setValue(@"distributionPAK", PAK);
        envelope.BxData.setValue(@"media", (utility.getEnglishTextInputString(Medij)));
        envelope.BxData.setValue(@"validity", (utility.getEnglishTextInputString(Rok)));
        envelope.BxData.setValue(@"totalPrice", CenaSaPorezom);
        envelope.BxData.setValue(@"identificationIssuer", (Constants.SOAP_INDIVIDUAL_COUNTRY_CODE)); //send CountryCode
        //envelope.BxData.setValue(@"identificationIssuerName", (Constants.SOAP_INDIVIDUAL_COUNTRY)); //send Country Name
        //------------------------------------------------------------------------------            
        envelope.BxData.setValue(@"userAgentStringApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentStringApplicant"].ToString());
        envelope.BxData.setValue(@"ipApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentIP"].ToString());
        envelope.BxData.setValue(@"continentApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentContinent"].ToString());
        envelope.BxData.setValue(@"countryApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentCountry"].ToString());
        envelope.BxData.setValue(@"countryCodeApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentCountryCode"].ToString());
        envelope.BxData.setValue(@"cityApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentCity"].ToString());
        envelope.BxData.setValue(@"osApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentOS"].ToString());
        envelope.BxData.setValue(@"ispApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentISP"].ToString());
        envelope.BxData.setValue(@"browserApplicant", Session["zahtev-izdavanje-pravno-lice-userAgentBrowser"].ToString());
        envelope.BxData.setValue(@"ipOperator", string.Empty);

        string jmbgFinal = string.Empty;
        string includeUniqueCitizensNumber = (utility.getEnglishTextInputString(SertJMBG)).ToString();
        if (includeUniqueCitizensNumber.Equals("0"))
        {
            jmbgFinal = Constants.withoutJMBG;
        }
        else
        {
            jmbgFinal = JMBG;
        }
        /*
        certificatesAuthorizedUsers.Add(new PisMessServiceReference.CertificatesAuthorizedUser
        { 
            Name = Ime,
            LastName = Prezime,
            Jmbg = jmbgFinal,
            Email = AdresaEPoste1,
            PhoneNumber = Telefon,
            HwMedium = utility.getEnglishTextItemText(Medij),
            Expiry = utility.getEnglishTextItemText(Rok),
            RequestNumber = ""
        });
        */
        return envelope;
    }

    protected List<PisMessServiceReference.Parameter> getDocumentParametersList(Utility utility, string legalEntityName)
    {
        List<PisMessServiceReference.Parameter> documentParameters = new List<PisMessServiceReference.Parameter>();

        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "requestNumber",
                ParameterValue = Session["zahtev-izdavanje-pravno-lice-brojzahteva"].ToString()
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
                ParameterName = "street",
                ParameterValue = txtulica.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "streetNo",
                ParameterValue = txtbroj.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "postNo",
                ParameterValue = txtpostanskibroj.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "city",
                ParameterValue = txtmesto.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "businessRegistrationNumber",
                ParameterValue = txtmaticnibroj.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "sortCode",
                ParameterValue = txtsifradel.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "pib",
                ParameterValue = txtpib.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "mustPayPdv",
                ParameterValue = utility.getItemText(Convert.ToInt32(ddlobveznikpdv.SelectedValue))
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "legalEntityPhone",
                ParameterValue = txtkontakttel.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "legalEntityEmail",
                ParameterValue = txtadresaeposte.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "legalEntityRepresentativeFunction",
                ParameterValue = txtimezz.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "legalEntityRepresentativeName",
                ParameterValue = txtprezimezz.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "legalEntityRepresentativeLastName",
                ParameterValue = string.Empty
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "price",
                ParameterValue = txtcenasaporezom.Text
            });

        return documentParameters;
    }

    protected string CreateDocumentLegalEntityContract(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess, List<PisMessServiceReference.Parameter> documentParameters)
    {
        //templateDocumentType: LegalEntityContract
        //-----------------------------------------
        log.Info("documentParameters to pismess: " + documentParameters.ToArray());
        string responseMessage = string.Empty;
        List<string> response1 = new List<string>(ServiceCaller.CallServiceCreateDoc(PisMessServiceReference.TemplateDocumentTypeSerbianPost.LegalEntityContract, documentParameters.ToArray()));
        if (response1[0].Equals("0"))
        {
            //success
            responseMessage = response1[1]; //write file path to some text box
        }
        else
        {
            //error
            responseMessage = response1[0]; //write error description to some text box
            throw new Exception("Response error while creating LegalEntityContract, response from PissMess: " + responseMessage);
        }

        return responseMessage;
    }

    protected string CreateDocumentGovernmentContract(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess, List<PisMessServiceReference.Parameter> documentParameters)
    {
        //templateDocumentType: GovernmentContract
        //-----------------------------------------
        string responseMessage = string.Empty;
        List<string> response = new List<string>(ServiceCaller.CallServiceCreateDoc(PisMessServiceReference.TemplateDocumentTypeSerbianPost.GovernmentContract, documentParameters.ToArray()));
        if (response[0].Equals("0"))
        {
            //success
            responseMessage = response[1]; //write file path to some text box

        }
        else
        {
            //error
            responseMessage = response[0]; //write error description to some text box
            throw new Exception("Response error while creating GovernmentContract, response from PissMess: " + responseMessage);
        }
        return responseMessage;
    }

    protected string CreateDocumentLegalEntityContractAttachment(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess, List<PisMessServiceReference.Parameter> documentParameters, List<PisMessServiceReference.CertificatesAuthorizedUser> certificatesAuthorizedUsers)
    {
        //templateDocumentType: CreateDocumentLegalEntityContractAttachment
        //-----------------------------------------
        string responseMessage = string.Empty;
        List<string> response = new List<string>(ServiceCaller.CallServiceCreateDocLegalEntity(documentParameters, certificatesAuthorizedUsers));
        if (response[0].Equals("0"))
        {
            //success
            responseMessage = response[1]; //write file path to some text box
        }
        else
        {
            //error
            responseMessage = response[0]; //write error description to some text box
            throw new Exception("Response error while creating CreateDocumentLegalEntityContractAttachment, response from PissMess: " + responseMessage);
        }
        return responseMessage;
    }

    protected void ImportValuesInDatabase(Utility utility, string maticnibroj, string nazivpravnoglica, string pib, bool obveznikpdv, string sifradel, string ulica, string broj, string postanskibroj, string pak, string mesto, string telefon, string adresaeposte, bool inhouse)
    {
        try
        {
            utility.upisiPravnoLice(maticnibroj, nazivpravnoglica, pib, obveznikpdv, sifradel, ulica, broj, postanskibroj, pak, mesto, telefon, adresaeposte, inhouse);
        }
        catch (Exception ex)
        {
            throw new Exception("Eror while importing values in database. " + ex.Message);
        }
    }

    protected bool CheckIfFullNameExist(Utility utility, string nazivpravnoglica)
    {
        bool returnValue = false;
        try
        {
            List<FulNameLegalEntity> FulNameLegalEntityList = new List<FulNameLegalEntity>();
            FulNameLegalEntityList = utility.pronadjiNazivPravnogLica();

            foreach (var fullname in FulNameLegalEntityList)
            {
                if (fullname.FullName == nazivpravnoglica)
                {
                    returnValue = true;
                }
            }
            return returnValue;
        }
        catch (Exception ex)
        {
            throw new Exception("Eror while selecting values from database. " + ex.Message);
        }
    }

    protected void UpdateValuesInDatabase(Utility utility, string maticnibroj, string nazivpravnoglica, string pib, bool obveznikpdv, string sifradel, string ulica, string broj, string postanskibroj, string pak, string mesto, string telefon, string adresaeposte, bool inhouse)
    {
        try
        {
            utility.editujPravnoLice(maticnibroj, nazivpravnoglica, pib, obveznikpdv, sifradel, ulica, broj, postanskibroj, pak, mesto, telefon, adresaeposte, inhouse);
        }
        catch (Exception ex)
        {
            throw new Exception("Eror while editting values in database. " + ex.Message);
        }
    }

    protected void UpdateOrderNumberLastUsed(Utility utility, int legalEntityOrderNumber, int IDOrderNumber)
    {
        try
        {
            utility.editujPoslednjeKoriscenOrderNumber(legalEntityOrderNumber, IDOrderNumber);
        }
        catch (Exception ex)
        {
            throw new Exception("Eror while editting values in database. " + ex.Message);
        }
    }


    public int legalEntityOrderNumber = 0;

    protected void btnEnterRequest_Click1(object sender, EventArgs e)
    {
        try
        {            
            List<PisMessServiceReference.Parameter> documentParameters = new List<PisMessServiceReference.Parameter>();
            List<PisMessServiceReference.CertificatesAuthorizedUser> certificatesAuthorizedUsers = new List<PisMessServiceReference.CertificatesAuthorizedUser>();
            Utility utility1 = new Utility();

            if (txtnazivpravnoglica.Text == string.Empty)
            {
                if (txtwritename.Text == string.Empty)
                {
                    legalEntityName = ddlLegalEntityName.SelectedItem.Text;
                }
                else
                {
                    legalEntityName = txtwritename.Text;
                }
            }
            else
            {
                legalEntityName = txtnazivpravnoglica.Text;
            }

            try
            {
                log.Info("Start getting legal entity Order Number.");
                legalEntityOrderNumber = utility1.getOrderNumber(Constants.LEGAL_ENTITY_ID_ORDER_NUMBER);
                Session["zahtev-izdavanje-pravno-lice-brojzahteva"] = legalEntityOrderNumber;
                UpdateOrderNumberLastUsed(utility1, legalEntityOrderNumber, Constants.LEGAL_ENTITY_ID_ORDER_NUMBER);
                log.Info("Finished getting OrderNumber and editting LastUsed. legalEntityOrderNumber for Legal Entity is " + legalEntityOrderNumber);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting OrderNumber and editting LastUsed. " + ex.Message);
            }

            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                log.Info("Start sending SOAP message for i = " + i + " row.");

                Utility utility = new Utility();
                
                BxSoapEnvelope envelope = createSoapEnvelope(utility, i, certificatesAuthorizedUsers, legalEntityName);

                //envelope.createBxSoapEnvelope();   create SOAP.xml 
                string SOAPresponse = BxSoap.SOAPManual(envelope.createBxSoapEnvelope());
                Utils.ParseSoapEnvelope(SOAPresponse, out BrojZahteva, out BrojZahtevaPravnoLice);

                if (BrojZahteva == string.Empty)
                {
                    throw new Exception("RequestNumber is empty!");
                }

                addRequestNumberToCertificatesAuthorizedUsers(utility, i, certificatesAuthorizedUsers, BrojZahteva);

                log.Info("Finished sending SOAP message for i = " + i + " row. Response from BlueX: First RequestNumber for IssuingIndividual is: " + BrojZahteva + ". Second RequestNumber for IssuingIndividual is: " + BrojZahtevaPravnoLice);                                      
            }

            documentParameters = getDocumentParametersList(utility1, legalEntityName);

            pisMess = new PisMessServiceReference.PisMessServiceClient();
            
            log.Info("Start creating PDF Files.");

            var CreateDocumentLegalEntityContractTask = Task.Run(() => CreateDocumentLegalEntityContract(utility1, pisMess, documentParameters));
            var CreateDocumentGovernmentContractTask = Task.Run(() => CreateDocumentGovernmentContract(utility1, pisMess, documentParameters));
            var CreateDocumentLegalEntityContractAttachmentTask = Task.Run(() => CreateDocumentLegalEntityContractAttachment(utility1, pisMess, documentParameters, certificatesAuthorizedUsers));

            Task.WaitAll(new[] { CreateDocumentLegalEntityContractTask, CreateDocumentGovernmentContractTask, CreateDocumentLegalEntityContractAttachmentTask });

            //string fileName1 = CreateDocumentLegalEntityContract(utility1, pisMess, documentParameters);
            Session["zahtev-izdavanje-pravno-lice-filename1"] = CreateDocumentLegalEntityContractTask.Result;

            //string filename2 = CreateDocumentGovernmentContract(utility1, pisMess, documentParameters);
            Session["zahtev-izdavanje-pravno-lice-filename2"] = CreateDocumentGovernmentContractTask.Result;

            //string filename = CreateDocumentLegalEntityContractAttachment(utility1, pisMess, documentParameters, certificatesAuthorizedUsers);
            Session["zahtev-izdavanje-pravno-lice-filename"] = CreateDocumentLegalEntityContractAttachmentTask.Result;

            log.Info("Finished creating PDF files!");

            log.Info("Start importing Legal Entity values in database.");

            if (documentParameters.Count > 0)
            {
                bool pdvpayer = true;
                string ispdvpayer = utility1.getItemValueAddedTax(Convert.ToInt32(ddlobveznikpdv.SelectedValue)).ToString();
                if (ispdvpayer == Constants.isLegalEntityFalse)
                {
                    pdvpayer = false;
                }
                else
                {
                    pdvpayer = true;
                }

                if (CheckIfFullNameExist(utility1, legalEntityName))
                {
                    bool inHousePTT = false;
                    if (legalEntityName == Constants.PTT_CEPP)
                    {
                        inHousePTT = true;
                    }
                    UpdateValuesInDatabase(utility1, txtmaticnibroj.Text, legalEntityName, txtpib.Text, pdvpayer, txtsifradel.Text, txtulica.Text, txtbroj.Text, txtpostanskibroj.Text, txtpak.Text, txtmesto.Text, txtkontakttel.Text, txtadresaeposte.Text, inHousePTT);
                }
                else
                {
                    ImportValuesInDatabase(utility1, txtmaticnibroj.Text, legalEntityName, txtpib.Text, pdvpayer, txtsifradel.Text, txtulica.Text, txtbroj.Text, txtpostanskibroj.Text, txtpak.Text, txtmesto.Text, txtkontakttel.Text, txtadresaeposte.Text, false);
                }
            }
            else
            {
                throw new Exception("Eror while importing values in database, documentParameters.Count is empty. ");
            }

            log.Info("Finished importing Legal Entity values in database.");

            Response.Redirect("zahtev-izdavanje-pravno-lice-podnet.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
        }
        catch (AggregateException aex)
        {
            log.Error(aex.InnerException.Message, aex);
            throw aex;
        }
        catch (Exception ex)
        {
            log.Error("Error while sending request. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);
        }       
    }

    protected void addRequestNumberToCertificatesAuthorizedUsers(Utility utility, int i, List<PisMessServiceReference.CertificatesAuthorizedUser> certificatesAuthorizedUsers, string requestNumber)
    {
        string Ime = string.Empty;
        string Prezime = string.Empty;
        string JMBG = string.Empty;
        string AdresaEPoste = string.Empty;
        string AdresaEPoste1 = string.Empty;
        string Telefon = string.Empty;
        string Medij = string.Empty;
        string Rok = string.Empty;
        string SertJMBG = string.Empty;

        Ime = GridView1.Rows[i].Cells[1].Text;
        Prezime = GridView1.Rows[i].Cells[2].Text;
        JMBG = GridView1.Rows[i].Cells[3].Text;
        AdresaEPoste = GridView1.Rows[i].Cells[4].Text;
        if (AdresaEPoste == "&nbsp;" || AdresaEPoste == string.Empty)
        {
            AdresaEPoste1 = string.Empty;
        }
        else
        {
            AdresaEPoste1 = AdresaEPoste;
        }
        Telefon = GridView1.Rows[i].Cells[5].Text;
        Medij = GridView1.Rows[i].Cells[6].Text;
        Rok = GridView1.Rows[i].Cells[7].Text;

        SertJMBG = GridView1.DataKeys[i]["SertJmbg"].ToString();

        string jmbgFinal = string.Empty;
        string includeUniqueCitizensNumber = (utility.getEnglishTextInputString(SertJMBG)).ToString();
        if (includeUniqueCitizensNumber.Equals("0"))
        {
            jmbgFinal = Constants.withoutJMBG;
        }
        else
        {
            jmbgFinal = JMBG;
        }

        certificatesAuthorizedUsers.Add(new PisMessServiceReference.CertificatesAuthorizedUser
        {
            Name = Ime,
            LastName = Prezime,
            Jmbg = jmbgFinal,
            Email = AdresaEPoste1,
            PhoneNumber = Telefon,
            HwMedium = utility.getEnglishTextItemText(Medij),
            Expiry = utility.getEnglishTextItemText(Rok),
            RequestNumber = requestNumber
        });
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
        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnRegistrationNumberValidation"]))
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
                rowLegalEntityDDL1.Visible = true;
                rowLegalEntityDDL2.Visible = true;

                rbChooseNameCheckedChanged();
                
                //ddlLegalEntityName.Items.Insert(0, utility.getItemText(Constants.DefaultIdItemLegal));
                ddlLegalEntityName.DataSource = entityFullNames;
                ddlLegalEntityName.DataBind();

                Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] = ddlLegalEntityName;
                SetFocusOnDropDownLists();
            }
            else
            {
                rowLegalEntityName.Visible = true;
                rowLegalEntityDDL.Visible = false;
                rowLegalEntityDDL1.Visible = false;
                rowLegalEntityDDL2.Visible = false;
                foreach (var entity in LegalEntities)
                {
                    txtnazivpravnoglica.Text = entity.FullName;
                    txtpib.Text = entity.PIB;
                    IsPDVpayer(entity.PDVpayer);
                    txtsifradel.Text = entity.BysinessTypeCode;
                    txtmesto.Text = entity.City;
                    txtulica.Text = entity.Street;
                    txtbroj.Text = entity.HouseNumber;
                    txtpostanskibroj.Text = entity.ZipCode;
                    txtpak.Text = entity.PAK;
                    txtkontakttel.Text = entity.PhoneNumber;
                    txtadresaeposte.Text = entity.Email;
                }
                if (errLabelIN.Text != string.Empty)
                {
                    Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtmaticnibroj;
                    SetFocusOnTextbox();
                }
                else
                {
                    Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtwebadresa;
                    SetFocusOnTextbox();
                }
            }
        }
        else
        {
            newIN = txtmaticnibroj.Text;
            errorMessage = string.Empty;
        }
    }

    protected void ClearTextboxesAndDropDownLists()
    {
        txtnazivpravnoglica.Text = string.Empty;
        txtpib.Text = string.Empty;

        string IDItem1 = string.Empty;
        SetUpDefaultItem(ddlobveznikpdv.ClientID, out IDItem1);
        ddlobveznikpdv.SelectedValue = IDItem1;

        txtsifradel.Text = string.Empty;
        txtmesto.Text = string.Empty;
        txtulica.Text = string.Empty;
        txtbroj.Text = string.Empty;
        txtpostanskibroj.Text = string.Empty;
        txtpak.Text = string.Empty;
        txtkontakttel.Text = string.Empty;
        txtadresaeposte.Text = string.Empty;
        txtwebadresa.Text = string.Empty;
        txtimezz.Text = string.Empty;
        txtprezimezz.Text = string.Empty;
        ScriptManager.RegisterStartupScript(this, GetType(), "DisabledButton", "DisabledButton();", true);
    }

    protected void cvmaticnibroj_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newIN = txtmaticnibroj.Text;
        string errMessage = string.Empty;
        List<LegalEntityVariable> LegalEntities = new List<LegalEntityVariable>();
        if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnRegistrationNumberValidation"]))
        {
            args.IsValid = getIdentificationNumber(newIN, out errMessage, out LegalEntities);
            cvmaticnibroj.ErrorMessage = errMessage;
            errLabelIN.Text = string.Empty;
        }
        else
        {
            args.IsValid = UtilsValidation.ValidateIdentificationNumber(newIN, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtmaticnibrojIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnRegistrationNumberValidation"]), out errMessage);
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
                ErrorMessage = "Matični broj mora sadržati " + Constants.LEGAL_ENTITY_MATICNI_BROJ + " cifara.";
                returnValue = false;
            }
            else if (newIN.Length == Constants.LEGAL_ENTITY_MATICNI_BROJ)
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
            returnValue = UtilsValidation.ValidateIdentificationNumber(newIN, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtmaticnibrojIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnRegistrationNumberValidation"]), out ErrorMessage);
            if (returnValue)
            {
                Utility utility = new Utility();
                LegalEntities = utility.pronadjiPromenljiveLegalEntity(newIN);
                if (LegalEntities.Count == 0)
                {
                    //ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3380);
                    returnValue = true;
                    ErrorMessage = string.Empty;
                    Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtnazivpravnoglica;
                    SetFocusOnTextbox();
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
                string IDItem1 = string.Empty;
                SetUpDefaultItem(ddlobveznikpdv.ClientID, out IDItem1);
                ddlobveznikpdv.SelectedValue = IDItem1;
                txtsifradel.Text = string.Empty;
                txtmesto.Text = string.Empty;
                txtulica.Text = string.Empty;
                txtbroj.Text = string.Empty;
                txtpostanskibroj.Text = string.Empty;
                txtpak.Text = string.Empty;
                txtkontakttel.Text = string.Empty;
                txtadresaeposte.Text = string.Empty;
                Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtwebadresa;
                SetFocusOnTextbox();
            }
            else if (entity.FullName == SelectedValue)
            {
                txtnazivpravnoglica.Text = entity.FullName;
                txtpib.Text = entity.PIB;
                IsPDVpayer(entity.PDVpayer);
                txtsifradel.Text = entity.BysinessTypeCode;
                txtmesto.Text = entity.City;
                txtulica.Text = entity.Street;
                txtbroj.Text = entity.HouseNumber;
                txtpostanskibroj.Text = entity.ZipCode;
                txtpak.Text = entity.PAK;
                txtkontakttel.Text = entity.PhoneNumber;
                txtadresaeposte.Text = entity.Email;
                Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtwebadresa;
                SetFocusOnTextbox();
            }
        }
    }

    protected void IsPDVpayer(bool entityPDVpayer)
    {
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string ddlstring = ddlobveznikpdv.ClientID;
        Utility utility = new Utility();

        if (entityPDVpayer)
        {
            int ItemValue = 1;
            int selectedvaluetrue = utility.getIDItem(ItemValue, page, ddlstring);
            ddlobveznikpdv.SelectedValue = selectedvaluetrue.ToString();
        }
        else
        {
            int ItemValue = 0;
            int selectedvaluefalse = utility.getIDItem(ItemValue, page, ddlstring);
            ddlobveznikpdv.SelectedValue = selectedvaluefalse.ToString();
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

    protected void rbWriteName_CheckedChanged(object sender, EventArgs e)
    {
        rbWriteNameCheckedChanged();
    }

    protected void cvwritename_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string newLegalNameformat = string.Empty;
            string errMessage = string.Empty;
            string newLegalName = txtwritename.Text;
            args.IsValid = UtilsValidation.ValidateNazivPravnogLica(newLegalName, Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-txtwritenameIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-TurnOnWriteNameValidation"]), out errMessage, out newLegalNameformat);
            cvwritename.ErrorMessage = errMessage;
        }
        catch (Exception)
        {
            cvwritename.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void rbChooseNameCheckedChanged()
    {
        Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"] = true;

        rbChooseName.Checked = true;
        rbWriteName.Checked = false;
        Utility utility = new Utility();
        ddlLegalEntityName.SelectedIndex = ddlLegalEntityName.Items.IndexOf(ddlLegalEntityName.Items.FindByText(utility.getItemText(Constants.DefaultIdItemLegal)));
        ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"]);

        ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlLegalEntityName.CssClass = SetCss5;
        txtwritename.ReadOnly = true;
        txtwritename.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtwritename.CssClass = SetCss5;
        cvLegalEntityName.Enabled = true;
        cvwritename.Enabled = false;
    }

    protected void rbWriteNameCheckedChanged()
    {
        Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"] = false;

        rbWriteName.Checked = true;
        rbChooseName.Checked = false;
        Utility utility = new Utility();
        ddlLegalEntityName.SelectedIndex = ddlLegalEntityName.Items.IndexOf(ddlLegalEntityName.Items.FindByText(utility.getItemText(Constants.DefaultIdItemLegal)));
        ddlLegalEntityName.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-ddlLegalEntityName"]);

        ddlLegalEntityName.BackColor = ColorTranslator.FromHtml(SetLightGray);
        ddlLegalEntityName.CssClass = SetCss5;
        txtwritename.ReadOnly = false;
        txtwritename.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtwritename.CssClass = SetCss5;
        cvLegalEntityName.Enabled = false;
        cvwritename.Enabled = true;

        ClearTextboxesAndDropDownLists();
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        if (ViewState["pravno-lice-CurrentTable"] != null)
        {
            DataTable dtCurrentTable = (DataTable)ViewState["pravno-lice-CurrentTable"];
            ViewState["pravno-lice-CurrentTable"] = dtCurrentTable;

            GridView1.DataSourceID = null;
            GridView1.DataSource = dtCurrentTable;
            GridView1.DataBind();
        }
    }

    protected void txtbroj_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged9();
        if (errLabelBroj.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtpostanskibroj;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged9()
    {
        Utility utility = new Utility();
        string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_VALIDATION);

        if (SettingValue == Constants.SETTING_VALUE_TRUE)
        {
            if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-SetUpWSPWrapperService"]))
            { 
                string PorukaKorisnik = string.Empty;
                string PostanskiBroj = string.Empty;
                string Pak = string.Empty;
                bool result = UtilsValidation.GetPostNumberAndPAK(txtmesto.Text, txtulica.Text, txtbroj.Text, out PorukaKorisnik, out PostanskiBroj, out Pak);
                txtpostanskibroj.Text = PostanskiBroj;
                txtpak.Text = Pak;
                errLabelBroj.Text = PorukaKorisnik;
            }
        }
    }

    protected void txtbroj1_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged10();
        if (errLabelBroj1.Text != string.Empty)
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-pravno-lice-event_controle"] = txtpostanskibroj1;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged10()
    {
        Utility utility = new Utility();
        string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_VALIDATION);

        if (SettingValue == Constants.SETTING_VALUE_TRUE)
        {
            if (Convert.ToBoolean(Session["zahtev-izdavanje-pravno-lice-SetUpWSPWrapperService"]))
            {
                string PorukaKorisnik = string.Empty;
                string PostanskiBroj = string.Empty;
                string Pak = string.Empty;
                bool result = UtilsValidation.GetPostNumberAndPAK(txtmesto1.Text, txtulica1.Text, txtbroj1.Text, out PorukaKorisnik, out PostanskiBroj, out Pak);
                txtpostanskibroj1.Text = PostanskiBroj;
                txtpak1.Text = Pak;
                errLabelBroj1.Text = PorukaKorisnik;
                if (PorukaKorisnik == string.Empty)
                {
                    cvbroj1.ErrorMessage = string.Empty;
                }
            }
        }
    }

    public void SetFocusOnTextbox()
    {
        try
        {
            if (Session["zahtev-izdavanje-pravno-lice-event_controle"] != null)
            {
                TextBox controle = (TextBox)Session["zahtev-izdavanje-pravno-lice-event_controle"];
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
            if (Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"] != null)
            {
                DropDownList padajucalista = (DropDownList)Session["zahtev-izdavanje-pravno-lice-event_controle-DropDownList"];
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
