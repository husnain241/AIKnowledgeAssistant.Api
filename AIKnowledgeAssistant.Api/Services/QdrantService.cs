using AIKnowledgeAssistant.Api.Configuration;
using AIKnowledgeAssistant.Api.Interfaces;
using Microsoft.Extensions.Options;
using Qdrant.Client;

public class QdrantService : IQdrantService
{
    private readonly QdrantClient _client;
    private readonly QdrantOptions _options;

    public QdrantService(
        QdrantClient client,
        IOptions<QdrantOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    // Methods will come here...
}