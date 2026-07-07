namespace AIKnowledgeAssistant.Api.Interfaces
{
    public interface IEmbeddingService
    {
        Task<List<float>> GenerateEmbeddingAsync(string text);

    }
}
