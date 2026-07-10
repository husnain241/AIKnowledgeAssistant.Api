namespace AIKnowledgeAssistant.Api.Models
{
    public class ConversationDocument
    {
        public int ConversationId { get; set; }

        public Conversation Conversation { get; set; } = null!;

        public int DocumentId { get; set; }

        public Document Document { get; set; } = null!;

        public DateTime AttachedAt { get; set; } = DateTime.UtcNow;
    }
}
