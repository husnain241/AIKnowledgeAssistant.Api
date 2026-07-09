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
    private readonly IEmbeddingService _embeddingService;
    private readonly IQdrantService _qdrantService;



    public ChatService(
    Client client,
    IOptions<GeminiOptions> options,
    ApplicationDbContext context, ILogger<ChatService> logger, IEmbeddingService embeddingService, IQdrantService qdrantService)
    {
        _client = client;
        _geminiOptions = options.Value;
        _context = context;
        _logger = logger;
        _embeddingService = embeddingService;
        _qdrantService = qdrantService;


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

        //implement question Embedding and Qdrant search here to retrieve relevant context for the AI model
        var questionEmbedding = await _embeddingService.GenerateEmbeddingAsync(request.Message);
        var chunkIds = await _qdrantService.SearchSimilarAsync(questionEmbedding);

        var relevantChunks = await _context.DocumentChunks
    .Where(c => chunkIds.Contains(c.Id))
    .OrderBy(c => c.ChunkIndex)
    .ToListAsync();

        var context = string.Join(
    "\n\n",
    relevantChunks.Select(c => c.Content));

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
Text = $"""
You are an AI Knowledge Assistant.

Answer ONLY using the provided context.

If the answer is not available in the context, reply:
"I couldn't find this information in the uploaded documents."

Do not make up information.
Do not use your own knowledge.

Context:
{context}

Question:
{request.Message}
"""       }
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


    public async Task<List<ConversationListDto>> GetAllConversationsAsync()
    {
        return await _context.Conversations
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ConversationListDto
            {
                Id = c.Id,
                Title = c.Title,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ConversationDetailDto> GetConversationByIdAsync(int id)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conversation is null)
            throw new Exception("Conversation not found.");

        return new ConversationDetailDto
        {
            Id = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,

            Messages = conversation.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Role = m.Role,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                })
                .ToList()
        };

    }

    public async Task RenameConversationAsync(int id, RenameConversationDto dto)
    {
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conversation is null)
            throw new Exception("Conversation not found.");

        conversation.Title = dto.Title;

        await _context.SaveChangesAsync();
    }
    public async Task DeleteConversationAsync(int id)
    {
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == id);

        if (conversation is null)
            throw new Exception("Conversation not found.");

        _context.Conversations.Remove(conversation);

        await _context.SaveChangesAsync();
    }
}       