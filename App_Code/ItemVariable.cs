using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ItemVariable
/// </summary>
public class ItemVariable
{
    public int IDItem { get; set; }
    public string ItemText { get; set; }
    public bool IsDefault { get; set; }

    public ItemVariable(int id, string text, bool iddefault)
    {
        IDItem = id;
        ItemText = text;
        IsDefault = iddefault;
    }
}