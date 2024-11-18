using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SearchEngine.Core.Indexing;
using SearchEngine.Core.Indexing.Models;
using SearchEngine.Core.Query;

namespace SearchEngine.Core
{
    public class Searcher
    {
        private readonly Indexer _indexer;
        private readonly RankingAlgorithm _rankingAlgorithm;
        private readonly CompositeParser _parser;
        private readonly QueryNormalizer _queryNormalizer;

        private readonly ConcurrentDictionary<string, CachedResult> _cache;

        public Searcher(Indexer indexer, RankingAlgorithm rankingAlgorithm)
        {
            _indexer = indexer;
            _rankingAlgorithm = rankingAlgorithm;
            _parser = new CompositeParser();
            _queryNormalizer = new QueryNormalizer();
            _cache = new ConcurrentDictionary<string, CachedResult>();
        }

        public PagedResult Search(string rawQuery, int page = 1, int pageSize = 10)
        {
            var query = _parser.Parse(rawQuery);

            // Generate a unique cache key
            var cacheKey = GenerateCacheKey(query);

            // Check if results for this query are cached
            if (_cache.TryGetValue(cacheKey, out var cachedResult))
            {
                return GetPagedResult(cachedResult.RankedResults, page, pageSize);
            }

            // Perform the full search if not cached
            var tokenResults = GetDocumentsByTokens(_queryNormalizer.Normalize(query.Tokens));
            var phraseResults = GetDocumentsByPhrases(query.Phrases);
            var booleanResults = ApplyBooleanExpressions(query.BooleanExpressions, tokenResults);

            var allResults = tokenResults.Union(phraseResults).Union(booleanResults).ToList();
            var rankedResults = _rankingAlgorithm.Rank(allResults).ToList();

            // Cache the ranked results
            _cache[cacheKey] = new CachedResult { RankedResults = rankedResults, Timestamp = DateTime.UtcNow };

            return GetPagedResult(rankedResults, page, pageSize);
        }

        public List<string> Suggest(string prefix, int maxSuggestions = 5, bool caseSensitive = false)
        {
            var indexedTerms = _indexer.GetIndex().Keys;

            return indexedTerms
                .Where(term => caseSensitive
                    ? term.StartsWith(prefix)
                    : term.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Take(maxSuggestions)
                .ToList();
        }

        private string GenerateCacheKey(ParsedQuery query)
        {
            var normalizedTokens = string.Join(" ", _queryNormalizer.Normalize(query.Tokens));
            var phrases = string.Join(" ", query.Phrases);
            var booleanExprs = string.Join(" ", query.BooleanExpressions.Select(be => $"{be.Operator} {be.Token}"));
            return $"{normalizedTokens} | {phrases} | {booleanExprs}";
        }

        private PagedResult GetPagedResult(List<Document> rankedResults, int page, int pageSize)
        {
            var paginatedResults = rankedResults
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult
            {
                Results = paginatedResults,
                TotalResults = rankedResults.Count,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)rankedResults.Count / pageSize)
            };
        }
        private IEnumerable<Document> GetDocumentsByTokens(IEnumerable<string> tokens)
        {
            var results = new List<Document>();

            foreach (var token in tokens)
            {
                if (_indexer.GetIndex().ContainsKey(token))
                {
                    results.AddRange(_indexer.GetIndex()[token]);
                }
            }

            return results;
        }

        private IEnumerable<Document> GetDocumentsByPhrases(IEnumerable<string> phrases)
        {
            var results = new List<Document>();

            foreach (var phrase in phrases)
            {
                var tokens = _queryNormalizer.Normalize(phrase.Split(' ').ToList());
                results.AddRange(
                    _indexer.GetIndex()
                        .Where(entry => tokens.All(token => entry.Key.Contains(token)))
                        .SelectMany(entry => entry.Value)
                );
            }

            return results;
        }

        private IEnumerable<Document> ApplyBooleanExpressions(
            List<(string Operator, string Token)> booleanExpressions,
            IEnumerable<Document> initialResults)
        {
            var resultSet = new HashSet<Document>(initialResults);

            foreach (var (op, token) in booleanExpressions)
            {
                var tokenResults = GetDocumentsByTokens(new List<string> { token });
                if (op == "AND")
                {
                    resultSet.IntersectWith(tokenResults);
                }
                else if (op == "OR")
                {
                    resultSet.UnionWith(tokenResults);
                }
                else if (op == "NOT")
                {
                    resultSet.ExceptWith(tokenResults);
                }
            }

            return resultSet;
        }
    }
}