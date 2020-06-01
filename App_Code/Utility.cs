using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

/// <summary>
/// Summary description for Utility
/// </summary>
public class Utility
{
    public string postaconnectionstring = ConfigurationManager.ConnectionStrings["POSTAConnectionString"].ToString();
    //Lofg4Net declare log variable
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public Utility()
    {

    }

    public bool IsAvailableConnection()
    {
        try
        {
            SqlConnection objConn = new SqlConnection(postaconnectionstring);
            objConn.Open();
            objConn.Close();
        }
        catch (SqlException ex)
        {
            log.Error("Error while connecting to Database. " + ex.Message);
            return false;
        }
        return true;
    }

    public List<WebControl> pronadjiKontrole(string page)
    {
        try
        {
            List<WebControl> responses = new List<WebControl>();
            SqlConnection objConn = new SqlConnection(postaconnectionstring);
            SqlCommand objCmd = new SqlCommand("SELECT WebControl.ControlId, WebControl.ValidationActive,WebControl.IsRequired FROM WebControl INNER JOIN WebPage ON WebControl.IDWebPage = WebPage.IDWebPage WHERE WebPage.FileName ='" + page + "'", objConn);
            objCmd.CommandType = System.Data.CommandType.Text;
            objConn.Open();
            SqlDataReader reader = objCmd.ExecuteReader();
            while (reader.Read())
            {
                responses.Add(new WebControl((reader.GetSqlString(0)).ToString(), reader.GetBoolean(1), reader.GetBoolean(2)));
            }
            objConn.Close();

            return responses;
        }
        catch (Exception ex)
        {
            log.Error("Error while sellecting pronadjiKontrole. " + ex.Message);
            throw new Exception("Error while sellecting pronadjiKontrole. " + ex.Message);
        }
    }

    public List<WebControl> pronadjiKontrolePoTipu(string page, string controltype)
    {
        try
        {
            List<WebControl> responses = new List<WebControl>();
            SqlConnection objConn = new SqlConnection(postaconnectionstring);
            SqlCommand objCmd = new SqlCommand("SELECT WebControl.ControlId, WebControl.ValidationActive,WebControl.IsRequired FROM WebControl INNER JOIN WebPage ON WebControl.IDWebPage = WebPage.IDWebPage WHERE WebPage.FileName ='" + page + "' AND WebControl.ControlType = '" + controltype + "'", objConn);
            objCmd.CommandType = System.Data.CommandType.Text;
            objConn.Open();
            SqlDataReader reader = objCmd.ExecuteReader();
            while (reader.Read())
            {
                responses.Add(new WebControl((reader.GetSqlString(0)).ToString(), reader.GetBoolean(1), reader.GetBoolean(2)));
            }
            objConn.Close();

            return responses;
        }
        catch (Exception ex)
        {
            log.Error("Error while sellecting pronadjiKontrolePoTipu. " + ex.Message);
            throw new Exception("Error while sellecting pronadjiKontrolePoTipu. " + ex.Message);
        }
    }

    public List<PTTVariable> pronadjiPromenljivePTT(string filename, string controlid)
    {
        List<PTTVariable> responses = new List<PTTVariable>();

        string upit = @"SELECT  Item.IDItem, ISNULL(LegalEntity.City, N'') AS City, ISNULL(LegalEntity.Street, N'') AS Street, ISNULL(LegalEntity.HouseNumber, N'') AS HouseNumber, ISNULL(LegalEntity.ZipCode, N'') AS ZipCode, 
                         ISNULL(LegalEntity.PAK, N'') AS PAK, ISNULL(LegalEntity.InHouse, 0) AS InHouse, ControlItem.IsAllowed, CASE WHEN Item.Itemtextenglish = 'LEGAL_ENTITY_ADDRESS' THEN 1 ELSE 0 END AS IsLegalEntity

                         FROM  LegalEntity INNER JOIN
                         ItemEntity ON LegalEntity.IDLegalEntity = ItemEntity.IDLegalEntity RIGHT OUTER JOIN
                         WebPage INNER JOIN
                         WebControl ON WebPage.IDWebPage = WebControl.IDWebPage INNER JOIN
                         ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                         Item ON ControlItem.IDItem = Item.IDItem ON ItemEntity.IDItem = Item.IDItem
                         WHERE  (WebControl.ControlId = @controlid) AND (WebPage.FileName = @filename)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(new PTTVariable(reader.GetInt32(0), reader.GetSqlString(1).ToString(), reader.GetSqlString(2).ToString(), reader.GetSqlString(3).ToString(), reader.GetSqlString(4).ToString(), reader.GetSqlString(5).ToString(), reader.GetBoolean(6), reader.GetBoolean(7), reader.GetInt32(8)));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting PTTVariable. " + ex.Message);
                    throw new Exception("Error while getting PTTVariable. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public string getEnglishText(int idItem)
    {
        string itemTextEnglish = string.Empty;

        string upit = @"SELECT        TOP (1) ItemTextEnglish
                       FROM            dbo.Item
                       WHERE        (IDItem = @idItem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemTextEnglish = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemTextEnglish. " + ex.Message);
                    throw new Exception("Error while getting itemTextEnglish. " + ex.Message);
                }
            }
        }

        return itemTextEnglish;
    }

