using System.Xml.Serialization;

namespace Core.Parsing;

public static class XmlParsing
{
    public static T? Parse<T>(string filepath)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StreamReader(filepath);
        return (T?)serializer.Deserialize(reader);
    }
}