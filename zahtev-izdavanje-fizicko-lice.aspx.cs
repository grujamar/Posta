using BlueXSOAP;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public partial class zahtev_izdavanje_fizicko_lice : System.Web.UI.Page
{
    public string SetDarkGray = Constants.SetDarkGray;
    public string SetLightGray = Constants.SetLightGray;
    public string SetWhite = Constants.SetWhite;
    public string SetCss1 = Constants.SetCss1;
    public string SetCss2 = Constants.SetCss2;
    public string SetCss4 = Constants.SetCss4;
    public string includeUniqueJMBG;
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
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"] = true;
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg1"] = true;
                Session["zahtev-izdavanje-fizicko-lice-ddlvrstadokumenta"] = true;
                Session["zahtev-izdavanje-fizicko-lice-ddlsertadresa"] = true;
                Session["zahtev-izdavanje-fizicko-lice-ddlrok"] = true;
                Session["zahtev-izdavanje-fizicko-lice-ddlmedij"] = true;
                Session["zahtev-izdavanje-fizicko-lice-ddlnacinplacanja"] = true;

                string IDItem1 = string.Empty;
                string IDItem2 = string.Empty;
                string IDItem3 = string.Empty;
                string IDItem4 = string.Empty;
                string IDItem5 = string.Empty;

                SetUpDefaultItem(ddlvrstadokumenta.ClientID, out IDItem1);
                ddlvrstadokumenta.SelectedValue = IDItem1;
                SetUpDefaultItem(ddlsertadresa.ClientID, out IDItem2);
                ddlsertadresa.SelectedValue = IDItem2;
                SetUpDefaultItem(ddlrokkoriscenjasert.ClientID, out IDItem3);
                ddlrokkoriscenjasert.SelectedValue = IDItem3;
                SetUpDefaultItem(ddlmedijsert.ClientID, out IDItem4);
                ddlmedijsert.SelectedValue = IDItem4;
                SetUpDefaultItem(ddlnacinplacanja.ClientID, out IDItem5);
                ddlnacinplacanja.SelectedValue = IDItem5;

                txtcenasaporezom.Text = string.Empty;
                myDiv1.Visible = false;
                myDiv2.Visible = false;
                myDiv3.Visible = false;
                myDiv4.Visible = false;
                myDiv6.Visible = false;
                myDiv8.Visible = false;
                txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
                txtcenasaporezom.ReadOnly = true;
                txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetLightGray);
                txtdatumrodjenja.ReadOnly = true;
                //txtdatumizdavanja.ReadOnly = true;
                //txtdatumisteka.ReadOnly = true;
                //-------TABINDEX---------------
                Session["zahtev-izdavanje-fizicko-lice-event_controle"] = txtime;
                txtmesto.TabIndex = -1;
                txtulica.TabIndex = -1;
                txtbroj.TabIndex = -1;
                txtpostanskibroj.TabIndex = -1;
                txtpak.TabIndex = -1;
                SetFocusOnTextbox();
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
        //-------------------------
        //-------Geolocation-------
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
                    log.Debug("Error while setting control's " + control.Controlid + " text: " + ex.Message);
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
                    log.Debug("Error while setting control's " + control.Controlid + " text: " + ex.Message);
                }
            }            
        }

        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"]) == false)
        {
            ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"]);
            ddlsertjmbg.CssClass = SetCss1;            
        }
        else
        {
            ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg1"]);
            ddlsertjmbg.CssClass = SetCss1;
        }
               
        ddlvrstadokumenta.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlvrstadokumenta"]);
        ddlvrstadokumenta.CssClass = SetCss2;
        ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertadresa"]);
        ddlsertadresa.CssClass = SetCss4;
        ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlrok"]);
        ddlrokkoriscenjasert.CssClass = SetCss1;
        ddlmedijsert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlmedij"]);
        ddlmedijsert.CssClass = SetCss1;
        ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlnacinplacanja"]);
        ddlnacinplacanja.CssClass = SetCss1;
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
            Utility utility1 = new Utility();
            string page = Path.GetFileName(Page.AppRelativeVirtualPath);
            Controls = new List<WebControl>();
            Controls = utility1.pronadjiKontrole(page);

            foreach (var control in Controls)
            {
                if (control.Id == txtmesto.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnCityValidation"] = control.ControlStatus;
                    TurnOnAjaxValidation = control.ControlStatus;
                    ValidateAjax(TurnOnAjaxValidation);
                }
                else if (control.Id == txtjmbg.ClientID)
                {
                    TurnOnJMBGValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnJMBGValidation"] = TurnOnJMBGValidation;
                }
                else if (control.Id == txtadresaeposte.ClientID)
                {
                    TurnOnEmailValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnEmailValidation"] = TurnOnEmailValidation;
                }
                else if (control.Id == txttelefon.ClientID)
                {
                    TurnOnPhoneValidation = control.ControlStatus;
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnPhoneValidation"] = TurnOnPhoneValidation;
                }
                else if (control.Id == txtime.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtprezime.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnSurnameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtbrojiddokumenta.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnIDDocumentNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtimeinstitucije.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnInstitutionNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtulica.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnStreetValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtbroj.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnHouseNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpostanskibroj.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnPostNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpak.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnPAKValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtdatumizdavanja.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnIssueDateValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtdatumisteka.ClientID)
                {
                    Session["zahtev-izdavanje-fizicko-lice-TurnOnExpiryDateValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            TurnOnAjaxValidation = Constants.VALIDATION_FALSE;
            ValidateAjax(TurnOnAjaxValidation);
            TurnOnJMBGValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnJMBGValidation"] = TurnOnJMBGValidation;
            TurnOnEmailValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnEmailValidation"] = TurnOnEmailValidation;
            TurnOnPhoneValidation = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnPhoneValidation"] = TurnOnPhoneValidation;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnCityValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnSurnameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnIDDocumentNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnInstitutionNameValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnStreetValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnHouseNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnPostNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnPAKValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnIssueDateValidation"] = Constants.VALIDATION_FALSE;
            Session["zahtev-izdavanje-fizicko-lice-TurnOnExpiryDateValidation"] = Constants.VALIDATION_FALSE;
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
            if (control.Id == txtime.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezime.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtprezimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtjmbg.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtjmbgIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbrojiddokumenta.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtbrojiddokumentaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtimeinstitucije.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtimeinstitucijeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumizdavanja.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtdatumizdavanjaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtdatumisteka.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtdatumistekaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtadresaeposte.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtadresaeposteIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txttelefon.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txttelefonIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtmesto.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtmestoIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtulica.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtulicaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbroj.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtbrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpostanskibroj.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtpostanskibrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpak.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-txtpakIsRequired"] = control.IsRequired;
            }
        }
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
            if (control.Id == ddlsertjmbg.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbgIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlvrstadokumenta.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-ddlvrstadokumentaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlsertadresa.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-ddlsertadresaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlrokkoriscenjasert.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-ddlrokkoriscenjasertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlmedijsert.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-ddlmedijsertIsRequired"] = control.IsRequired;
            }
            else if (control.Id == ddlnacinplacanja.ClientID)
            {
                Session["zahtev-izdavanje-fizicko-lice-ddlnacinplacanjaIsRequired"] = control.IsRequired;
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
            errLabelBroj.Text = string.Empty;
        }
        else if (!InHouseVariable && IsAllowedVariable)
        {
            ValidateAjax(Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnCityValidation"]));
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
            errLabelBroj.Text = string.Empty;
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
            errLabelBroj.Text = string.Empty;
            txtmesto.TabIndex = -1;
            txtulica.TabIndex = -1;
            txtbroj.TabIndex = -1;
            txtpostanskibroj.TabIndex = -1;
            txtpak.TabIndex = -1;
        }
        Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] = ((DropDownList)sender);
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
                Session["zahtev-izdavanje-fizicko-lice-CityVariable"] = CityVariable;
                Session["zahtev-izdavanje-fizicko-lice-StreetVariable"] = StreetVariable;
                Session["zahtev-izdavanje-fizicko-lice-HouseNumberVariable"] = HouseNumberVariable;
                Session["zahtev-izdavanje-fizicko-lice-ZipCodeVariable"] = ZipCodeVariable;
                Session["zahtev-izdavanje-fizicko-lice-PAKVariable"] = PAKVariable;
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


    protected void ddlvrstadokumenta_SelectedIndexChanged(object sender, EventArgs e)
    {        
        int SelectedValue = Convert.ToInt32(ddlvrstadokumenta.SelectedValue);
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string ddlstring = ddlvrstadokumenta.ClientID;
        //Get PTT Variables from Database
        SetUpPTTVariables(SelectedValue, page, ddlstring, out CityVariable, out StreetVariable, out HouseNumberVariable, out ZipCodeVariable, out PAKVariable, out InHouseVariable, out IsAllowedVariable);

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
        Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ShowDatepicker()
    {
        //call function pickdate() every time after PostBack in ASP.Net
        ScriptManager.RegisterStartupScript(this, GetType(), "", "pickdate();", true);
        //Avoid: jQuery DatePicker TextBox selected value Lost after PostBack in ASP.Net
        txtdatumisteka.Text = Request.Form[txtdatumisteka.UniqueID];
        txtdatumizdavanja.Text = Request.Form[txtdatumizdavanja.UniqueID];
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
        Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ddlmedijsert_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetPrice();
        Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ddlnacinplacanja_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlnacinplacanja.SelectedValue);
        Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    protected void ddlsertjmbg_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlsertjmbg.SelectedValue);
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        string ddlsertjmbgString = ddlsertjmbg.ClientID;
        Utility utility = new Utility();
        int includeUniqueJMBGfinish = utility.getItemValue(SelectedValue, page, ddlsertjmbgString);

        includeUniqueJMBG = includeUniqueJMBGfinish.ToString();

        Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
    }

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA MATIČNOG BROJA-------------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txtjmbg_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged();
        if (errLabel.Text != string.Empty)
        {
            Session["zahtev-izdavanje-fizicko-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] = ddlsertjmbg;
            SetFocusOnDropDownLists();
        }
    }

    private void CheckIfChannelHasChanged()
    {
        bool RRforeigner = false;
        string newJMBG = txtjmbg.Text;
        string errorMessage = string.Empty;
        string jmbgformat = string.Empty;
        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnJMBGValidation"]))
        {
            UtilsValidation.validateJMBG(newJMBG, Constants.ID_ITEM_DDLSERTJMBG, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtjmbgIsRequired"]), out errorMessage, out jmbgformat, out RRforeigner);
            errLabel.Text = errorMessage;
            txtdatumrodjenja.Text = jmbgformat;

            if (RRforeigner)
            {
                ddlsertjmbg.SelectedValue = GetItemID(Constants.YES, Constants.ITEM_YES_NO).ToString();
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"] = false;
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg1"] = true;
            }
            else
            {
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"] = true;
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg1"] = true;
            }
        }
        else
        {
            newJMBG = txtjmbg.Text;
            errorMessage = string.Empty;
        }
        
    }

    protected void cvjmbg_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool RRforeigner = false;
        string newJMBG = txtjmbg.Text;
        string errMessage = string.Empty;
        string jmbgformat = string.Empty;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnJMBGValidation"]))
        {
            args.IsValid = UtilsValidation.validateJMBG(newJMBG, Constants.ID_ITEM_DDLSERTJMBG, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtjmbgIsRequired"]), out errMessage, out jmbgformat, out RRforeigner);
            cvjmbg.ErrorMessage = errMessage;
            txtdatumrodjenja.Text = jmbgformat;

            if (RRforeigner)
            {
                ddlsertjmbg.SelectedValue = GetItemID(Constants.YES, Constants.ITEM_YES_NO).ToString();
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"] = false;
            }
            else
            {
                Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"] = true;
            }
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationJMBG(newJMBG, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtjmbgIsRequired"]), out errMessage, out jmbgformat);
            cvjmbg.ErrorMessage = errMessage;
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

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Telefonskog BROJA----------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txttelefon_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged1();
        if (errLabelNumber.Text != string.Empty)
        {
            Session["zahtev-izdavanje-fizicko-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] = ddlsertadresa;
            SetFocusOnDropDownLists();
        }
    }

    private void CheckIfChannelHasChanged1()
    {
        string newNumber = txttelefon.Text;
        string errorMessage = string.Empty;
        string numberformat = string.Empty;
        bool LegalEntityPhone = false;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnPhoneValidation"]))
        {
            UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txttelefonIsRequired"]), LegalEntityPhone, out errorMessage, out numberformat);
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
        bool LegalEntityPhone = false;

        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnPhoneValidation"]))
        {
            args.IsValid = UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
            txttelefon.Text = numberformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationNumber(newNumber, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
        }
    }

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Adrese E Pošte-------------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txtadresaeposte_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged2();
        if (errLabelMail.Text != string.Empty)
        {
            Session["zahtev-izdavanje-fizicko-lice-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["zahtev-izdavanje-fizicko-lice-event_controle"] = txttelefon;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged2()
    {
        string newMail = txtadresaeposte.Text;
        string errorMessage = string.Empty;
        string mailformat = string.Empty;
        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnEmailValidation"]))
        {
            UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtadresaeposteIsRequired"]) , out errorMessage, out mailformat);
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
            args.IsValid = UtilsValidation.ValidateName(txtime.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtimeIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnNameValidation"]), out ErrorMessage1, out nameformat);
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
            args.IsValid = UtilsValidation.ValidateSurname(txtprezime.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtprezimeIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnSurnameValidation"]), out ErrorMessage1, out surnameformat);
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
            args.IsValid = UtilsValidation.ValidateIDDocument(txtbrojiddokumenta.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtbrojiddokumentaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnIDDocumentNumberValidation"]), out ErrorMessage1, out documentidformat);
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
            args.IsValid = UtilsValidation.ValidateInstitutionName(txtimeinstitucije.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtimeinstitucijeIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnInstitutionNameValidation"]), out ErrorMessage1, out InstitutionNameformat);
            cvimeinstitucije.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvimeinstitucije.ErrorMessage = string.Empty;
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

            args.IsValid = UtilsValidation.ValidateSertJMBG(ddlsertjmbg.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbgIsRequired"]), IDItem, out ErrorMessage1);
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

            args.IsValid = UtilsValidation.ValidateSertAdresa(ddlvrstadokumenta.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlvrstadokumentaIsRequired"]), IDItem, out ErrorMessage1);
            cvvrstadokumenta.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvvrstadokumenta.ErrorMessage = string.Empty;
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

            args.IsValid = UtilsValidation.ValidateSertAdresa(ddlsertadresa.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertadresaIsRequired"]), IDItem, out ErrorMessage1);
            cvsertadresa.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvsertadresa.ErrorMessage = string.Empty;
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
                log.Debug("datumizdavanja je: " + datumizdavanja);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateIssuingDate(datumizdavanja, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtdatumizdavanjaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnIssueDateValidation"]), out ErrorMessage1);
                cvdatumizdavanja.ErrorMessage = ErrorMessage1;
            }
            else
            {
                if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtdatumizdavanjaIsRequired"]))
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
            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtdatumizdavanjaIsRequired"]))
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
                log.Debug("datumisteka je: " + datumisteka);
                string ErrorMessage1 = string.Empty;

                args.IsValid = UtilsValidation.ValidateExpiryDate(datumisteka, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtdatumistekaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnExpiryDateValidation"]), out ErrorMessage1);
                cvdatumisteka.ErrorMessage = ErrorMessage1;
            }
            else
            {
                if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtdatumistekaIsRequired"]))
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
            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtdatumistekaIsRequired"]))
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

    protected void cvadresaeposte_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string mailformat = string.Empty;

            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnEmailValidation"]))
            {
                args.IsValid = UtilsValidation.ValidateMail(txtadresaeposte.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtadresaeposteIsRequired"]), out ErrorMessage1, out mailformat);
                cvadresaeposte.ErrorMessage = ErrorMessage1;
            }
            else
            {
                args.IsValid = UtilsValidation.WithoutValidationMail(txtadresaeposte.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtadresaeposteIsRequired"]), out ErrorMessage1, out mailformat);
                cvadresaeposte.ErrorMessage = ErrorMessage1;
            }
        }
        catch (Exception)
        {
            cvadresaeposte.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvmesto_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string cityformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateCity(txtmesto.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtmestoIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnCityValidation"]), out ErrorMessage1, out cityformat);
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
            args.IsValid = UtilsValidation.ValidateStreet(txtulica.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtulicaIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnStreetValidation"]), out ErrorMessage1, out streetformat);
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
            args.IsValid = UtilsValidation.ValidateHouseNumber(txtbroj.Text, errLabelBroj.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtbrojIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnHouseNumberValidation"]), out ErrorMessage1, out housenumberformat);
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
            args.IsValid = UtilsValidation.ValidatePostNumber(txtpostanskibroj.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtpostanskibrojIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnPostNumberValidation"]), out ErrorMessage1, out postnumberformat);
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
            args.IsValid = UtilsValidation.ValidatePAK(txtpak.Text, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-txtpakIsRequired"]), Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnPAKValidation"]), out ErrorMessage1, out pakformat);
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

            args.IsValid = UtilsValidation.ValidateRok(ddlrokkoriscenjasert.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlrokkoriscenjasertIsRequired"]), IDItem, out ErrorMessage1);
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

            args.IsValid = UtilsValidation.ValidateMedij(ddlmedijsert.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlmedijsertIsRequired"]), IDItem, out ErrorMessage1);
            cvmedijsert.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvmedijsert.ErrorMessage = string.Empty;
            args.IsValid = false;
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

            args.IsValid = UtilsValidation.ValidateNacinPlacanja(ddlnacinplacanja.SelectedValue, Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlnacinplacanjaIsRequired"]), IDItem, out ErrorMessage1);
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
                    Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg1"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-ddlvrstadokumenta"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-ddlsertadresa"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-ddlrok"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-ddlmedij"] = false;
                    Session["zahtev-izdavanje-fizicko-lice-ddlnacinplacanja"] = false;

                    ValidateAjax(false);

                    txtime.ReadOnly = true;
                    txtime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtprezime.ReadOnly = true;
                    txtprezime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtjmbg.ReadOnly = true;
                    txtjmbg.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtdatumrodjenja.ReadOnly = true;
                    txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg1"]);
                    ddlsertjmbg.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlsertjmbg.CssClass = SetCss1;
                    ddlvrstadokumenta.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlvrstadokumenta"]);
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
                    txtadresaeposte.ReadOnly = true;
                    txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txttelefon.ReadOnly = true;
                    txttelefon.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertadresa"]);
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
                    ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlrok"]);
                    ddlrokkoriscenjasert.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlrokkoriscenjasert.CssClass = SetCss1;
                    ddlmedijsert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlmedij"]);
                    ddlmedijsert.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlmedijsert.CssClass = SetCss1;
                    ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlnacinplacanja"]);
                    ddlnacinplacanja.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtcenasaporezom.ReadOnly = true;
                    ddlnacinplacanja.CssClass = SetCss1;
                    txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    myDiv6.Visible = true;
                    myDiv5.Visible = false;
                    myDiv8.Visible = true;
                    //Disable datepicker
                    ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);
                    if (txtdatumisteka.Text != null)
                    {
                        Session["zahtev-izdavanje-fizicko-lice-datumisteka"] = txtdatumisteka.Text;
                    }
                    else
                    {
                        Session["zahtev-izdavanje-fizicko-lice-datumisteka"] = string.Empty;
                    }
                    if (txtdatumizdavanja.Text != null)
                    {
                        Session["zahtev-izdavanje-fizicko-lice-datumizdavanja"] = txtdatumizdavanja.Text;
                    }
                    else
                    {
                        Session["zahtev-izdavanje-fizicko-lice-datumizdavanja"] = string.Empty;
                    }
                    txtdatumzahteva.Text = DateTime.Now.ToString("dd.MM.yyyy");
                    Session["zahtev-izdavanje-fizicko-lice-datumzahteva"] = txtdatumzahteva.Text;
                    Session["zahtev-izdavanje-fizicko-lice-cenasaporezom"] = txtcenasaporezom.Text;

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
                    Session["zahtev-izdavanje-fizicko-lice-userAgentBrowser"] = userAgentBrowser;
                    Session["zahtev-izdavanje-fizicko-lice-userAgentStringApplicant"] = userAgentStringApplicant;
                    Session["zahtev-izdavanje-fizicko-lice-userAgentOS"] = userAgentOS;
                    Session["zahtev-izdavanje-fizicko-lice-userAgentIP"] = userAgentIP;
                    Session["zahtev-izdavanje-fizicko-lice-userAgentContinent"] = userAgentContinent;
                    Session["zahtev-izdavanje-fizicko-lice-userAgentCountry"] = userAgentCountry;
                    Session["zahtev-izdavanje-fizicko-lice-userAgentCountryCode"] = userAgentCountryCode;
                    Session["zahtev-izdavanje-fizicko-lice-userAgentCity"] = userAgentCity;
                    Session["zahtev-izdavanje-fizicko-lice-userAgentISP"] = userAgentISP;                    
                }
            }
            else if (!Page.IsValid)
            {
                errLabel.Text = string.Empty;
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

    //public string BrojZahteva = Utils.Generate15UniqueDigits();
    public string BrojZahteva = string.Empty;
    public string BrojZahtevaPravnoLice = string.Empty;
    public bool isResponseZero = true;

    protected BxSoapEnvelope createSoapEnvelope(Utility utility)
    {
        BxSoapEnvelope envelope = new BxSoapEnvelopeRequestToken();

        envelope.BxData.setValue(@"productionProfile", @"Qualified Electronic Certificate");
        envelope.BxData.setValue(@"givenName", txtime.Text);
        envelope.BxData.setValue(@"lastName", txtprezime.Text);
        envelope.BxData.setValue(@"uniqueCitizensNumber", txtjmbg.Text);
        envelope.BxData.setValue(@"dateOfBirth", txtdatumrodjenja.Text);
        envelope.BxData.setValue(@"includeUniqueCitizensNumber", ((utility.getItemValueAddedTax(Convert.ToInt32(ddlsertjmbg.SelectedValue))).ToString())); //da li se upisuje JMBG ili ne u sertifikat 1 ili 0
        envelope.BxData.setValue(@"identificationDocumentType", (utility.getEnglishText(Convert.ToInt32(ddlvrstadokumenta.SelectedValue))));
        envelope.BxData.setValue(@"identificationDocumentNumber", txtbrojiddokumenta.Text);
        //envelope.BxData.setValue(@"identificationIssuer", txtimeinstitucije.Text);
        envelope.BxData.setValue(@"identificationIssuerName", txtimeinstitucije.Text);
        envelope.BxData.setValue(@"paymentMethod", (utility.getEnglishText(Convert.ToInt32(ddlnacinplacanja.SelectedValue))));
        envelope.BxData.setValue(@"totalPrice", txtcenasaporezom.Text);
        envelope.BxData.setValue(@"identificationDocumentValidUntil", Session["zahtev-izdavanje-fizicko-lice-datumisteka"].ToString());
        envelope.BxData.setValue(@"identificationDocumentValidFrom", Session["zahtev-izdavanje-fizicko-lice-datumizdavanja"].ToString());
        envelope.BxData.setValue(@"emailAddress", txtadresaeposte.Text);
        envelope.BxData.setValue(@"phoneNumber", txttelefon.Text);
        envelope.BxData.setValue(@"deliveryLocation", (utility.getEnglishText(Convert.ToInt32(ddlsertadresa.SelectedValue))));
        envelope.BxData.setValue(@"distributionCity", txtmesto.Text);
        envelope.BxData.setValue(@"distributionStreet", txtulica.Text);
        envelope.BxData.setValue(@"distributionHouseNumber", txtbroj.Text);
        envelope.BxData.setValue(@"distributionPostalCode", txtpostanskibroj.Text);
        envelope.BxData.setValue(@"distributionPAK", txtpak.Text);
        envelope.BxData.setValue(@"media", (utility.getEnglishText(Convert.ToInt32(ddlmedijsert.SelectedValue))));
        envelope.BxData.setValue(@"validity", (utility.getEnglishText(Convert.ToInt32(ddlrokkoriscenjasert.SelectedValue))));
        envelope.BxData.setValue(@"identificationIssuer", (Constants.SOAP_INDIVIDUAL_COUNTRY_CODE)); //send CountryCode
        //envelope.BxData.setValue(@"identificationIssuerName", (Constants.SOAP_INDIVIDUAL_COUNTRY)); //send Country Name        
        //------------------------------------------------------------------------------            
        envelope.BxData.setValue(@"userAgentStringApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentStringApplicant"].ToString());
        envelope.BxData.setValue(@"ipApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentIP"].ToString());
        envelope.BxData.setValue(@"continentApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentContinent"].ToString());
        envelope.BxData.setValue(@"countryApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentCountry"].ToString());
        envelope.BxData.setValue(@"countryCodeApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentCountryCode"].ToString());
        envelope.BxData.setValue(@"cityApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentCity"].ToString());
        envelope.BxData.setValue(@"osApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentOS"].ToString());
        envelope.BxData.setValue(@"ispApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentISP"].ToString());
        envelope.BxData.setValue(@"browserApplicant", Session["zahtev-izdavanje-fizicko-lice-userAgentBrowser"].ToString());
        envelope.BxData.setValue(@"ipOperator", string.Empty);
        return envelope;
    }

    protected string CreateDocumentIssuingIndividual(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess)
    {
        string jmbg = string.Empty;
        string includeUniqueCitizensNumber = (utility.getItemValueAddedTax(Convert.ToInt32(ddlsertjmbg.SelectedValue))).ToString();
        if (includeUniqueCitizensNumber.Equals("0"))
        {
            jmbg = Constants.withoutJMBG;
        }
        else
        {
            jmbg = txtjmbg.Text;
        }


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
        Session["zahtev-izdavanje-fizicko-lice-brojzahteva"].ToString(),
        txtime.Text,
        txtprezime.Text,
        jmbg,
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

        //response = pisMess.CreateDocument(PisMessServiceReference.TemplateDocumentTypeSerbianPost.IssuingIndividual, parameters);
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
            Session["zahtev-izdavanje-fizicko-lice-brojzahteva"].ToString(),
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

        //response = pisMess.CreateDocument(PisMessServiceReference.TemplateDocumentTypeSerbianPost.PaymentOrder, parameters);
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
            log.Debug("Start sending SOAP message.");

            Utility utility = new Utility();

            BxSoapEnvelope envelope = createSoapEnvelope(utility);            
            //envelope.createBxSoapEnvelope();   create SOAP.xml 
            string SOAPresponse = BxSoap.SOAPManual(envelope.createBxSoapEnvelope());
            Utils.ParseSoapEnvelope(SOAPresponse, out BrojZahteva, out BrojZahtevaPravnoLice);

            if (BrojZahteva == string.Empty)
            {
                throw new Exception("RequestNumber is empty!");
            }

            Session["zahtev-izdavanje-fizicko-lice-brojzahteva"] = BrojZahteva;

            log.Debug("Finished sending SOAP message! RequestNumber is: " + BrojZahteva);
            log.Debug("Start creating PDF Files.");

            pisMess = new PisMessServiceReference.PisMessServiceClient();

            //string fileName = CreateDocumentIssuingIndividual(utility, pisMess);
            //fileName = CreateDocumentPaymentOrder(utility, pisMess);
            var CreateDocumentIssuingIndividualTask = Task.Run(() => CreateDocumentIssuingIndividual(utility, pisMess));
            var CreateDocumentPaymentOrderTask = Task.Run(() => CreateDocumentPaymentOrder(utility, pisMess));

            Task.WaitAll(new[] { CreateDocumentIssuingIndividualTask, CreateDocumentPaymentOrderTask });

            Session["zahtev-izdavanje-fizicko-lice-filename"] = CreateDocumentIssuingIndividualTask.Result;            
            Session["zahtev-izdavanje-fizicko-lice-filenamePaymentOrder"] = CreateDocumentPaymentOrderTask.Result;
            
            log.Debug("Finished creating PDF files!");
            
            Response.Redirect("zahtev-izdavanje-fizicko-lice-podnet.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
        }
        catch (AggregateException aex)
        {
            log.Error(aex.InnerException.Message, aex);
            throw aex;
        }
        catch (Exception ex)
        {
            log.Error("Error while sending request. " + ex.Message + "; " + ex.InnerException);
            //Disable datepicker
            txtdatumizdavanja.Text = Session["zahtev-izdavanje-fizicko-lice-datumizdavanja"].ToString();
            txtdatumisteka.Text = Session["zahtev-izdavanje-fizicko-lice-datumisteka"].ToString();
            ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);
        }        
    }

    protected void btnReEnterRequest_Click1(object sender, EventArgs e)
    {
        Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg1"] = false;
        Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"] = true;
        Session["zahtev-izdavanje-fizicko-lice-ddlvrstadokumenta"] = true;
        Session["zahtev-izdavanje-fizicko-lice-ddlsertadresa"] = true;
        Session["zahtev-izdavanje-fizicko-lice-ddlrok"] = true;
        Session["zahtev-izdavanje-fizicko-lice-ddlmedij"] = true;
        Session["zahtev-izdavanje-fizicko-lice-ddlnacinplacanja"] = true;

        txtime.ReadOnly = false;
        txtime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtprezime.ReadOnly = false;
        txtprezime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtjmbg.ReadOnly = false;
        txtjmbg.BackColor = ColorTranslator.FromHtml(SetWhite);

        if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"]) == false)
        {
            ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg"]);
            ddlsertjmbg.CssClass = SetCss1;
        }
        else
        {
            ddlsertjmbg.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertjmbg1"]);
            ddlsertjmbg.CssClass = SetCss1;
        }
        ddlsertjmbg.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlvrstadokumenta.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlvrstadokumenta"]);
        ddlvrstadokumenta.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlvrstadokumenta.CssClass = SetCss2;
        txtbrojiddokumenta.ReadOnly = false;
        txtbrojiddokumenta.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtimeinstitucije.ReadOnly = false;
        txtimeinstitucije.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtdatumizdavanja.ReadOnly = true;
        txtdatumizdavanja.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtdatumisteka.ReadOnly = true;
        txtdatumisteka.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtadresaeposte.ReadOnly = false;
        txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetWhite);
        txttelefon.ReadOnly = false;
        txttelefon.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertadresa"]);
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
        ddlrokkoriscenjasert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlrok"]);
        ddlrokkoriscenjasert.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlrokkoriscenjasert.CssClass = SetCss1;
        ddlmedijsert.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlmedij"]);
        ddlmedijsert.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlmedijsert.CssClass = SetCss1;
        ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlnacinplacanja"]);
        ddlnacinplacanja.BackColor = ColorTranslator.FromHtml(SetWhite);
        ddlnacinplacanja.CssClass = SetCss1;
        myDiv6.Visible = false;
        myDiv5.Visible = true;
        myDiv8.Visible = false;

        txtdatumisteka.Text = Session["zahtev-izdavanje-fizicko-lice-datumisteka"].ToString();
        txtdatumizdavanja.Text = Session["zahtev-izdavanje-fizicko-lice-datumizdavanja"].ToString();

        txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtcenasaporezom.ReadOnly = true;
        txtdatumrodjenja.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtdatumrodjenja.ReadOnly = true;
    }

    protected void txtbroj_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged9();
        if (errLabelBroj.Text != string.Empty)
        {
            Session["zahtev-izdavanje-fizicko-lice-event_controle"] = ((TextBox)sender);
        }
        else
        {
            Session["zahtev-izdavanje-fizicko-lice-event_controle"] = txtpostanskibroj;
        }
    }

    private void CheckIfChannelHasChanged9()
    {
        Utility utility = new Utility();
        string SettingValue = utility.getSettingsValueGlobalSettings(Constants.GLOBAL_VALIDATION);

        if (SettingValue == Constants.SETTING_VALUE_TRUE)
        {
            if (Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-TurnOnCityValidation"]))
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
            if (Session["zahtev-izdavanje-fizicko-lice-event_controle"] != null)
            {
                TextBox controle = (TextBox)Session["zahtev-izdavanje-fizicko-lice-event_controle"];
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
            if (Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"] != null)
            {
                DropDownList padajucalista = (DropDownList)Session["zahtev-izdavanje-fizicko-lice-event_controle-DropDownList"];
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


