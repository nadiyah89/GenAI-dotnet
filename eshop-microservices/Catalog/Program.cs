using Google.GenAI;
using Microsoft.Extensions.AI;
using Catalog.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<CatalogDbContext>(connectionName: "catalogdb");
builder.Services.AddScoped<ProductAIService>();
builder.Services.AddScoped<ProductService>();

// Add Gemini AI
var apiKey = builder.Configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException(
        "Missing configuration: Gemini:ApiKey.");

var client = new Client(apiKey: apiKey);

// Chat Client
IChatClient chatClient =
    client.AsIChatClient("gemini-3.6-flash");

builder.Services.AddChatClient(chatClient);

// Embedding Generator
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
    new GeminiEmbeddingGenerator(client);

builder.Services.AddEmbeddingGenerator(embeddingGenerator);

// Add Vector DB Search Operations
builder.AddQdrantClient("vectordb");

builder.Services.AddQdrantCollection<ulong, ProductVector>(
    "product-vectors");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseMigration();

app.MapProductEndpoints();

app.Run();