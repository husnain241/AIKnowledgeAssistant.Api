namespace AIKnowledgeAssistant.Api.DTOs
{
    public class ConversationDetailDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<MessageDto> Messages { get; set; } = [];
    }
}
