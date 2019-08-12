using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Constants
{
    public const int ITEM_NOT_SELECTED = 1;
    public const int ITEM_YES_NO = 2;
    public const int ITEM_DOCUMENT_TYPE = 3;
    public const int ITEM_ADDRESS = 4;
    public const int ITEM_CERTIFICATE_TYPE = 5;
    public const int ITEM_COUNTRY = 6;
    public const int ITEM_ENTITY = 7;
    public const int ITEM_MEDIA = 8;
    public const int ITEM_PAYMENTTYPE = 9;
    public const int ITEM_VALIDITY = 10;
    public const int ITEM_PRICE = 11;
    public const int ITEM_STATUS_CHANGE = 12;
    public const int ITEM_CHALLENGE_RESPONSE = 13;
    public const int ITEM_STATUS_CHECK = 14;

    public const int ITEM_PATH = 18;
    public const int ITEM_PHONE_PREFIX = 19;

    public const int ITEM_PDF_DOWNLOAD_PATH = 25;
    public const int ITEM_PKCS12_DOWNLOAD_PATH = 26;

    //--------------ERROR------------
    public const int ITEM_ERROR = 21;
    public const int ERROR_2330 = 2330;
    public const int ERROR_2331 = 2331;
    public const int ERROR_2332 = 2332;
    public const int ERROR_2333 = 2333;
    public const int ERROR_2334 = 2334;
    public const int ERROR_2335 = 2335;
    public const int ERROR_2336 = 2336;
    public const int ERROR_2337 = 2337;
    public const int ERROR_2338 = 2338;
    public const int ERROR_2339 = 2339;
    public const int ERROR_2340 = 2340;
    public const int ERROR_2341 = 2341;
    public const int ERROR_2342 = 2342;
    public const int ERROR_2343 = 2343;
    public const int ERROR_2344 = 2344;
    public const int ERROR_2345 = 2345;
    public const int ERROR_2346 = 2346;
    public const int ERROR_2347 = 2347;
    public const int ERROR_2351 = 2351;
    public const int ERROR_2358 = 2358;
    public const int ERROR_2359 = 2359;
    public const int ERROR_2360 = 2360;
    public const int ERROR_2361 = 2361;
    public const int ERROR_2362 = 2362;
    public const int ERROR_2363 = 2363;
    public const int ERROR_3357 = 3357;
    public const int ERROR_3371 = 3371;
    public const int ERROR_3372 = 3372;
    public const int ERROR_3373 = 3373;
    public const int ERROR_3374 = 3374;
    public const int ERROR_3375 = 3375;
    public const int ERROR_3376 = 3376;
    public const int ERROR_3379 = 3379;
    public const int ERROR_3380 = 3380;

    public const int KVALIFIKOVANI_ELEKTRONSKI_SERTIFIKAT = 1;
    public const int WEB_ELEKTRONSKI_SERTIFIKAT = 2;
    public const int SER_ELEKTRONSKI_SERTIFIKAT = 3;
    public const int TSA_ELEKTRONSKI_SERTIFIKAT = 4;
    public const int VREMENSKI_PREPAID_ELEKTRONSKI_SERTIFIKAT = 5;
    public const int VREMENSKI_POSTPAID_ELEKTRONSKI_SERTIFIKAT = 6;

    public const string ISSUING = "ISSUING";
    public const string UNBLOCKING = "UNBLOCKING";
    public const string CERTFOLDER = "CERTFOLDER";
    public const string MINLENSERIAL = "MINLENSERIAL";
    public const string MAXLENSERIAL = "MAXLENSERIAL";
    public const string PKCS12TIMEOUT = "PKCS12TIMEOUT";

    public const string SetDarkGray = "#e2e2e2";
    public const string SetLightGray = "#f5f5f5";
    public const string SetWhite = "#ffffff";
    public const string SetCss = "txtbox";
    public const string SetCss1 = "txtbox1";
    public const string SetCss2 = "txtbox2";
    public const string SetCss3 = "txtbox3";
    public const string SetCss4 = "txtbox4";
    public const string SetCss5 = "txtbox5";

    public const int PASSPORT = 5;
    public const string withoutJMBG = "Bez JMBG";
    public const int DefaultIdItemLegal = 1;
    public const int ItemValue_NACINPROMENE_REVOCATION = 1;
    public const int ItemValue_NACINPROMENE_SUSPENSION = 2;
    public const int ItemValue_NACINPROMENE_SUSPENSION_REVOCATION = 3;

    public const string isLegalEntityTrue = "1";
    public const string isLegalEntityFalse = "0";

    //todo
    public const int REQUEST_NUMBER = 20000001;
    public const int USI = 200000001;
    public const int ITEM_VALUE_WAITING = 1;
    public const int ITEM_VALUE_REFUSED = 2;
    public const int ITEM_VALUE_REJECTED = 19;
    public const int ITEM_VALUE_REQUESTED = 3;
    public const int ITEM_VALUE_APPROVED = 0;
    public const int ITEM_VALUE_COMPLETED = 10;
    public const int ITEM_VALUE_REFUSED_19 = 19;

    public const int ITEM_VALUE_CHECKSTATUS_REQUESTED = 0;
    public const int ITEM_VALUE_CHECKSTATUS_CONFIRMED = 1;
    public const int ITEM_VALUE_CHECKSTATUS_DENIED = 2;
    public const int ITEM_VALUE_CHECKSTATUS_VALIDATING = 3;
    public const int ITEM_VALUE_CHECKSTATUS_DOCUMENTACIONCOMPLETED = 4;
    public const int ITEM_VALUE_CHECKSTATUS_CONTRACTCONCLUDED = 5;
    public const int ITEM_VALUE_CHECKSTATUS_INVOICEISSUED = 6;
    public const int ITEM_VALUE_CHECKSTATUS_ORDERPAYED = 7;
    public const int ITEM_VALUE_CHECKSTATUS_ISSUING = 8;
    public const int ITEM_VALUE_CHECKSTATUS_CHANGECOMPLETED = 9;
    public const int ITEM_VALUE_CHECKSTATUS_UNBLOCKCOMPLETED = 10;

    public const int IDITEM_UNSUSPENSION = 1270;
    public const int IDITEM_SUSPEND = 1271;
    public const int IDITEM_OTHER_REASON = 1272;
    public const string RESPONSE_STATUS_CHANGE_SUCCESS = "Success";
    public const int IDITEM_REQUEST_UNBLOCK_STATUS = 1309;


    public const string SOAP_BX = "bx";
    public const string SOAP_LINK = "http://namespaces.bluex.com/bluex/bluexml";
    public const string SOAP_BX_DATA = "//bx:data";
    public const string SOAP_BX_VALUE_REQUEST_NUMBER = "//bx:value[@name='requestNumber']";
    public const string SOAP_BX_VALUE_ORDER_NUMBER = "//bx:value[@name='reqid']";
    public const string SOAP_CERTIFICATE = "//certificate";
    public const string SOAP_BX_VALUE_TYPE = "//bx:value[@name='type']";
    public const string SOAP_BX_VALUE_STATUS = "//bx:value[@name='status']";
    public const string SOAP_BX_VALUE_RESPONSE = "//bx:value[@name='response']";
    public const string SOAP_BX_VALUE_USI = "//bx:value[@name='USI']";
    public const string SOAP_BX_VALUE_VALIDFROM = "//bx:value[@name='validFrom']";
    public const string SOAP_BX_VALUE_VALIDTO = "//bx:value[@name='validTo']";
    public const string SOAP_BX_VALUE_GIVENAME = "//bx:value[@name='givenName']";
    public const string SOAP_BX_VALUE_LASTNAME = "//bx:value[@name='lastName']";
    public const string SOAP_BX_VALUE_PKCS12 = "//bx:value[@name='pkcs12']";

    public const int IDITEM_UNBLOCK_PRICE = 331;
    public const string REQUEST_CHALLENGE_RESPONSE_TRUE = "1";
    public const string REQUEST_CHALLENGE_RESPONSE_FALSE = "0";

    //Legal Entity
    public const int LEGAL_ENTITY_PIB = 9;
    public const int LEGAL_ENTITY_MATICNI_BROJ = 8;
    public const int LEGAL_ENTITY_SIFRA_DELATNOSTI = 4;
    public const int LEGAL_ENTITY_POSTANSKI_BROJ = 5;

    public const int LEGAL_ENTITY_ID_ORDER_NUMBER = 1;

    public const int REVOCATION_OCSP_CHECK_TYPE = 1;
    public const int REVOCATION_CRL_LDAP_CHECK_TYPE = 2;
    public const int REVOCATION_CRL_HTTP_CHECK_TYPE = 3;

    public const string REVOCATION_CERITIFATE_STATUS_REVOKE = "Revoked";
    public const string REVOCATION_CERITIFATE_STATUS_UNREVOKE = "Good";
    public const string REVOCATION_CERITIFATE_STATUS_OTHER = "Unknown";

    public const string REVOCATION_CERITIFATE_STATUS_REVOKE_MESSAGE = "Sertifikat je opozvan.";
    public const string REVOCATION_CERITIFATE_STATUS_UNREVOKE_MESSAGE = "Sertifikat je ispravan (nije opozvan).";
    public const string REVOCATION_CERITIFATE_STATUS_OTHER_MESSAGE = "Status sertifikata je nepoznat.";

    public const string REVOCATION_SERIAL_NO_PROPERTY_NAME_MIN = "MinLength";
    public const string REVOCATION_SERIAL_NO_PROPERTY_NAME_MAX = "MaxLength";

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //Ako je kod JMBG-a DDMMGGGRRBBBK, RR=06(stalni stranac) ili RR=66(privremeni stranac) tada u DropDown listi ddlsertjmbg postoji samo DA (U sertifika ce biti upisan JMBG)
    public const int ID_ITEM_DDLSERTJMBG = 3;

    //////REQUIRED FIELDS//////
    public const string CONTROL_ТYPE_LABEL = "label";
    public const string CONTROL_ТYPE_BUTTON = "button";
    public const string CONTROL_ТYPE_LINK_BUTTON = "LinkButton";
    public const bool IS_VISIBLE = true;
    public const string CONTROL_TYPE_TEXTBOX = "TextBox";
    public const string CONTROL_TYPE_DROPDOWNLIST = "DropDownList";
    public const string CONTROL_TYPE_RADIOBUTTON = "RadioButton";
    public const string CONTROL_TYPE_CHECKBOX = "CheckBox";
    public const string CONTROL_TYPE_HYPERLINK = "HyperLink";
    public const string CONTROL_TYPE_LINK_BUTTON = "LinkButton";

    public const string GLOBAL_VALIDATION = "VALIDATION";
    public const string GLOBAL_GEOLOCATIONS = "GEOLOCATIONS";
    public const string SETTING_VALUE_TRUE = "1";
    public const bool VALIDATION_FALSE = false;

    public const int JIK = 9;
    public const int BROJ_ZAHTEVA = 8;

    public const int YES = 1;
    public const int NO = 2;

    public const int INCLUDE_UNIQUE_CITIZENS_NUMBER_NO = 3;

    public const string SOAP_INDIVIDUAL_COUNTRY = "Srbija";
    public const string SOAP_INDIVIDUAL_COUNTRY_CODE = "RS";

    public static readonly byte[] CryptKey = new byte[] { 0x61, 0xC2, 0xD2, 0xC9, 0x27, 0xBC, 0x60, 0xA9, 0xF1, 0xD6, 0x4A, 0xB4, 0xE8, 0xC5, 0x28, 0x3F, 0x97, 0xF8, 0x8E, 0x3D, 0xE2, 0xAB, 0x15, 0x49, 0x92, 0x8C, 0x18, 0xEA, 0xB0, 0x37, 0xEE, 0xF3 };
    public static readonly byte[] AuthKey = new byte[] { 0x43, 0x9A, 0x93, 0x29, 0x2, 0x8, 0x43, 0x25, 0x43, 0x9, 0x75, 0x24, 0x41, 0x7, 0x93, 0x87, 0x68, 0x84, 0xA6, 0xE0, 0x80, 0xBC, 0x90, 0xB5, 0xE2, 0x4B, 0x92, 0xC9, 0xF3, 0x2F, 0x3, 0x1C };

    public const string PTT_CEPP = "PTT-CePP";


    /////////////////////////////////////
    public const string rightBrackets = "]";
    public const string leftBrackets = "[";

    public const string ErrorGettingRequestNumber = " Error getting REQID/UID";
    public const string AuthorizationCodeDoesNotMatch = " Authorization Code does not match";
    public const string TransferServiceFailed = " Transfer Service failed";
    public const string RrequestIsNotInRequiredState = " Error: the request is not in a required state";
    /////////////////////////////////////
}