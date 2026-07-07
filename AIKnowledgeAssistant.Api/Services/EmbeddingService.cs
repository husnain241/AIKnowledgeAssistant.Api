using AIKnowledgeAssistant.Api.Interfaces;
using Google.GenAI;

namespace AIKnowledgeAssistant.Api.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly Client _client;

        public EmbeddingService(Client client)
        {
            _client = client;
        }

        public async Task<List<double>> GenerateEmbeddingAsync(string text)
        {
            var response = await _client.Models.EmbedContentAsync(
                model: "text-embedding-004",
                contents: text);

            return response.Embeddings[0].Values.ToList();
        }
    }
}
