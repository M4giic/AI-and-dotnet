using OpenAI.Chat;

namespace SimpleUsages.SimpleCalls;

public class Audio
{
    public void MakeAudioCall(string apiKey)
    {
        ChatClient client = new("gpt-4o-audio-preview", apiKey);

        // Input audio is provided to a request by adding an audio content part to a user message
        string audioFilePath = Path.Combine("Assets", "pytanie-od-Wikuni.wav");
        byte[] audioFileRawBytes = File.ReadAllBytes(audioFilePath);
        BinaryData audioData = BinaryData.FromBytes(audioFileRawBytes);
        List<ChatMessage> messages =
        [
            new SystemChatMessage("You are a helpful assistant. Answer to the question asked by user"),
            new UserChatMessage(ChatMessageContentPart.CreateInputAudioPart(audioData, ChatInputAudioFormat.Wav)),
        ];

        // Output audio is requested by configuring ChatCompletionOptions to include the appropriate
        // ResponseModalities values and corresponding AudioOptions.
        ChatCompletionOptions options = new()
        {
            ResponseModalities = ChatResponseModalities.Text | ChatResponseModalities.Audio,
            AudioOptions = new(ChatOutputAudioVoice.Alloy, ChatOutputAudioFormat.Mp3),
        };

        ChatCompletion completion = client.CompleteChat(messages, options);

        void PrintAudioContent()
        {
            if (completion.OutputAudio is ChatOutputAudio outputAudio)
            {
                Console.WriteLine($"Response audio transcript: {outputAudio.Transcript}");
                string outputFilePath = $"{outputAudio.Id}.mp3";
                using (FileStream outputFileStream = File.OpenWrite(outputFilePath))
                {
                    outputFileStream.Write(outputAudio.AudioBytes);
                }
                Console.WriteLine($"Response audio written to file: {outputFilePath}");
                Console.WriteLine($"Valid on followup requests until: {outputAudio.ExpiresAt}");
            }
        }

        PrintAudioContent();

        // To refer to past audio output, create an assistant message from the earlier ChatCompletion, use the earlier
        // response content part, or use ChatMessageContentPart.CreateAudioPart(string) to manually instantiate a part.

        messages.Add(new AssistantChatMessage(completion));
        messages.Add("Can you say that like a pirate?");

        completion = client.CompleteChat(messages, options);

        PrintAudioContent();
    }   
}