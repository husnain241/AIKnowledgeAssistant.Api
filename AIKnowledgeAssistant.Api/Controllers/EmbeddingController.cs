using AIKnowledgeAssistant.Api.DTOs;
using AIKnowledgeAssistant.Api.Interfaces;
using Google.GenAI;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmbeddingController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly Client _client;


        public EmbeddingController(IEmbeddingService embeddingService, Client client)
        {
            _embeddingService = embeddingService;
            _client = client;
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestEmbedding(EmbeddingRequestDto request)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(request.Text);

            return Ok(new
            {
                Dimensions = embedding.Count,
                Sample = embedding.Take(5)
            });
        }
        [HttpGet("models")]
        public async Task<IActionResult> GetModels()
        {
            var result = new List<object>();

            var pager = await _client.Models.ListAsync();

           await foreach (var model in pager)
            {
                result.Add(new
                {
                    model.Name,
                    model.DisplayName,
                    model.SupportedActions
                });
            }
            return Ok(result);
        }
    }
}