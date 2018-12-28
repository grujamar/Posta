using BlueXSOAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

public class BxSoapEnvelopeCertificateStatusCheck : BxSoapEnvelope
{
    public BxSoapEnvelopeCertificateStatusCheck() : base()
    {
        BxData = new BxDataCertificateStatusCheck();
    }

    public override XmlDocument createBxSoapEnvelope()
    {
        XmlDocument soapEnvelopeXml = new XmlDocument();
        string xmlContent = SoapEnvelopeHeader;
        xmlContent += @"<bx:action>Request Expiration Check</bx:action>";
        xmlContent += BxData.ToString();
        xmlContent += SoapEnvelopeFooter;

        soapEnvelopeXml.LoadXml(xmlContent);

        //////////////
        // samo za test
        /*
        XmlTextWriter writer = new XmlTextWriter("C://Users//Marko//Desktop//Projekti SVI//Posta//SOAP Poruka//SOAP.xml", null);
        writer.Formatting = Formatting.Indented;
        soapEnvelopeXml.Save(writer);
        */
        //////////////
        return soapEnvelopeXml;
    }
}