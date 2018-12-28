using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace BlueXSOAP
{
    public class BxSoapEnvelopePKCS12 : BxSoapEnvelope
    {
        public BxSoapEnvelopePKCS12() : base()
        {
            BxData = new BxDataPKCS12();
        }

        public override XmlDocument createBxSoapEnvelope()
        {
            XmlDocument soapEnvelopeXml = new XmlDocument();
            string xmlContent = SoapEnvelopeHeader;
            xmlContent += @"<bx:action>Retrieve Product</bx:action>";
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
}