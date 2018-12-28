using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class zahtev_izdavanje_fizicko_lice_stranac_bez_JMBG_podnet : System.Web.UI.Page
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
            if (Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-brojzahteva"] != null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "requestsend();", true);
                txtbrojzahteva.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-brojzahteva"].ToString();
                txtdatumzahteva.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-datumzahteva"].ToString();
                txtcenasaporezom.Text = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-cenasaporezom"].ToString();

                string NavigateUrlFilename = Utils.ConvertToTildaPath(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-filename"].ToString());
                btnPrintRequest.NavigateUrl = @NavigateUrlFilename;
                string NavigateUrlFilenamePaymentOrder = Utils.ConvertToTildaPath(Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-filenamePaymentOrder"].ToString());
                btnPrintPaymentOrder.NavigateUrl = @NavigateUrlFilenamePaymentOrder;
            }
            else
            {
                Response.Redirect("zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG.aspx", false); // this will tell .NET framework not to stop the execution of the current thread and hence the error will be resolved.
            }
        }
    }

    protected void btnPrintRequest_Click(object sender, EventArgs e)
    {
        try
        {
            /*
            string pdfSaveAsFileName = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-filename"].ToString();
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(pdfSaveAsFileName) + "");
            Response.TransmitFile(pdfSaveAsFileName);
            */
            Page page = (Page)HttpContext.Current.Handler;
            Utils.DownloadPDF(page, Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-filename"].ToString());
        }
        catch (Exception ex)
        {
            log.Error("Error while downloading PDF file. " + ex.Message);
        }

    }

    protected void btnPrintPaymentOrder_Click(object sender, EventArgs e)
    {
        try
        {
            /*
            string pdfSaveAsFileNamePaymentOrder = Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-filenamePaymentOrder"].ToString();
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(pdfSaveAsFileNamePaymentOrder) + "");
            Response.TransmitFile(pdfSaveAsFileNamePaymentOrder);
            */
            Page page = (Page)HttpContext.Current.Handler;
            Utils.DownloadPDF(page, Session["zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG-filenamePaymentOrder"].ToString());
        }
        catch (Exception ex)
        {
            log.Error("Error while downloading PDF file Payment Order. " + ex.Message);
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

                    //tekstualnopolje.Enabled = control.IsEnabled;
                    //tekstualnopolje.Visible = control.IsVisible;
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
}