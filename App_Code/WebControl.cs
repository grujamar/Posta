using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WebControl
/// </summary>
public class WebControl
{
    public string Id { get; set; }
    public bool ControlStatus { get; set; }
    public bool IsRequired { get; set; }

    public WebControl(string id, bool status, bool isrequired)
    {
        Id = id;
        ControlStatus = status;
        IsRequired = isrequired;
    }
}