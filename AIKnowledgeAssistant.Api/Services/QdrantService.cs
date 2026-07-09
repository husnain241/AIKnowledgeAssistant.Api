using AIKnowledgeAssistant.Api.Configuration;
using AIKnowledgeAssistant.Api.Interfaces;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Services;

public class QdrantService : IQdrantService
{
    private readonly QdrantClient _client;
    private readonly QdrantOptions _options;

    public QdrantService(QdrantClient client, IOptions<QdrantOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task CreateCollectionAsync()
    {
        // CollectionExistsAsync avoids an exception-driven flow on repeated startup calls
        var exists = await _client.CollectionExistsAsync(_options.CollectionName);
        if (exists)
            return;

        await _client.CreateCollectionAsync(
            collectionName: _options.CollectionName,
            vectorsConfig: new VectorParams
            {
                Size = 3072,                 // matches Gemini text-embedding-004 output dimensionality
                Distance = Distance.Cosine    // cosine is the standard choice for text embeddings
            });
    }

    public async Task StoreEmbeddingAsync(int chunkId, List<double> embedding, string content)
    {
        var point = new PointStruct
        {
            Id = (ulong)chunkId,    
            Vectors = embedding.Select(d => (float)d).ToArray(), // gRPC vectors are float[], not double[]
            Payload =
            {
                ["chunkId"] = chunkId,
                ["content"] = content
            }
        };

        await _client.UpsertAsync(_options.CollectionName, new List<PointStruct> { point });
    }

    public async Task<List<int>> SearchSimilarAsync(List<double> embedding, int top = 5)
    {
        var queryVector = embedding.Select(d => (float)d).ToArray();

        var results = await _client.QueryAsync(
            collectionName: _options.CollectionName,
            query: queryVector,
            limit: (ulong)top,
            payloadSelector: true // include payload so we can read chunkId back
        );

        return results
            .Select(r => (int)r.Payload["chunkId"].IntegerValue)
            .ToList();
    }
}