using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Xml;
using System.Xml.XPath;

public static class BxSoap
{
    public static string SettingsFile { get; private set; }
    public static string Url { get; set; }
    public static string Action { get; set; }
    public static string SerialNumber { get; set; }
    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static BxSoap()
    {
        SettingsFile = AppDomain.CurrentDomain.BaseDirectory + "SOAPsettings.xml";
        initializeSetup();
    }

    public static string SOAPManual(XmlDocument envelope)
    {
        string ret = string.Empty;
        try
        {
            log.Debug("BxSaop(SOAPManual) function starting");
            /////Uvedeno zbog greske:the request was aborted could not create ssl/tls secure channel.
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            ////////////////////////////////////////////////
            //XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest();

            /////Uvedeno zbog greske:the request was aborted could not create ssl/tls secure channel.
            webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.PreAuthenticate = true;

            List<X509Certificate2> certificates = GetCurrentUserCertificates();
            log.Debug("List<X509Certificate2> certificates count is " + certificates.Count);

            if (certificates.Count == 0)
            {
                throw new Exception("No certificates found.");
            }

            try
            {
                foreach (X509Certificate2 certificate in certificates)
                {
                    if (certificate.SerialNumber.Equals(SerialNumber, StringComparison.InvariantCultureIgnoreCase))
                    {
                        webRequest.ClientCertificates.Add(certificate);
                        log.Debug("Get certificate from list. Certificate is: " + certificate.SerialNumber);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Greska u petlji " + ex.Message);
            }

            if (webRequest.ClientCertificates.Count == 0)
            {
                throw new Exception("No certificate with wanted serial number found.");
            }

            try
            {
                ////////////////////////////////////////////////
                InsertSoapEnvelopeIntoWebRequest(envelope, webRequest);
                log.Debug("SOAP message is:  " + envelope.InnerXml);
                string result = string.Empty;

                log.Debug("webRequest: " + webRequest.RequestUri.ToString() + " " + webRequest.Host + " " + webRequest.HaveResponse.ToString() + " " + webRequest.Connection + " " + webRequest.Address.ToString() + " webRequest.ClientCertificates.Count: " + webRequest.ClientCertificates.Count);

                log.Debug("Start getting result ");
                WebResponse response = null;
                try
                {
                    response = webRequest.GetResponse();
                }
                catch (WebException ex)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                        log.Debug("Web exception happened: " + result);  
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("webRequest.GetResponse() error " + ex.Message + " ||| " + ex.InnerException + " ||| " + ex.StackTrace);
                }
                //using ()
                //{
                log.Debug("End getting result ");

                if (result == string.Empty)
                {
                    if (response == null)
                    {
                        throw new Exception("response is null");
                    }
                    log.Debug("Response is " + response.ToString());
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        result = rd.ReadToEnd();
                    }
                    //}
                }
                response.Dispose();

                ret = result;
            }
            catch (WebException e)
            {
                ret = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
                log.Error("Error while getting response from BlueX. " + ret);
                throw new Exception("Error while getting response from BlueX. " + ret);
            }
            catch (Exception e)
            {
                log.Error("Some error " + e);
                throw new Exception("Some error " + e);
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function SOAPManual. " + ex.Message);
            throw new Exception("Error in function SOAPManual. " + ex.Message);
        }
        return ret;
    }

    public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    private static HttpWebRequest CreateWebRequest()
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
        webRequest.Headers.Add(@"SOAP:Action");

        webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        webRequest.Accept = "text/xml";
        webRequest.Method = Action;
        return webRequest;
    }
    
    private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument envelope, HttpWebRequest webRequest)
    {
        try
        {
            if (envelope == null)
            {
                throw new Exception("Envelope is null");
            }
            if (webRequest == null)
            {
                throw new Exception("WebRequest is null");
            }
            using (Stream stream = webRequest.GetRequestStream())
            {
                envelope.Save(stream);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error in InsertSoapEnvelopeIntoWebRequest function. " + ex.Message);
        }
    }

    public static void initializeSetup()
    {
        getSettings();       
    }

    public static void getSettings()
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            XPathNavigator navigator = xmlDoc.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element);//settings
            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();//<setup>
                if (navigator.Name == "setup")
                {
                    do
                    {
                        navigator.MoveToFirstChild();
                        if (navigator.Name == "url")
                        {
                            Url = navigator.Value;
                        }
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        if (navigator.Name == "action")
                        {
                            Action = navigator.Value;
                        }
                        navigator.MoveToFollowing(XPathNodeType.Element);
                        if (navigator.Name == "serialnumber")
                        {
                            SerialNumber = navigator.Value;
                        }
                        log.Debug("Get Settings : URL - " + Url + " . Action - " + Action + " . SerialNumber - " + SerialNumber);
                        navigator.MoveToFollowing(XPathNodeType.Element);
                            
                        navigator.MoveToParent();                       

                    } while (navigator.MoveToNext());              
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Error while reading configuration data.. " + ex.Message);
        }
    }

    public static List<X509Certificate2> GetCurrentUserCertificates()
    {
        List<X509Certificate2> certificates = new List<X509Certificate2>();
        X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        log.Debug("store.Name is " + store.Name);
        log.Debug("store.Certificates.Count is " + store.Certificates.Count);
        foreach (X509Certificate2 certificate in store.Certificates)
        {
            certificates.Add(certificate);
            log.Debug("Certificates from store: " + certificate);
        }
        store.Close();
        return certificates;
    }
}