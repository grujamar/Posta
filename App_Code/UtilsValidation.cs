using ISO7064;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Services;

public static class UtilsValidation
{
    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA MATIČNOG BROJA-------------------------------------
    //------------------------------------------------------------------------------------------------
    public static bool validateJMBG(string newJMBG, int IDControlItem, bool isRequired, out string ErrorMessage, out string jmbgformat, out bool RRforeigner)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage = string.Empty;
        jmbgformat = string.Empty;
        RRforeigner = false;

        try
        {
            if (isRequired)
            {
                if (newJMBG == string.Empty)
                {
                    ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2330);
                    returnValue = false;
                }
                else
                {
                    returnValue = ValidationJMBGWithoutRequired(newJMBG, out ErrorMessage, out jmbgformat, out RRforeigner);
                }
            }
            else
            {
                if (newJMBG != string.Empty)
                {
                    returnValue = ValidationJMBGWithoutRequired(newJMBG, out ErrorMessage, out jmbgformat, out RRforeigner);
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        catch (Exception)
        {
            ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
            returnValue = false;
        }

        return returnValue;
    }

    public static bool ValidationJMBGWithoutRequired(string newJMBG, out string ErrorMessage, out string jmbgformat, out bool RRforeigner)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage = string.Empty;
        jmbgformat = string.Empty;
        RRforeigner = false;

        if (!Utils.allowNumbers(newJMBG))
        {
            ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
            returnValue = false;
        }
        else if (newJMBG.Length == 13)
        {
            int[] a = new int[13];

            try
            {
                for (int i = 0; i < newJMBG.Length; i++)
                {
                    a[i] = Convert.ToInt32(newJMBG.Substring(i, 1));
                }
            }
            catch (Exception)
            {

            }
            int[] b = new int[6];
            b[0] = Convert.ToInt32("" + newJMBG.Substring(0, 1) + newJMBG.Substring(1, 1));                          // DD
            b[1] = Convert.ToInt32("" + newJMBG.Substring(2, 1) + newJMBG.Substring(3, 1));                          // MM
            b[2] = Convert.ToInt32("" + newJMBG.Substring(4, 1) + newJMBG.Substring(5, 1) + newJMBG.Substring(6, 1));// GGG
            b[3] = Convert.ToInt32("" + newJMBG.Substring(7, 1) + newJMBG.Substring(8, 1));                          // RR
            b[4] = Convert.ToInt32("" + newJMBG.Substring(9, 1) + newJMBG.Substring(10, 1) + newJMBG.Substring(11, 1));// BBB
            b[5] = Convert.ToInt32("" + newJMBG.Substring(12, 1));                                              // K

            bool k = true;
            if (b[0] <= 0 || b[0] > 31)                    // DD
                k = false;
            else if (b[1] <= 0 || b[1] > 12)                 // MM
                k = false;
            else if (b[1] == 2 && b[1] > 28 && b[2] % 4 != 0)
                k = false;
            else if (b[2] < 0 || b[2] > 999)                // GGG
                k = false;
            else if (b[3] < 0 || b[3] > 99)                  // RR
                k = false;
            else if (b[3] == 66)                  // RR     //privremeni maticni brojevi za strance 66 - treba da prolaze bez kontrole
                k = true;                                
            //else if (b[3] == 06)                      //stalno nastanjeni stranci 06 - treba da se kontrolisu po modulu 11
                //k = false;
            else if (b[4] < 0 || b[4] > 999)                 // BBB    //000-499 – muški   //500-999 – ženski
                k = false;
            else if (b[4] > 999)          // BBB
                k = false;
            else
            {                                       // K
                int pom = 11 - ((7 * (a[0] + a[6]) + 6 * (a[1] + a[7]) + 5 * (a[2] + a[8]) + 4 * (a[3] + a[9]) + 3 * (a[4] + a[10]) + 2 * (a[5] + a[11])) % 11);
                if (pom > 9)
                    pom = 0;
                if (b[5] != pom)
                {
                    k = false;
                }
            }
            if (!k)
            {
                ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                returnValue = false;
            }
            else
            {
                //errLabel.Text = "Ispravan JMBG!";                   
                string jmbg = newJMBG;
                string C0 = newJMBG.Substring(0, 1) + newJMBG.Substring(1, 1);
                string C1 = newJMBG.Substring(2, 1) + newJMBG.Substring(3, 1);
                string C2 = newJMBG.Substring(4, 1) + newJMBG.Substring(5, 1) + newJMBG.Substring(6, 1);
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //Ako je kod JMBG-a DDMMGGGRRBBBK, RR=06(stalni stranac) ili RR=66(privremeni stranac) tada u DropDown listi ddlsertjmbg postoji samo DA (U sertifika ce biti upisan JMBG)
                string C3 = newJMBG.Substring(7, 1) + newJMBG.Substring(8, 1);
                if (C3 == "06" || C3 == "66")
                {
                    RRforeigner = true;
                }

                if (C3 == "06")
                {
                    //**************Stalno nastanjeni stranci 06 treba da se kontrolišu po međunarodnom standardu ISO 7064, MODUL (11,10).
                    if (CheckDigits.VerifyNumericCheckDigit(newJMBG, false))
                    {
                        ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                        returnValue = false;
                    }
                }
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                if (Utils.OneOrTwo(C2) == "101")
                {
                    ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2333);
                    returnValue = false;
                }
                else
                {
                    jmbgformat = C0 + "." + C1 + "." + Utils.OneOrTwo(C2) + C2;
                    returnValue = true;
                    ErrorMessage = string.Empty;
                }
            }
        }
        else
        {
            ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2334);
            returnValue = false;
        }

        return returnValue;
    }


    public static bool WithoutValidationJMBG(string newJMBG, bool isRequired, out string ErrorMessage, out string jmbgformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage = string.Empty;
        jmbgformat = string.Empty;

        if (isRequired)
        {
            if (newJMBG == string.Empty)
            {
                ErrorMessage = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2330);
                returnValue = false;
            }
            else
            {
                ErrorMessage = string.Empty;
                returnValue = true;
            }
        }
        else
        {
            ErrorMessage = string.Empty;
            returnValue = true;
        }

        return returnValue;
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA TELEFONSKOG BROJA----------------------------------
    //------------------------------------------------------------------------------------------------
    public static List<PhonePrefixVariable> PhonePrefixVariables;
    public static List<String> phonePrefixes;
    public static string PhonePrefix = string.Empty;

    public static bool ValidateNumber(string newNumber, bool isRequired, bool LegalEntityPhone, out string ErrorMessage1, out string numberformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        numberformat = newNumber;

        try
        {
            if (isRequired)
            {
                if (newNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = ValidateNumberWithoutRequired(numberformat, LegalEntityPhone, out ErrorMessage1, out numberformat);
                }
            }
            else
            {
                if (newNumber != string.Empty)
                {
                    returnValue = ValidateNumberWithoutRequired(numberformat, LegalEntityPhone, out ErrorMessage1, out numberformat);
                }
                else
                {
                    returnValue = true;
                }
            }

        }
        catch (Exception)
        {
            ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
            returnValue = false;
        }

        return returnValue;
    }

    public static List<PhonePrefixVariable> SetUpPhonePrefixVariables()
    {
        PhonePrefixVariables = new List<PhonePrefixVariable>();

        Utility utility = new Utility();
        PhonePrefixVariables = utility.pronadjiPrefikseMobilnihTelefona(Constants.ITEM_PHONE_PREFIX);
        return PhonePrefixVariables;
    }

    public static bool WithoutValidationNumber(string newNumber, bool isRequired, bool LegalEntityPhone, out string ErrorMessage1, out string numberformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        numberformat = string.Empty;

        if (isRequired)
        {
            if (newNumber == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                ErrorMessage1 = string.Empty;
                returnValue = true;
            }
        }
        else
        {
            ErrorMessage1 = string.Empty;
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateNumberWithoutRequired(string newNumber, bool LegalEntityPhone, out string ErrorMessage1, out string numberformat)
    {
        Utility utility = new Utility();
        int lowEnd = 8;
        int hightEnd = 12;
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        numberformat = newNumber;

        if (!Utils.allowNumbers(newNumber))
        {
            ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
            returnValue = false;
        }
        else if (newNumber.Length >= lowEnd && newNumber.Length <= hightEnd)
        {
            if (!LegalEntityPhone)
            {
                for (int arrayLength = lowEnd; arrayLength <= hightEnd; arrayLength++)
                {
                    if (arrayLength.ToString() == (newNumber.Length).ToString())
                    {
                        int[] a = new int[arrayLength];

                        for (int i = 0; i < arrayLength; i++)
                        {
                            a[i] = Convert.ToInt32(newNumber.Substring(i, 1));
                        }

                        int[] b = new int[1];
                        b[0] = Convert.ToInt32("" + newNumber.Substring(0, 1) + newNumber.Substring(1, 1) + newNumber.Substring(2, 1));

                        string[] c = new string[1];
                        c[0] = Utils.getPreparedAreaNumber(b[0].ToString());

                        PhonePrefixVariables = SetUpPhonePrefixVariables();
                        phonePrefixes = new List<String>();
                        foreach (var phoneprefixvariable in PhonePrefixVariables)
                        {
                            if (phoneprefixvariable.Active)
                            {
                                PhonePrefix = phoneprefixvariable.ItemText;
                                phonePrefixes.Add(PhonePrefix);
                            }
                        }
                        //VIP 060,061 MTS 062,063,069 Telekom 064, 065 и 066    pozivni za fiksni od 010-039   za ove nisam uradio!!!!!--0230,0280,0290,0390--
                        //string[] phonePrefixes = { "060", "061", "062", "063", "069", "064", "065", "066" };
                        //string[] strings1 = { "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "030", "031", "032", "033", "034", "035", "036", "037", "038", "039" };

                        bool k = false;
                        if (phonePrefixes.Contains(c[0]))
                            k = true;
                        /*
                        else if (strings1.Contains(c[0]))
                            k = true;                       
                        */
                        if (!k)
                        {
                            ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                            returnValue = false;
                        }
                        else
                        {
                            numberformat = newNumber;
                            ErrorMessage1 = string.Empty;
                            returnValue = true;
                        }
                    }
                }
            }
            else
            {
                numberformat = newNumber;
                ErrorMessage1 = string.Empty;
                returnValue = true;
            }
        }
        else
        {
            ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2334);
            returnValue = false;
        }

        return returnValue;
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA EMAIL ADRESE---------------------------------------
    //------------------------------------------------------------------------------------------------

    public static bool ValidateMail(string newMail, bool isRequired, out string ErrorMessage2, out string mailformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage2 = string.Empty;
        mailformat = string.Empty;

        try
        {
            if (isRequired)
            {
                if (newMail == string.Empty)
                {
                    ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                //Check Email Id is valid
                else if (Utils.IsValidEmailId(newMail))
                {
                    mailformat = newMail;
                    returnValue = true;
                    ErrorMessage2 = string.Empty;
                }
                else
                {
                    mailformat = newMail;
                    ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                    returnValue = false;
                }
            }
            else
            {
                if (newMail != string.Empty)
                { 
                    if (Utils.IsValidEmailId(newMail))
                    {
                        mailformat = newMail;
                        returnValue = true;
                        ErrorMessage2 = string.Empty;
                    }
                    else
                    {
                        mailformat = newMail;
                        ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        catch (Exception)
        {
            ErrorMessage2 = string.Empty;
            returnValue = false;
        }

        return returnValue;
    }


    public static bool WithoutValidationMail(string newMail, bool isRequired, out string ErrorMessage1, out string mailformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        mailformat = string.Empty;

        if (isRequired)
        {
            if (newMail == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    //-------------------------------------PROVERA PIB-a----------------------------------------------
    //------------------------------------------------------------------------------------------------

    public static bool ValidatePIB(string newPIB, bool isRequired, out string ErrorMessage2, out string pibformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage2 = string.Empty;
        pibformat = newPIB;

        try
        {
            if (isRequired)
            {
                if (newPIB == string.Empty)
                {
                    ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = ValidatePIBWithoutRequired(newPIB, out ErrorMessage2, out pibformat);
                }
            }
            else
            {
                if (newPIB != string.Empty)
                {
                    returnValue = ValidatePIBWithoutRequired(newPIB, out ErrorMessage2, out pibformat);
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        catch (Exception)
        {
            ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
            returnValue = false;
        }

        return returnValue;
    }

    public static bool ValidatePIBWithoutRequired(string newPIB, out string ErrorMessage2, out string pibformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage2 = string.Empty;
        pibformat = newPIB;

        if (!Utils.allowNumbers(newPIB))
        {
            ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
            returnValue = false;
        }
        else if (newPIB.Length < Constants.LEGAL_ENTITY_PIB)
        {
            ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2336) + Constants.LEGAL_ENTITY_PIB + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2337);
            returnValue = false;
        }
        else if (newPIB.Length == Constants.LEGAL_ENTITY_PIB)
        {
            bool k = false;

            //**************Kontrolni broj računa se za niz od osam numeričkih cifara po međunarodnom standardu ISO 7064, MODUL (11,10).
            if (CheckDigits.VerifyNumericCheckDigit(newPIB, false))
                k = true;

            /*
            int[] b = new int[2];
            b[0] = Convert.ToInt32("" + newPIB.Substring(0, 1) + newPIB.Substring(1, 1) + newPIB.Substring(2, 1) + newPIB.Substring(3, 1) + newPIB.Substring(4, 1) + newPIB.Substring(5, 1) + newPIB.Substring(6, 1) + newPIB.Substring(7, 1));
            b[1] = Convert.ToInt32("" + newPIB.Substring(8, 1));

            string[] c = new string[2];
            c[0] = Utils.getPreparedPIB(b[0].ToString());
            c[1] = b[1].ToString();

            //**************Kontrolni broj računa se za niz od osam numeričkih cifara po međunarodnom standardu ISO 7064, MODUL (11,10).
            string kalknumber1 = Utils.AddCheckDigit(c[0]);

            bool k = false;
            //**************Redni brojevi registracije poreskih obveznika počinju od broja 10000001 i završavaju se brojem 99999999.
            if (Enumerable.Range(10000001, 99999999).Contains(b[0]) && c[1].Equals(kalknumber1))
                k = true;
            */
            if (!k)
            {
                ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                returnValue = false;
            }
            else
            {
                //errLabelNumber.Text = "Ispravan Broj telefona!";                   
                //string C0 = newPIB.Substring(0, 1) + newPIB.Substring(1, 1) + newPIB.Substring(2, 1) + newPIB.Substring(3, 1) + newPIB.Substring(4, 1) + newPIB.Substring(5, 1) + newPIB.Substring(6, 1) + newPIB.Substring(7, 1);
                //string C1 = newPIB.Substring(8, 1);
                //pibformat = C0 + C1;
                pibformat = newPIB;
                ErrorMessage2 = string.Empty;
                returnValue = true;
            }
        }
        else
        {
            ErrorMessage2 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2334);
            returnValue = false;
        }

        return returnValue;
    }

    public static bool WithoutValidationPIB(string newPIB, bool isRequired, out string ErrorMessage1, out string pibformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        pibformat = string.Empty;

        if (isRequired)
        {
            if (newPIB == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                ErrorMessage1 = string.Empty;
                returnValue = true;
            }
        }
        else
        {
            ErrorMessage1 = string.Empty;
            returnValue = true;
        }

        return returnValue;
    }
    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------

    public static bool ValidateName(string newName, bool isRequired, bool isValidating, out string ErrorMessage1, out string nameformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        nameformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newName == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLatinLettersMinusSpaceApostrophe(newName))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2338);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newName == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newName != string.Empty)
                {
                    if (!Utils.allowLatinLettersMinusSpaceApostrophe(newName))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2338);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateSurname(string newSurname, bool isRequired, bool isValidating, out string ErrorMessage1, out string nameformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        nameformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newSurname == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLatinLettersMinusSpaceApostrophe(newSurname))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2338);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newSurname == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newSurname != string.Empty)
                {
                    if (!Utils.allowLatinLettersMinusSpaceApostrophe(newSurname))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2338);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }


    public static bool ValidateIDDocument(string newIDDocument, bool isRequired, bool isValidating, out string ErrorMessage1, out string IDDocumentformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        IDDocumentformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newIDDocument == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLettersNumbersSpace(newIDDocument))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2339);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newIDDocument == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newIDDocument != string.Empty)
                {
                    if (!Utils.allowLettersNumbersSpace(newIDDocument))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2339);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateInstitutionName(string newInstitutionName, bool isRequired, bool isValidating, out string ErrorMessage1, out string InstitutionNameformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        InstitutionNameformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newInstitutionName == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLettersSpace(newInstitutionName))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2340);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newInstitutionName == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newInstitutionName != string.Empty)
                {
                    if (!Utils.allowLettersSpace(newInstitutionName))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2340);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateSertJMBG(string SelectedValue, bool isRequired, string IDItem, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            returnValue = true;
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateSertAdresa(string SelectedValue, bool isRequired, string IDItem, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateIssuingDate(DateTime datumizdavanja, bool isRequired, bool isValidating, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (datumizdavanja.ToString() == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3357);
                    returnValue = false;
                }
                else if (datumizdavanja > DateTime.ParseExact(DateTime.Now.ToString("dd.MM.yyy"), "dd.MM.yyyy", null))
                {
                    log.Info("DateTimeNow je: " + DateTime.ParseExact(DateTime.Now.ToString("dd.MM.yyy"), "dd.MM.yyyy", null));
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3357);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (datumizdavanja.ToString() == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3357);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                log.Info("datumizdavanja string je: " + datumizdavanja.ToString("dd.MM.yyy"));
                if (datumizdavanja.ToString("dd.MM.yyy") != string.Empty)
                {
                    log.Info("DateTimeNow je: " + DateTime.ParseExact(DateTime.Now.ToString("dd.MM.yyy"), "dd.MM.yyyy", null));
                    if (datumizdavanja > DateTime.ParseExact(DateTime.Now.ToString("dd.MM.yyy"), "dd.MM.yyyy", null))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3357);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }
        return returnValue;
    }

    public static bool ValidateExpiryDate(DateTime datumisteka, bool isRequired, bool isValidating, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (datumisteka.ToString() == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (datumisteka < DateTime.ParseExact(DateTime.Now.ToString("dd.MM.yyy"), "dd.MM.yyyy", null))
                {
                    log.Info("DateTimeNow je: " + DateTime.ParseExact(DateTime.Now.ToString("dd.MM.yyy"), "dd.MM.yyyy", null));
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2341);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (datumisteka.ToString() == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                log.Info("datumisteka string je: " + datumisteka.ToString("dd.MM.yyy"));
                if (datumisteka.ToString() != string.Empty)
                {
                    if (datumisteka < DateTime.ParseExact(DateTime.Now.ToString("dd.MM.yyy"), "dd.MM.yyyy", null))
                    {
                        log.Info("DateTimeNow je: " + DateTime.ParseExact(DateTime.Now.ToString("dd.MM.yyy"), "dd.MM.yyyy", null));
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2341);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateDateOfBirth(DateTime datumrodjenja, bool isRequired, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (datumrodjenja.ToString() == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateCity(string newCity, bool isRequired, bool isValidating, out string ErrorMessage1, out string newCityformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newCityformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newCity == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLettersSpaceBracketsLines(newCity))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2340);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newCity == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newCity != string.Empty)
                {
                    if (!Utils.allowLettersSpace(newCity))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2340);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateStreet(string newStreet, bool isRequired, bool isValidating, out string ErrorMessage1, out string newStreetformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newStreetformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newStreet == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLettersNumbersDotMinusSpace(newStreet))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2342);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newStreet == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newStreet != string.Empty)
                {
                    if (!Utils.allowLettersNumbersDotMinusSpace(newStreet))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2342);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateHouseNumber(string newHouseNumber, string errLabel, bool isRequired, bool isValidating, out string ErrorMessage1, out string newHouseNumberformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newHouseNumberformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newHouseNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLettersNumbersMinusSlashSpace(newHouseNumber))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2343);
                    returnValue = false;
                }
                else if (errLabel != string.Empty)
                {
                    ErrorMessage1 = errLabel;
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newHouseNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newHouseNumber != string.Empty)
                {
                    if (!Utils.allowLettersNumbersMinusSlashSpace(newHouseNumber))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2343);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidatePostNumber(string newPostNumber, bool isRequired, bool isValidating, out string ErrorMessage1, out string newPostNumberformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newPostNumberformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newPostNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowNumbers(newPostNumber))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                    returnValue = false;
                }
                else if (newPostNumber.Length < Constants.LEGAL_ENTITY_POSTANSKI_BROJ)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2336) + Constants.LEGAL_ENTITY_POSTANSKI_BROJ + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2337);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newPostNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newPostNumber != string.Empty)
                {
                    if (!Utils.allowNumbers(newPostNumber))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidatePAK(string newPAKNumber, bool isRequired, bool isValidating, out string ErrorMessage1, out string newPAKformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newPAKformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newPAKNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowNumbers(newPAKNumber))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newPAKNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newPAKNumber != string.Empty)
                {
                    if (!Utils.allowNumbers(newPAKNumber))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateRok(string SelectedValue, bool isRequired, string IDItem, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateMedij(string SelectedValue, bool isRequired, string IDItem, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateNacinPlacanja(string SelectedValue, bool isRequired, string IDItem, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }


    public static bool ValidatePassportNumber(string newPassportNumber, bool isRequired, bool isValidating, out string ErrorMessage1, out string IDDocumentformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        IDDocumentformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newPassportNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLettersNumbersSpace(newPassportNumber))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2339);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newPassportNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newPassportNumber != string.Empty)
                {
                    if (!Utils.allowLettersNumbersSpace(newPassportNumber))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2339);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateImeDrzave(string SelectedValue, bool isRequired, string IDItem, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateAdresaEPoste(string newEmail, out string ErrorMessage1, out string newEmailformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newEmailformat = string.Empty;

        if (newEmail == string.Empty)
        {
            ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
            returnValue = false;
        }
        else
        {
            ErrorMessage1 = string.Empty;
            returnValue = true;
        }

        return returnValue;
    }


    public static bool WithoutValidationKontaktNumber(string newNumber, bool isRequired, bool LegalEntityPhone, out string ErrorMessage1, out string newNumberformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newNumberformat = string.Empty;

        if (isRequired)
        {
            if (newNumber == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                ErrorMessage1 = string.Empty;
                returnValue = true;
            }
        }
        else
        {
            ErrorMessage1 = string.Empty;
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateWEBurl(string newURL, bool isRequired, out string ErrorMessage1, out string newURLformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newURLformat = string.Empty;

        if (isRequired)
        {
            if (newURL == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else if (!Utils.IsValidURL(newURL))
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                returnValue = false;
            }
            else
            {
                ErrorMessage1 = string.Empty;
                returnValue = true;
            }
        }
        else
        {
            if (newURL != string.Empty)
            { 
                if (newURL != string.Empty)
                {                  
                    if (!Utils.IsValidURL(newURL))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                        returnValue = false;
                    }
                }
            }
            else
            {
                ErrorMessage1 = string.Empty;
                returnValue = true;
            }
        }

        newURLformat = newURL;
        return returnValue;
    }

    public static bool WithoutValidateWEBurl(string newURL, bool isRequired, out string ErrorMessage1, out string newURLformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newURLformat = string.Empty;

        if (isRequired)
        {
            if (newURL == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                ErrorMessage1 = string.Empty;
                returnValue = true;
            }
        }
        else
        {
            ErrorMessage1 = string.Empty;
            returnValue = true;
        }

        newURLformat = newURL;
        return returnValue;
    }

    public static bool ValidateNazivPravnogLica(string newLegalName, bool isRequired, bool isValidating, out string ErrorMessage1, out string newLegalNameformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newLegalNameformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newLegalName == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLatinLettersMinusSpaceApostropheAndPlusDotCommaQuotationMarksNumbers(newLegalName))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresakaItemValue(Constants.ITEM_ERROR, Constants.ERROR_8388);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newLegalName == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newLegalName != string.Empty)
                {
                    if (!Utils.allowLatinLettersMinusSpaceApostropheAndPlusDotCommaQuotationMarksNumbers(newLegalName))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresakaItemValue(Constants.ITEM_ERROR, Constants.ERROR_8388);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateObveznikPDV(string SelectedValue, bool isRequired, string IDItem, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateSifraDel(string newSifraDel, bool isRequired, bool isValidating, out string ErrorMessage1, out string newSifraDelformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        newSifraDelformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newSifraDel == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (newSifraDel.Length < Constants.LEGAL_ENTITY_SIFRA_DELATNOSTI)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2336) + Constants.LEGAL_ENTITY_SIFRA_DELATNOSTI + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2337);
                    returnValue = false;
                }
                else if (!Utils.allowNumbers(newSifraDel))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newSifraDel == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newSifraDel != string.Empty)
                {
                    if (!Utils.allowNumbers(newSifraDel))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateImeZZ(string newNameZZ, bool isRequired, bool isValidating, out string ErrorMessage1, out string nameZZformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        nameZZformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newNameZZ == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLatinLettersMinusSpaceApostrophe(newNameZZ))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2338);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newNameZZ == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newNameZZ != string.Empty)
                {
                    if (!Utils.allowLatinLettersMinusSpaceApostrophe(newNameZZ))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2338);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidatePrezimeZZ(string newSurnameZZ, bool isRequired, bool isValidating, out string ErrorMessage1, out string surnameZZformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        surnameZZformat = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (newSurnameZZ == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLatinLettersMinusSpaceApostrophe(newSurnameZZ))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2338);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (newSurnameZZ == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (newSurnameZZ != string.Empty)
                {
                    if (!Utils.allowLatinLettersMinusSpaceApostrophe(newSurnameZZ))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2338);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateGridView(DataTable dtCurrentTable, out string ErrorMessage1, out string gridformat)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        gridformat = string.Empty;

        if (dtCurrentTable.Rows.Count == 0)
        {
            ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2344);
            returnValue = false;
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateLegalEntityName(string FullNameDefault, string SelectedValue, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (FullNameDefault == SelectedValue)
        {
            ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
            returnValue = false;
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateIdentificationNumber(string IdentificationNumber, bool isRequired, bool isValidating, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (IdentificationNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowNumbers(IdentificationNumber))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                    returnValue = false;
                }
                else if (IdentificationNumber.Length < Constants.LEGAL_ENTITY_MATICNI_BROJ)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2336) + Constants.LEGAL_ENTITY_MATICNI_BROJ + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2337);
                    returnValue = false;
                }
                else if (!Utils.CheckRegistrationNumber(IdentificationNumber))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2332);
                    returnValue = false;
                }
                else
                {
                    ErrorMessage1 = string.Empty;
                    returnValue = true;
                }
            }
            else
            {
                if (IdentificationNumber == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    ErrorMessage1 = string.Empty;
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (IdentificationNumber != string.Empty)
                {
                    if (!Utils.allowNumbers(IdentificationNumber))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                        returnValue = false;
                    }
                }
                else
                {
                    ErrorMessage1 = string.Empty;
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateJIK(string JIK, bool isRequired, bool isValidating, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (JIK == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowNumbers(JIK))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                    returnValue = false;
                }
                else if (JIK.Length < Constants.JIK)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2336) + Constants.JIK + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2337);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (JIK == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (JIK != string.Empty)
                {
                    if (!Utils.allowNumbers(JIK))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateOldJIK(string JIK, bool isRequired, bool isValidating, out string ErrorMessage1, out bool radiobtnstatus)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;
        radiobtnstatus = true;

        if (isRequired)
        {
            if (isValidating)
            {
                if (JIK == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowNumbers(JIK))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                    returnValue = false;
                }
                else if (JIK.Length < Constants.JIK)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2336) + Constants.JIK + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2337);
                    returnValue = false;
                }
                else if (Convert.ToInt32(JIK) <= Constants.USI)
                {
                    ErrorMessage1 = string.Empty;
                    radiobtnstatus = false;
                    returnValue = true;
                }
                else if (Convert.ToInt32(JIK) > Constants.USI)
                {
                    ErrorMessage1 = string.Empty;
                    radiobtnstatus = true;
                    returnValue = true;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (JIK == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (JIK != string.Empty)
                {
                    if (!Utils.allowNumbers(JIK))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }


    public static bool ValidateDrugo(string Drugo, bool isRequired, bool isValidating, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (Drugo == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLatinLettersNumbersDotSpace(Drugo))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2346);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (Drugo == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (Drugo != string.Empty)
                {
                    if (!Utils.allowLatinLettersNumbersDotSpace(Drugo))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2346);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateOstalo(string Ostalo, bool isRequired, bool isValidating, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (Ostalo == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowLatinLettersNumbersDotSpace(Ostalo))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2347);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (Ostalo == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (Ostalo != string.Empty)
                {
                    if (!Utils.allowLatinLettersNumbersDotSpace(Ostalo))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2347);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                } 
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateNacinPromene(string SelectedValue, bool isRequired, string IDItem1, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem1)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateJmbgiBrojPasosa(string jmbgibrojpasosa, bool isRequired, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (jmbgibrojpasosa == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateKarticaToken(string karticatoken, bool isRequired, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (karticatoken == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateRadioButtons(RadioButton radiobutton1, RadioButton radiobutton2, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (radiobutton1.Checked || radiobutton2.Checked)
        {
            returnValue = true;
        }
        else
        {
            ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3375);
            returnValue = false;
        }

        return returnValue;
    }

    public static bool ValidateBrojZahteva(string brojzahteva, bool isRequired, bool isValidating, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (brojzahteva == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else if (!Utils.allowNumbers(brojzahteva))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_3379);
                    returnValue = false;
                }
                else if (brojzahteva.Length < Constants.BROJ_ZAHTEVA)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2336) + Constants.BROJ_ZAHTEVA + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2337);
                    returnValue = false;
                }
                else if ((Convert.ToInt32(brojzahteva) < Constants.REQUEST_NUMBER) || (Convert.ToInt32(brojzahteva) > Constants.REQUEST_NUMBER_END))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2351) + Constants.REQUEST_NUMBER + utility.pronadjiNaziveGresakaItemValue(Constants.ITEM_ERROR, Constants.ERROR_8402) + Constants.REQUEST_NUMBER_END;
                    returnValue = false;
                }
                else if (brojzahteva.Length == Constants.BROJ_ZAHTEVA)
                {
                    string OrderNumberRangeStart = (utility.pronadjiPocetakOpsegaKrovnogZahteva()).ToString();
                    if (brojzahteva.Substring(0, 1) == OrderNumberRangeStart.Substring(0, 1))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresakaItemValue(Constants.ITEM_ERROR, Constants.ERROR_8392);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (brojzahteva == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (brojzahteva != string.Empty)
                {
                    if (!Utils.allowNumbers(brojzahteva))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2331);
                        returnValue = false;
                    }
                    else if (brojzahteva.Length < Constants.BROJ_ZAHTEVA)
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2336) + Constants.BROJ_ZAHTEVA + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2337);
                        returnValue = false;
                    }
                    else if ((Convert.ToInt32(brojzahteva) < Constants.REQUEST_NUMBER) || (Convert.ToInt32(brojzahteva) > Constants.REQUEST_NUMBER_END))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2351) + Constants.REQUEST_NUMBER + utility.pronadjiNaziveGresakaItemValue(Constants.ITEM_ERROR, Constants.ERROR_8402) + Constants.REQUEST_NUMBER_END;
                        returnValue = false;
                    }
                    else if (brojzahteva.Length == Constants.BROJ_ZAHTEVA)
                    {
                        string OrderNumberRangeStart = (utility.pronadjiPocetakOpsegaKrovnogZahteva()).ToString();
                        if (brojzahteva.Substring(0, 1) == OrderNumberRangeStart.Substring(0, 1))
                        {
                            ErrorMessage1 = utility.pronadjiNaziveGresakaItemValue(Constants.ITEM_ERROR, Constants.ERROR_8392);
                            returnValue = false;
                        }
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool ValidateChallenge(string challenge, bool isRequared, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequared)
        {
            if (challenge == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }


    public static bool ValidateSertifikat(string sertifikat, bool isRequired, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (sertifikat == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateSerialNo(string serialno, bool isRequired, bool isValidating, int PropertyValueMin, int PropertyValueMax, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (isValidating)
            {
                if (serialno == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2358);
                    returnValue = false;
                }
                else if (!Utils.allowHEXLettersNumbers(serialno))
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2359);
                    returnValue = false;
                }
                else if (PropertyValueMin == PropertyValueMax)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2360) + PropertyValueMin + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2361);
                }
                else if (serialno.Length < PropertyValueMin || serialno.Length > PropertyValueMax)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2362) + PropertyValueMin + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2363) + PropertyValueMax + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2361);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                if (serialno == string.Empty)
                {
                    ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2358);
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }
        }
        else
        {
            if (isValidating)
            {
                if (serialno != string.Empty)
                {
                    if (!Utils.allowHEXLettersNumbers(serialno))
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2359);
                        returnValue = false;
                    }
                    else if (PropertyValueMin == PropertyValueMax)
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2360) + PropertyValueMin + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2361);
                    }
                    else if (serialno.Length < PropertyValueMin || serialno.Length > PropertyValueMax)
                    {
                        ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2362) + PropertyValueMin + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2363) + PropertyValueMax + utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2361);
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }       
        }

        return returnValue;
    }

    public static bool ValidateImeIzdavaoca(string SelectedValue, bool isRequired, string IDItem1, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem1)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateKodoviZaPreuzimanje(string kodovi, bool isRequired, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (kodovi == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool ValidateNacinSlanja(string SelectedValue, bool isRequired, string IDItem1, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem1)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }


    public static bool ValidateListaSertifikata(string SelectedValue, bool isRequired, string IDItem, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (SelectedValue == IDItem)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }


    public static bool ValidateReadOnlyFields(string FieldString, bool isRequired, out string ErrorMessage1)
    {
        Utility utility = new Utility();
        bool returnValue = true;
        ErrorMessage1 = string.Empty;

        if (isRequired)
        {
            if (FieldString == string.Empty)
            {
                ErrorMessage1 = utility.pronadjiNaziveGresaka(Constants.ITEM_ERROR, Constants.ERROR_2335);
                returnValue = false;
            }
            else
            {
                returnValue = true;
            }
        }
        else
        {
            returnValue = true;
        }

        return returnValue;
    }

    public static bool GetPostNumberAndPAK(string Mesto, string Ulica, string Broj, out string PorukaKorisnik, out string PostanskiBroj, out string Pak)
    {
        bool returnValue = true;
        PorukaKorisnik = string.Empty;
        PostanskiBroj = string.Empty;
        Pak = string.Empty;
        //////////////////PROVERA ADRESE  //////////////1 pošiljalac  29 (Danas za sutra do 12)////////////////////////
        List<string> responses = new List<string>();

        try
        {
            rs.posta.wsp.WSPWrapperService obj = new rs.posta.wsp.WSPWrapperService();

            Guid guid1 = Guid.NewGuid();
            int servis = 3;
            int idVrstaTransakcije = 6; //ProveraAdrese = 6 – Provera ispravnosti adrese

            Utility utility = new Utility();

            // Serializaion  
            Klijent klijent = new Klijent();
            klijent.Username = utility.pronadjiKorisnickoImeILozinku(Constants.ITEM_USER_PASS, Constants.USERNAME);
            klijent.Password = utility.pronadjiKorisnickoImeILozinku(Constants.ITEM_USER_PASS, Constants.PASSWORD);
            klijent.Jezik = "LAT";
            klijent.IdTipUredjaja = 1;
            klijent.ModelUredjaja = "Test";
            klijent.NazivUredjaja = "Test";
            klijent.VerzijaOS = "Test";
            klijent.VerzijaAplikacije = "Test";
            klijent.IPAdresa = "Test";
            klijent.Geolokacija = "Test";

            // Convert Klijent object to XML string format   
            string xmlData = Serijalizator.SerializeObject<Klijent>(klijent);

            // Serializaion
            ProveraAdreseIn proveraadresein = new ProveraAdreseIn();
            proveraadresein.TipAdrese = 1; //0 preuzimanje, 1 pošiljalac, 2 primalac
            proveraadresein.IdRukovanje = 1; //1 (Pismonosna dostava)  29 (Danas za sutra do 12)
                                             //string contextKeyNaselje = Context.Session["contextKey"].ToString();
                                             //proveraadresein.IdNaselje = getIdNaselje(contextKeyNaselje);
                                             //string contextKeyUlica = contextKey;
                                             //proveraadresein.IdUlica = getIdUlice(contextKeyUlica);
            string contextKeyNaselje = Mesto;
            proveraadresein.IdNaselje = getIdNaselje(contextKeyNaselje);
            string contextKeyUlica = Ulica;
            proveraadresein.IdUlica = getIdUlice(contextKeyUlica);
            proveraadresein.BrojPodbroj = Broj;
            proveraadresein.Posta = "";

            // Convert  object to XML string format   
            string xmlData2 = Serijalizator.SerializeObject<ProveraAdreseIn>(proveraadresein);

            string objStrKlijentString = xmlData;
            string objProveraAdreseIn = xmlData2;
            string ProveraAdreseOutString = string.Empty;
            string Rezultat = string.Empty;

            //Funkcija koja poziva Web servis
            int result = obj.Transakcija(objStrKlijentString, servis, idVrstaTransakcije, guid1, objProveraAdreseIn, out ProveraAdreseOutString, out Rezultat);

            if (ProveraAdreseOutString == null)
            {
                //deserialize
                Rezultat rezultatout = new Rezultat();
                rezultatout = Serijalizator.Deserialize<Rezultat>(Rezultat);

                PorukaKorisnik = rezultatout.PorukaKorisnik;

                returnValue = false;
            }
            else if (ProveraAdreseOutString != null)
            {
                //deserialize
                ProveraAdreseOut proveraadreseout = new ProveraAdreseOut();
                proveraadreseout = Serijalizator.Deserialize<ProveraAdreseOut>(ProveraAdreseOutString);

                PostanskiBroj = proveraadreseout.PostanskiBroj;
                Pak = proveraadreseout.Pak;

                PorukaKorisnik = string.Empty;

                returnValue = true;
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in function GetPostNumberAndPAK. " + ex.Message);
        }

        return returnValue;
    }

    private static int getIdNaselje(string nazivNaselja)
    {
        try
        {
            //Store and retreive values between web methods, with Session
            List<Naselje> Naselja = (List<Naselje>)HttpContext.Current.Session["WebServiceMesta-Naselja"];
            foreach (var naselje in Naselja)
            {
                if (nazivNaselja.Equals(naselje.Naziv, StringComparison.InvariantCultureIgnoreCase))
                {
                    return naselje.Id;
                }
            }
            return 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    private static int getIdUlice(string nazivUlice)
    {
        try
        {
            //Store and retreive values between web methods, with Session
            List<Ulica> Ulice = (List<Ulica>)HttpContext.Current.Session["WebServiceMesta-Ulice"];
            foreach (var ulice in Ulice)
            {
                if (nazivUlice.Equals(ulice.Naziv, StringComparison.InvariantCultureIgnoreCase))
                {
                    return ulice.Id;
                }
            }
            return 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}