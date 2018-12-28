using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CertificateChallengeResponse
/// </summary>
public class CertificateChallengeResponse
{
    public int RequestNumber { get; set; }
    public string Response { get; set; }

    public CertificateChallengeResponse(int requestno, string response)
    {
        RequestNumber = requestno;
        Response = response;
    }
}