using System;
using System.Collections.Generic;
using SearchEngine.Core.Indexing.Models;

public class CachedResult
{
    public List<Document> RankedResults { get; set; } // Full ranked results
    public DateTime Timestamp { get; set; } // Timestamp for cache entry
}