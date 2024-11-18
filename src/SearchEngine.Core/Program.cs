using System;
using System.Collections.Generic;
using SearchEngine.Core;
using SearchEngine.Core.Indexing;
using SearchEngine.Core.Indexing.Models;

class Program
{
    static void Main()
    {
        // Create some sample documents
        var documents = new List<Document>
        {
            new Document("1", "Hello world", new Metadata("Sample metadata 1", "Satrajit")),
            new Document("2", "Hello again", new Metadata("Sample metadata 2", "Satrajit")),
            new Document("3", "This is a test", new Metadata("Sample metadata 3", "Satrajit"))
        };

        // Initialize core components
        var indexer = new Indexer();
        var rankingAlgorithm = new RankingAlgorithm();
        var searcher = new Searcher(indexer, rankingAlgorithm);

        // Build the index
        indexer.BuildIndex(documents);

        // Perform a search
        var results = searcher.Search("hello");
        foreach (var result in results.Results)
        {
            Console.WriteLine($"Document ID: {result.Id}, Preview: {result.Content}");
        }
    }
}