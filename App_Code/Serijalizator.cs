using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Summary description for Serijalizator
/// </summary>
public static class Serijalizator
{
    public static string SerializeObject<T>(this T toSerialize)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }

    public static T Deserialize<T>(this string toDeserialize)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        using (StringReader textReader = new StringReader(toDeserialize))
        {
            return (T)xmlSerializer.Deserialize(textReader);
        }
    }

}