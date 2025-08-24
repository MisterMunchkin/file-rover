using Microsoft.Extensions.AI;
using System.Net.Http.Json;
using Dto.Embedding;


namespace file_rover.service.model
{

    public class TextEmbeddingService : IEmbeddingGenerator<string, Embedding<float>>
    {
        public static readonly string ModelUrl = "http://127.0.0.1:1234/v1/embeddings";
        public static readonly string ModelName = "text-embedding-nomic-embed-text-v1.5";

        private readonly HttpClient _httpClient = new HttpClient();

        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                model = ModelName,
                input = values
            };

            var response = await _httpClient.PostAsJsonAsync(ModelUrl, request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<NomicEmbeddingResponse>(cancellationToken: cancellationToken);

            if (result?.data == null)
            {
                throw new Exception("No embeddings returned from the service.");
            }

            return [.. result.data.Select(e => new Embedding<float>(e.embedding.ToArray()))];
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            throw new NotImplementedException();
        }
    }
}