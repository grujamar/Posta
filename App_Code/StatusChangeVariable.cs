using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for StatusChangeVariable
/// </summary>
public class StatusChangeVariable
{
    public int IDItem { get; set; }
    public bool IsAllowed { get; set; }
    public bool IsDefault { get; set; }
    public int ItemValue { get; set; }

    public StatusChangeVariable(int iditem, bool isallowed, bool isdefault, int itemvalue)
    {
        IDItem = iditem;
        IsAllowed = isallowed;
        IsDefault = isdefault;
        ItemValue = itemvalue;
    }
}