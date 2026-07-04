using AIKnowledgeAssistant.Api.Data;

namespace AIKnowledgeAssistant.Api.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
