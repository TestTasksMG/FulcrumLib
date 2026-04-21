namespace FulcrumLib;

//TODO may use record instead but it may cause issues with Entity Framework (no preference for using record was mentioned in task description as mandatory)
public class Book
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int PageCount { get; set; }
}
