namespace AIKnowledgeAssistant.Api.DTOs;

public class ChatResponseDto
{
    public int ConversationId { get; set; }

    public string Answer { get; set; } = string.Empty;
}