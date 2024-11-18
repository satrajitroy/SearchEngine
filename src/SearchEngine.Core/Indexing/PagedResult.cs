using System.Collections.Generic;
using SearchEngine.Core.Indexing.Models;

public class PagedResult
{
    public List<Document> Results { get; set; } // The current page of results
    public int TotalResults { get; set; } // Total number of matching documents
    public int CurrentPage { get; set; } // Current page number
    public int TotalPages { get; set; } // Total number of pages
}