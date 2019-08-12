using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Net;
using System.Data;
using System.Xml;
using log4net;
using System.Xml.XPath;
using System.Security.Cryptography;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for Utils
/// </summary>
public static class Utils
{
    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static string OneOrTwo(string C2)
    {
        string C2prepared;
        if (C2.Substring(0, 1) == "9")
        {
            C2prepared = "1";
        }
        else if (C2.Substring(0, 1) == "0")
        {
            C2prepared = "2";
        }
        else
        {
            C2prepared = "101";
        }
        return C2prepared;
    }

    static object locker = new object();
    public static string Generate15UniqueDigits()
    {
        lock (locker)
        {
            Thread.Sleep(100);
            return DateTime.Now.ToString("yyyyMMddHHmmssf");
        }
    }

    public static string getPreparedNumber(string number)
    {
        addLeadingZeros(ref number, 7);
        return number;
    }

    public static string addLeadingZeros(ref string s, int length)
    {
        if (s.Length < length)
        {
            int addZerosCnt = length - s.Length;
            for (int i = 0; i < addZerosCnt; i++)
            {
                s = "0" + s;
            }
        }
        return s;
    }

    public static string getPreparedAreaNumber(string areanumber)
    {
        addLeadingZerosArea(ref areanumber, 3);
        return areanumber;
    }

    public static string addLeadingZerosArea(ref string s, int length)
    {
        if (s.Length < length)
        {
            int addZerosCnt = length - s.Length;
            for (int i = 0; i < addZerosCnt; i++)
            {
                s = "0" + s;
            }
        }
        return s;
    }

    public static string getPreparedPIB(string PIB)
    {
        addLeadingZerosPIB(ref PIB, 8);
        return PIB;
    }

    public static string addLeadingZerosPIB(ref string s, int length)
    {
        if (s.Length < length)
        {
            int addZerosCnt = length - s.Length;
            for (int i = 0; i < addZerosCnt; i++)
            {
                s = "0" + s;
            }
        }
        return s;
    }

    public static string getPreparedEditablePrice(string price)
    {
        addLeadingZerosPrice(ref price, 5);
        return price;
    }

