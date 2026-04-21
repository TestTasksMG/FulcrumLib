using FulcrumLib;
using Xunit;

public class BookTests
{
    [Fact]
    public void Book_DefaultValuesAreCorrect()
    {
        var book = new Book();

        Assert.Equal(string.Empty, book.Title);
        Assert.Equal(string.Empty, book.Author);
        Assert.Equal(0, book.PageCount);
    }

    [Fact]
    public void Book_CanSetProperties()
    {
        var book = new Book
        {
            Title = "It",
            Author = "King",
            PageCount = 412
        };

        Assert.Equal("It", book.Title);
        Assert.Equal("King", book.Author);
        Assert.Equal(412, book.PageCount);
    }
}