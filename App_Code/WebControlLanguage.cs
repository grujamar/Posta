using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class WebControlLanguage
{
    public string Controlid { get; set; }
    public string PageTittle { get; set; }
    public string ControlTittle { get; set; }
    public bool ValidationActive { get; set; }
    public bool IsVisible { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsRequired { get; set; }
    public string ControlType { get; set; }

    public WebControlLanguage(string controlid, string pagetittle, string controltittle, bool validationactive, bool isvisible, bool isenabled, bool isrequired, string controltype)
    {
        Controlid = controlid;
        PageTittle = pagetittle;
        ControlTittle = controltittle;
        ValidationActive = validationactive;
        IsVisible = isvisible;
        IsEnabled = isenabled;
        IsRequired = isrequired;
        ControlType = controltype;
    }
}