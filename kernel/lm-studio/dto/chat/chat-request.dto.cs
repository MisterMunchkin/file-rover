using System.Text.Json.Serialization;

namespace file_rover.kernel.lm_studio.dto.chat;

public class ChatRequest
{
    public ChatRequest()
    {
        Messages = new List<ChatMessage>();
        Temperature = 0.7f;
        MaxTokens = 2500;
        Stream = false;
        Model = "";
    }
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("messages")]
    public List<ChatMessage> Messages { get; set; }

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }
    
}
