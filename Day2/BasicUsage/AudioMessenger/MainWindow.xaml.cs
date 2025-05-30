using System.IO;
using System.Windows;
using NAudio.Wave;
using OpenAI;
using OpenAI.Chat;

namespace AudioMessenger;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private WaveInEvent? waveIn;
    private WaveFileWriter? writer;
    private string? recordedFilePath;
    private string? transcriptionResult;
    private string? openAIApiKey = "api-key"; // TODO: Set your API key here or load from config
    private string? responseAudioFilePath;

    public MainWindow()
    {
        InitializeComponent();
        LoadInputDevices();
        LoadOutputDevices();
        RecordButton.Click += RecordButton_Click;
        SendButton.Click += SendButton_Click;
        PlayButton.Click += PlayButton_Click;
        PlayMessageButton.Click += PlayMessageButton_Click;
    }

    private void LoadInputDevices()
    {
        InputDeviceComboBox.Items.Clear();
        for (int i = 0; i < WaveIn.DeviceCount; i++)
        {
            var caps = WaveIn.GetCapabilities(i);
            InputDeviceComboBox.Items.Add(caps.ProductName);
        }
        if (InputDeviceComboBox.Items.Count > 0)
            InputDeviceComboBox.SelectedIndex = 0;
    }

    private void LoadOutputDevices()
    {
        OutputDeviceComboBox.Items.Clear();
        for (int i = 0; i < WaveOut.DeviceCount; i++)
        {
            var caps = WaveOut.GetCapabilities(i);
            OutputDeviceComboBox.Items.Add(caps.ProductName);
        }
        if (OutputDeviceComboBox.Items.Count > 0)
            OutputDeviceComboBox.SelectedIndex = 0;
    }

    private void RecordButton_Click(object sender, RoutedEventArgs e)
    {
        if (waveIn == null)
        {
            int deviceIndex = InputDeviceComboBox.SelectedIndex;
            if (deviceIndex < 0) return;
            recordedFilePath = System.IO.Path.GetTempFileName() + ".wav";
            waveIn = new WaveInEvent { DeviceNumber = deviceIndex };
            writer = new WaveFileWriter(recordedFilePath, waveIn.WaveFormat);
            waveIn.DataAvailable += (s, a) => writer.Write(a.Buffer, 0, a.BytesRecorded);
            waveIn.RecordingStopped += (s, a) => {
                writer?.Dispose(); waveIn.Dispose(); waveIn = null; writer = null;
                Dispatcher.Invoke(() => DrawWaveform(recordedFilePath));
            };
            waveIn.StartRecording();
            RecordButton.Content = "Stop Recording";
        }
        else
        {
            waveIn.StopRecording();
            RecordButton.Content = "Start Recording";
        }
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(recordedFilePath) || !File.Exists(recordedFilePath))
        {
            MessageBox.Show("No recording found.");
            return;
        }
        TranscriptionTextBox.Text = "Transcribing...";
        transcriptionResult = await TranscribeWithWhisper(recordedFilePath);
        TranscriptionTextBox.Text = transcriptionResult ?? "No transcription.";
    }

    private async Task<string?> TranscribeWithWhisper(string filePath)
    {
        var apiKey = openAIApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            MessageBox.Show("OpenAI API key is not set.");
            return null;
        }
        ChatClient client = new("gpt-4o-audio-preview", apiKey);
        byte[] audioFileRawBytes = File.ReadAllBytes(filePath);
        BinaryData audioData = BinaryData.FromBytes(audioFileRawBytes);
        List<ChatMessage> messages = new()
        {
            new SystemChatMessage("You are a helpful assistant. Answer to the question asked by user"),
            new UserChatMessage(ChatMessageContentPart.CreateInputAudioPart(audioData, ChatInputAudioFormat.Wav)),
        };
        ChatCompletionOptions options = new()
        {
            ResponseModalities = ChatResponseModalities.Text | ChatResponseModalities.Audio,
            AudioOptions = new(ChatOutputAudioVoice.Alloy, ChatOutputAudioFormat.Mp3),
        };
        ChatCompletion completion = await client.CompleteChatAsync(messages, options);
        string transcription = "";
        if (completion.OutputAudio is ChatOutputAudio outputAudio)
        {
            responseAudioFilePath = $"{outputAudio.Id}.mp3";
            await using (FileStream outputFileStream = File.OpenWrite(responseAudioFilePath))
            {
                outputFileStream.Write(outputAudio.AudioBytes);
            }
            transcription = outputAudio.Transcript;
        }
        return transcription;
    }

    private void DrawWaveform(string filePath)
    {
        WaveformCanvas.Children.Clear();
        if (!File.Exists(filePath)) return;
        try
        {
            using var reader = new AudioFileReader(filePath);
            int width = (int)WaveformCanvas.ActualWidth;
            int height = (int)WaveformCanvas.ActualHeight;
            if (width == 0 || height == 0) return;
            float[] buffer = new float[reader.WaveFormat.SampleRate];
            int read;
            List<float> samples = new();
            while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                samples.AddRange(buffer.Take(read));
            int step = samples.Count / width;
            if (step == 0) step = 1;
            for (int x = 0; x < width; x++)
            {
                int start = x * step;
                int end = Math.Min(start + step, samples.Count);
                float max = 0;
                for (int i = start; i < end; i++)
                    max = Math.Max(max, Math.Abs(samples[i]));
                double y = height / 2 - max * (height / 2);
                double y2 = height / 2 + max * (height / 2);
                var line = new System.Windows.Shapes.Line
                {
                    X1 = x, X2 = x, Y1 = y, Y2 = y2,
                    Stroke = System.Windows.Media.Brushes.Lime,
                    StrokeThickness = 1
                };
                WaveformCanvas.Children.Add(line);
            }
        }
        catch { /* ignore errors */ }
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(responseAudioFilePath) || !File.Exists(responseAudioFilePath))
        {
            MessageBox.Show("No response audio found. Please send a message and get a response first.");
            return;
        }
        int deviceIndex = OutputDeviceComboBox.SelectedIndex;
        var reader = new AudioFileReader(responseAudioFilePath);
        var output = new WaveOutEvent { DeviceNumber = deviceIndex };
        output.Init(reader);
        output.Play();
    }

    private void PlayMessageButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(recordedFilePath) || !File.Exists(recordedFilePath))
        {
            MessageBox.Show("No recording found.");
            return;
        }
        int deviceIndex = OutputDeviceComboBox.SelectedIndex;
        var reader = new AudioFileReader(recordedFilePath);
        var output = new WaveOutEvent { DeviceNumber = deviceIndex };
        output.Init(reader);
        output.Play();
    }
}
