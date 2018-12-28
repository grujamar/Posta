using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CertificateStatus
/// </summary>
public class CertificateStatus
{
    public string Item { get; set; }
    public string ItemTextEnglish { get; set; }
    public string Notification { get; set; }
    public int ItemValue { get; set; }

    public CertificateStatus(string item, string itemtextenglish, string notification, int itemvalue)
    {
        Item = item;
        ItemTextEnglish = itemtextenglish;
        Notification = notification;
        ItemValue = itemvalue;
    }
}