using System.Net.Http.Headers;
using System.Text.Json;
using Dto.Chat;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace file_rover.service.model
{
    public class ChatCompletionService : IChatCompletionService
    {
         // public property for the model url endpoint
        public static readonly string ModelUrl = "http://127.0.0.1:1234/v1/chat/completions";
        public static readonly string ModelName = "openai/gpt-oss-20b";

        public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), ModelUrl))
                {

                    // iterate though chatHistory and generate a json document based on the Root class
                    var root = new ChatRequest();
                    for (int i = 0; i < chatHistory.Count; i++)
                    {
                        var message = chatHistory[i];

                        var msg = new ChatMessage
                        {
                            Role = message.Role.ToString().ToLower(),
                            Content = message.Content ?? string.Empty
                        };
                        root.Messages.Add(msg);
                    }

                    // validate if ModelName is not empty and add it to the root object
                    if (!string.IsNullOrEmpty(ModelName))
                    {
                        root.Model = ModelName;
                    }

                    // generate the json string from the root object
                    var jsonString = JsonSerializer.Serialize(root);
                    request.Content = new StringContent(jsonString);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    var httpResponse = await httpClient.SendAsync(request);

                    // get the response content
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    // deserialize the response content into a ChatResponse object
                    var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseContent);

                    var chatResponseMessageContent = chatResponse?.Choices[0].Message.Content ?? null;
                    if (chatResponseMessageContent == null)
                    {
                        throw new Exception("Chat response message content is null.");
                    }
                 

                    // add httpResponse content to chatHistory
                    chatHistory.AddAssistantMessage(chatResponseMessageContent);
                }
            }

            return chatHistory;
        }

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}