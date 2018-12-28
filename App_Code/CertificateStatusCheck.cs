using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CertificateStatusCheck
{
    public string USI { get; set; }
    public string ValidFrom { get; set; }
    public string ValidTo { get; set; }
    public string GivenName { get; set; }
    public string LastName { get; set; }

    public CertificateStatusCheck(string usi, string validfrom, string validto, string givenname, string lastname)
    {
        USI = usi;
        ValidFrom = validfrom;
        ValidTo = validto;
        GivenName = givenname;
        LastName = lastname;
    }
}