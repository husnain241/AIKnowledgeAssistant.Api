using System.ComponentModel.DataAnnotations;

namespace AIKnowledgeAssistant.Api.DTOs
{
    public class UploadDocumentDto
    {
        [Required]
        public int ConversationId { get; set; }

        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
