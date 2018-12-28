using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            /*
            string page1 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("zahtev-izdavanje-fizicko-lice.aspx", Constants.CryptKey, Constants.AuthKey);
            page1 = page1.Replace("+", "%252b");
            HyperLink1.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page1);

            string page2 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("zahtev-izdavanje-fizicko-lice-stranac-bez-JMBG.aspx", Constants.CryptKey, Constants.AuthKey);
            page2 = page2.Replace("+", "%252b");
            HyperLink2.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page2);

            string page3 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("zahtev-izdavanje-pravno-lice.aspx", Constants.CryptKey, Constants.AuthKey);
            page3 = page3.Replace("+", "%252b");
            HyperLink3.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page3);

            string page4 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("zahtev-izdavanje-pravno-lice-stranac-bez-JMBG.aspx", Constants.CryptKey, Constants.AuthKey);
            page4 = page4.Replace("+", "%252b");
            HyperLink4.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page4);

            string page5 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("zahtev-promena-statusa-sertifikata.aspx", Constants.CryptKey, Constants.AuthKey);
            page5 = page5.Replace("+", "%252b");
            HyperLink5.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page5);

            string page6 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("Zahtev-promena-statusa.aspx", Constants.CryptKey, Constants.AuthKey);
            page6 = page6.Replace("+", "%252b");
            HyperLink6.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page6);

            string page7 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("Zahtev-dobijanje-koda-deblokada-kartice.aspx", Constants.CryptKey, Constants.AuthKey);
            page7 = page7.Replace("+", "%252b");
            HyperLink7.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page7);

            string page8 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("Zahtev-provera-statusa.aspx", Constants.CryptKey, Constants.AuthKey);
            page8 = page8.Replace("+", "%252b");
            HyperLink8.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page8);

            string page9 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("zahtev-provera-datuma-isticanja-sertifikata.aspx", Constants.CryptKey, Constants.AuthKey);
            page9 = page9.Replace("+", "%252b");
            HyperLink9.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page9);

            string page10 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("provera-opozvanosti-sertifikata.aspx", Constants.CryptKey, Constants.AuthKey);
            page10 = page10.Replace("+", "%252b");
            HyperLink10.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page10);

            string page12 = AuthenticatedEncryption.AuthenticatedEncryption.Encrypt("Preuzimanje-softverskog-sertifikata-PKCS12.aspx", Constants.CryptKey, Constants.AuthKey);
            page12 = page12.Replace("+", "%252b");
            HyperLink12.NavigateUrl = string.Format("~/Uputstvo.aspx?d={0}", page12);
            */
        }
    }
}