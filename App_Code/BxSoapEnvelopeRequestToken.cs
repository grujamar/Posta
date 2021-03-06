﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlueXSOAP
{
    //Klasa koja definise SOAP envelope za izdavanje sertifikata pojedinacnim korisnicima
    public class BxSoapEnvelopeRequestToken : BxSoapEnvelope
    {
        public BxSoapEnvelopeRequestToken():base()
        { 
            BxData = new BxDataRequestToken();
        }

        public override XmlDocument createBxSoapEnvelope()
        { 
            XmlDocument soapEnvelopeXml = new XmlDocument();
            string xmlContent = SoapEnvelopeHeader;
            xmlContent += @"<bx:action>Register Token from Form</bx:action>";
            xmlContent += BxData.ToString();
            xmlContent += SoapEnvelopeFooter;

            try
            {
                xmlContent.Replace("'", "&apos;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;").Replace(" ", "&nbsp;");
                soapEnvelopeXml.LoadXml(xmlContent);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in createBxSoapEnvelope. " + ex.Message);
            }

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