    public static string addLeadingZerosPrice(ref string s, int length)
    {
        if (s.Length < length)
        {
            int addZerosCnt = length - s.Length;
            for (int i = 0; i < addZerosCnt; i++)
            {
                s = "0" + s;
            }
        }
        return s;
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public static bool allowLatinLettersMinusSpaceApostrophe(string InputString)
    {
        try
        {
            //Serbian Latin letter a  b  c	č	ć	d	dž	đ	e	f	g	h	i	j	k   l	lj	m	n	nj	o	p	r	s	š	t	u	v	z	ž
            //* means 'match 0 or any number of characters'.
            Regex regex = new Regex(@"^([a-zA-ZČĆĐŠŽžšđćč '-]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowLatinLettersNumbersDotSpace(string InputString)
    {
        try
        {
            //Serbian Latin letter a  b  c	č	ć	d	dž	đ	e	f	g	h	i	j	k   l	lj	m	n	nj	o	p	r	s	š	t	u	v	z	ž
            //* means 'match 0 or any number of characters'.
            Regex regex = new Regex(@"^([a-zA-Z0-9ČĆĐŠŽžšđćč. ]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowLettersNumbersSpace(string InputString)
    {
        try
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9ČĆĐŠŽžšđćč ]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowLetters(string InputString)
    {
        try
        {
            Regex regex = new Regex(@"^([a-zA-ZČĆĐŠŽžšđćč]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowLettersSpace(string InputString)
    {
        try
        {
            Regex regex = new Regex(@"^([a-zA-ZČĆĐŠŽžšđćč ]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowLettersSpaceBracketsLines(string InputString)
    {
        try
        {
            Regex regex = new Regex(@"^([a-zA-ZČĆĐŠŽžšđćč ()-]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowNumbers(string InputString)
    {
        try
        {
            Regex regex = new Regex(@"^([0-9]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowLettersNumbersDotMinusSpace(string InputString)
    {
        try
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9ČĆĐŠŽžšđćč .-]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowLettersNumbersMinusSlashSpace(string InputString)
    {
        try
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9\/ČĆĐŠŽžšđćč -]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool allowHEXLettersNumbers(string InputString)
    {
        try
        {
            Regex regex = new Regex(@"^([a-fA-F0-9]*)$");
            Match match = regex.Match(InputString);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool IsValidEmailId(string InputEmail)
    {
        try
        {
            //Regex To validate Email Address
            Regex regex = new Regex("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$");
            Match match = regex.Match(InputEmail);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool IsValidURL(string InputURL)
    {
        try
        {
            //Regex To validate URL https://www.regextester.com/94502   https://mathiasbynens.be/demo/url-regex
            Regex regex = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
            Match match = regex.Match(InputURL);
            if (match.Success)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-----------------------------------------------------GetUserAgent string-------------------------------
    public static String GetIP(out string externalIP)
    {
        try
        {
            WebClient client = new WebClient();
            externalIP = client.DownloadString("http://checkip.dyndns.org/");
            externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                         .Matches(externalIP)[0].ToString();
            return externalIP;
        }
        catch 
        {
            return externalIP = string.Empty;
        }
       
    }

    public static String GetIPAddress()
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;
        string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length != 0)
            {
                return addresses[0];
            }
        }
        return context.Request.ServerVariables["REMOTE_ADDR"];
    }

    public static void GetLocation(string IPAddress, out string Continent, out string Country, out string CountryCode, out string City, out string ISP)
    {
        Continent = string.Empty;
        Country = string.Empty;
        CountryCode = string.Empty;
        City = string.Empty;
        ISP = string.Empty;

        if (IPAddress != string.Empty)
        {
            //Create a WebRequest with the current Ip
            //WebRequest _objWebRequest = WebRequest.Create("http://www.freegeoip.net/xml/" + IPAddress);
            WebRequest _objWebRequest = WebRequest.Create("http://ip-api.com/xml/" + IPAddress);

            //Create a Web Proxy
            //WebProxy _objWebProxy = new WebProxy("http://www.freegeoip.net/xml/" + IPAddress, true);
            WebProxy _objWebProxy = new WebProxy("http://ip-api.com/xml/" + IPAddress, true);

            //Assign the proxy to the WebRequest
            _objWebRequest.Proxy = _objWebProxy;

            //Set the timeout in Seconds for the WebRequest
            _objWebRequest.Timeout = 2000;

            try
            {
                string Continent1 = string.Empty;
                string Country1 = string.Empty;
                string CountryCode1 = string.Empty;
                string City1 = string.Empty;
                string ISP1 = string.Empty;
                //Get the WebResponse 
                WebResponse _objWebResponse = _objWebRequest.GetResponse();
                //Read the Response in a XMLTextReader
                XmlTextReader _objXmlTextReader = new XmlTextReader(_objWebResponse.GetResponseStream());

                //Create a new DataSet
                DataSet _objDataSet = new DataSet();
                //Read the Response into the DataSet
                _objDataSet.ReadXml(_objXmlTextReader);

                foreach (DataTable table in _objDataSet.Tables)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        if (dr["status"].ToString() == "success")
                        {
                            Continent1 = dr["timezone"].ToString();
                            Country1 = dr["country"].ToString(); ;
                            CountryCode1 = dr["countryCode"].ToString();
                            City1 = dr["city"].ToString();
                            ISP1 = dr["isp"].ToString();
                        }
                        else if (dr["status"].ToString() == "fail")
                        {
                            log.Error("Error in GetLocation. Failed request return: " + dr["message"].ToString());
                        }
                    }
                }
                Continent = Continent1;
                Country = Country1;
                CountryCode = CountryCode1;
                City = City1;
                ISP = ISP1;
            }
            catch (Exception ex)
            {
                log.Error("Error while getting IP location, failed request from geo IP-API service return. " + ex.Message);
            }
        }
        else if (IPAddress == string.Empty)
        {
            Continent = string.Empty;
            Country = string.Empty;
            CountryCode = string.Empty;
            City = string.Empty;
        }

    } // End of GetLocation	


    public static string getOS(string RequestUserAgent, out string os)
    {
        if (RequestUserAgent.IndexOf("Windows NT 10.0") > 0)
        {
            os = "Windows 10";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 5.1") > 0)
        {
            os = "Windows XP";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 5.01") > 0)
        {
            os = "Windows 2000, Service Pack 1 (SP1)";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 5.0") > 0)
        {
            os = "Windows 2000";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 4.0") > 0)
        {
            os = "Microsoft Windows NT 4.0";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Win 9x 4.90") > 0)
        {
            os = "Windows Millennium Edition (Windows Me)";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows 98") > 0)
        {
            os = "Windows 98";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows 95") > 0)
        {
            os = "Windows 95";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows CE") > 0)
        {
            os = "Windows CE";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 5.2") > 0)
        {
            os = "Windows Server 2003; Windows XP x64 Edition";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 6.0") > 0)
        {
            os = "Windows Vista";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 6.1") > 0)
        {
            os = "Windows 7";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 6.2") > 0)
        {
            os = "Windows 8";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Windows NT 6.3") > 0)
        {
            os = "Windows 8.1";
            return os;
        }
        else if (RequestUserAgent.IndexOf("Intel Mac OS X") > 0)
        {
            //os = "Mac OS or older version of Windows";
            os = "Intel Mac OS X";
            return os;
        }
        else
        {
            os = "You are using older version of Windows or Mac OS";
            return os;
        }

    }
    //-------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------

    public static void ParseSoapEnvelope(string soapresponse, out string brojzahteva, out string brojzahtevapravnolice)
    {
        brojzahteva = string.Empty;
        brojzahtevapravnolice = string.Empty;
        string brojzahtevaPrepared = string.Empty;
        string brojzahtevapravnolicePrepared = string.Empty;

        try
        {
            //todo test: parse response SOAP message
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapresponse);  //loading soap message as string

            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Envelope
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Header
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Body

            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//bx:BlueXMLRequestMessage
                if (navigator.Name == "bx:BlueXMLRequestMessage")
                {
                    if (navigator.HasChildren)
                    {
                        navigator.MoveToFirstChild();//bx:data

                        if (navigator.HasChildren)
                        {
                            navigator.MoveToFirstChild();//certificate
                            do
                            {
                                if (navigator.HasChildren)
                                {
                                    navigator.MoveToFirstChild(); //bx:value name="requestNumber">
                                    brojzahtevaPrepared = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    brojzahtevapravnolicePrepared = navigator.Value;

                                    navigator.MoveToParent();
                                }
                            } while (navigator.MoveToNext());
                        }
                    }
                }
            }

            brojzahteva = brojzahtevaPrepared;
            brojzahtevapravnolice = brojzahtevapravnolicePrepared;
        }
        catch (Exception ex)
        {            
            log.Error("Error while parsing SOAP envelope message. " + ex.Message);
            throw new Exception("Error while parsing SOAP envelope message. " + ex.Message, ex.InnerException);
        }
    }

    public static List<CertificateRequestStatus> ParseSoapEnvelopeRequestStatus(string soapresponse)
    {
        List<CertificateRequestStatus> responses = new List<CertificateRequestStatus>();

        try
        {
            //todo test: parse response SOAP message
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapresponse);  //loading soap message as string

            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Envelope
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Header
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Body

            string requestNumberTemp = string.Empty;
            string typeTemp = string.Empty;
            string statusTemp = string.Empty;


            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//bx:BlueXMLRequestMessage
                if (navigator.Name == "bx:BlueXMLRequestMessage")
                {
                    if (navigator.HasChildren)
                    {
                        navigator.MoveToFirstChild();//bx:data

                        if (navigator.HasChildren)
                        {
                            navigator.MoveToFirstChild();//certificate
                            do
                            {
                                if (navigator.HasChildren)
                                {
                                    navigator.MoveToFirstChild(); //bx:value name="requestNumber">
                                    requestNumberTemp = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    typeTemp = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    statusTemp = navigator.Value;

                                    responses.Add(new CertificateRequestStatus(Convert.ToInt32(requestNumberTemp), typeTemp, statusTemp));

                                    navigator.MoveToParent();
                                }
                            } while (navigator.MoveToNext());
                        }
                    }
                }
            }             
        }
        catch (Exception ex)
        {
            log.Error("Error while parsing SOAP envelope message in ParseSoapEnvelopeRequestStatus. " + ex.Message);
            throw new Exception("Error while parsing SOAP envelope message in ParseSoapEnvelopeRequestStatus. " + ex.Message, ex.InnerException);
        }
        return responses;
    }

    public static List<CertificateChallengeResponse> ParseSoapEnvelopeChallengeResponse(string soapresponse)
    {
        List<CertificateChallengeResponse> responses = new List<CertificateChallengeResponse>();

        try
        {
            //todo test: parse response SOAP message
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapresponse);  //loading soap message as string


            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Envelope
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Header
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Body

            string requestno = string.Empty;
            string response = string.Empty;


            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//bx:BlueXMLRequestMessage
                if (navigator.Name == "bx:BlueXMLRequestMessage")
                {
                    if (navigator.HasChildren)
                    {
                        navigator.MoveToFirstChild();//bx:data

                        if (navigator.HasChildren)
                        {
                            navigator.MoveToFirstChild();//certificate
                            do
                            {
                                if (navigator.HasChildren)
                                {
                                    navigator.MoveToFirstChild(); //bx:value name="requestNumber">
                                    requestno = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    response = navigator.Value;

                                    responses.Add(new CertificateChallengeResponse(Convert.ToInt32(requestno), response));

                                    navigator.MoveToParent();
                                }
                            } while (navigator.MoveToNext());
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while parsing SOAP envelope message in ParseSoapEnvelopeChallengeResponse. " + ex.Message);
            throw new Exception("Error while parsing SOAP envelope message in ParseSoapEnvelopeChallengeResponse.  " + ex.Message, ex.InnerException);
        }
        return responses;
    }

    public static List<CertificateStatusCheck> ParseSoapEnvelopeCertificateStatusCheck(string soapresponse)
    {
        List<CertificateStatusCheck> responses = new List<CertificateStatusCheck>();

        try
        {
            //todo test: parse response SOAP message
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapresponse);  //loading soap message as string


            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Envelope
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Header
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Body

            string USI = string.Empty;
            string ValidFrom = string.Empty;
            string ValidTo = string.Empty;
            string GivenName = string.Empty;
            string LastName = string.Empty;
            string response = string.Empty;

            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//bx:BlueXMLRequestMessage
                if (navigator.Name == "bx:BlueXMLRequestMessage")
                {
                    if (navigator.HasChildren)
                    {
                        navigator.MoveToFirstChild();//bx:data

                        if (navigator.HasChildren)
                        {
                            navigator.MoveToFirstChild();//certificate
                            do
                            {
                                if (navigator.HasChildren)
                                {
                                    navigator.MoveToFirstChild(); //bx:value name="requestNumber">
                                    USI = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    ValidFrom = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    ValidTo = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    GivenName = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    LastName = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    response = navigator.Value;

                                    responses.Add(new CertificateStatusCheck(USI, ValidFrom, ValidTo, GivenName, LastName));

                                    navigator.MoveToParent();
                                }
                            } while (navigator.MoveToNext());
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while parsing SOAP envelope message in ParseSoapEnvelopeCertificateStatusCheck. " + ex.Message);
            throw new Exception("Error while parsing SOAP envelope message in ParseSoapEnvelopeCertificateStatusCheck.  " + ex.Message, ex.InnerException);
        }
        return responses;
    }


    public static void ParseSoapEnvelopeStatusChange(string soapresponse, out string USI, out string requestNumber, out string response)
    {
        USI = string.Empty;
        requestNumber = string.Empty;
        response = string.Empty;
        string USIPrepared = string.Empty;
        string requestNumberPrepared = string.Empty;
        string responsePrepared = string.Empty;

        try
        {
            //todo test: parse response SOAP message
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapresponse);  //loading soap message as string

            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Envelope
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Header
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Body

            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//bx:BlueXMLRequestMessage
                if (navigator.Name == "bx:BlueXMLRequestMessage")
                {
                    if (navigator.HasChildren)
                    {
                        navigator.MoveToFirstChild();//bx:data

                        if (navigator.HasChildren)
                        {
                            navigator.MoveToFirstChild();//certificate
                            do
                            {
                                if (navigator.HasChildren)
                                {
                                    navigator.MoveToFirstChild(); //bx:value name="requestNumber">
                                    requestNumberPrepared = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    responsePrepared = navigator.Value;

                                    navigator.MoveToParent();
                                }
                            } while (navigator.MoveToNext());
                        }
                    }
                }
            }
            USI = USIPrepared;
            requestNumber = requestNumberPrepared;
            response = responsePrepared;
        }
        catch (Exception ex)
        {
            log.Error("Error while parsing SOAP envelope message in ParseSoapEnvelopeStatusChange. " + ex.Message);
            throw new Exception("Error while parsing SOAP envelope message in ParseSoapEnvelopeStatusChange.  " + ex.Message, ex.InnerException);
        }
    }


    /// <summary>
    /// Get string value between [first] a and [last] b.
    /// </summary>
    public static string Between(this string value, string a, string b)
    {
        int posA = value.IndexOf(a);
        int posB = value.LastIndexOf(b);
        if (posA == -1)
        {
            return "";
        }
        if (posB == -1)
        {
            return "";
        }
        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= posB)
        {
            return "";
        }
        return value.Substring(adjustedPosA, posB - adjustedPosA);
    }


    public static void ParseSoapEnvelopePKCS12(string soapresponse, out string USI, out string pkcs12, out string error)
    {
        USI = string.Empty;
        pkcs12 = string.Empty;
        error = string.Empty;
        string USIPrepared = string.Empty;
        string pkcs12Prepared = string.Empty;
        string errorPrepared = string.Empty;

        try
        {
            //todo test: parse response SOAP message
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapresponse);  //loading soap message as string

            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Envelope
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Header
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Body

            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//bx:BlueXMLRequestMessage
                if (navigator.Name == "bx:BlueXMLRequestMessage")
                {
                    if (navigator.HasChildren)
                    {
                        navigator.MoveToFirstChild();//bx:data

                        if (navigator.HasChildren)
                        {
                            navigator.MoveToFirstChild();//certificate

                            if (navigator.Name == "error")
                            {
                                errorPrepared = navigator.Value;
                            }
                            else
                            { 
                                do
                                {
                                    if (navigator.HasChildren)
                                    {
                                        navigator.MoveToFirstChild(); //bx:value name="requestNumber">
                                        USIPrepared = navigator.Value;
                                        navigator.MoveToFollowing(XPathNodeType.Element);
                                        pkcs12Prepared = navigator.Value;

                                        navigator.MoveToParent();
                                    }
                                } while (navigator.MoveToNext());
                            }
                        }
                    }
                }
            }
            USI = USIPrepared;
            pkcs12 = pkcs12Prepared;
            error = errorPrepared;
        }
        catch (Exception ex)
        {
            log.Error("Error while parsing SOAP envelope message in ParseSoapEnvelopePKCS12. " + ex.Message);
            throw new Exception("Error while parsing SOAP envelope message in ParseSoapEnvelopePKCS12. " + ex.Message, ex.InnerException);
        }
    }

    public static bool CheckRegistrationNumber(string registrationNumber)
    {
        bool ret = true;
        string controlDigit = registrationNumber.Substring(7, 1);
        string calculatedControlDigit = GetControlDigit(registrationNumber);
        if (!controlDigit.Equals(calculatedControlDigit))
        { 
            ret = false;
            log.Error("Neispravna kontrolna cifra matičnog broja. U broju je: " + controlDigit + ", a treba da bude: " + calculatedControlDigit);
        }
        return ret;
    }

    public static string GetControlDigit(string registrationNumber)
    {
        /*
            K = 11 - ((2 x (b0 + b6)
        + 3 x b5
        + 4 x b4
        + 5 x b3
        + 6 x b2
        + 7 x b1) % 11)
            */
        //07527578 pis
        // K = 11 - (14 + 15 + 28 + 10 + 30 + 49) % 11 = 11 - 3 = 8

        int k, rnLen;
        int multiplier;
        int oneCharacter, i;

        k = 0;
        rnLen = registrationNumber.Length;
        multiplier = 2;

        for (i = rnLen - 2; i >= 0; i += -1)  //ispituju se cifre od pozicije 6 do 0
        {
            oneCharacter = Convert.ToInt32(registrationNumber.Substring(i, 1));

            k = k + oneCharacter * multiplier;

            if (multiplier == 7)
                multiplier = 2;
            else
                multiplier = multiplier + 1;

        }

        k = 11 - (k % 11);

        if (k > 9)
            k = 0;

        return k.ToString();
    }
    /*
    public static string Encrypt(string clearText)
    {
        try
        {
            //+ DateTime.Now.ToString("yyyyMMddHHmmss")
            string EncryptionKey1 = System.Configuration.ConfigurationManager.AppSettings["EncryptionKey1"].ToString();
            string EncryptionKey = EncryptionKey1 + DateTime.Now.ToString("yyyyMMddHH");
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        catch (Exception)
        {
            throw new Exception("Error in Encrypt function. ");
        }
    }
    
    public static string Decrypt(string cipherText)
    {
        try
        {
            string EncryptionKey1 = System.Configuration.ConfigurationManager.AppSettings["EncryptionKey1"].ToString();
            string EncryptionKey = EncryptionKey1 + DateTime.Now.ToString("yyyyMMddHH");
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        catch (Exception)
        {
            throw new Exception("Error in Decrypt function. ");
        }
    }
    */
    public static string ConvertToTildaPath(string OriginalPath)
    {
        string returnvalue = string.Empty;

        try
        {
            log.Info("Putanja za dokument je: " + OriginalPath + ".");
            Utility utility = new Utility();
            string PartOfOriginalPart = utility.getItemTextIDTypeOfItem(Constants.ITEM_PDF_DOWNLOAD_PATH);
            string TildaPath = OriginalPath.Replace(@PartOfOriginalPart, @"~\");
            log.Info("Tilda Putanja za dokument je: " + TildaPath + ".");
            returnvalue = TildaPath;
        }
        catch (Exception ex)
        {
            log.Error("Error while converting to Tilda path. " + ex.Message);
        }

        return returnvalue;
    }

    public static string ConvertToLocalPath(string OriginalPath)
    {
        string returnvalue = string.Empty;

        try
        {
            log.Info("Putanja za dokument je: " + OriginalPath + ".");
            Utility utility = new Utility();
            string PartOfOriginalPart = utility.getItemTextIDTypeOfItem(Constants.ITEM_PKCS12_DOWNLOAD_PATH);
            string PartOfLocalPath = utility.getItemTextIDTypeOfItem(Constants.ITEM_PDF_DOWNLOAD_PATH);
            string LocalPath = OriginalPath.Replace(@PartOfOriginalPart, @PartOfLocalPath);
            log.Info("Lokalna Putanja za dokument je: " + LocalPath + ".");
            returnvalue = LocalPath;
        }
        catch (Exception ex)
        {
            log.Error("Error while converting to Local path. " + ex.Message);
        }

        return returnvalue;
    }


    public static void DownloadPDF(System.Web.UI.Page callingPage, string pdfSaveAsFileName)
    {
        try
        {
            string fileName = @"attachment; filename=""" + Path.GetFileName(pdfSaveAsFileName) + "";
            callingPage.Response.ContentType = "Application/pdf";
            callingPage.Response.AppendHeader("Content-Disposition", fileName);
            callingPage.Response.TransmitFile(pdfSaveAsFileName);
            callingPage.Response.Flush();
        }
        catch (Exception ex)
        {
            log.Error("Error in function DownloadPDF. " + ex.Message);
        }
    }

    public static void DownloadCertificate(System.Web.UI.Page callingPage, string pdfSaveAsFileName)
    {
        try
        {

        }
        catch (Exception ex)
        {
            log.Error("Error in function DownloadCertificate. " + ex.Message);
        }
    }

    public static bool IsActivePage(string page)
    {
        bool returnvalue = true;

        try
        {
            Utility utility = new Utility();
            returnvalue = utility.pronadjiDaLiJeStranicaAktivna(page);
        }
        catch (Exception ex)
        {
            log.Error("IsActivePage error. " + ex.Message);
            throw new Exception("IsActivePage error. " + ex.Message);
        }

        return returnvalue;
    }

    public static bool IsActiveAgreemePage(string page)
    {
        bool returnvalue = true;

        Utility utility = new Utility();
        returnvalue = utility.pronadjiDaLiJeStranicaUputstvoAktivna(page);

        return returnvalue;
    }

    public static void CheckIPGeolocationData(string ipGeolocationDataStatus, string userAgentIP, string ipGeolocationDataCountry, string ipGeolocationDataCountryCode, string ipGeolocationDataCity, string ipGeolocationDataIsp, string ipGeolocationDataContinent, out string userAgentCountry, out string userAgentCountryCode, out string userAgentCity, out string userAgentISP, out string userAgentContinent)
    {
        userAgentCountry = string.Empty;
        userAgentCountryCode = string.Empty;
        userAgentCity = string.Empty;
        userAgentISP = string.Empty;
        userAgentContinent = string.Empty;

        try
        {
            if (ipGeolocationDataStatus == null)
            {
                log.Debug("ipGeolocationData.Status is null. ");
                userAgentCountry = string.Empty;
                userAgentCountryCode = string.Empty;
                userAgentCity = string.Empty;
                userAgentISP = string.Empty;
                userAgentContinent = string.Empty;
            }
            else
            {
                if (!ipGeolocationDataStatus.Equals("0"))
                {
                    //throw new Exception("Error from PisMess: ipGeolocationData is null. ");
                    log.Error("Error from PisMess: " + ipGeolocationDataStatus + "IP address is: " + userAgentIP);
                    userAgentCountry = string.Empty;
                    userAgentCountryCode = string.Empty;
                    userAgentCity = string.Empty;
                    userAgentISP = string.Empty;
                    userAgentContinent = string.Empty;
                }
                else
                {
                    userAgentCountry = ipGeolocationDataCountry;
                    userAgentCountryCode = ipGeolocationDataCountryCode;
                    userAgentCity = ipGeolocationDataCity;
                    userAgentISP = ipGeolocationDataIsp;
                    userAgentContinent = ipGeolocationDataContinent;
                    if (userAgentCountry == null || userAgentCountryCode == null || userAgentCity == null || userAgentISP == null || userAgentContinent == null)
                    {
                        userAgentCountry = string.Empty;
                        userAgentCountryCode = string.Empty;
                        userAgentCity = string.Empty;
                        userAgentISP = string.Empty;
                        userAgentContinent = string.Empty;
                    }                   
                }
            }
            log.Debug("IPGeolocationData! userAgentCountry is: " + userAgentCountry + " userAgentCountryCode is: " + userAgentCountryCode + " userAgentCity is: " + userAgentCity + " userAgentISP is: " + userAgentISP + " userAgentContinent is: " + userAgentContinent);
        }
        catch (Exception ex)
        {
            log.Error("Error in function CheckIPGeolocationData. " + ex.Message);
            throw new Exception("Error in function CheckIPGeolocationData. " + ex.Message, ex.InnerException);
        }
    }


    public static List<CertificateRequestStatusPostExpressID> ParseSoapEnvelopeRequestStatusPostExpressID(string soapresponse)
    {
        List<CertificateRequestStatusPostExpressID> responses = new List<CertificateRequestStatusPostExpressID>();

        try
        {
            //todo test: parse response SOAP message
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapresponse);  //loading soap message as string

            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Envelope
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Header
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//env:Body

            string postexpressID = string.Empty;
            string requestNumberTemp = string.Empty;
            string typeTemp = string.Empty;
            string statusTemp = string.Empty;


            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//bx:BlueXMLRequestMessage
                if (navigator.Name == "bx:BlueXMLRequestMessage")
                {
                    if (navigator.HasChildren)
                    {
                        navigator.MoveToFirstChild();//bx:data

                        if (navigator.HasChildren)
                        {
                            //Kada Response bude dobar
                            navigator.MoveToFirstChild();//postExpressID 
                            postexpressID=navigator.Value;
                            navigator.MoveToNext();//certificate

                            //navigator.MoveToFirstChild();//certificate
                            do
                            {
                                if (navigator.HasChildren)
                                {
                                    navigator.MoveToFirstChild(); //bx:value name="requestNumber">
                                    requestNumberTemp = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    typeTemp = navigator.Value;
                                    navigator.MoveToFollowing(XPathNodeType.Element);
                                    statusTemp = navigator.Value;

                                    responses.Add(new CertificateRequestStatusPostExpressID(postexpressID, Convert.ToInt32(requestNumberTemp), typeTemp, statusTemp));

                                    navigator.MoveToParent();
                                }
                            } while (navigator.MoveToNext());
                        } 
                    } 
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while parsing SOAP envelope message in ParseSoapEnvelopeRequestStatusPostExpressID. " + ex.Message);
            throw new Exception("Error while parsing SOAP envelope message in ParseSoapEnvelopeRequestStatusPostExpressID. " + ex.Message, ex.InnerException);
        }
        return responses;
    }
}