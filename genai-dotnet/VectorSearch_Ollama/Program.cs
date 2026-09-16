using CommunityToolkit.VectorData.InMemory;
using Microsoft.Extensions.AI;
using OllamaSharp;
using VectorSearch_Ollama;


// ============================================================
// Create Ollama embedding generator
// ============================================================

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
    new OllamaApiClient(
        new Uri("http://localhost:11434"),
        "all-minilm");


// ============================================================
// Create in-memory vector store
// ============================================================

var vectorStore = new InMemoryVectorStore();

var moviesStore =
    vectorStore.GetCollection<int, Movie>("movies");

await moviesStore.EnsureCollectionExistsAsync();


// ============================================================
// Generate embeddings and store movies
// ============================================================

foreach (var movie in MovieData.Movies)
{
    Console.WriteLine(
        $"Generating embedding for: {movie.Title}");

    var embedding =
        await embeddingGenerator.GenerateVectorAsync(
            movie.Description);

    movie.Vector = embedding;

    await moviesStore.UpsertAsync(movie);

    Console.WriteLine(
        $"Stored: {movie.Title} " +
        $"({movie.Vector.Length} dimensions)");
}

Console.WriteLine("\nAll movies stored successfully.");


// ============================================================
// Generate embedding for user's query
// ============================================================

var query = "A science fiction movie about space travel";

var queryEmbedding =
    await embeddingGenerator.GenerateVectorAsync(query);


// ============================================================
// Search vector store
// ============================================================

var searchResults =
    moviesStore.SearchAsync(queryEmbedding, top: 2);


// ============================================================
// Display results
// ============================================================

await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}

Console.ReadKey();