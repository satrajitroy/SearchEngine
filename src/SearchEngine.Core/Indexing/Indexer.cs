using System.Collections.Generic;
using System.Linq;
using SearchEngine.Core.Indexing.Models;

namespace SearchEngine.Core.Indexing
{
    public class Indexer
    {
        private readonly Dictionary<string, List<Document>> _index = new();

        public void BuildIndex(IEnumerable<Document> documents)
        {
            foreach (var document in documents)
            {
                var tokens = Tokenize(document.Content);
                foreach (var token in tokens)
                {
                    if (!_index.ContainsKey(token))
                    {
                        _index[token] = new List<Document>();
                    }
                    _index[token].Add(document);
                }
            }
        }

        public void UpdateIndex(Document document)
        {
            // Re-index the document
            RemoveFromIndex(document.Id);
            BuildIndex(new[] { document });
        }

        public void RemoveFromIndex(string documentId)
        {
            foreach (var key in _index.Keys.ToList())
            {
                _index[key] = _index[key].Where(d => d.Id != documentId).ToList();
                if (!_index[key].Any())
                {
                    _index.Remove(key);
                }
            }
        }

        private IEnumerable<string> Tokenize(string content)
        {
            // Basic tokenization logic
            return content
                .ToLower()
                .Split(new[] { ' ', '.', ',', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Distinct();
        }

        public Dictionary<string, List<Document>> GetIndex()
        {
            return _index;
        }
    }
}