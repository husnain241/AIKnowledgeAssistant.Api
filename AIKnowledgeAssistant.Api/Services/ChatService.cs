using AIKnowledgeAssistant.Api.Configuration;
using AIKnowledgeAssistant.Api.Data;
using AIKnowledgeAssistant.Api.DTOs;
using AIKnowledgeAssistant.Api.Interfaces;
using AIKnowledgeAssistant.Api.Models;
using AIKnowledgeAssistantAPI.Data;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AIKnowledgeAssistant.Api.Services;

public class ChatService : IChatService
{
    private readonly GeminiOptions _geminiOptions;
    private readonly Client _client;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatService> _logger;



    public ChatService(
    Client client,
    IOptions<GeminiOptions> options,
    ApplicationDbContext context, ILogger<ChatService> logger)
    {
        _client = client;
        _geminiOptions = options.Value;
        _context = context;
        _logger = logger;

    }

    public async Task<ChatResponseDto> AskAsync(ChatRequestDto request)
    {
        _logger.LogInformation("AskAsync started.");
        Conversation conversation;


        if (request.ConversationId == null)
        {
            _logger.LogInformation("Creating a new conversation.");
            conversation = new Conversation
            {
                Title = GenerateTitle(request.Message),
                CreatedAt = DateTime.UtcNow
            };

            _context.Conversations.Add(conversation);

            await _context.SaveChangesAsync();
        }
        else
        {
               conversation = await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId);

            if (conversation == null)
            {
                throw new Exception("Conversation not found.");
            }
        }


        List<Message> orderedMessages = new();

        if (request.ConversationId != null)
        {
            orderedMessages = conversation.Messages
                .OrderBy(m => m.CreatedAt)
                .ToList();
        }


        var userMessage = new Message
        {
            Conversation = conversation,
            Role = Roles.User,
            Content = request.Message,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(userMessage);

        await _context.SaveChangesAsync();


        try
        {
            var contents = orderedMessages
     .Select(m => new Content
     {
         Role = m.Role == Roles.Assistant ? "model" : "user",
         Parts = new List<Part>
         {
            new Part
            {
                Text = m.Content
            }
         }
     })
     .ToList();
            contents.Add(new Content
            {
                Role = "user",
                Parts = new List<Part>
    {
        new Part
        {
            Text = request.Message
        }
    }
            });

            var response = await _client.Models.GenerateContentAsync(
                model: _geminiOptions.Model,
                contents: contents);

            var answer = response.Candidates[0].Content.Parts[0].Text;

            var aiMessage = new Message
            {
                Conversation = conversation,
                Role = Roles.Assistant,
                Content = answer,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(aiMessage);
            await _context.SaveChangesAsync();

            return new ChatResponseDto
            {
                ConversationId = conversation.Id,
                Answer = answer
            };
        }
        catch (Exception)
        {
            return new ChatResponseDto
            {
                ConversationId = conversation.Id,
                Answer = "Sorry, something went wrong while generating the AI response."
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