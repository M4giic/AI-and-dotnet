// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices.JavaScript;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Files;
using OpenAI.Images;
using System.Data.SQLite; // Add this using directive

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

Console.WriteLine("Hello, World!");

var apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;

ChatClient chatClient = new ChatClient(model: "gpt-4.1", apiKey: apiKey);

OpenAIClient openAIClient = new(apiKey);
OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();

// SQLite database setup
string dbPath = "files.db";
if (!File.Exists(dbPath))
{
    SQLiteConnection.CreateFile(dbPath);
}
using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
{
    connection.Open();
    string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Files (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FileName TEXT NOT NULL,
            FileId TEXT NOT NULL
        );";
    using (var command = new SQLiteCommand(createTableQuery, connection))
    {
        command.ExecuteNonQuery();
    }

    var directoryPath = "C:\\Users\\jendr\\Documents\\GitHub\\moje\\EventManagement\\Day2\\Dokumenty"; 
    // Example: loop through files in directory and upload each
    foreach (var filePath in Directory.GetFiles(directoryPath))
    {
        string fileName = Path.GetFileName(filePath);

        // Check if file is already in the database
        string doesFileExistQuery = "SELECT COUNT(*) FROM Files WHERE FileName = @FileName";
        using (var checkCommand = new SQLiteCommand(doesFileExistQuery, connection))
        {
            checkCommand.Parameters.AddWithValue("@FileName", fileName);
            long count = (long)checkCommand.ExecuteScalar();
            if (count > 0)
            {
                Console.WriteLine($"File '{fileName}' already exists in the database. Skipping upload.");
                continue;
            }
        }

        using Stream fileStream = File.OpenRead(filePath);
        var document = BinaryData.FromStream(fileStream);
        Console.WriteLine($"Uploading file: {filePath}");
        OpenAIFile uploadFile = fileClient.UploadFile(
            document,
            fileName,
            FileUploadPurpose.Assistants);

        Console.WriteLine($"File uploaded: {uploadFile.Id}");

        // Insert into SQLite database
        string insertQuery = "INSERT INTO Files (FileName, FileId) VALUES (@FileName, @FileId)";
        using (var insertCommand = new SQLiteCommand(insertQuery, connection))
        {
            insertCommand.Parameters.AddWithValue("@FileName", fileName);
            insertCommand.Parameters.AddWithValue("@FileId", uploadFile.Id);
            insertCommand.ExecuteNonQuery();
        }
    }

    // Print contents of the database
    Console.WriteLine("\nFiles in database:");
    string selectQuery = "SELECT FileName, FileId FROM Files";
    using (var selectCommand = new SQLiteCommand(selectQuery, connection))
    using (var reader = selectCommand.ExecuteReader())
    {
        while (reader.Read())
        {
            string fileName = reader.GetString(0);
            string fileId = reader.GetString(1);
            Console.WriteLine($"FileName: {fileName}, FileId: {fileId}");
        }
    }

    // Build fileCollection from DB
    var fileParts = new List<ChatMessageContentPart>();
    using (var selectCommand = new SQLiteCommand(selectQuery, connection))
    using (var reader = selectCommand.ExecuteReader())
    {
        while (reader.Read())
        {
            string fileId = reader.GetString(1);
            fileParts.Add(ChatMessageContentPart.CreateFilePart(fileId));
        }
    }
    ChatMessageContentPart[] fileCollection = fileParts.ToArray();

    // Pass all members of fileCollection to UserChatMessage
    var messageParts = new List<ChatMessageContentPart>
    {
        ChatMessageContentPart.CreateTextPart("Please find the highest grossing company in this collection")
    };
    messageParts.AddRange(fileCollection);

    ChatCompletion reponse = chatClient.CompleteChat(
        new List<ChatMessage>
        {
            new SystemChatMessage("You are a helpful assistant that can answer questions about uploaded files."),
            new UserChatMessage(messageParts.ToArray())
        });
    
    Console.WriteLine($"[ASSISTANT]: {reponse.Content[0].Text}");
}

Console.WriteLine("Koniec programu. Naciśnij dowolny klawisz, aby zakończyć.");
