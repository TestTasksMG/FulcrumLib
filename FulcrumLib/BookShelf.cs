namespace FulcrumLib;

public class BookShelf
{
    private List<Book> _books = new();

    public void AddBulk(IEnumerable<Book> books)
    {
        if (books == null)
            throw new ArgumentNullException(nameof(books));

        //TODO add validation (was not mentioned in task description as mandatory)

        foreach (var book in books)
            Add(book);
    }

    public void Add(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        //TODO add validation (was not mentioned in task description as mandatory)
        //TODO maybe check for attempts to add duplicate values (was not mentioned in task description as mandatory)

        _books.Add(book);
    }

    public void Clear()
    {
        _books.Clear();
    }

    public void Sort()
    {
        _books = _books
            .OrderBy(b => b.Author ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ThenBy(b => b.Title ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// if query is omitted or empty string or null then returns all Book's
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public List<Book> SearchByTitle(string query = "")
    {
        if (string.IsNullOrWhiteSpace(query))
            return _books.ToList();

        return _books
            .Where(b => (b.Title ?? string.Empty)
                .Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void FillFromFile(string filePath, bool replace = false)
    {
        var loaded = BookPersistence.LoadFromXmlFile(filePath);

        if (replace)
            _books.Clear();

        AddBulk(loaded);
    }

    public void SaveToFile(string filePath)
    {
        BookPersistence.SaveToXmlFile(filePath, _books);
    }
}