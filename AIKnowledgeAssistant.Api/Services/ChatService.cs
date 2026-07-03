using AIKnowledgeAssistant.Api.DTOs;
using AIKnowledgeAssistant.Api.Interfaces;

namespace AIKnowledgeAssistant.Api.Services;

public class ChatService : IChatService
{
    public async Task<ChatResponseDto> AskAsync(ChatRequestDto request)
    {
        return await Task.FromResult(new ChatResponseDto
        {
            Answer = $"You said: {request.Message}"
        });
    }
}   