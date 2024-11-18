using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SearchEngine.Core.Indexing.Models
{
    public class Document
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // Unique identifier for the document

        [JsonPropertyName("content")]
        public string Content { get; set; } // Full content of the document


        [JsonPropertyName("author")] public Metadata Metadata { get; set; }

        [JsonIgnore]
        public Dictionary<string, int> TokenFrequency { get; private set; } // Token frequency map for efficient ranking

        public Document(string id, string content, Metadata metadata)
        {
            Id = id;
            Content = content;
            Metadata = metadata;
            TokenFrequency = TokenizeAndCount(content);
        }

        private Dictionary<string, int> TokenizeAndCount(string content)
        {
            // Tokenize and count frequencies
            return content
                .ToLower()
                .Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(token => token)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        public IEnumerable<string> GetTokens()
        {
            // Return unique tokens from the content
            return TokenFrequency.Keys;
        }

        public void UpdateContent(string newContent)
        {
            Content = newContent;
            Metadata.UpdatedAt = DateTime.UtcNow;
            TokenFrequency = TokenizeAndCount(newContent);
        }

        public override string ToString()
        {
            return $"{Id}: {Metadata.Title ?? "Untitled"} (Author: {Metadata.Author ?? "Unknown"})";
        }
    }
}
