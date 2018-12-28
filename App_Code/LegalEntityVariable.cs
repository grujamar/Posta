using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for LegalEntityVariable
/// </summary>
public class LegalEntityVariable
{
    public int IDLegalEntity { get; set; }
    public string FullName { get; set; }
    public string PIB { get; set; }
    public bool PDVpayer { get; set; }
    public string BysinessTypeCode { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string ZipCode { get; set; }
    public string PAK { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public LegalEntityVariable(int idLegalEntity, string fullname, string pib, bool pdvpayer, string btc, string street, string hn, string zc, string pak, string city, string phone, string email)
    {
        IDLegalEntity = idLegalEntity;
        FullName = fullname;
        PIB = pib;
        PDVpayer = pdvpayer;
        BysinessTypeCode = btc;
        Street = street;
        HouseNumber = hn;
        ZipCode = zc;
        PAK = pak;
        City = city;
        PhoneNumber = phone;
        Email = email;
    }
}