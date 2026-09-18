using ChatApp_RAG.Services;
using ChatApp_RAG_Qdrant.Web.Components;
using ChatApp_RAG_Qdrant.Web.Services;
using ChatApp_RAG_Qdrant.Web.Services.Ingestion;
using Google.GenAI;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Gemini
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException("Gemini API key not found.");

var client = new Client(apiKey: apiKey);

IChatClient chatClient =
    client.AsIChatClient("gemini-3.6-flash");

var embeddingGenerator =
    new GeminiEmbeddingGenerator(client);

builder.Services.AddChatClient(chatClient)
    .UseFunctionInvocation()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData =
            builder.Environment.IsDevelopment());

builder.Services.AddEmbeddingGenerator(embeddingGenerator);

// Qdrant
builder.AddQdrantClient("vectordb");
builder.Services.AddQdrantVectorStore();
builder.Services.AddQdrantCollection<Guid, IngestedChunk>(
    IngestedChunk.CollectionName);

builder.Services.AddSingleton<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

builder.Services.AddKeyedSingleton(
    "ingestion_directory",
    new DirectoryInfo(
        Path.Combine(builder.Environment.WebRootPath, "Data")));

var app = builder.Build();

app.MapDefaultEndpoints();

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