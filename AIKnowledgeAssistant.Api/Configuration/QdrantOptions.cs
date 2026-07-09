namespace AIKnowledgeAssistant.Api.Configuration
{
    public class QdrantOptions
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 6334;
        public string ApiKey { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;
    }
}
