using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.Json;
using SearchEngine.Core.Indexing.Models;

public class InvertedIndexStorage
{
    private readonly string _connectionString;

    public InvertedIndexStorage(string databasePath)
    {
        _connectionString = $"Data Source={databasePath};Version=3;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Index (
                Token TEXT PRIMARY KEY,
                DocumentIds TEXT NOT NULL
            );";
        using var command = new SQLiteCommand(createTableQuery, connection);
        command.ExecuteNonQuery();
    }

    public void SaveToken(string token, List<string> documentIds)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var insertQuery = @"
            INSERT OR REPLACE INTO Index (Token, DocumentIds)
            VALUES (@Token, @DocumentIds);";

        using var command = new SQLiteCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@Token", token);
        command.Parameters.AddWithValue("@DocumentIds", string.Join(",", documentIds));
        command.ExecuteNonQuery();
    }

    public List<string> GetDocumentsForToken(string token)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var selectQuery = "SELECT DocumentIds FROM Index WHERE Token = @Token;";
        using var command = new SQLiteCommand(selectQuery, connection);
        command.Parameters.AddWithValue("@Token", token);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return reader.GetString(0).Split(',').ToList();
        }

        return new List<string>();
    }
}