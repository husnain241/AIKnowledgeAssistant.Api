using AIKnowledgeAssistant.Api.Interfaces;
using AIKnowledgeAssistant.Api.Models;
using AIKnowledgeAssistantAPI.Data;
using System;
using UglyToad.PdfPig;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IChunkingService _chunkingService;
    private readonly IEmbeddingService _embeddingService;




    public DocumentService(ApplicationDbContext context, IChunkingService chunkingService, IEmbeddingService embeddingService)
    {
        _context = context;
        _chunkingService = chunkingService;
        _embeddingService = embeddingService;
    }

    public async Task UploadAsync(IFormFile file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        if (file.Length == 0)
            throw new Exception("The uploaded file is empty.");

        if (!Path.GetExtension(file.FileName)
            .Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Only PDF files are allowed.");
        }

        if (!string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Invalid file type. Please upload a PDF.");
        }

        const long maxFileSize = 10 * 1024 * 1024; // 10 MB

        if (file.Length > maxFileSize)
        {
            throw new Exception("The maximum allowed file size is 10 MB.");
        }


        string extractedText = string.Empty;

        try
        {
            using var stream = file.OpenReadStream();
            using var pdf = PdfDocument.Open(stream);

            foreach (var page in pdf.GetPages())
            {
                extractedText += page.Text + Environment.NewLine;
            }
        }
        catch (Exception ex)
        {


            throw new Exception("Unable to read the uploaded PDF.");
        }

        if (string.IsNullOrWhiteSpace(extractedText))
        {
            throw new Exception("No readable text was found in the PDF.");
        }

        var document = new Document
        {
            FileName = file.FileName,
            Content = extractedText,
            UploadedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);

        await _context.SaveChangesAsync();

    /// now start to saving a chunking process
    
        var chunks = _chunkingService.ChunkText(extractedText);
        foreach (var (chunk, index) in chunks.Select((value, i) => (value, i)))
        {
            var documentChunk = new DocumentChunk
            {
                DocumentId = document.Id,
                Content = chunk,
                ChunkIndex = index
            };
            _context.DocumentChunks.Add(documentChunk);
            await _embeddingService.GenerateEmbeddingAsync(documentChunk.Content);

        }
        await _context.SaveChangesAsync();






    }
}