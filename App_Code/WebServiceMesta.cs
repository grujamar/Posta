using log4net;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;


/// <summary>
/// Summary description for WebServiceMesta
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]

public class WebServiceMesta : System.Web.Services.WebService {

    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public WebServiceMesta () 
    {
        
    }

    public List<Naselje> Naselja { get; set; }
    public List<Ulica> Ulice { get; set; }
    public List<Rezultat> Rezultati { get; set; }

    [WebMethod(EnableSession = true)]
    public string[] GetMesta(string prefixText, int count)
    {
        List<string> responses = new List<string>();
        try
        {              
            rs.posta.wsp.WSPWrapperService obj = new rs.posta.wsp.WSPWrapperService();

            Guid guid1 = Guid.NewGuid();
            int servis = 3;
            int idVrstaTransakcije = 3; //GetNaselje = 3 – Spisak naselja koji u sebi sadrže prosleđeni tekst

            // Serializaion  
            Klijent klijent = new Klijent();
                klijent.Username = "cepp";
                klijent.Password = "c3pp";
                klijent.Jezik = "LAT";
                klijent.IdTipUredjaja = 1;
                klijent.ModelUredjaja = "Test";
                klijent.NazivUredjaja = "Test";
                klijent.VerzijaOS = "Test";
                klijent.VerzijaAplikacije = "Test";
                klijent.IPAdresa = "Test";
                klijent.Geolokacija = "Test";

            // Convert Klijent object to XML string format   
            string xmlData = Serijalizator.SerializeObject<Klijent>(klijent);

            // Serializaion
            GetNaseljeIn getnaseljein = new GetNaseljeIn();
            getnaseljein.Naziv = prefixText;
            getnaseljein.BrojRezultata = count;
            getnaseljein.PoredjenjePoDeluNaziva = true;

            // Convert GetNaseljeIn object to XML string format   
            string xmlData2 = Serijalizator.SerializeObject<GetNaseljeIn>(getnaseljein);

            string objStrKlijentString = xmlData;
            string objGetNaseljeIn = xmlData2;
            string GetNaseljeOutString = string.Empty;
            string Rezultat = string.Empty;

            //Funkcija koja poziva Web servis
            int result = obj.Transakcija(objStrKlijentString, servis, idVrstaTransakcije, guid1, objGetNaseljeIn, out GetNaseljeOutString, out Rezultat);

            //deserialize
            GetNaseljeOut getNaseljeOut = new GetNaseljeOut();
            getNaseljeOut = Serijalizator.Deserialize<GetNaseljeOut>(GetNaseljeOutString);

            string returnString = string.Empty;// Serijalizator.SerializeObject<List<Naselje>>(naselja);
            
            Naselja = new List<Naselje>();

            if (GetNaseljeOutString == null)
            {
                //deserialize
                Rezultat rezultatout = new Rezultat();
                rezultatout = Serijalizator.Deserialize<Rezultat>(Rezultat);

                responses.Add(rezultatout.PorukaKorisnik);

            }
            else if (GetNaseljeOutString != null)
            { 
                foreach (var naselje in getNaseljeOut.Naselja)
                {
                    //returnString += naselje.Naziv + ", ";
                    Naselja.Add(naselje);
                    responses.Add(naselje.Naziv);                
                }
            }
            //Store and retreive values between web methods, with Session
            Context.Session["WebServiceMesta-Naselja"] = Naselja;
            
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetMesta. " + ex.Message);
        }

        return responses.ToArray();
    }

    //contextKey is using for passing parameter from AjaxCompleteExtender2(UseContextKey = "true") and AjaxCompleteExtender1(OnClientItemSelected="getSelected" Javascript function)

    [WebMethod(EnableSession = true)]
    public string[] GetUlica(string prefixText, int count, string contextKey)
    {
        List<string> responses = new List<string>();

        try
        {
            rs.posta.wsp.WSPWrapperService obj = new rs.posta.wsp.WSPWrapperService();

            Guid guid1 = Guid.NewGuid();
            int servis = 3;
            int idVrstaTransakcije = 4; //GetUlica = 4 – Spisak ulica za naselje koje u sebi sadrže prosleđeni tekst

            // Serializaion  
            Klijent klijent = new Klijent();
            klijent.Username = "cepp";
            klijent.Password = "c3pp";
            klijent.Jezik = "LAT";
            klijent.IdTipUredjaja = 1;
            klijent.ModelUredjaja = "Test";
            klijent.NazivUredjaja = "Test";
            klijent.VerzijaOS = "Test";
            klijent.VerzijaAplikacije = "Test";
            klijent.IPAdresa = "Test";
            klijent.Geolokacija = "Test";

            // Convert Klijent object to XML string format   
            string xmlData = Serijalizator.SerializeObject<Klijent>(klijent);

            // Serializaion
            GetUlicaIn getulicain = new GetUlicaIn();
            getulicain.IdNaselje = getIdNaselje(contextKey);
            getulicain.Naziv = prefixText;
            getulicain.BrojRezultata = count;
            getulicain.PoredjenjePoDeluNaziva = true;

            // Convert GetNaseljeIn object to XML string format   
            string xmlData2 = Serijalizator.SerializeObject<GetUlicaIn>(getulicain);

            string objStrKlijentString = xmlData;
            string objGetNaseljeIn = xmlData2;
            string GetUliceOutString = string.Empty;
            string Rezultat = string.Empty;

            //Funkcija koja poziva Web servis
            int result = obj.Transakcija(objStrKlijentString, servis, idVrstaTransakcije, guid1, objGetNaseljeIn, out GetUliceOutString, out Rezultat);

            //deserialize
            GetUlicaOut getUliceOut = new GetUlicaOut();
            getUliceOut = Serijalizator.Deserialize<GetUlicaOut>(GetUliceOutString);

            //string returnString = string.Empty;
            
            Ulice = new List<Ulica>();

            if (GetUliceOutString == null)
            {
                //deserialize
                Rezultat rezultatout = new Rezultat();
                rezultatout = Serijalizator.Deserialize<Rezultat>(Rezultat);

                responses.Add(rezultatout.PorukaKorisnik);

            }
            else if (GetUliceOutString != null)
            {
                foreach (var ulica in getUliceOut.Ulice)
                {
                    //returnString += ulica.Naziv + ", ";
                    Ulice.Add(ulica);
                    responses.Add(ulica.Naziv);
                }
            }
            //Store and retreive values between web methods, with Session
            Context.Session["WebServiceMesta-Ulice"] = Ulice;
            Context.Session["WebServiceMesta-contextKey"] = contextKey;           
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetUlica. " + ex.Message);
        }
        return responses.ToArray();
    }

