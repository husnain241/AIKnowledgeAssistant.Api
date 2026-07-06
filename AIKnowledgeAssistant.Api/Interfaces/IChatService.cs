using AIKnowledgeAssistant.Api.DTOs;

namespace AIKnowledgeAssistant.Api.Interfaces;

public interface IChatService
{
    Task<ChatResponseDto> AskAsync(ChatRequestDto request);
    Task<List<ConversationListDto>> GetAllConversationsAsync();

    Task<ConversationDetailDto> GetConversationByIdAsync(int id);

    Task RenameConversationAsync(int id, RenameConversationDto dto);

    Task DeleteConversationAsync(int id);

}