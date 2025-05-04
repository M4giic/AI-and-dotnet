using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Files;

namespace SimpleUsages.SimpleCalls;

public class Files
{
    public void UploadFile(string  apiKey, string fileName) 
    {
        OpenAIClient openAIClient = new(apiKey);
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        string filePath = Path.Combine("Assets", fileName);
        using Stream fileStream = File.OpenRead(filePath);
        var document = BinaryData.FromStream(fileStream);
        // Code to upload a file
        Console.WriteLine($"Uploading file: {filePath}");
        OpenAIFile uploadFile = fileClient.UploadFile(
            document,
            fileName,
            FileUploadPurpose.Assistants);
        
        Console.WriteLine($"File uploaded: {uploadFile.Id}");

        List<ChatMessage> messages = new List<ChatMessage>()
        {
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Please describe the following file."),
                ChatMessageContentPart.CreateFilePart(uploadFile.Id)),
        };
        
        ChatClient chatClient = new("gpt-4o", apiKey);
        
        ChatCompletion completion = chatClient.CompleteChat(messages);
        
        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
    }
}