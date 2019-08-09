using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ServiceCaller
/// </summary>
public class ServiceCaller
{
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static void CallService()
    {
        PisMessServiceReference.PisMessServiceClient client = new PisMessServiceReference.PisMessServiceClient();
    }

    public static string[] CallServiceCreateDoc(PisMessServiceReference.TemplateDocumentTypeSerbianPost documentType, PisMessServiceReference.Parameter[] parameters)
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 16;

        PisMessServiceReference.PisMessServiceClient pisMess = new PisMessServiceReference.PisMessServiceClient();
        log.Debug("Start calling pisMess.CreateDocument for: "  + documentType.ToString() + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        return pisMess.CreateDocument(documentType, parameters);
    }

    public static string[] CallServiceCreateDocLegalEntity(List<PisMessServiceReference.Parameter> documentParameters, List<PisMessServiceReference.CertificatesAuthorizedUser> certificatesAuthorizedUsers)
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 16;

        PisMessServiceReference.PisMessServiceClient pisMess = new PisMessServiceReference.PisMessServiceClient();
        log.Debug("Start calling pisMess.CreateDocumentLegalEntityContractAttachment: " + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        return pisMess.CreateDocumentLegalEntityContractAttachment(documentParameters.ToArray(), certificatesAuthorizedUsers.ToArray());
    }
}