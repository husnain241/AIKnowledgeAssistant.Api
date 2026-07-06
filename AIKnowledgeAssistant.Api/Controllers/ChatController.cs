using AIKnowledgeAssistant.Api.DTOs;
using AIKnowledgeAssistant.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> Ask(ChatRequestDto request)
    {
        var response = await _chatService.AskAsync(request);

        return Ok(response);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetAllConversations()
    {
        var conversations = await _chatService.GetAllConversationsAsync();

        return Ok(conversations);
    }

    [HttpGet("conversations/{id}")]
    public async Task<IActionResult> GetConversationById(int id)
    {
        var conversation = await _chatService.GetConversationByIdAsync(id);

        return Ok(conversation);
    }

    [HttpPut("conversations/{id}")]
    public async Task<IActionResult> RenameConversation(int id, RenameConversationDto dto)
    {
        await _chatService.RenameConversationAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("conversations/{id}")]
    public async Task<IActionResult> DeleteConversation(int id)
    {
        await _chatService.DeleteConversationAsync(id);

        return NoContent();
    }
}