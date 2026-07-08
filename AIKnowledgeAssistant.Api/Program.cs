using AIKnowledgeAssistant.Api.Configuration;
using AIKnowledgeAssistant.Api.Interfaces;
using AIKnowledgeAssistant.Api.Services;
using AIKnowledgeAssistantAPI.Data;
using Google.GenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the ApplicationDbContext with the dependency injection container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DBConnection"));
});
builder.Services.Configure<QdrantOptions>(
    builder.Configuration.GetSection("Qdrant"));

// Register the ChatService with the dependency injection container
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IChunkingService, ChunkingService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IQdrantService, QdrantService>();

// Register the Gemimi service with the dependency injection container
builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection("Gemini"));
builder.Services.AddSingleton<Client>(sp =>
{
    var options = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;

    return new Client(apiKey: options.ApiKey);
});
// Register the Qdrant service with the dependency injection container
builder.Services.AddSingleton<QdrantClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<QdrantOptions>>().Value;

    return new QdrantClient(
        host: options.Url,
        apiKey: options.ApiKey);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
