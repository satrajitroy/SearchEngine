using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.Json;
using SearchEngine.Core.Indexing.Models;

namespace SearchEngine.Core.Storage
{
    public class DocumentStorage
    {
        private readonly string _connectionString;

        public DocumentStorage(string databasePath)
        {
            _connectionString = $"Data Source={databasePath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Documents (
                    Id TEXT PRIMARY KEY,
                    Content TEXT NOT NULL,
                    Metadata TEXT NOT NULL
                );";
            using var command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }

        public void SaveDocument(Document document)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            var insertQuery = @"
                INSERT OR REPLACE INTO Documents (Id, Content, Metadata)
                VALUES (@Id, @Content, @Metadata);";

            using var command = new SQLiteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@Id", document.Id);
            command.Parameters.AddWithValue("@Content", document.Content);
            command.Parameters.AddWithValue("@Metadata", JsonSerializer.Serialize(document.Metadata));
            command.ExecuteNonQuery();
        }

        public Document GetDocument(string id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            var selectQuery = "SELECT Content, Metadata FROM Documents WHERE Id = @Id;";
            using var command = new SQLiteCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var content = reader.GetString(0);
                var metadata = JsonSerializer.Deserialize<Metadata>(reader.GetString(1));
                return new Document(id, content, metadata);
            }

            return null; // Document not found
        }

        public void DeleteDocument(string id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            var deleteQuery = "DELETE FROM Documents WHERE Id = @Id;";
            using var command = new SQLiteCommand(deleteQuery, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        public List<Document> GetAllDocuments()
        {
            var documents = new List<Document>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            var selectQuery = "SELECT Id, Content, Metadata FROM Documents;";
            using var command = new SQLiteCommand(selectQuery, connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetString(0);
                var content = reader.GetString(1);
                var metadata = JsonSerializer.Deserialize<Metadata>(reader.GetString(2));
                documents.Add(new Document(id, content, metadata));
            }

            return documents;
        }
    }
}
