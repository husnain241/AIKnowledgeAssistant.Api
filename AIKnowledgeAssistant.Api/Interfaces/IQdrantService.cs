namespace AIKnowledgeAssistant.Api.Interfaces
{
    public interface IQdrantService
    {
        Task CreateCollectionAsync();

        Task StoreEmbeddingAsync(
            int chunkId,
            List<double> embedding,
            string content);

        Task<List<int>> SearchSimilarAsync(
            List<double> embedding,
            int top = 5);
    }
}
