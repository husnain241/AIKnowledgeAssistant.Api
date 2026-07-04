using AIKnowledgeAssistant.Api.Configuration;
using AIKnowledgeAssistant.Api.DTOs;
using AIKnowledgeAssistant.Api.Interfaces;
using Google.GenAI;
using Microsoft.Extensions.Options;

namespace AIKnowledgeAssistant.Api.Services;

public class ChatService : IChatService
{
    private readonly GeminiOptions _geminiOptions;
    private readonly Client _client;

    public ChatService(IOptions<GeminiOptions> options)
    {
        _geminiOptions = options.Value;

        _client = new Client(apiKey: _geminiOptions.ApiKey);
    }

    public async Task<ChatResponseDto> AskAsync(ChatRequestDto request)
    {
        try
        {
            var response = await _client.Models.GenerateContentAsync(
                model: _geminiOptions.Model,
                contents: request.Message);
                
            return new ChatResponseDto
            {
                Answer = response.Candidates[0].Content.Parts[0].Text
            };
        }
        catch (Exception ex)
        {
            return new ChatResponseDto
            {
                Answer = $"An error occurred: {ex.Message}"
            };
        }
    }
}