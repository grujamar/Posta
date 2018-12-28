using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CertificateRequestStatusPostExpressID
{
    public string PostExpressID { get; set; }
    public int RequestNumber { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    
    public CertificateRequestStatusPostExpressID(string postexpressid, int requestno, string type, string status)
    {
        PostExpressID = postexpressid;
        RequestNumber = requestno;
        Type = type;
        Status = status;
    }
}