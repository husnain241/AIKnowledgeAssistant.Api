using AIKnowledgeAssistant.Api.Configuration;
using AIKnowledgeAssistant.Api.Interfaces;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;   // <-- this line is missing


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
        var exists = await _client.CollectionExistsAsync(_options.CollectionName);

        if (!exists)
        {
            await _client.CreateCollectionAsync(
                collectionName: _options.CollectionName,
                vectorsConfig: new VectorParams
                {
                    Size = 3072,
                    Distance = Distance.Cosine
                });
        }

        // Hamesha ensure karo payload index exist kare
        await _client.CreatePayloadIndexAsync(
            collectionName: _options.CollectionName,
            fieldName: "documentId",
            schemaType: PayloadSchemaType.Integer);
    }
    public async Task StoreEmbeddingAsync(int chunkId, int documentId, List<double> embedding, string content)
    {
        var point = new PointStruct
        {
            Id = (ulong)chunkId,    
            Vectors = embedding.Select(d => (float)d).ToArray(), // gRPC vectors are float[], not double[]
            Payload =
            {
                ["chunkId"] = chunkId,
                ["documentId"] = documentId,
                ["content"] = content
            }
        };

        await _client.UpsertAsync(_options.CollectionName, new List<PointStruct> { point });
    }

    public async Task<List<int>> SearchSimilarAsync(List<double> embedding, List<int> documentIds, int top = 5)
    {
        var queryVector = embedding.Select(d => (float)d).ToArray();

        var filter = new Filter
        {
            Must =
        {
            Match("documentId", documentIds.Select(id => (long)id).ToList())
        }
        };

        var results = await _client.QueryAsync(
            collectionName: _options.CollectionName,
            query: queryVector,
            filter: filter,
            limit: (ulong)top,
            payloadSelector: true
        );

        return results
            .Select(r => (int)r.Payload["chunkId"].IntegerValue)
            .ToList();
    }
}