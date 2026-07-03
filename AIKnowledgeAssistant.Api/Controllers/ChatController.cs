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
}