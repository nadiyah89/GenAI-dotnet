using Google.GenAI;
using Microsoft.Extensions.AI;
using ChatApp_RAG.Components;
using ChatApp_RAG.Services;
using ChatApp_RAG.Services.Ingestion;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// -----------------------------
// Gemini configuration
// -----------------------------

var apiKey = builder.Configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException(
        "Gemini API key not found.");

var geminiClient = new Client(apiKey: apiKey);

IChatClient chatClient =
    geminiClient.AsIChatClient("gemini-3.6-flash");

IEmbeddingGenerator<string, Embedding<float>>
    embeddingGenerator =
        new GeminiEmbeddingGenerator(geminiClient);

// -----------------------------
// Vector store
// -----------------------------

var vectorStorePath =
    Path.Combine(
        AppContext.BaseDirectory,
        "vector-store.db");

var vectorStoreConnectionString =
    $"Data Source={vectorStorePath}";

builder.Services.AddSqliteVectorStore(
    _ => vectorStoreConnectionString);

builder.Services.AddSqliteCollection<Guid, IngestedChunk>(
    IngestedChunk.CollectionName,
    vectorStoreConnectionString);

// -----------------------------
// RAG services
// -----------------------------

builder.Services.AddSingleton<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

builder.Services.AddKeyedSingleton(
    "ingestion_directory",
    new DirectoryInfo(
        Path.Combine(
            builder.Environment.WebRootPath,
            "Data")));

// -----------------------------
// AI services
// -----------------------------

builder.Services
    .AddChatClient(chatClient)
    .UseFunctionInvocation()
    .UseLogging();

builder.Services.AddEmbeddingGenerator(
    embeddingGenerator);

var app = builder.Build();

// -----------------------------
// HTTP pipeline
// -----------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();