namespace AIKnowledgeAssistant.Api.DTOs
{
    public class ConversationListDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
