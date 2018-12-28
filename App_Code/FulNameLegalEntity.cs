using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FulNameLegalEntity
/// </summary>
public class FulNameLegalEntity
{
    public int IDLegalEntity { get; set; }
    public string FullName { get; set; }

    public FulNameLegalEntity(int idlegalentity, string fullname)
    {
        IDLegalEntity = idlegalentity;
        FullName = fullname;
    }
}