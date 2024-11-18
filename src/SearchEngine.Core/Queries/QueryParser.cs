using System;
using System.Collections.Generic;

namespace SearchEngine.Core.Query
{
    public class QueryParser
    {
        public List<string> Parse(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be empty or whitespace.");
            }

            // Tokenize the query
            var tokens = Tokenize(query);

            // Validate tokens
            Validate(tokens);

            return tokens;
        }

        private List<string> Tokenize(string query)
        {
            // Basic tokenization logic
            return new List<string>(
                query
                    .ToLower()
                    .Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            );
        }

        private void Validate(List<string> tokens)
        {
            // Example validation logic
            foreach (var token in tokens)
            {
                if (token.Length > 100)
                {
                    throw new ArgumentException($"Token '{token}' exceeds maximum allowed length.");
                }
            }
        }
    }
}