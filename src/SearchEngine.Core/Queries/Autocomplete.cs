using System.Collections.Generic;
using System.Linq;

namespace SearchEngine.Core.Query
{
    public class Autocomplete
    {
        private readonly List<string> _indexedTerms;

        public Autocomplete(IEnumerable<string> indexedTerms)
        {
            _indexedTerms = indexedTerms.ToList();
        }

        public List<string> Suggest(string prefix, int maxSuggestions = 5)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return new List<string>();
            }

            return _indexedTerms
                .Where(term => term.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
                .Take(maxSuggestions)
                .ToList();
        }
    }
}