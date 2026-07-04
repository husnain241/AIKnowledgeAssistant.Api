using System.ComponentModel.DataAnnotations;

public class ChatRequestDto
{
    public int? ConversationId { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;
}