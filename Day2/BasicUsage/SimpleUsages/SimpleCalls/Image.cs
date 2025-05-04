using OpenAI.Chat;
using OpenAI.Images;

namespace SimpleUsages.SimpleCalls;

public class Image
{
    public void GenerateImage(string apiKey)
    {
        ImageClient client = new("dall-e-3", apiKey);
        
        string prompt = "The concept for a living room that blends Scandinavian simplicity with Japanese minimalism for"
                        + " a serene and cozy atmosphere. It's a space that invites relaxation and mindfulness, with natural light"
                        + " and fresh air. Using neutral tones, including colors like white, beige, gray, and black, that create a"
                        + " sense of harmony. Featuring sleek wood furniture with clean lines and subtle curves to add warmth and"
                        + " elegance. Plants and flowers in ceramic pots adding color and life to a space. They can serve as focal"
                        + " points, creating a connection with nature. Soft textiles and cushions in organic fabrics adding comfort"
                        + " and softness to a space. They can serve as accents, adding contrast and texture.";

        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.High,
            Size = GeneratedImageSize.W1792xH1024,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Bytes
        };
        
        GeneratedImage image = client.GenerateImage(prompt, options);
        BinaryData bytes = image.ImageBytes;
        
        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
    }      

    public void DescribeImage(string apiKey, string fileName)
    {
        ChatClient  client = new("gpt-4o", apiKey);
        string imageFilePath = Path.Combine("Assets", fileName);
        using Stream imageStream = File.OpenRead(imageFilePath);
        BinaryData imageBytes = BinaryData.FromStream(imageStream);
        List<ChatMessage> messages =
        [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Please describe the following image."),
                ChatMessageContentPart.CreateImagePart(imageBytes, "image/png")),
        ];
        
        ChatCompletion completion = client.CompleteChat(messages);

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
    }      
    
    public void DescribeImageRandomByUrl(string apiKey)
    {
        ChatClient  client = new("gpt-4o", apiKey);
        List<ChatMessage> messages =
        [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Please describe the following image."),
                ChatMessageContentPart.CreateImagePart( new Uri("https://picsum.photos/400"))),
        ];
        
        ChatCompletion completion = client.CompleteChat(messages);

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
    }  
}