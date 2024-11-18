using System.Collections.Generic;
using SearchEngine.Core.Indexing.Models;

namespace SearchEngine.API.DTOs
{
    public class SearchResultDto
    {
        public List<SearchResultItemDto> Results { get; set; }
    }

    public class SearchResultItemDto
    {
        public string Id { get; set; }
        public string ContentPreview { get; set; }
        public Metadata Metadata { get; set; }
    }
}