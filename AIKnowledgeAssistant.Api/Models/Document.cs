namespace AIKnowledgeAssistant.Api.Models
{
    public class Document
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ICollection<DocumentChunk> Chunks { get; set; } = [];

        public ICollection<ConversationDocument> ConversationDocuments { get; set; }
    = new List<ConversationDocument>();
    }
}
