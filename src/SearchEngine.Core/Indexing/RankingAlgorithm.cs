using System.Collections.Generic;
using System.Linq;
using SearchEngine.Core.Indexing.Models;

namespace SearchEngine.Core.Indexing
{
    public class RankingAlgorithm
    {
        public IEnumerable<Document> Rank(IEnumerable<Document> documents)
        {
            // Basic ranking logic: sort by metadata creation time as a placeholder
            return documents
                .GroupBy(d => d.Id)
                .Select(g => g.First()) // Remove duplicates
                .OrderByDescending(d => d.Metadata.CreatedAt)
                .ToList();
        }
    }
}