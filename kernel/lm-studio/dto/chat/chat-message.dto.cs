using System.Text.Json.Serialization;

namespace file_rover.kernel.lm_studio.dto.chat;

public class ChatMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }
}
