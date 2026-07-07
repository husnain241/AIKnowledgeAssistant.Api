namespace AIKnowledgeAssistant.Api.Interfaces
{
    public interface IEmbeddingService
    {
        Task<List<double>> GenerateEmbeddingAsync(string text);

    }
}
