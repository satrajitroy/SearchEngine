using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SearchEngine.Core.Indexing.Models
{
    public class Metadata
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } // Optional title for the document

        [JsonPropertyName("author")]
        public string Author { get; set; } // Author or creator

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } // Creation timestamp

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } // Last updated timestamp

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } // List of tags or categories

        [JsonPropertyName("attrs")]
        public Dictionary<string, string> CustomAttributes { get; set; } // Key-value pairs for additional attributes

        public Metadata(
            string title = null,
            string author = null,
            DateTime? createdAt = null,
            DateTime? updatedAt = null,
            List<string> tags = null,
            Dictionary<string, string> customAttributes = null)
        {
            Title = title ?? "New document";
            Author = author ?? "Unknown";
            CreatedAt = createdAt ?? DateTime.UtcNow;
            UpdatedAt = updatedAt ?? DateTime.UtcNow;
            Tags = tags ?? new List<string>();
            CustomAttributes = customAttributes ?? new Dictionary<string, string>();
        }

        public void AddTag(string tag)
        {
            if (!Tags.Contains(tag))
            {
                Tags.Add(tag);
            }
        }

        public void RemoveTag(string tag)
        {
            Tags.Remove(tag);
        }

        public void AddAttribute(string key, string value)
        {
            CustomAttributes[key] = value;
        }

        public void RemoveAttribute(string key)
        {
            if (CustomAttributes.ContainsKey(key))
            {
                CustomAttributes.Remove(key);
            }
        }

        public override string ToString()
        {
            var tags = string.Join(", ", Tags);
            var attributes = string.Join(", ", CustomAttributes);
            return $"Author: {Author}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, Tags: [{tags}], CustomAttributes: [{attributes}]";
        }
    }
}