    public string getEnglishTextInputString(string itemText)
    {
        string itemTextEnglish = string.Empty;

        string upit = @"SELECT        TOP (1) ItemTextEnglish
                        FROM            dbo.Item
                        WHERE        (ItemText = @itemtext)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@itemtext", itemText);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemTextEnglish = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemTextEnglish while input string. " + ex.Message);
                    throw new Exception("Error while getting itemTextEnglish while input string. " + ex.Message);
                }
            }
        }

        return itemTextEnglish;
    }

    public string getEnglishTextItemText(string itemtext)
    {
        string itemTextEnglish = string.Empty;

        string upit = @"SELECT        TOP (1) ItemTextEnglish
                        FROM            dbo.Item
                        WHERE        (ItemText = @itemtext)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@itemtext", itemtext);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemTextEnglish = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemTextEnglish. " + ex.Message);
                    throw new Exception("Error while getting itemTextEnglish. " + ex.Message);
                }
            }
        }

        return itemTextEnglish;
    }

    public List<ItemVariable> getIdItemDefault(string filename, string controlid)
    {
        List<ItemVariable> responses = new List<ItemVariable>();

        string upit = @"SELECT  TOP (100) PERCENT Item.IDItem, Item.ItemText, ControlItem.IsDefault
                        FROM    WebControl INNER JOIN
                        WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                        ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                        Item ON ControlItem.IDItem = Item.IDItem
                        WHERE        (WebPage.FileName = @filename) AND (WebControl.ControlId = @controlid)
                        ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        responses.Add(new ItemVariable(reader.GetInt32(0), reader.GetSqlString(1).ToString(), reader.GetBoolean(2)));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting IdItemDefault. " + ex.Message);
                    throw new Exception("Error while getting IdItemDefault. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public string getItemText(int idItem)
    {
        string itemText = string.Empty;

        string upit = @"SELECT        TOP (1) ItemText
                       FROM            dbo.Item
                       WHERE        (IDItem = @idItem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemText = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemText. " + ex.Message);
                    throw new Exception("Error while getting itemText. " + ex.Message);
                }
            }
        }

        return itemText;
    }

    public int getItemValueAddedTax(int idItem)
    {
        int itemValue = 0;

        string upit = @"SELECT        TOP (1) ItemValue
                       FROM            dbo.Item
                       WHERE        (IDItem = @idItem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemValue = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemValue. " + ex.Message);
                    throw new Exception("Error while getting itemValue. " + ex.Message);
                }
            }
        }

        return itemValue;
    }

    public List<LegalEntityVariable> pronadjiPromenljiveLegalEntity(string IdentificationNumber)
    {
        List<LegalEntityVariable> responses = new List<LegalEntityVariable>();

        string upit = @"SELECT        IDLegalEntity, FullName, PIB, PDVpayer, BysinessTypeCode, Street, HouseNumber, ZipCode, PAK, City, PhoneNumber, EmailAddress
                        FROM          LegalEntity
                        WHERE        (IdentificationNumber = @IdentificationNumber)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@IdentificationNumber", IdentificationNumber);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(new LegalEntityVariable(reader.GetInt32(0), reader.GetSqlString(1).ToString(), reader.GetSqlString(2).ToString(), reader.GetBoolean(3), reader.GetSqlString(4).ToString(), reader.GetSqlString(5).ToString(), reader.GetSqlString(6).ToString(), reader.GetSqlString(7).ToString(), reader.GetSqlString(8).ToString(), reader.GetSqlString(9).ToString(), reader.GetSqlString(10).ToString(), reader.GetSqlString(11).ToString()));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting LegalEntityVariable. " + ex.Message);
                    throw new Exception("Error while getting LegalEntityVariable. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public int getIDItem(int itemvalue, string filename, string controlid)
    {
        int IDItem = 0;

        string upit = @"SELECT   TOP (100) PERCENT Item.IDItem
                        FROM     WebControl INNER JOIN
                        WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                        ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                        Item ON ControlItem.IDItem = Item.IDItem
                        WHERE  (WebPage.FileName = @filename) AND (WebControl.ControlId = @controlid) AND (Item.ItemValue = @itemvalue)
                        ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@itemvalue", System.Data.SqlDbType.Int).Value = itemvalue;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        IDItem = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting IDItem. " + ex.Message);
                    throw new Exception("Error while getting IDItem. " + ex.Message);
                }
            }
        }

        return IDItem;
    }

    public int getItemValue(int selectedvalue, string filename, string controlid)
    {
        int ItemValue = 0;

        string upit = @"SELECT  TOP (100) PERCENT Item.ItemValue
                        FROM   WebControl INNER JOIN
                        WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                        ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                        Item ON ControlItem.IDItem = Item.IDItem
                        WHERE  (WebControl.ControlId = @controlid) AND (WebPage.FileName = @filename) AND (Item.IDItem = @selectedvalue)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@selectedvalue", System.Data.SqlDbType.Int).Value = selectedvalue;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ItemValue = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting ItemValue. " + ex.Message);
                    throw new Exception("Error while getting ItemValue. " + ex.Message);
                }
            }
        }

        return ItemValue;
    }

    public List<StatusChangeVariable> pronadjiPromenljiveStatusChange(string filename, string controlid)
    {
        List<StatusChangeVariable> responses = new List<StatusChangeVariable>();

        string upit = @"SELECT  TOP (100) PERCENT Item.IDItem, ControlItem.IsAllowed, ControlItem.IsDefault, Item.ItemValue
                        FROM   WebPage INNER JOIN
                        WebControl ON WebPage.IDWebPage = WebControl.IDWebPage INNER JOIN
                        ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                        Item ON ControlItem.IDItem = Item.IDItem
                        WHERE   (WebControl.ControlId = @controlid) AND (WebPage.FileName = @filename) AND (dbo.ControlItem.Active = 1)
                        ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(new StatusChangeVariable(reader.GetInt32(0), reader.GetBoolean(1), reader.GetBoolean(2), reader.GetInt32(3)));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting StatusChangeVariable. " + ex.Message);
                    throw new Exception("Error while getting StatusChangeVariable. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public List<CertificateStatus> pronadjiPromenljiveStatusSertifikata(int IDTypeOfItem)
    {
        List<CertificateStatus> responses = new List<CertificateStatus>();

        string upit = @"SELECT Item.ItemText, Item.ItemTextEnglish, ISNULL(ItemCertificateStatus.Notification, N'') AS Notification, Item.ItemValue
                        FROM   Item INNER JOIN
                        TypeOfItem ON Item.IDTypeOfItem = TypeOfItem.IDTypeOfItem LEFT OUTER JOIN
                        ItemCertificateStatus ON Item.IDItem = ItemCertificateStatus.IDItem
                        WHERE  (TypeOfItem.IDTypeOfItem = @idtypeofitem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = IDTypeOfItem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(new CertificateStatus(reader.GetSqlString(0).ToString(), reader.GetSqlString(1).ToString(), reader.GetSqlString(2).ToString(), reader.GetInt32(3)));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting StatusVariable. " + ex.Message);
                    throw new Exception("Error while getting StatusVariable. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public void upisiPravnoLice(string maticnibroj, string nazivpravnoglica, string pib, bool obveznikpdv, string sifradel, string ulica, string broj, string postanskibroj, string pak, string mesto, string telefon, string adresaeposte, bool inhouse)
    {
        try
        {
            SqlConnection objConn = new SqlConnection(postaconnectionstring);
            SqlCommand objCmd = new SqlCommand(@"insert into LegalEntity (IdentificationNumber, FullName, PIB, PDVPayer, BysinessTypeCode, Street, HouseNumber, ZipCode, PAK, City, PhoneNumber, EmailAddress, InHouse) values (@maticnibroj, @nazivpravnoglica, @pib, @obveznikpdv, @sifradel, @ulica, @broj, @postanskibroj, @pak, @mesto, @telefon, @adresaeposte, @inhouse)", objConn);
            objCmd.CommandType = System.Data.CommandType.Text;

            objCmd.Parameters.AddWithValue("@maticnibroj", maticnibroj);
            objCmd.Parameters.AddWithValue("@nazivpravnoglica", nazivpravnoglica);
            objCmd.Parameters.AddWithValue("@pib", pib);
            objCmd.Parameters.AddWithValue("@obveznikpdv", obveznikpdv);
            objCmd.Parameters.AddWithValue("@sifradel", sifradel);
            objCmd.Parameters.AddWithValue("@ulica", ulica);
            objCmd.Parameters.AddWithValue("@broj", broj);
            objCmd.Parameters.AddWithValue("@postanskibroj", postanskibroj);
            objCmd.Parameters.AddWithValue("@pak", pak);
            objCmd.Parameters.AddWithValue("@mesto", mesto);
            objCmd.Parameters.AddWithValue("@telefon", telefon);
            objCmd.Parameters.AddWithValue("@adresaeposte", adresaeposte);
            objCmd.Parameters.AddWithValue("@inhouse", inhouse);
            objConn.Open();
            objCmd.ExecuteNonQuery();
            objConn.Close();
        }
        catch (Exception ex)
        {
            log.Error("Error while inserting LegalEntity values. " + ex.Message);
            throw new Exception("Error while inserting LegalEntity values. " + ex.Message);
        }
    }

    public void editujPravnoLice(string maticnibroj, string nazivpravnoglica, string pib, bool obveznikpdv, string sifradel, string ulica, string broj, string postanskibroj, string pak, string mesto, string telefon, string adresaeposte, bool inhouse)
    {
        try
        {
            SqlConnection objConn = new SqlConnection(postaconnectionstring);
            SqlCommand objCmd = new SqlCommand(@"update LegalEntity set IdentificationNumber = @maticnibroj, PIB = @pib, PDVPayer = @obveznikpdv, BysinessTypeCode = @sifradel, Street = @ulica, HouseNumber = @broj, ZipCode = @postanskibroj, PAK = @pak, City=@mesto, PhoneNumber = @telefon, EmailAddress = @adresaeposte, InHouse = @inhouse where FullName = @nazivpravnoglica", objConn);
            objCmd.CommandType = System.Data.CommandType.Text;

            objCmd.Parameters.AddWithValue("@maticnibroj", maticnibroj);
            objCmd.Parameters.AddWithValue("@nazivpravnoglica", nazivpravnoglica);
            objCmd.Parameters.AddWithValue("@pib", pib);
            objCmd.Parameters.AddWithValue("@obveznikpdv", obveznikpdv);
            objCmd.Parameters.AddWithValue("@sifradel", sifradel);
            objCmd.Parameters.AddWithValue("@ulica", ulica);
            objCmd.Parameters.AddWithValue("@broj", broj);
            objCmd.Parameters.AddWithValue("@postanskibroj", postanskibroj);
            objCmd.Parameters.AddWithValue("@pak", pak);
            objCmd.Parameters.AddWithValue("@mesto", mesto);
            objCmd.Parameters.AddWithValue("@telefon", telefon);
            objCmd.Parameters.AddWithValue("@adresaeposte", adresaeposte);
            objCmd.Parameters.AddWithValue("@inhouse", inhouse);
            objConn.Open();
            objCmd.ExecuteNonQuery();
            objConn.Close();
        }
        catch (Exception ex)
        {
            log.Error("Error while editting LegalEntity values. " + ex.Message);
            throw new Exception("Error while editting LegalEntity values. " + ex.Message);
        }
    }

    public List<FulNameLegalEntity> pronadjiNazivPravnogLica()
    {
        List<FulNameLegalEntity> responses = new List<FulNameLegalEntity>();

        string upit = @"SELECT        IDLegalEntity, FullName
                        FROM          LegalEntity";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(new FulNameLegalEntity(reader.GetInt32(0), reader.GetSqlString(1).ToString()));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting FulNameLegalEntity. " + ex.Message);
                    throw new Exception("Error while getting FulNameLegalEntity. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public string getItemTextSatus(string itemTextEnglish)
    {
        string itemText = string.Empty;

        string upit = @"SELECT        TOP (1) ItemText
                        FROM          Item
                        WHERE        (ItemTextEnglish = @itemtextenglish)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@itemtextenglish", itemTextEnglish);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemText = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemText. " + ex.Message);
                    throw new Exception("Error while getting itemText. " + ex.Message);
                }
            }
        }

        return itemText;
    }

    public string getItemTextEnglishSatus(string itemText)
    {
        string itemTextEng = string.Empty;

        string upit = @"SELECT        TOP (1) ItemTextEnglish
                        FROM          Item
                        WHERE        (ItemText = @itemtext)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@itemtext", itemText);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemTextEng = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemTextEng. " + ex.Message);
                    throw new Exception("Error while getting itemTextEng. " + ex.Message);
                }
            }
        }

        return itemTextEng;
    }

    public string getCertificateRoot(string SettingName)
    {
        string CertificateRoot = string.Empty;

        string upit = @"SELECT        SettingValue
                        FROM            dbo.GlobalSetting
                        WHERE        (SettingName = @settingname)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@settingname", SettingName);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        CertificateRoot = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting CertificateRoot. " + ex.Message);
                    throw new Exception("Error while getting CertificateRoot. " + ex.Message);
                }
            }
        }

        return CertificateRoot;
    }

    public string getCertificateName(int idItem)
    {
        string CertificateName = string.Empty;

        string upit = @"SELECT        TOP (1) CertificateName
                        FROM        dbo.ItemRevocationCheckMethod
                        WHERE        (IDItem = @idItem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        CertificateName = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting CertificateName. " + ex.Message);
                    throw new Exception("Error while getting CertificateName. " + ex.Message);
                }
            }
        }

        return CertificateName;
    }

    public int getOrderNumber(int IDOrderNumber)
    {    
        int LastUsed = 0;
        int OrderNumber = 0;

        string upit = @"SELECT TOP (1) LastUsed
                        FROM  OrderNumberRange
                        WHERE (IDOrderNumberRange = @idordernumber)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idordernumber", System.Data.SqlDbType.Int).Value = IDOrderNumber;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        LastUsed = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting LastUsed. " + ex.Message);
                    throw new Exception("Error while getting LastUsed. " + ex.Message);
                }
            }
        }
        OrderNumber = LastUsed + IDOrderNumber;
        log.Info("Last used is: " + LastUsed + ". OrderNumber is: " + OrderNumber);
        return OrderNumber;
    }

    public void editujPoslednjeKoriscenOrderNumber(int OrderNumber, int IDOrderNumber)
    {
        try
        {
            SqlConnection objConn = new SqlConnection(postaconnectionstring);
            SqlCommand objCmd = new SqlCommand(@"update OrderNumberRange set LastUsed = @ordernumber where IDOrderNumberRange = @IDOrderNumberRange", objConn);
            objCmd.CommandType = System.Data.CommandType.Text;

            objCmd.Parameters.Add("@ordernumber", System.Data.SqlDbType.Int).Value = OrderNumber;
            objCmd.Parameters.Add("@IDOrderNumberRange", System.Data.SqlDbType.Int).Value = IDOrderNumber;
            objConn.Open();
            objCmd.ExecuteNonQuery();
            objConn.Close();
        }
        catch (Exception ex)
        {
            log.Error("Error while editting OrderNumber value. " + ex.Message);
            throw new Exception("Error while editting OrderNumber value. " + ex.Message);
        }
    }

    public string getURLocsp(string filename, string controlid, int revocationchecktype, int selectedvalue)
    {
        string url = string.Empty;

        string upit = @"SELECT        TOP (1) PERCENT ItemRevocationCheckMethod.URL
                        FROM          WebControl INNER JOIN
                         WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                         ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                         Item ON ControlItem.IDItem = Item.IDItem LEFT OUTER JOIN
                         ItemRevocationCheckMethod ON Item.IDItem = ItemRevocationCheckMethod.IDItem
                         WHERE   (WebPage.FileName = @filename) AND (WebControl.ControlId = @controlid) AND (ControlItem.Active = 1) AND (ItemRevocationCheckMethod.RevocationCheckType = @revocationchecktype) AND (ItemRevocationCheckMethod.IDItem = @selectedvalue)
                         ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objCmd.Parameters.Add("@revocationchecktype", System.Data.SqlDbType.Int).Value = revocationchecktype;
                    objCmd.Parameters.Add("@selectedvalue", System.Data.SqlDbType.Int).Value = selectedvalue;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        url = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting revocation URL. " + ex.Message);
                    throw new Exception("Error while getting revocation URL. " + ex.Message);
                }
            }
        }

        return url;
    }

    public string getURLcrl(string filename, string controlid, int revocationchecktype)
    {
        string url = string.Empty;

        string upit = @"SELECT        TOP (1) PERCENT ItemRevocationCheckMethod.URL
                        FROM          WebControl INNER JOIN
                         WebPage ON WebControl.IDWebPage = WebPage.IDWebPage INNER JOIN
                         ControlItem ON WebControl.IDWebControl = ControlItem.IDWebControl INNER JOIN
                         Item ON ControlItem.IDItem = Item.IDItem LEFT OUTER JOIN
                         ItemRevocationCheckMethod ON Item.IDItem = ItemRevocationCheckMethod.IDItem
                         WHERE   (WebPage.FileName = @filename) AND (WebControl.ControlId = @controlid) AND (ControlItem.Active = 1) AND (ItemRevocationCheckMethod.RevocationCheckType = @revocationchecktype)
                         ORDER BY ControlItem.IsDefault DESC, ControlItem.SortOrder";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objCmd.Parameters.Add("@revocationchecktype", System.Data.SqlDbType.Int).Value = revocationchecktype;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        url = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting revocation URL. " + ex.Message);
                    throw new Exception("Error while getting revocation URL. " + ex.Message);
                }
            }
        }

        return url;
    }


    public List<SerialNoVariable> pronadjiPromenljiveSerialNo(string filename, string controlid)
    {
        List<SerialNoVariable> responses = new List<SerialNoVariable>();

        string upit = @"SELECT        TOP (100) PERCENT ISNULL(dbo.Property.PropertyName, N'') AS PropertyName, ISNULL(CAST(dbo.WebControlProperties.PropertyValue AS int), 0) AS PropertyValue
                        FROM            dbo.WebControl INNER JOIN
                                        dbo.WebPage ON dbo.WebControl.IDWebPage = dbo.WebPage.IDWebPage LEFT OUTER JOIN
                                        dbo.Property INNER JOIN
                                        dbo.WebControlProperties ON dbo.Property.IDProperty = dbo.WebControlProperties.IDProperty ON dbo.WebControl.IDWebControl = dbo.WebControlProperties.IDWebControl
                        WHERE        (dbo.WebPage.FileName = @filename) AND (dbo.WebControl.ControlId = @controlid)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(new SerialNoVariable(reader.GetSqlString(0).ToString(), reader.GetInt32(1)));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting SerialNoVariable. " + ex.Message);
                    throw new Exception("Error while getting SerialNoVariable. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public List<PhonePrefixVariable> pronadjiPrefikseMobilnihTelefona(int IDTypeOfItem)
    {
        List<PhonePrefixVariable> responses = new List<PhonePrefixVariable>();

        string upit = @"SELECT        TOP (100) PERCENT IDItem, ItemText, Active
                        FROM          Item
                        WHERE        (IDTypeOfItem = @idtypeofitem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = IDTypeOfItem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(new PhonePrefixVariable(reader.GetInt32(0), reader.GetSqlString(1).ToString(), reader.GetBoolean(2)));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting PhonePrefixVariable. " + ex.Message);
                    throw new Exception("Error while getting PhonePrefixVariable. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public List<WebControlLanguage> pronadjiSvaPoljaNaStranici(string filename)
    {
        List<WebControlLanguage> responses = new List<WebControlLanguage>();

        string upit = @"SELECT        TOP (100) PERCENT WebControlLanguage.ControlId, WebPageLanguage.PageTitle, WebControlLanguage.ControlTitle, WebControlLanguage.ValidationActive, WebControlLanguage.IsVisible, 
                         WebControlLanguage.IsEnabled, WebControlLanguage.IsRequired, WebControlLanguage.ControlType
                        FROM            WebControlLanguage INNER JOIN
                         WebPageLanguage ON WebControlLanguage.IDWebPage = WebPageLanguage.IDWebPage
                        WHERE        (WebPageLanguage.FileName = @filename)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        responses.Add(new WebControlLanguage(reader.GetSqlString(0).ToString(), reader.GetSqlString(1).ToString(), reader.GetSqlString(2).ToString(), reader.GetBoolean(3), reader.GetBoolean(4), reader.GetBoolean(5), reader.GetBoolean(6), reader.GetSqlString(7).ToString()));
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting WebControlLanguage. " + ex.Message);
                    throw new Exception("Error while getting WebControlLanguage. " + ex.Message);
                }
            }
        }

        return responses;
    }

    public string pronadjiNaziveGresaka(int idtypeofitem, int IDItem)
    {
        string itemText = string.Empty;

        string upit = @"SELECT        TOP (1) ItemText
                       FROM           dbo.Item
                       WHERE        (IDItem = @iditem) AND (IDTypeOfItem = @idtypeofitem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@iditem", System.Data.SqlDbType.Int).Value = IDItem;
                    objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = idtypeofitem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemText = reader.GetString(0);
                    }                   
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemText for Error message. " + ex.Message);
                    throw new Exception("Error while getting itemText for Error message. " + ex.Message);
                }
            }
        }

        return itemText;
    }

    public string pronadjiNaziveGresakaItemValue(int idtypeofitem, int ItemValue)
    {
        string itemText = string.Empty;

        string upit = @"SELECT        TOP (1) ItemText
FROM            dbo.Item
WHERE        (IDTypeOfItem = @idtypeofitem) AND (ItemValue = @itemvalue)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@itemvalue", System.Data.SqlDbType.Int).Value = ItemValue;
                    objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = idtypeofitem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemText = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemText in pronadjiNaziveGresakaItemValue for Error message. " + ex.Message);
                    throw new Exception("Error while getting itemText in pronadjiNaziveGresakaItemValue for Error message. " + ex.Message);
                }
            }
        }

        return itemText;
    }


    public string getSettingsValueGlobalSettings(string Validation)
    {
        string SettingsValue = string.Empty;

        string upit = @"SELECT        SettingValue
                        FROM            dbo.GlobalSetting
                        WHERE        (SettingName = @validation)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@validation", Validation);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        SettingsValue = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting SettingsValue. " + ex.Message);
                    throw new Exception("Error while getting SettingsValue. " + ex.Message);
                }
            }
        }

        return SettingsValue;
    }

    public bool pronadjiDaLiJeStranicaAktivna(string filename)
    {
        bool Active = true;

        string upit = @"SELECT        Active
                    FROM        dbo.WebPage
                    WHERE        (FileName = @filename)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Active = reader.GetBoolean(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting Active Value. " + ex.Message);
                    throw new Exception("Error while getting Active Value. " + ex.Message);
                }
            }
        }

        return Active;
    }

    public bool pronadjiDaLiJeStranicaUputstvoAktivna(string filename)
    {
        bool ActiveAgreeme = true;

        string upit = @"SELECT        ShowAgreement
                        FROM            dbo.WebPage
                        WHERE        (FileName = @filename)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ActiveAgreeme = reader.GetBoolean(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting ActiveAgreeme Value. " + ex.Message);
                    throw new Exception("Error while getting ActiveAgreeme Value. " + ex.Message);
                }
            }
        }

        return ActiveAgreeme;
    }

    public string getPropertyValue(string controlid, string filename)
    {
        string PropertyValue = string.Empty;

        string upit = @"SELECT        TOP (1) PERCENT dbo.WebControlProperties.PropertyValue
                        FROM            dbo.WebControl INNER JOIN
                         dbo.WebPage ON dbo.WebControl.IDWebPage = dbo.WebPage.IDWebPage LEFT OUTER JOIN
                         dbo.Property INNER JOIN
                         dbo.WebControlProperties ON dbo.Property.IDProperty = dbo.WebControlProperties.IDProperty ON dbo.WebControl.IDWebControl = dbo.WebControlProperties.IDWebControl
                        WHERE        (dbo.WebControl.ControlId = @controlid) AND (dbo.WebPage.FileName = @filename)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@filename", filename);
                    objCmd.Parameters.AddWithValue("@controlid", controlid);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        PropertyValue = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting PropertyValue. " + ex.Message);
                    throw new Exception("Error while getting PropertyValue. " + ex.Message);
                }
            }
        }

        return PropertyValue;
    }

    public string getCountryCode(int idItem)
    {
        string CountryCode = string.Empty;

        string upit = @"SELECT        TOP (1) dbo.ItemCountry.CountryCode
                        FROM            dbo.Item INNER JOIN
                         dbo.ItemCountry ON dbo.Item.IDItem = dbo.ItemCountry.IDItem
                        WHERE        (dbo.Item.IDItem = @idItem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idItem", System.Data.SqlDbType.Int).Value = idItem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        CountryCode = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting CountryCode. " + ex.Message);
                    throw new Exception("Error while getting CountryCode. " + ex.Message);
                }
            }
        }

        return CountryCode;
    }

    public string getCountryCodeInputString(string itemText)
    {
        string CountryCode = string.Empty;

        string upit = @"SELECT        TOP (1) dbo.ItemCountry.CountryCode
                        FROM            dbo.Item INNER JOIN
                         dbo.ItemCountry ON dbo.Item.IDItem = dbo.ItemCountry.IDItem
                        WHERE        (dbo.Item.ItemText = @itemtext)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@itemText", itemText);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        CountryCode = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting CountryCode while input string. " + ex.Message);
                    throw new Exception("Error while CountryCode while input string. " + ex.Message);
                }
            }
        }

        return CountryCode;
    }

    public string getItemTextIDTypeOfItem(int IDTypeOfItem)
    {
        string itemText = string.Empty;

        string upit = @"SELECT        TOP (1) ItemText
                        FROM            dbo.Item
                        WHERE        (IDTypeOfItem = @idtypeofitem)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = IDTypeOfItem;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        itemText = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting itemText in function getItemTextIDTypeOfItem. " + ex.Message);
                    throw new Exception("Error while getting itemText in function getItemTextIDTypeOfItem. " + ex.Message);
                }
            }
        }

        return itemText;
    }


    public string getRevocationMethod(string revocationmethodenglish)
    {
        string revocationmethod = string.Empty;

        string upit = @"SELECT        ItemText
                        FROM            dbo.Item
                        WHERE        (ItemTextEnglish = @itemtextenglish)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@itemtextenglish", revocationmethodenglish);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        revocationmethod = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting revocationmethod in function getRevocationMethod. " + ex.Message);
                    throw new Exception("Error while getting revocationmethod in function getRevocationMethod. " + ex.Message);
                }
            }
        }

        return revocationmethod;
    }

    public string getMinAndMaxSerialLength(string SettingName)
    {
        string SerialLength = string.Empty;

        string upit = @"SELECT        SettingValue
                        FROM            dbo.GlobalSetting
                        WHERE        (SettingName = @settingname)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@settingname", SettingName);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        SerialLength = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting SerialLength. " + ex.Message);
                    throw new Exception("Error while getting SerialLength. " + ex.Message);
                }
            }
        }

        return SerialLength;
    }

    public string getpkcs12timeout(string SettingName)
    {
        string pkcs12timeout = string.Empty;

        string upit = @"SELECT        SettingValue
                        FROM            dbo.GlobalSetting
                        WHERE        (SettingName = @settingname)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@settingname", SettingName);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        pkcs12timeout = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting pkcs12timeout. " + ex.Message);
                    throw new Exception("Error while getting pkcs12timeout. " + ex.Message);
                }
            }
        }

        return pkcs12timeout;
    }



    public string pronadjiKorisnickoImeILozinku(int IDTypeOfItem, string userParameter)
    {
        string response = string.Empty;

        string upit = @"SELECT        dbo.Item.ItemText
FROM            dbo.Item INNER JOIN
                         dbo.TypeOfItem ON dbo.Item.IDTypeOfItem = dbo.TypeOfItem.IDTypeOfItem
WHERE        (dbo.TypeOfItem.IDTypeOfItem = @idtypeofitem) AND (dbo.Item.ItemTextEnglish = @itemtextenglish)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.Add("@idtypeofitem", System.Data.SqlDbType.Int).Value = IDTypeOfItem;
                    objCmd.Parameters.AddWithValue("@itemtextenglish", userParameter);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        response = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting StatusVariable. " + ex.Message);
                    throw new Exception("Error while getting StatusVariable. " + ex.Message);
                }
            }
        }

        return response;
    }


    public int pronadjiPocetakOpsegaKrovnogZahteva()
    {
        int response = 0;

        string upit = @"SELECT RangeStart AS Expr1
                        FROM   dbo.OrderNumberRange";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        response = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting RangeStart. " + ex.Message);
                    throw new Exception("Error while getting RangeStart. " + ex.Message);
                }
            }
        }

        return response;
    }

    public string getItemTextActivityCode(string itemText, bool active)
    {
        string ItemText = string.Empty;

        string upit = @"SELECT        ItemText
                        FROM            dbo.Item
                        WHERE        (ItemText = @itemtext) AND (Active = @active)";

        using (SqlConnection objConn = new SqlConnection(postaconnectionstring))
        {
            using (SqlCommand objCmd = new SqlCommand(upit, objConn))
            {
                try
                {
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.Parameters.AddWithValue("@itemText", itemText);
                    objCmd.Parameters.AddWithValue("@active", active);
                    objConn.Open();
                    SqlDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ItemText = reader.GetString(0);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error while getting ItemText Activity Code. " + ex.Message);
                    throw new Exception("Error while getting ItemText Activity Code. " + ex.Message);
                }
            }
            return ItemText;
        }
    }
}