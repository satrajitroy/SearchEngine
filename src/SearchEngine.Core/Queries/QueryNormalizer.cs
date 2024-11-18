using System.Collections.Generic;
using System.Linq;

namespace SearchEngine.Core.Query
{
    public class QueryNormalizer
    {
        private static readonly HashSet<string> StopWords = new()
        {
            "a", "an", "the", "and", "or", "but", "with", "in", "on", "at", "by"
        };

        public List<string> Normalize(List<string> tokens)
        {
            return tokens
                .Select(token => token.ToLower())
                .Where(token => !StopWords.Contains(token)) // Remove stopwords
                .Distinct() // Remove duplicates
                .ToList();
        }
    }
}