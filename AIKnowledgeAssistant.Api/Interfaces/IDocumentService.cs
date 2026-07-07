namespace AIKnowledgeAssistant.Api.Interfaces
{
    public interface IDocumentService
    {
        Task UploadAsync(IFormFile file);

    }
}
