using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PTTVariable
{
    public int IDItem { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }   
    public string ZIPCode { get; set; }
    public string PAK { get; set; }
    public bool InHouse { get; set; }
    public bool IsAllowed { get; set; }
    public int IsLegalEntity { get; set; }

    public PTTVariable(int iditem, string city, string street, string hn, string zip, string pak, bool inhouse, bool isallowed, int islegalentity)
    {
        IDItem = iditem;
        City = city;
        Street = street;
        HouseNumber = hn;
        ZIPCode = zip;
        PAK = pak;
        InHouse = inhouse;
        IsAllowed = isallowed;
        IsLegalEntity = islegalentity;
    }
}