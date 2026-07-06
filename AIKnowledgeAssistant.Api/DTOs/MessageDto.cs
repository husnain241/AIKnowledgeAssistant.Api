namespace AIKnowledgeAssistant.Api.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }

        public string Role { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
