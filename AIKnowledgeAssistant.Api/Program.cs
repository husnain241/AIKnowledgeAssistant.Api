using AIKnowledgeAssistant.Api.Configuration;
using AIKnowledgeAssistant.Api.Interfaces;
using AIKnowledgeAssistant.Api.Services;
using AIKnowledgeAssistant.Api.Configuration;
using Google.GenAI;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the ChatService with the dependency injection container
builder.Services.AddScoped<IChatService, ChatService>();

// Register the Gemimi service with the dependency injection container
builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection("Gemini"));
builder.Services.AddSingleton<Client>(sp =>
{
    var options = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;

    return new Client(apiKey: options.ApiKey);
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
