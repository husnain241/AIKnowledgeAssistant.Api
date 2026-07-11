namespace AIKnowledgeAssistant.Api.Interfaces
{
    public interface IQdrantService
    {
        Task CreateCollectionAsync();
        Task StoreEmbeddingAsync(int chunkId, int documentId, List<double> embedding, string content);
        Task<List<int>> SearchSimilarAsync(List<double> embedding, List<int> documentIds, int top = 5);
    }
}
