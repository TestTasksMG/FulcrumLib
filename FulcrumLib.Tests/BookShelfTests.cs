using FulcrumLib;
using Xunit;

public class BookShelfTests
{
    private (Book shining, Book it, Book duck, Book mermaid) GetTestData()
    {
        return (
            new Book { Author = "King", Title = "The Shining", PageCount = 500 },
            new Book { Author = "King", Title = "It", PageCount = 1000 },
            new Book { Author = "Andersen", Title = "The Ugly Duckling", PageCount = 80 },
            new Book { Author = "Andersen", Title = "The Little Mermaid", PageCount = 100 }
        );
    }

    private static void AssertBook(Book book, string author, string title, int pageCount)
    {
        Assert.Equal(author, book.Author);
        Assert.Equal(title, book.Title);
        Assert.Equal(pageCount, book.PageCount);
    }

    [Fact]
    public void Add_ShouldAppendToShelf()
    {
        var shelf = new BookShelf();
        var (shining, it, _, _) = GetTestData();

        shelf.Add(shining);

        var result1 = shelf.SearchByTitle();

        Assert.Single(result1);
        AssertBook(result1[0], "King", "The Shining", 500);

        shelf.Add(it);

        var result2 = shelf.SearchByTitle();

        Assert.Equal(2, result2.Count);

        AssertBook(result2[0], "King", "The Shining", 500);
        AssertBook(result2[1], "King", "It", 1000);
    }

    [Fact]
    public void Add_ShouldThrowExceptionWhenNullIsPassedToIt()
    {
        var shelf = new BookShelf();

        Assert.Throws<ArgumentNullException>(() => shelf.Add(null!));
    }

    [Fact]
    public void AddBulk_ShouldAddMultipleBooks()
    {
        var shelf = new BookShelf();
        var (shining, it, duck, _) = GetTestData();

        shelf.Add(shining);

        shelf.AddBulk(new[] { it, duck });

        var result = shelf.SearchByTitle();

        Assert.Equal(3, result.Count);

        AssertBook(result[0], "King", "The Shining", 500);
        AssertBook(result[1], "King", "It", 1000);
        AssertBook(result[2], "Andersen", "The Ugly Duckling", 80);
    }

    [Fact]
    public void AddBulk_ShouldThrowExceptionWhenNullIsPassedToIt()
    {
        var shelf = new BookShelf();

        Assert.Throws<ArgumentNullException>(() => shelf.AddBulk(null!));
    }

    [Fact]
    public void Clear_ShouldRemoveAllBooks()
    {
        var shelf = new BookShelf();
        var (shining, it, duck, _) = GetTestData();

        shelf.AddBulk(new[] { shining, it, duck });

        shelf.Clear();

        var result = shelf.SearchByTitle();

        Assert.Empty(result);
    }

    [Fact]
    public void Sort_ShouldOrderByAuthorThenTitle()
    {
        var shelf = new BookShelf();
        var (shining, it, duck, mermaid) = GetTestData();

        shelf.AddBulk(new List<Book> { shining, it, duck, mermaid });
        shelf.Sort();

        var result = shelf.SearchByTitle();

        Assert.Equal(4, result.Count);

        AssertBook(result[0], "Andersen", "The Little Mermaid", 100);
        AssertBook(result[1], "Andersen", "The Ugly Duckling", 80);
        AssertBook(result[2], "King", "It", 1000);
        AssertBook(result[3], "King", "The Shining", 500);
    }

    [Fact]
    public void SearchByTitle_ShouldReturnPartialMatches()
    {
        var shelf = new BookShelf();
        var (shining, it, duck, mermaid) = GetTestData();

        shelf.AddBulk(new List<Book> { shining, it, duck, mermaid });

        var result = shelf.SearchByTitle("the");

        Assert.Equal(3, result.Count);

        AssertBook(result[0], "King", "The Shining", 500);
        AssertBook(result[1], "Andersen", "The Ugly Duckling", 80);
        AssertBook(result[2], "Andersen", "The Little Mermaid", 100);
    }

    [Fact]
    public void SearchByTitle_WhenEmptyOrNullOrDefault_ShouldReturnAllBooks()
    {
        var shelf = new BookShelf();
        var (_, it, duck, _) = GetTestData();

        shelf.Add(it);
        shelf.Add(duck);

        var resultEmpty = shelf.SearchByTitle("");
        var resultNull = shelf.SearchByTitle(null!);
        var resultDefault = shelf.SearchByTitle();

        Assert.Equal(2, resultEmpty.Count);
        Assert.Equal(2, resultNull.Count);
        Assert.Equal(2, resultDefault.Count);

        AssertBook(resultEmpty[0], "King", "It", 1000);
        AssertBook(resultEmpty[1], "Andersen", "The Ugly Duckling", 80);

        AssertBook(resultNull[0], "King", "It", 1000);
        AssertBook(resultNull[1], "Andersen", "The Ugly Duckling", 80);

        AssertBook(resultDefault[0], "King", "It", 1000);
        AssertBook(resultDefault[1], "Andersen", "The Ugly Duckling", 80);
    }

    [Fact]
    public void SaveToFile_ShouldCreateFile()
    {
        var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xml");
        try
        {
            var shelf = new BookShelf();
            var (_, it, _, _) = GetTestData();

            shelf.Add(it);

            shelf.SaveToFile(filePath);

            Assert.True(File.Exists(filePath));

            // not asserting contents of saved file because SaveToFile() and FillFromFile() are designed to be implementation-agnostic
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    // also implicitly tests SaveToFile() method of BookShelf class
    [Fact]
    public void FillFromFile_ShouldAppendByDefault()
    {
        var filePath = Path.GetTempFileName();
        try
        {
            var shelf = new BookShelf();
            var (_, it, duck, _) = GetTestData();

            shelf.Add(it);

            var fileShelf = new BookShelf();
            fileShelf.Add(duck);
            fileShelf.SaveToFile(filePath);

            shelf.FillFromFile(filePath);

            var result = shelf.SearchByTitle();

            Assert.Equal(2, result.Count);

            AssertBook(result[0], "King", "It", 1000);
            AssertBook(result[1], "Andersen", "The Ugly Duckling", 80);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    // also implicitly tests SaveToFile() method of BookShelf class
    [Fact]
    public void FillFromFile_ShouldReplaceWhenRequested_WithSameDataThatWasPassedToSaveToFile()
    {
        var filePath = Path.GetTempFileName();

        try
        {
            var shelf = new BookShelf();
            var (_, it, duck, mermaid) = GetTestData();

            shelf.Add(it);

            var fileShelf = new BookShelf();
            fileShelf.Add(mermaid);
            fileShelf.Add(duck);
            fileShelf.SaveToFile(filePath);

            shelf.FillFromFile(filePath, replace: true);

            var result = shelf.SearchByTitle();

            Assert.Equal(2, result.Count);

            AssertBook(result[0], "Andersen", "The Little Mermaid", 100);
            AssertBook(result[1], "Andersen", "The Ugly Duckling", 80);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}