using BlueXSOAP;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class provera_statusa_zahteva : System.Web.UI.Page
{
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
            AvoidCashing();

            if (!Page.IsPostBack)
            {
                Container1.Visible = true;               
                Container00.Visible = false;
                Container000.Visible = false;
                Container0.Visible = false;
                txtbrojzahteva.Text = string.Empty;
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
                if (control.Id == txtbrojzahteva.ClientID)
                {
                    Session["provera-statusa-zahteva-TurnOnRequestNumberValidation"] = control.ControlStatus;
                }
            }
        }
        else
        {
            Session["provera-statusa-zahteva-TurnOnRequestNumberValidation"] = Constants.VALIDATION_FALSE;
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
            if (control.Id == txtstatus.ClientID)
            {
                Session["provera-statusa-zahteva-txtstatusIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtbrojzahteva.ClientID)
            {
                Session["provera-statusa-zahteva-txtbrojzahtevaIsRequired"] = control.IsRequired;
            }
            else if (control.Id == txtSertifikat.ClientID)
            {
                Session["provera-statusa-zahteva-txtSertifikatIsRequired"] = control.IsRequired;
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
            if (control.Id == ddlListaSertifikata.ClientID)
            {
                Session["provera-statusa-zahteva-ddlListaSertifikataIsRequired"] = control.IsRequired;
            }
        }
    }

    //---------------------------------------------------------------
    //---------------------------------------------------------------

    protected void cvbrojzahteva_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage = string.Empty;
            args.IsValid = UtilsValidation.ValidateBrojZahteva(txtbrojzahteva.Text, Convert.ToBoolean(Session["provera-statusa-zahteva-txtbrojzahtevaIsRequired"]), Convert.ToBoolean(Session["provera-statusa-zahteva-TurnOnRequestNumberValidation"]), out ErrorMessage);
            cvbrojzahteva.ErrorMessage = ErrorMessage;
            
            /*
            if (txtbrojzahteva.Text != string.Empty)
            {
                string newRequest = txtbrojzahteva.Text;
                string errMessage = string.Empty;
                string requestformat = string.Empty;
                args.IsValid = ValidateRequest(newRequest, out errMessage, out requestformat);
                cvbrojzahteva.ErrorMessage = errMessage;
                txtbrojzahteva.Text = requestformat;
                errLabel.Text = string.Empty;
            }
            */           
        }
        catch (Exception)
        {
            cvbrojzahteva.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    public bool ValidateRequest(string newRequest, out string ErrorMessage1, out string requestformat)
    {
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        requestformat = newRequest;
        int ValueRequest = Convert.ToInt32(newRequest);
        Utility utility = new Utility();

        try
        {
            if (ValueRequest > Constants.REQUEST_NUMBER)
            {
                ErrorMessage1 = string.Empty;
                returnValue = true;
                requestformat = ValueRequest.ToString();
            }
            else
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2351) + Constants.REQUEST_NUMBER + ".";
                returnValue = false;
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while ValidateRequest. " + ex.Message);
        }

        return returnValue;
    }

    protected void cvlistasertifikata_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string ddlListaSertifikataString = ddlListaSertifikata.ClientID;
            string IDItem1 = string.Empty;
            SetUpDefaultItem(ddlListaSertifikataString, out IDItem1);

            args.IsValid = UtilsValidation.ValidateListaSertifikata(ddlListaSertifikata.SelectedValue, Convert.ToBoolean(Session["provera-statusa-zahteva-ddlListaSertifikataIsRequired"]), IDItem1, out ErrorMessage1);
            cvlistasertifikata.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvlistasertifikata.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    protected void cvstatus_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage1 = string.Empty;
            string nameformat = string.Empty;
            args.IsValid = UtilsValidation.ValidateReadOnlyFields(txtstatus.Text, Convert.ToBoolean(Session["provera-statusa-zahteva-txtstatusIsRequired"]), out ErrorMessage1);
            cvstatus.ErrorMessage = ErrorMessage1;
        }
        catch (Exception)
        {
            cvstatus.ErrorMessage = string.Empty;
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

    protected void cvsertifikat_ServerValidate(object source, ServerValidateEventArgs args)
    {
        try
        {
            string ErrorMessage = string.Empty;
            args.IsValid = UtilsValidation.ValidateSertifikat(txtSertifikat.Text, Convert.ToBoolean(Session["provera-statusa-zahteva-txtSertifikatIsRequired"]), out ErrorMessage);
            cvsertifikat.ErrorMessage = ErrorMessage;
        }
        catch (Exception)
        {
            cvsertifikat.ErrorMessage = string.Empty;
            args.IsValid = false;
        }
    }

    public List<CertificateRequestStatusPostExpressID> Certificates;
    public List<CertificateStatus> Statuses;
    public string Item;
    public string ItemTextEnglish;
    public string Notification;
    public int ItemValue;

    protected void btnSubmit_Click1(object sender, EventArgs e)
    {
        Utility utility = new Utility();
        try
        {
            if (Page.IsValid)
            {
                try
                {
                    txtSertifikat.Text = string.Empty;
                    txtstatus.Text = string.Empty;
                    //-----------------GetUserAgent string---------------------------
                    /*
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
                    */
                    Session["provera-statusa-zahteva-userAgentBrowser"] = userAgentBrowser;
                    Session["provera-statusa-zahteva-userAgentStringApplicant"] = userAgentStringApplicant;
                    Session["provera-statusa-zahteva-userAgentOS"] = userAgentOS;
                    Session["provera-statusa-zahteva-userAgentIP"] = userAgentIP;
                    Session["provera-statusa-zahteva-userAgentContinent"] = userAgentContinent;
                    Session["provera-statusa-zahteva-userAgentCountry"] = userAgentCountry;
                    Session["provera-statusa-zahteva-userAgentCountryCode"] = userAgentCountryCode;
                    Session["provera-statusa-zahteva-userAgentCity"] = userAgentCity;
                    Session["provera-statusa-zahteva-userAgentISP"] = userAgentISP;
                    
                    log.Debug("Start sending first SOAP message with requestNumber.");

                    BxSoapEnvelope envelope = new BxSoapEnvelopeRequestStatus();

                    envelope.BxData.setValue(@"requestNumber", txtbrojzahteva.Text);
                    //todo samo zameni
                    //envelope.BxData.setValue(@"requestNumber", "2"); //fiksirano za test

                    string SOAPresponse = string.Empty;
                    try
                    {
                        SOAPresponse = BxSoap.SOAPManual(envelope.createBxSoapEnvelope());
                        log.Debug("Response is: " + SOAPresponse);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error. Response from BlueX is: " + SOAPresponse + ex.Message);
                    }

                    Certificates = Utils.ParseSoapEnvelopeRequestStatusPostExpressID(SOAPresponse);

                    if (Certificates == null)
                    {
                        throw new Exception("Error. Certificates List is null. " + SOAPresponse);
                    }
                    //todo za test
                    //Certificates.Add(new CertificateRequestStatus(8, "Production/Qualified Electronic Certificate Legal Enitity.xml-dev", "tokenContractConcluded"));
                    //Certificates.Add(new CertificateRequestStatus(9, "Production/Qualified Electronic Certificate Foreigner Legal Enitity.xml-dev", "tokenOrderPayed"));
                    Session["provera-statusa-zahteva-Certificates"] = Certificates;

                    log.Debug("Successfully send first SOAP message with requestNumber.");

                    ddlListaSertifikata.Items.Clear();

                    if (Certificates.Count > 1)
                    {
                        Container0.Visible = false;
                        Container00.Visible = true;
                        Container000.Visible = true;

                        List<String> certificates = new List<String>();
                        foreach (var certificate in Certificates)
                        {
                            string CertificateTypeSrb = string.Empty;
                            SetUpCertificateTypeSrb(certificate.Type, out CertificateTypeSrb);
                            certificates.Add(CertificateTypeSrb);
                            txtPostExpressBroj.Text = certificate.PostExpressID;
                        }
                
                        ddlListaSertifikata.Items.Insert(0, utility.getItemText(Constants.DefaultIdItemLegal));
                        ddlListaSertifikata.DataSource = certificates;
                        ddlListaSertifikata.DataBind();
                    }
                    else if (Certificates.Count == 1)
                    {
                        Container0.Visible = true;
                        Container00.Visible = false;
                        Container000.Visible = true;
                        Container1.Visible = true;

                        foreach (var certificate in Certificates)
                        {
                            string CertificateTypeSrb = string.Empty;
                            SetUpCertificateTypeSrb(certificate.Type, out CertificateTypeSrb);
                            txtSertifikat.Text = CertificateTypeSrb;
                            txtPostExpressBroj.Text = certificate.PostExpressID;

                            SetUpCertificateStatusVariables(Constants.ITEM_CHALLENGE_RESPONSE, Constants.ITEM_STATUS_CHECK, certificate.Status, out Item, out ItemTextEnglish, out Notification, out ItemValue);

                            switch (ItemValue)
                            {
                                case 0:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 1:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 2:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 3:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 4:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 5:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 6:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 7:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 8:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 9:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 10:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 11:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 12:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 13:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 14:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 15:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 16:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 17:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 18:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 19:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 20:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 21:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 22:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 23:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 24:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 25:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 26:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                case 27:
                                    SetUpCertificateStatus(Item, Notification, ItemValue);
                                    break;
                                default:
                                    //
                                    break;
                            }
                        }
                    }
                    else
                    {
                        log.Debug("Za navedeni broj zahteva " + txtbrojzahteva.Text + "nema statusa.");
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Notification();", true);
                    }
                    ScriptManager.RegisterStartupScript(this, GetType(), "successalert", "successalert();", true);
                }
                catch (Exception ex)
                {
                    log.Debug("Za navedeni broj zahteva " + txtbrojzahteva.Text + "nema statusa. " + ex.Message);
                    txtstatus.Text = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3373);
                    txtstatus.ForeColor = System.Drawing.Color.Red;
                    Container000.Visible = false;
                    Container00.Visible = false;
                    Container0.Visible = false;
                    ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Notification();", true);
                    //log.Error("Error while sending request. " + ex.Message);
                    //ScriptManager.RegisterStartupScript(this, GetType(), "erroralertSendSOAP", "erroralertSendSOAP();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "Disable", "DisableCalendar();", true);
                }
            }
            else if (!Page.IsValid)
            {
                Container000.Visible = false;
                Container00.Visible = false;
                Container0.Visible = false;
                txtstatus.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, GetType(), "erroralert", "erroralert();", true);
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while sending SOAP message. " + ex.Message);
            ScriptManager.RegisterStartupScript(this, GetType(), "errorSOAPalert", "errorSOAPalert();", true);
        }
    }

    protected void SetUpCertificateStatus(string Item, string Notification, int ItemValue)
    {
        txtstatus.Text = Item;
        Container1.Visible = true;

        if (ItemValue == Constants.ITEM_VALUE_REFUSED)
        {
            txtstatus.ForeColor = System.Drawing.Color.Red;
        }
        else if (ItemValue == Constants.ITEM_VALUE_REJECTED)
        {
            txtstatus.ForeColor = System.Drawing.Color.Red;
        }
        else
        {
            txtstatus.ForeColor = System.Drawing.Color.Green;
        }
    }

    //-----------------SetUp CertificateStatus Variables---------------------------
    //-----------------------------------------------------------------------------
    protected void SetUpCertificateStatusVariables(int IDTypeOfItem, int IDTypeOfItem14, string CertificateStatus, out string Item, out string ItemTextEnglish, out string Notification, out int ItemValue)
    {
        Item = string.Empty;
        ItemTextEnglish = string.Empty;
        Notification = string.Empty;
        ItemValue = 0;

        Utility utility = new Utility();
        Statuses = utility.pronadjiPromenljiveStatusSertifikata(IDTypeOfItem);

        foreach (var status in Statuses)
        {
            if (status.ItemTextEnglish == CertificateStatus)
            {
                Item = status.Item;
                Notification = status.Notification;
                ItemValue = status.ItemValue;
            }
            else
            {
            }
        }

        if (Notification == string.Empty)
        {
            Statuses = utility.pronadjiPromenljiveStatusSertifikata(IDTypeOfItem14);

            foreach (var status in Statuses)
            {
                if (status.ItemTextEnglish == CertificateStatus)
                {
                    Item = status.Item;
                    Notification = status.Notification;
                    ItemValue = status.ItemValue;
                }
                else
                {
                }
            }
        }
    }
    //---------------------------------------------------------------
    //---------------------------------------------------------------

    protected void SetUpCertificateTypeSrb(string certificateType, out string certificateTypeSrb)
    {
        certificateTypeSrb = string.Empty;
        Utility utility = new Utility();

        certificateTypeSrb = utility.getItemTextSatus(certificateType);       
    }

    protected void SetUpCertificateTypeEng(string selectedvalue, out string certificateTypeEng)
    {
        certificateTypeEng = string.Empty;
        Utility utility = new Utility();

        certificateTypeEng = utility.getItemTextEnglishSatus(selectedvalue);
    }

    protected void ddlListaSertifikata_SelectedIndexChanged(object sender, EventArgs e)
    {
        Utility utility = new Utility();
        string SelectedValue = ddlListaSertifikata.SelectedItem.Text;
        string FullNameDefault = utility.getItemText(Constants.DefaultIdItemLegal);

        string CertificateTypeEng = string.Empty;
        SetUpCertificateTypeEng(SelectedValue, out CertificateTypeEng);

        Certificates = (List<CertificateRequestStatusPostExpressID>)Session["provera-statusa-zahteva-Certificates"];

        foreach (var certificate in Certificates)
        {
            if (SelectedValue == FullNameDefault)
            {
                Container1.Visible = true;
                txtstatus.Text = string.Empty;
            }
            else if (certificate.Type == CertificateTypeEng)
            {
                string CertificateTypeSrb = string.Empty;
                SetUpCertificateTypeSrb(certificate.Type, out CertificateTypeSrb);
                txtSertifikat.Text = CertificateTypeSrb;

                SetUpCertificateStatusVariables(Constants.ITEM_CHALLENGE_RESPONSE, Constants.ITEM_STATUS_CHECK, certificate.Status, out Item, out ItemTextEnglish, out Notification, out ItemValue);

                switch (ItemValue)
                {
                    case 0:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 1:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 2:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 3:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 4:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 5:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 6:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 7:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 8:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 9:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 10:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 11:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 12:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 13:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 14:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 15:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 16:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 17:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 18:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 19:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 20:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 21:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 22:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 23:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 24:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 25:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 26:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    case 27:
                        SetUpCertificateStatus(Item, Notification, ItemValue);
                        break;
                    default:
                        //
                        break;
                }
            }
        }
    }
}