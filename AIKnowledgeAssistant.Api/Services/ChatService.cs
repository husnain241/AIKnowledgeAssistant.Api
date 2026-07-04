using AIKnowledgeAssistant.Api.Configuration;
using AIKnowledgeAssistant.Api.DTOs;
using AIKnowledgeAssistant.Api.Interfaces;
using AIKnowledgeAssistant.Api.Models;
using AIKnowledgeAssistantAPI.Data;
using Google.GenAI;
using Microsoft.Extensions.Options;

namespace AIKnowledgeAssistant.Api.Services;

public class ChatService : IChatService
{
    private readonly GeminiOptions _geminiOptions;
    private readonly Client _client;
    private readonly ApplicationDbContext _context;


    public ChatService(IOptions<GeminiOptions> options, ApplicationDbContext context)
    {
        _geminiOptions = options.Value;

        _client = new Client(apiKey: _geminiOptions.ApiKey);
        _context = context;
    }

    public async Task<ChatResponseDto> AskAsync(ChatRequestDto request)
    {
        if (request.ConversationId == null)
        {
            var conversation = new Conversation
            {
                Title = GenerateTitle(request.Message),
                CreatedAt = DateTime.UtcNow
            };

            _context.Conversations.Add(conversation);

            await _context.SaveChangesAsync();
        }


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

    //helper method to generate a title for the conversation based on the first message
    private static string GenerateTitle(string message)
    {
        return message.Length <= 50
            ? message
            : message.Substring(0, 50) + "...";
    }
}   