    private int getIdNaselje(string nazivNaselja)
    {
        try
        {
            //Store and retreive values between web methods, with Session
            List<Naselje> Naselja = (List<Naselje>)Context.Session["WebServiceMesta-Naselja"];
            foreach (var naselje in Naselja)
            {
                if (nazivNaselja.Equals(naselje.Naziv, StringComparison.InvariantCultureIgnoreCase))
                {
                    return naselje.Id;
                }
            }           
        }
        catch (Exception ex)
        {
            log.Error("Error in function getIdNaselje. " + ex.Message);
        }
        return 0;
    }


    private int getIdUlice(string nazivUlice)
    {
        try
        {
            //Store and retreive values between web methods, with Session
            List<Ulica> Ulice = (List<Ulica>)Context.Session["WebServiceMesta-Ulice"];
            foreach (var ulice in Ulice)
            {
                if (nazivUlice.Equals(ulice.Naziv, StringComparison.InvariantCultureIgnoreCase))
                {
                    return ulice.Id;
                }
            }          
        }
        catch (Exception ex)
        {
            log.Error("Error in function getIdUlice. " + ex.Message);
        }
        return 0;
    }

    /*
    [WebMethod(EnableSession = true)]
    public string[] GetPostanskiBroj(string prefixText, int count, string contextKey)
    {
        try
        {
            //////////////////PROVERA ADRESE  //////////////1 pošiljalac  29 (Danas za sutra do 12)////////////////////////
            rs.posta.wsp.WSPWrapperService obj = new rs.posta.wsp.WSPWrapperService();

            Guid guid1 = Guid.NewGuid();
            int servis = 3;
            int idVrstaTransakcije = 6; //ProveraAdrese = 6 – Provera ispravnosti adrese

            // Serializaion  
            Klijent klijent = new Klijent();
            klijent.Username = "cepp";
            klijent.Password = "c3pp";
            klijent.Jezik = "LAT";
            klijent.IdTipUredjaja = 1;
            klijent.ModelUredjaja = "Test";
            klijent.NazivUredjaja = "Test";
            klijent.VerzijaOS = "Test";
            klijent.VerzijaAplikacije = "Test";
            klijent.IPAdresa = "Test";
            klijent.Geolokacija = "Test";

            // Convert Klijent object to XML string format   
            string xmlData = Serijalizator.SerializeObject<Klijent>(klijent);

            // Serializaion
            ProveraAdreseIn proveraadresein = new ProveraAdreseIn();
            proveraadresein.TipAdrese = 1; //0 preuzimanje, 1 pošiljalac, 2 primalac
            proveraadresein.IdRukovanje = 1; //1 (Pismonosna dostava)  29 (Danas za sutra do 12)
            string contextKeyNaselje = Context.Session["contextKey"].ToString();
            proveraadresein.IdNaselje = getIdNaselje(contextKeyNaselje);
            string contextKeyUlica = contextKey;
            proveraadresein.IdUlica = getIdUlice(contextKeyUlica);
            proveraadresein.BrojPodbroj = prefixText;
            proveraadresein.Posta = "";

            // Convert  object to XML string format   
            string xmlData2 = Serijalizator.SerializeObject<ProveraAdreseIn>(proveraadresein);

            string objStrKlijentString = xmlData;
            string objProveraAdreseIn = xmlData2;
            string ProveraAdreseOutString = string.Empty;
            string Rezultat = string.Empty;

            //Funkcija koja poziva Web servis
            int result = obj.Transakcija(objStrKlijentString, servis, idVrstaTransakcije, guid1, objProveraAdreseIn, out ProveraAdreseOutString, out Rezultat);

            List<string> responses = new List<string>();

            if (ProveraAdreseOutString == null)
            {
                //deserialize
                Rezultat rezultatout = new Rezultat();
                rezultatout = Serijalizator.Deserialize<Rezultat>(Rezultat);

                responses.Add(rezultatout.PorukaKorisnik);
            }
            else if (ProveraAdreseOutString != null)
            { 
                    //deserialize
               ProveraAdreseOut proveraadreseout = new ProveraAdreseOut();
               proveraadreseout = Serijalizator.Deserialize<ProveraAdreseOut>(ProveraAdreseOutString);

               responses.Add(proveraadreseout.PostanskiBroj);

               Context.Session["WebServiceMesta-PAK"] = proveraadreseout.Pak;
            }
            return responses.ToArray();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    */
    /*
    [WebMethod(EnableSession = true)]
    public string[] GetPAK(string prefixText, int count, string contextKey)
    {

        try
        {
            List<string> responses = new List<string>();

            responses.Add(Session["WebServiceMesta-PAK"].ToString());

            return responses.ToArray();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    */
}