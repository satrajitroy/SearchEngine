using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SearchEngine.API.DTO;
using SearchEngine.Core;

namespace SearchEngine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly Searcher _searcher;

        public SearchController(Searcher searcher)
        {
            _searcher = searcher;
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty.");
            }

            var results = _searcher.Search(query);
            var response = new SearchResultDto
            {
                Results = results.Results.Select(r => new SearchResultItemDto
                {
                    Id = r.Id,
                    ContentPreview = r.Content.Substring(0, Math.Min(r.Content.Length, 100)) + "...",
                    Metadata = r.Metadata
                }).ToList()
            };

            return Ok(response);
        }
    }
}