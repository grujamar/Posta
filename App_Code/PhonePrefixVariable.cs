using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PhonePrefixVariable
{
    public int IDItem { get; set; }
    public string ItemText { get; set; }
    public bool Active { get; set; }

    public PhonePrefixVariable(int iditem, string itemtext, bool active)
    {
        IDItem = iditem;
        ItemText = itemtext;
        Active = active;
    }
}