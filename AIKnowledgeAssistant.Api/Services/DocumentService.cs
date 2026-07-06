using AIKnowledgeAssistant.Api.Interfaces;
using AIKnowledgeAssistantAPI.Data;
using System;
using UglyToad.PdfPig;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;

    public DocumentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task UploadAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new Exception("File is empty.");

        string extractedText = string.Empty;

        using var stream = file.OpenReadStream();
        using var document = PdfDocument.Open(stream);

        foreach (var page in document.GetPages())
        {
            extractedText += page.Text;
        }
            
        var pdf = new Document
        {
            FileName = file.FileName,
            Content = extractedText,
            UploadedAt = DateTime.UtcNow
        };

        _context.Documents.Add(pdf);

        await _context.SaveChangesAsync();
    }
}