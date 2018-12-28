using BlueXSOAP;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class zahtev_deblokada : System.Web.UI.Page
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
    public PisMessServiceReference.PisMessServiceClient pisMess;
    public List<PTTVariable> PTTVariables;
    public string CityVariable;
    public string StreetVariable;
    public string HouseNumberVariable;
    public string ZipCodeVariable;
    public string PAKVariable;
    public bool InHouseVariable;
    public bool IsAllowedVariable;

    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public List<WebControl> Controls;
    //promenljive za validaciju 
    public bool TurnOnJMBGValidation;
    public bool TurnOnEmailValidation;
    public bool TurnOnPhoneValidation;
    public bool TurnOnAjaxValidation;

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
                Session["Zahtev-promena-statusa-ddlnacinplacanja"] = true;
                Session["Zahtev-promena-statusa-rbIsNotAttached"] = true;
                Session["Zahtev-promena-statusa-rbIsAttached"] = true;
                Session["Zahtev-promena-statusa-datumzahteva"] = string.Empty;
                Session["Zahtev-promena-statusa-ddlsertadresa"] = true;

                string IDItem1 = string.Empty;
                string IDItem2 = string.Empty;
                SetUpDefaultItem(ddlnacinplacanja.ClientID, out IDItem1);
                ddlnacinplacanja.SelectedValue = IDItem1;
                SetUpDefaultItem(ddlsertadresa.ClientID, out IDItem2);
                ddlsertadresa.SelectedValue = IDItem2;
                txtcenasaporezom.Text = utility.getItemText(Constants.IDITEM_UNBLOCK_PRICE);
                Session["Zahtev-promena-statusa-Price"] = txtcenasaporezom.Text;
                Container1.Visible = false;
                myDiv6.Visible = false;
                txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
                txtcenasaporezom.ReadOnly = true;
                Container2.Visible = false;
                Container3.Visible = false;
                Container00.Visible = true;
                rbIsAttached.Checked = false;
                rbIsNotAttached.Checked = false;
                rbIsAttached.TabIndex = 7;
                rbIsNotAttached.TabIndex = 8;

                //-------TABINDEX---------------
                Session["Zahtev-promena-statusa-event_controle"] = txtime;
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

            ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-ddlnacinplacanja"]);

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

            rbIsAttached.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-rbIsAttached"]);
            rbIsNotAttached.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-rbIsNotAttached"]);
            ddlsertadresa.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-ddlsertadresa"]);
            ddlsertadresa.CssClass = SetCss4;
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
                if (control.Id == txtadresaeposte.ClientID)
                {
                    TurnOnEmailValidation = control.ControlStatus;
                    Session["Zahtev-promena-statusa-TurnOnEmailValidation"] = TurnOnEmailValidation;
                }
                else if (control.Id == txttelefon.ClientID)
                {
                    TurnOnPhoneValidation = control.ControlStatus;
                    Session["Zahtev-promena-statusa-TurnOnPhoneValidation"] = TurnOnPhoneValidation;
                }
                else if (control.Id == txtmesto.ClientID)
                {
                    Session["Zahtev-promena-statusa-TurnOnCityValidation"] = control.ControlStatus;
                    TurnOnAjaxValidation = control.ControlStatus;
                    ValidateAjax(TurnOnAjaxValidation);
                }
                else if (control.Id == txtime.ClientID)
                {
                    Session["Zahtev-promena-statusa-TurnOnNameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtprezime.ClientID)
                {
                    Session["Zahtev-promena-statusa-TurnOnSurnameValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtulica.ClientID)
                {
                    Session["Zahtev-promena-statusa-TurnOnStreetValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtbroj.ClientID)
                {
                    Session["Zahtev-promena-statusa-TurnOnHouseNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpostanskibroj.ClientID)
                {
                    Session["Zahtev-promena-statusa-TurnOnPostNumberValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtpak.ClientID)
                {
                    Session["Zahtev-promena-statusa-TurnOnPAKValidation"] = control.ControlStatus;
                }
                else if (control.Id == txtjik.ClientID)
                {
                    Session["Zahtev-promena-statusa-TurnOnSecondJIKValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            TurnOnEmailValidation = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnEmailValidation"] = TurnOnEmailValidation;
            TurnOnPhoneValidation = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnPhoneValidation"] = TurnOnPhoneValidation;
            TurnOnAjaxValidation = Constants.VALIDATION_FALSE;
            ValidateAjax(TurnOnAjaxValidation);
            Session["Zahtev-promena-statusa-TurnOnCityValidation"] = Constants.VALIDATION_FALSE;
            //------------------------------------------
            Session["Zahtev-promena-statusa-TurnOnCityValidation"] = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnNameValidation"] = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnSurnameValidation"] = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnStreetValidation"] = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnHouseNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnPostNumberValidation"] = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnPAKValidation"] = Constants.VALIDATION_FALSE;
            Session["Zahtev-promena-statusa-TurnOnSecondJIKValidation"] = Constants.VALIDATION_FALSE;
        }
    }

    protected void ValidateAjax(bool TurnOnAjaxValidation)
    {
        autoCompleteMestoBoravka.Enabled = TurnOnAjaxValidation;
        autoCompleteUlicaBoravka.Enabled = TurnOnAjaxValidation;
    }

    protected void SetUpIsRequiredTextBoxes()
    {
        Utility utility1 = new Utility();
        string page = Path.GetFileName(Page.AppRelativeVirtualPath);
        Controls = new List<WebControl>();
        Controls = utility1.pronadjiKontrolePoTipu(page, Constants.CONTROL_TYPE_TEXTBOX);

        foreach (var control in Controls)
        {
            if (control.Id == txtjmbgibrojpasosa.ClientID)
            {
                Session["zahtev-promena-statusa-txtjmbgibrojpasosaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txttelefon.ClientID)
            {
                Session["zahtev-promena-statusa-txttelefonIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtadresaeposte.ClientID)
            {
                Session["zahtev-promena-statusa-txtadresaeposteIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtmesto.ClientID)
            {
                Session["zahtev-promena-statusa-txtmestoIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtulica.ClientID)
            {
                Session["zahtev-promena-statusa-txtulicaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpostanskibroj.ClientID)
            {
                Session["zahtev-promena-statusa-txtpostanskibrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtime.ClientID)
            {
                Session["zahtev-promena-statusa-txtimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtprezime.ClientID)
            {
                Session["zahtev-promena-statusa-txtprezimeIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbroj.ClientID)
            {
                Session["zahtev-promena-statusa-txtbrojIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtpak.ClientID)
            {
                Session["zahtev-promena-statusa-txtpakIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtkarticatoken.ClientID)
            {
                Session["zahtev-promena-statusa-txtkarticatokenIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtjik.ClientID)
            {
                Session["zahtev-promena-statusa-txtjikIsRequired"] = control.IsRequired;
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
            if (control.Id == ddlnacinplacanja.ClientID)
            {
                Session["zahtev-promena-statusa-ddlnacinplacanjaIsRequired"] = control.IsRequired;
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

    protected void rbIsAttached_CheckedChanged(object sender, EventArgs e)
    {           
        if (rbIsAttached.Checked == true)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "unhook", "unhook();", true);
            rbIsAttached.Checked = true;
            rbIsNotAttached.Checked = false;
            Container2.Visible = true;
            Container3.Visible = false;
            txtmesto.Text = string.Empty;
            txtmesto.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtmesto.ReadOnly = true;
            txtulica.Text = string.Empty;
            txtulica.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtulica.ReadOnly = true;
            txtbroj.Text = string.Empty;
            txtbroj.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtbroj.ReadOnly = true;
            txtpostanskibroj.Text = string.Empty;
            txtpostanskibroj.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtpostanskibroj.ReadOnly = true;
            txtpak.Text = string.Empty;
            txtpak.BackColor = ColorTranslator.FromHtml(SetLightGray);
            txtpak.ReadOnly = true;
            Session["zahtev-promena-statusa-event_controle-RadioButton"] = rbIsAttached;
            SetFocusOnRadioButton();
        }
    }

    protected void rbIsNotAttached_CheckedChanged(object sender, EventArgs e)
    {        
        if (rbIsNotAttached.Checked == true)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "unhook", "unhook();", true);
            rbIsNotAttached.Checked = true;
            rbIsAttached.Checked = false;
            Container2.Visible = false;
            Container3.Visible = true;
            txtkarticatoken.Text = string.Empty;
            Session["zahtev-promena-statusa-event_controle-RadioButton"] = rbIsNotAttached;
            SetFocusOnRadioButton();
        }
    }

    protected void ddlnacinplacanja_SelectedIndexChanged(object sender, EventArgs e)
    {
        int SelectedValue = Convert.ToInt32(ddlnacinplacanja.SelectedValue);
        Session["zahtev-promena-statusa-event_controle-DropDownList"] = ((DropDownList)sender);
        SetFocusOnDropDownLists();
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
            args.IsValid = UtilsValidation.ValidateName(txtime.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtimeIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnNameValidation"]), out ErrorMessage1, out nameformat);
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
            args.IsValid = UtilsValidation.ValidateSurname(txtprezime.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtprezimeIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnSurnameValidation"]), out ErrorMessage1, out surnameformat);
            cvprezime.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvprezime.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvjmbgibrojpasosa_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateJmbgiBrojPasosa(txtjmbgibrojpasosa.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtjmbgibrojpasosaIsRequired"]), out ErrorMessage1);
            cvjmbgibrojpasosa.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvjmbgibrojpasosa.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvmesto_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string cityformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateCity(txtmesto.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtmestoIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnCityValidation"]), out ErrorMessage1, out cityformat);
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
            args.IsValid = UtilsValidation.ValidateStreet(txtulica.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtulicaIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnStreetValidation"]), out ErrorMessage1, out streetformat);
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
            args.IsValid = UtilsValidation.ValidateHouseNumber(txtbroj.Text, errLabelBroj.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtbrojIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnHouseNumberValidation"]), out ErrorMessage1, out housenumberformat);
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
            args.IsValid = UtilsValidation.ValidatePostNumber(txtpostanskibroj.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtpostanskibrojIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnPostNumberValidation"]), out ErrorMessage1, out postnumberformat);
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
            args.IsValid = UtilsValidation.ValidatePAK(txtpak.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtpakIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnPAKValidation"]), out ErrorMessage1, out pakformat);
            cvpak.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvpak.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvkarticatoken_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateJmbgiBrojPasosa(txtkarticatoken.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtjmbgibrojpasosaIsRequired"]), out ErrorMessage1);
            cvkarticatoken.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvkarticatoken.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA Telefonskog BROJA----------------------------------
    //------------------------------------------------------------------------------------------------
    protected void txttelefon_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged1();
        if (errLabelNumber.Text != string.Empty)
        {
            Session["Zahtev-promena-statusa-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["Zahtev-promena-statusa-event_controle"] = txtadresaeposte;
            SetFocusOnTextbox();
        }
    }

    private void CheckIfChannelHasChanged1()
    {
        string newNumber = txttelefon.Text;
        string errorMessage = string.Empty;
        string numberformat = string.Empty;
        bool LegalEntityPhone = false;

        if (Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnPhoneValidation"]))
        {
            UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-promena-statusa-txttelefonIsRequired"]), LegalEntityPhone, out errorMessage, out numberformat);
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

        if (Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnPhoneValidation"]))
        {
            args.IsValid = UtilsValidation.ValidateNumber(newNumber, Convert.ToBoolean(Session["zahtev-promena-statusa-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
            txttelefon.Text = numberformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationNumber(newNumber, Convert.ToBoolean(Session["zahtev-promena-statusa-txttelefonIsRequired"]), LegalEntityPhone, out errMessage, out numberformat);
            cvtelefon.ErrorMessage = errMessage;
        }
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    protected void cvnacinplacanja_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlnacinplacanjaString = ddlnacinplacanja.ClientID;
            string IDItem1 = string.Empty;
            SetUpDefaultItem(ddlnacinplacanjaString, out IDItem1);

            args.IsValid = UtilsValidation.ValidateNacinPlacanja(ddlnacinplacanja.SelectedValue, Convert.ToBoolean(Session["zahtev-promena-statusa-ddlnacinplacanjaIsRequired"]), IDItem1, out ErrorMessage1);
            cvnacinplacanja.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvnacinplacanja.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvnacindeblokade_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {              
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateRadioButtons(rbIsAttached, rbIsNotAttached, out ErrorMessage1);
            cvnacindeblokade.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvnacindeblokade.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
        
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------PROVERA E ADRESE----------------------------------
    //--------------------------------------------------------------------------------------
    protected void txtadresaeposte_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged();
        if (errLabel.Text != string.Empty)
        {
            Session["Zahtev-promena-statusa-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["Zahtev-promena-statusa-event_controle-DropDownList"] = ddlnacinplacanja;
            SetFocusOnDropDownLists();
        }
    }

    private void CheckIfChannelHasChanged()
    {
        string newMail = txtadresaeposte.Text;
        string errorMessage = string.Empty;
        string mailformat = string.Empty;

        if (Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnEmailValidation"]))
        {
            UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-promena-statusa-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
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

        if (Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnEmailValidation"]))
        {
            args.IsValid = UtilsValidation.ValidateMail(newMail, Convert.ToBoolean(Session["zahtev-promena-statusa-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
            cvadresaeposte.ErrorMessage = errorMessage;
            newMail = mailformat;
        }
        else
        {
            args.IsValid = UtilsValidation.WithoutValidationMail(newMail, Convert.ToBoolean(Session["zahtev-promena-statusa-txtadresaeposteIsRequired"]), out errorMessage, out mailformat);
            cvadresaeposte.ErrorMessage = errorMessage;
            newMail = mailformat;
        }
    }

    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------

    protected void btnSubmit_Click1(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "successalert();", true);
                if (true)
                {
                    Utility utility = new Utility();

                    ValidateAjax(false);

                    txtime.ReadOnly = true;
                    txtime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtprezime.ReadOnly = true;
                    txtprezime.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtjmbgibrojpasosa.ReadOnly = true;
                    txtjmbgibrojpasosa.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtadresaeposte.ReadOnly = true;
                    txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txttelefon.ReadOnly = true;
                    txttelefon.BackColor = ColorTranslator.FromHtml(SetDarkGray);
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
                    Session["Zahtev-promena-statusa-ddlsertadresa"] = false;
                    ddlsertadresa.Enabled = Convert.ToBoolean(Session["zahtev-izdavanje-fizicko-lice-ddlsertadresa"]);
                    ddlsertadresa.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlsertadresa.CssClass = SetCss4;
                    Session["Zahtev-promena-statusa-ddlnacinplacanja"] = false;
                    ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-ddlnacinplacanja"]);
                    ddlnacinplacanja.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    ddlnacinplacanja.CssClass = SetCss1;
                    txtcenasaporezom.ReadOnly = true;
                    ddlnacinplacanja.CssClass = SetCss1;
                    txtcenasaporezom.Text = utility.getItemText(Constants.IDITEM_UNBLOCK_PRICE);
                    txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtjik.ReadOnly = true;
                    txtjik.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    txtkarticatoken.ReadOnly = true;
                    txtkarticatoken.BackColor = ColorTranslator.FromHtml(SetDarkGray);
                    Container1.Visible = true;
                    myDiv6.Visible = true;
                    myDiv5.Visible = false;
                    Session["Zahtev-promena-statusa-rbIsNotAttached"] = false;
                    Session["Zahtev-promena-statusa-rbIsAttached"] = false;
                    rbIsAttached.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-rbIsAttached"]);
                    rbIsNotAttached.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-rbIsNotAttached"]);

                    txtdatumzahteva.Text = DateTime.Now.ToString("dd.MM.yyyy");
                    Session["Zahtev-promena-statusa-datumzahteva"] = txtdatumzahteva.Text;
                    Session["Zahtev-promena-statusa-cenasaporezom"] = txtcenasaporezom.Text;

                    //-----------------GetUserAgent string---------------------------
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
                    Session["Zahtev-promena-statusa-userAgentBrowser"] = userAgentBrowser;
                    Session["Zahtev-promena-statusa-userAgentStringApplicant"] = userAgentStringApplicant;
                    Session["Zahtev-promena-statusa-userAgentOS"] = userAgentOS;
                    Session["Zahtev-promena-statusa-userAgentIP"] = userAgentIP;
                    Session["Zahtev-promena-statusa-userAgentContinent"] = userAgentContinent;
                    Session["Zahtev-promena-statusa-userAgentCountry"] = userAgentCountry;
                    Session["Zahtev-promena-statusa-userAgentCountryCode"] = userAgentCountryCode;
                    Session["Zahtev-promena-statusa-userAgentCity"] = userAgentCity;
                    Session["Zahtev-promena-statusa-userAgentISP"] = userAgentISP;
                }
            }
            else if (!Page.IsValid)
            {
                errLabel.Text = string.Empty;
                errLabelNumber.Text = string.Empty;
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
    public string ResponseStatus = string.Empty;
    public string USI = string.Empty;
    public string Mesto = string.Empty;
    public string Ulica = string.Empty;
    public string Broj = string.Empty;
    public string PostanskiBroj = string.Empty;
    public string PAK = string.Empty;
    public string SerialNumber = string.Empty;
    public string requestChallengeResponse = string.Empty;
    public bool isResponseZero = true;
    public string responseMessage = string.Empty;

    protected BxSoapEnvelope createSoapEnvelope(Utility utility, string Mesto, string Ulica, string Broj, string PostanskiBroj, string PAK, string requestChallengeResponse, string SerialNumber)
    {
        BxSoapEnvelope envelope = new BxSoapEnvelopeUnblockMedium();

        envelope.BxData.setValue(@"USI", txtjik.Text);
        envelope.BxData.setValue(@"givenName", txtime.Text);
        envelope.BxData.setValue(@"lastName", txtprezime.Text);
        envelope.BxData.setValue(@"uniqueCitizensNumber", txtjmbgibrojpasosa.Text);
        envelope.BxData.setValue(@"emailAddress", txtadresaeposte.Text);
        envelope.BxData.setValue(@"phoneNumber", txttelefon.Text);
        envelope.BxData.setValue(@"paymentMethod", (utility.getEnglishText(Convert.ToInt32(ddlnacinplacanja.SelectedValue))));
        envelope.BxData.setValue(@"requestChallengeResponse", requestChallengeResponse);
        //envelope.BxData.setValue(@"deliveryLocation", (utility.getEnglishText(Convert.ToInt32(ddlsertadresa.SelectedValue))));
        envelope.BxData.setValue(@"distributionCity", Mesto);
        envelope.BxData.setValue(@"distributionStreet", Ulica);
        envelope.BxData.setValue(@"distributionHouseNumber", Broj);
        envelope.BxData.setValue(@"distributionPostalCode", PostanskiBroj);
        envelope.BxData.setValue(@"distributionPAK", PAK);
        envelope.BxData.setValue(@"tokenSerialNumber", SerialNumber);
        envelope.BxData.setValue(@"totalPrice", txtcenasaporezom.Text); //Fiksirano iz baze 360,00
        //------------------------------------------------------------------------------            
        envelope.BxData.setValue(@"userAgentStringApplicant", Session["Zahtev-promena-statusa-userAgentStringApplicant"].ToString());
        envelope.BxData.setValue(@"ipApplicant", Session["Zahtev-promena-statusa-userAgentIP"].ToString());
        envelope.BxData.setValue(@"continentApplicant", Session["Zahtev-promena-statusa-userAgentContinent"].ToString());
        envelope.BxData.setValue(@"countryApplicant", Session["Zahtev-promena-statusa-userAgentCountry"].ToString());
        envelope.BxData.setValue(@"countryCodeApplicant", Session["Zahtev-promena-statusa-userAgentCountryCode"].ToString());
        envelope.BxData.setValue(@"cityApplicant", Session["Zahtev-promena-statusa-userAgentCity"].ToString());
        envelope.BxData.setValue(@"osApplicant", Session["Zahtev-promena-statusa-userAgentOS"].ToString());
        envelope.BxData.setValue(@"ispApplicant", Session["Zahtev-promena-statusa-userAgentISP"].ToString());
        envelope.BxData.setValue(@"browserApplicant", Session["Zahtev-promena-statusa-userAgentBrowser"].ToString());
        envelope.BxData.setValue(@"ipOperator", string.Empty);
        return envelope;
    }

    protected List<PisMessServiceReference.Parameter> getDocumentParametersList(Utility utility, string requestChallengeResponse)
    {
        List<PisMessServiceReference.Parameter> documentParameters = new List<PisMessServiceReference.Parameter>();

        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "requestNumber",
                //todo samo zameni
                ParameterValue = Session["Zahtev-promena-statusa-brojzahteva"].ToString()
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "firstName",
                ParameterValue = txtime.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "lastName",
                ParameterValue = txtprezime.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "jmbgOrPassportCountry",
                ParameterValue = txtjmbgibrojpasosa.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "challengeResponse",
                ParameterValue = requestChallengeResponse
            });

        return documentParameters;
    }

    protected List<PisMessServiceReference.Parameter> getDocumentParametersListPaymentOrder(Utility utility)
    {
        List<PisMessServiceReference.Parameter> documentParameters = new List<PisMessServiceReference.Parameter>();

        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "requestNumber",
                //todo samo zameni
                ParameterValue = Session["Zahtev-promena-statusa-brojzahteva"].ToString()
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "firstName",
                ParameterValue = txtime.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "lastName",
                ParameterValue = txtprezime.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "price",
                ParameterValue = txtcenasaporezom.Text
            });
        documentParameters.Add(
            new PisMessServiceReference.Parameter
            {
                ParameterName = "purposeOfPayment",
                ParameterValue = Constants.UNBLOCKING
            });

        return documentParameters;
    }

    protected string CreateDocumentUnblockingRequest(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess, List<PisMessServiceReference.Parameter> documentParameters)
    {
        //templateDocumentType: UnblockingRequest
        //-----------------------------------------
        string responseMessage = string.Empty;
        List<string> response = new List<string>(ServiceCaller.CallServiceCreateDoc(PisMessServiceReference.TemplateDocumentTypeSerbianPost.UnblockingRequest, documentParameters.ToArray()));
        if (response[0].Equals("0"))
        {
            //success
            responseMessage = response[1]; //write file path to some text box
        }
        else
        {
            //error
            responseMessage = response[0]; //write error description to some text box
            throw new Exception("Response error while creating UnblockingRequest document, response from PissMess: " + responseMessage);
        }
        return responseMessage;
    }

    protected string CreateDocumentUnblockingRequestPaymentOrder(Utility utility, PisMessServiceReference.PisMessServiceClient pisMess, List<PisMessServiceReference.Parameter> documentParameters)
    {
        //templateDocumentType: UnblockingRequestPaymentOrder
        //-----------------------------------------
        string responseMessage = string.Empty;
        List<string> response = new List<string>(ServiceCaller.CallServiceCreateDoc(PisMessServiceReference.TemplateDocumentTypeSerbianPost.PaymentOrder, documentParameters.ToArray()));
        if (response[0].Equals("0"))
        {
            //success
            responseMessage = response[1]; //write file path to some text box
        }
        else
        {
            //error
            responseMessage = response[0]; //write error description to some text box
            throw new Exception("Response error while creating UnblockingRequestPaymentOrder document, response from PissMess: " + responseMessage);
        }
        return responseMessage;
    }

    protected void btnEnterRequest_Click1(object sender, EventArgs e)
    {
        try
        {
            log.Debug("Start sending SOAP message.");

            if (rbIsAttached.Checked == true)
            {
                Mesto = txtmesto.Text;
                Ulica = txtulica.Text;
                Broj = txtbroj.Text;
                PostanskiBroj = txtpostanskibroj.Text;
                PAK = txtpak.Text;
                requestChallengeResponse = Constants.REQUEST_CHALLENGE_RESPONSE_FALSE;
            }
            else
            {
                SerialNumber = txtkarticatoken.Text;
                requestChallengeResponse = Constants.REQUEST_CHALLENGE_RESPONSE_TRUE;
            }

            Utility utility = new Utility();

            pisMess = new PisMessServiceReference.PisMessServiceClient();
            List<PisMessServiceReference.Parameter> documentParameters = new List<PisMessServiceReference.Parameter>();

            BxSoapEnvelope envelope = createSoapEnvelope(utility, Mesto, Ulica, Broj, PostanskiBroj, PAK, requestChallengeResponse, SerialNumber);

            //envelope.createBxSoapEnvelope();   //create SOAP.xml 
            string SOAPresponse = BxSoap.SOAPManual(envelope.createBxSoapEnvelope());

            Utils.ParseSoapEnvelopeStatusChange(SOAPresponse, out USI, out BrojZahteva, out ResponseStatus);

            if (ResponseStatus != Constants.RESPONSE_STATUS_CHANGE_SUCCESS)
            {
                throw new Exception("ResponseStatus is not success!");
            }

            Session["Zahtev-promena-statusa-brojzahteva"] = BrojZahteva;

            log.Debug("Successfully send SOAP message! RequestNumber for Unblock Status is: " + BrojZahteva);

            log.Debug("Start creating PDF Files.");

            documentParameters = getDocumentParametersList(utility, requestChallengeResponse);
            var CreateDocumentUnblockingRequestTask = Task.Run(() => CreateDocumentUnblockingRequest(utility, pisMess, documentParameters));
       
            documentParameters = getDocumentParametersListPaymentOrder(utility);
            var CreateDocumentUnblockingRequestPaymentOrderTask = Task.Run(() => CreateDocumentUnblockingRequestPaymentOrder(utility, pisMess, documentParameters));

            Task.WaitAll(new[] { CreateDocumentUnblockingRequestTask, CreateDocumentUnblockingRequestPaymentOrderTask });

            //string fileName = CreateDocumentUnblockingRequest(utility, pisMess, documentParameters);
            Session["Zahtev-promena-statusa-filename"] = CreateDocumentUnblockingRequestTask.Result;
            //string fileNamePaymentOrder = CreateDocumentUnblockingRequestPaymentOrder(utility, pisMess, documentParameters);
            Session["Zahtev-promena-statusa-fileNamePaymentOrder"] = CreateDocumentUnblockingRequestPaymentOrderTask.Result;

            log.Debug("Finished creating PDF files!");

            Response.Redirect("zahtev-deblokada-podnet.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
        }
        catch (AggregateException aex)
        {
            log.Error(aex.InnerException.Message, aex);
            throw aex;
        }
        catch (Exception ex)
        {
            log.Error("Error while sending request. " + ex.Message + "; " + ex.InnerException);
            ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);            
        }        
    }



    protected void btnReEnterRequest_Click1(object sender, EventArgs e)
    {
        txtime.ReadOnly = false;
        txtime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtprezime.ReadOnly = false;
        txtprezime.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtjmbgibrojpasosa.ReadOnly = false;
        txtjmbgibrojpasosa.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtadresaeposte.ReadOnly = false;
        txtadresaeposte.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtjik.ReadOnly = false;
        txtjik.BackColor = ColorTranslator.FromHtml(SetWhite);
        txttelefon.ReadOnly = false;
        txttelefon.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtulica.ReadOnly = false;
        txtulica.BackColor = ColorTranslator.FromHtml(SetWhite);                       
        txtpostanskibroj.ReadOnly = false;
        txtpostanskibroj.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtbroj.ReadOnly = false;
        txtbroj.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtpak.ReadOnly = false;
        txtpak.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtmesto.ReadOnly = false;
        txtmesto.BackColor = ColorTranslator.FromHtml(SetWhite);

        Session["Zahtev-promena-statusa-ddlsertadresa"] = true;
        ddlsertadresa.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-ddlsertadresa"]);
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
        Session["Zahtev-promena-statusa-ddlnacinplacanja"] = true;        
        ddlnacinplacanja.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-ddlnacinplacanja"]);
        ddlnacinplacanja.BackColor = ColorTranslator.FromHtml(SetWhite);
        txtcenasaporezom.BackColor = ColorTranslator.FromHtml(SetLightGray);
        txtcenasaporezom.Text = Session["Zahtev-promena-statusa-Price"].ToString();
        txtcenasaporezom.ReadOnly = true;
        ddlnacinplacanja.CssClass = SetCss1;
        txtkarticatoken.ReadOnly = false;
        txtkarticatoken.BackColor = ColorTranslator.FromHtml(SetWhite);
        Container1.Visible = false;
        myDiv6.Visible = false;
        myDiv5.Visible = true;
        if (Convert.ToInt32(txtjik.Text) <= Constants.USI)
        {
            Session["Zahtev-promena-statusa-rbIsNotAttached"] = false;
        }
        else
        {
            Session["Zahtev-promena-statusa-rbIsNotAttached"] = true;
        }
        Session["Zahtev-promena-statusa-rbIsAttached"] = true;
        rbIsAttached.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-rbIsAttached"]);
        rbIsNotAttached.Enabled = Convert.ToBoolean(Session["Zahtev-promena-statusa-rbIsNotAttached"]);        
    }

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
            if (Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnCityValidation"]))
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
            ValidateAjax(Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnCityValidation"]));
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
        Session["Zahtev-promena-statusa-event_controle-DropDownList"] = ((DropDownList)sender);
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
                Session["Zahtev-promena-statusa-CityVariable"] = CityVariable;
                Session["Zahtev-promena-statusa-StreetVariable"] = StreetVariable;
                Session["Zahtev-promena-statusa-HouseNumberVariable"] = HouseNumberVariable;
                Session["Zahtev-promena-statusa-ZipCodeVariable"] = ZipCodeVariable;
                Session["Zahtev-promena-statusa-PAKVariable"] = PAKVariable;
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

    protected void cvsertadresa_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlsertadresaString = ddlsertadresa.ClientID;
            string IDItem = string.Empty;
            SetUpDefaultItem(ddlsertadresaString, out IDItem);

            args.IsValid = UtilsValidation.ValidateSertAdresa(ddlsertadresa.SelectedValue, Convert.ToBoolean(Session["Zahtev-promena-statusa-ddlsertadresaIsRequired"]), IDItem, out ErrorMessage1);
            cvsertadresa.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvsertadresa.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    public void SetFocusOnTextbox()
    {
        try
        {
            if (Session["Zahtev-promena-statusa-event_controle"] != null)
            {
                TextBox controle = (TextBox)Session["Zahtev-promena-statusa-event_controle"];
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
            if (Session["Zahtev-promena-statusa-event_controle-DropDownList"] != null)
            {
                DropDownList padajucalista = (DropDownList)Session["Zahtev-promena-statusa-event_controle-DropDownList"];
                //padajucalista.Focus();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "foc", "document.getElementById('" + padajucalista.ClientID + "').focus();", true);
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
            if (Session["Zahtev-promena-statusa-event_controle-RadioButton"] != null)
            {
                RadioButton radiodugme = (RadioButton)Session["Zahtev-promena-statusa-event_controle-RadioButton"];
                //radiodugme.Focus();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "foc", "document.getElementById('" + radiodugme.ClientID + "').focus();", true);
            }
        }
        catch (InvalidCastException inEx)
        {
            log.Error("Problem with setting focus on control. Error: " + inEx);
        }
    }
    ///////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    protected void txtjik_TextChanged(object sender, EventArgs e)
    {
        CheckIfChannelHasChanged2();
        if (errLabelJik.Text != string.Empty)
        {
            Session["Zahtev-promena-statusa-event_controle"] = ((TextBox)sender);
            SetFocusOnTextbox();
        }
        else
        {
            Session["Zahtev-promena-statusa-event_controle-RadioButton"] = rbIsAttached;
            SetFocusOnRadioButton();
            
        }
    }

    private void CheckIfChannelHasChanged2()
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "unhook", "unhook();", true);
        string newJik = txtjik.Text;
        string errorMessage = string.Empty;
        string jikformat = string.Empty;
        bool radiobtnstatus = true;

        if (Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnSecondJIKValidation"]))
        {
            UtilsValidation.ValidateOldJIK(newJik, Convert.ToBoolean(Session["zahtev-promena-statusa-txtjikIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnSecondJIKValidation"]), out errorMessage, out radiobtnstatus);
            errLabelJik.Text = errorMessage;
            newJik = jikformat;
            if (!radiobtnstatus)
            {
                Session["Zahtev-promena-statusa-rbIsNotAttached"] = false;
                Container3.Visible = false;
                rbIsNotAttached.Checked = false;
            }
            else
            {
                Session["Zahtev-promena-statusa-rbIsNotAttached"] = true;
            }
        }
        else
        {
            newJik = txtjik.Text;
            errorMessage = string.Empty;
        }
    }

    protected void cvjik_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            bool radiobtnstatus = true;
            string ErrorMessage1 = string.Empty;
            args.IsValid = UtilsValidation.ValidateOldJIK(txtjik.Text, Convert.ToBoolean(Session["zahtev-promena-statusa-txtjikIsRequired"]), Convert.ToBoolean(Session["Zahtev-promena-statusa-TurnOnSecondJIKValidation"]), out ErrorMessage1, out radiobtnstatus);
            cvjik.ErrorMessage = ErrorMessage1;
            errLabelJik.Text = string.Empty;
            if (!radiobtnstatus)
            {
                Session["Zahtev-promena-statusa-rbIsNotAttached"] = false;
                Container3.Visible = false;
                rbIsNotAttached.Checked = false;
            }
            else
            {
                Session["Zahtev-promena-statusa-rbIsNotAttached"] = true;
            }
        }
        catch (Exception)
        {
            cvjik.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }
}