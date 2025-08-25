using System.Text.Json.Serialization;

namespace Dto.Chat
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ChatResponseChoice
    {
        public int ChatResponseindex { get; set; }

        [JsonPropertyName("message")]
        public required ChatResponseMessage Message { get; set; }


        [JsonPropertyName("finish_reason")]
        public required string FinishReason { get; set; }
    }

    public class ChatResponseMessage
    {
        [JsonPropertyName("role")]
        public required string Role { get; set; }

        [JsonPropertyName("content")]
        public required string Content { get; set; }
    }

    public class ChatResponse
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("object")]
        public required string Object { get; set; }

        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("model")]
        public required string Model { get; set; }

        [JsonPropertyName("choices")]
        public required List<ChatResponseChoice> Choices { get; set; }

        [JsonPropertyName("usage")]
        public required ChatResponseUsage Usage { get; set; }
    }

    public class ChatResponseUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}