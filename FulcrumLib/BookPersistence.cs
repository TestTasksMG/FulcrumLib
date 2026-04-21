using System.Xml.Serialization;

namespace FulcrumLib;

public static class BookPersistence
{
    //TODO add public property to allow external code to read and change base folder used for relative file paths (was not mentioned in task description as mandatory)

    public static List<Book> LoadFromXmlFile(string filePath)
    {
        if (!File.Exists(filePath))
            return new List<Book>();

        var serializer = new XmlSerializer(typeof(List<Book>));

        //TODO handle file reading errors (was not mentioned in task description as mandatory)

        using var stream = File.OpenRead(filePath);
        return (List<Book>)serializer.Deserialize(stream)!;
    }

    public static void SaveToXmlFile(string filePath, List<Book> books)
    {
        var serializer = new XmlSerializer(typeof(List<Book>));

        //TODO handle file saving errors (was not mentioned in task description as mandatory)

        using var stream = File.Create(filePath);
        serializer.Serialize(stream, books);
    }
}