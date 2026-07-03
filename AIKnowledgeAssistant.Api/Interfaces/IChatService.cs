using AIKnowledgeAssistant.Api.DTOs;

namespace AIKnowledgeAssistant.Api.Interfaces;

public interface IChatService
{
    Task<ChatResponseDto> AskAsync(ChatRequestDto request);
}