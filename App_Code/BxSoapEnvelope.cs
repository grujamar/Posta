using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlueXSOAP
{
    public abstract class BxSoapEnvelope
    {
        public BxData BxData { get; set; }
        public const string SoapEnvelopeHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>
                   <env:Envelope xmlns:env=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:bx=""http://namespaces.bluex.com/bluex/bluexml"">
                   <env:Header/>
                   <env:Body>
                   <bx:BlueXMLRequestMessage xmlns:bx=""http://namespaces.bluex.com/bluex/bluexml"">";
        public const string SoapEnvelopeFooter = @"</bx:BlueXMLRequestMessage>
	                    </env:Body>
                    </env:Envelope>";

        public BxSoapEnvelope()
        { }

        public abstract XmlDocument createBxSoapEnvelope();
    }
}
