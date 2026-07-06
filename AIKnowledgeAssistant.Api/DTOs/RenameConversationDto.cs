using System.ComponentModel.DataAnnotations;

namespace AIKnowledgeAssistant.Api.DTOs
{
    public class RenameConversationDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
    }
}
