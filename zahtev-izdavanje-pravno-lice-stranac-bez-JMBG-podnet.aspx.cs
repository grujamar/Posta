using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class zahtev_izdavanje_pravno_lice_stranac_bez_JMBG_podnet : System.Web.UI.Page
{
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

        if (!Page.IsPostBack)
        {
            if (Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-brojzahteva"] != null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "requestsend();", true);
                txtbrojzahteva.Text = Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-brojzahteva"].ToString();
                txtdatumzahteva.Text = Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-datumzahteva"].ToString();
                txtcenasaporezom.Text = Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-cenasaporezom"].ToString();

                string NavigateUrlContractLegalEntity = Utils.ConvertToTildaPath(Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename1"].ToString());
                btnContractLegalEntity.NavigateUrl = @NavigateUrlContractLegalEntity;
                string NavigateUrlContractState = Utils.ConvertToTildaPath(Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename2"].ToString());
                btnContractState.NavigateUrl = @NavigateUrlContractState;
                string NavigateUrlContractAtachment = Utils.ConvertToTildaPath(Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename"].ToString());
                btnContractAtachment.NavigateUrl = @NavigateUrlContractAtachment;
                try
                {
                    string activityCode = utility.getItemTextActivityCode(Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-sifradel"].ToString(), Constants.ACTIVITY_CODE_TRUE);
                    if (activityCode != string.Empty)
                    {
                        //za sifre delatnosti
                        //Preuzmi ugovor za državni organ i Preuzmi prilog ugovora
                        ContractLegalEntity.Visible = false;
                    }
                    else
                    {
                        //za ostale sifre
                        //Preuzmi ugovor za pravno lice i opštinu i Preuzmi prilog ugovora
                        ContractState.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while checking activity code. " + ex.Message);
                }
            }
            else
            {
                Response.Redirect("zahtev-izdavanje-pravno-lice-stranac-bez-JMBG.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
            }
        }
    }

    protected void btnContractLegalEntity_Click(object sender, EventArgs e)
    {
        try
        {
            /*
            string pdfContractLegalEntity = Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename1"].ToString();
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(pdfContractLegalEntity) + "");
            //todo ovo je kada bude bila aplikacija publish-ovana na server
            Response.TransmitFile(pdfContractLegalEntity);
            */
            Page page = (Page)HttpContext.Current.Handler;
            Utils.DownloadPDF(page, Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename1"].ToString());
        }
        catch (Exception ex)
        {
            log.Error("Error while downloading PDF file pdfContractLegalEntity. " + ex.Message);
        }
    }

    protected void btnContractState_Click(object sender, EventArgs e)
    {
        try
        {
            /*
            string pdfContractGovernment = Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename2"].ToString();
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(pdfContractGovernment) + "");
            //todo ovo je kada bude bila aplikacija publish-ovana na server
            Response.TransmitFile(pdfContractGovernment);
            */
            Page page = (Page)HttpContext.Current.Handler;
            Utils.DownloadPDF(page, Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename2"].ToString());
        }
        catch (Exception ex)
        {
            log.Error("Error while downloading PDF file pdfContractGovernment. " + ex.Message);
        }
    }

    protected void btnContractAtachment_Click(object sender, EventArgs e)
    {
        try
        {
            /*
            string pdfContractAtachment = Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename"].ToString();
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(pdfContractAtachment) + "");
            //todo ovo je kada bude bila aplikacija publish-ovana na server
            Response.TransmitFile(pdfContractAtachment);
            */
            Page page = (Page)HttpContext.Current.Handler;
            Utils.DownloadPDF(page, Session["zahtev-izdavanje-pravno-lice-stranac-bez-JMBG-filename"].ToString());
        }
        catch (Exception ex)
        {
            log.Error("Error while downloading PDF file pdfContractAtachment. " + ex.Message);
        }
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
                }
                catch (Exception ex)
                {
                    log.Info("Error while setting control's " + control.Controlid + " text: " + ex.Message);
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

            try
            {
                if (control.ControlTittle.Equals("*"))
                {
                    if (!control.IsVisible)
                    {
                        Label labela = (Label)FindControlRecursive(Page, control.Controlid);
                        labela.Text = "&nbsp;";
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
}