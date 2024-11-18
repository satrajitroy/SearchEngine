using System;
using System.Collections.Generic;

public class PhraseParser
{
    public List<string> Parse(string query, out List<string> phrases)
    {
        phrases = new List<string>();
        var tokens = new List<string>();

        var quoteStart = query.IndexOf('"');
        while (quoteStart != -1)
        {
            var quoteEnd = query.IndexOf('"', quoteStart + 1);
            if (quoteEnd == -1)
            {
                throw new ArgumentException("Mismatched quotes in query.");
            }

            var phrase = query.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
            phrases.Add(phrase);

            // Remove the quoted phrase from the query
            query = query.Remove(quoteStart, quoteEnd - quoteStart + 1);
            quoteStart = query.IndexOf('"');
        }

        tokens.AddRange(query.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return tokens;
    }
}