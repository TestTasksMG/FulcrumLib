using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using FulcrumLib;
using Xunit;

namespace FulcrumLib.Tests;

// TODO only basic file saving/loading tests are present, adding tests for things like filesystem access permissions, shared access to file conflicts, filename case sensivity etc would take too much time and as such probably are beyond the scope of this test task?

public class BookPersistenceTests
{
    [Fact]
    public void LoadFromXmlFile_ReturnsEmptyList_WhenFileDoesNotExist()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xml");

        var result = BookPersistence.LoadFromXmlFile(nonExistentPath);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void LoadFromXmlFile_ReturnsBooks_WhenValidXmlExists()
    {
        var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xml");

        try
        {
            var originalBooks = new List<Book>
            {
                new() { Author = "Andersen", Title = "The Little Mermaid", PageCount = 100 },
                new() { Author = "King", Title = "It", PageCount = 1000 }
            };

            var serializer = new XmlSerializer(typeof(List<Book>));

            using (var stream = File.Create(filePath))
            {
                serializer.Serialize(stream, originalBooks);
            }

            var loadedBooks = BookPersistence.LoadFromXmlFile(filePath);

            Assert.Equal(originalBooks.Count, loadedBooks.Count);

            Assert.Equal(originalBooks[0].Author, loadedBooks[0].Author);
            Assert.Equal(originalBooks[0].Title, loadedBooks[0].Title);
            Assert.Equal(originalBooks[0].PageCount, loadedBooks[0].PageCount);

            Assert.Equal(originalBooks[1].Author, loadedBooks[1].Author);
            Assert.Equal(originalBooks[1].Title, loadedBooks[1].Title);
            Assert.Equal(originalBooks[1].PageCount, loadedBooks[1].PageCount);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void SaveToXmlFile_PersistsDataCorrectly()
    {
        var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xml");

        try
        {
            var books = new List<Book>
            {
                new() { Author = "Andersen", Title = "The Little Mermaid", PageCount = 100 },
                new() { Author = "King", Title = "It", PageCount = 1000 }
            };

            BookPersistence.SaveToXmlFile(filePath, books);

            var loadedText = File.ReadAllText(filePath);

            var serializer = new XmlSerializer(typeof(List<Book>));
            string expectedText;

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, books);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                expectedText = reader.ReadToEnd();
            }

            Assert.Equal(expectedText, loadedText);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}