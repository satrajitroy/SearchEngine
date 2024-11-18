using System;
using System.Collections.Generic;

namespace SearchEngine.Core.Query
{
    public class CompositeParser
    {
        private readonly QueryParser _baseParser = new();
        private readonly BooleanParser _booleanParser = new();
        private readonly PhraseParser _phraseParser = new();

        public ParsedQuery Parse(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be empty or whitespace.");
            }

            // Step 1: Parse quoted phrases
            var tokens = _phraseParser.Parse(query, out var phrases);

            // Step 2: Parse Boolean operators
            var logicalTokens = _booleanParser.Parse(tokens, out var booleanExpressions);

            // Step 3: Tokenize and normalize
            var finalTokens = _baseParser.Parse(string.Join(" ", logicalTokens));

            return new ParsedQuery
            {
                Tokens = finalTokens,
                Phrases = phrases,
                BooleanExpressions = booleanExpressions
            };
        }
    }

    public class ParsedQuery
    {
        public List<string> Tokens { get; set; } // Basic tokens
        public List<string> Phrases { get; set; } // Quoted phrases
        public List<(string Operator, string Token)> BooleanExpressions { get; set; } // Logical expressions
    }
}