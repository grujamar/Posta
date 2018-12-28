using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CertificateRequestStatus
{
    public int RequestNumber { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }

    public CertificateRequestStatus(int requestno, string type, string status)
    {
        RequestNumber = requestno;
        Type = type;
        Status = status;
    }
}