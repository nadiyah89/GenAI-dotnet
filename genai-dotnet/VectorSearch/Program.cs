using CommunityToolkit.VectorData.InMemory;
using Google.GenAI;
using Microsoft.Extensions.Configuration;
using VectorSearch;

// ============================================================
// 1. Get Gemini API key
// ============================================================

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException(
        "Gemini API key not found.");


// ============================================================
// 2. Create Gemini client
// ============================================================

var client = new Client(apiKey: apiKey);


// ============================================================
// 3. Create in-memory vector store
// ============================================================

var vectorStore = new InMemoryVectorStore();

var moviesStore =
    vectorStore.GetCollection<int, Movie>("movies");

await moviesStore.EnsureCollectionExistsAsync();


// ============================================================
// 4. Generate embeddings and store movies
// ============================================================

foreach (var movie in MovieData.Movies)
{
    Console.WriteLine(
        $"Generating embedding for: {movie.Title}");

    var response = await client.Models.EmbedContentAsync(
        model: "gemini-embedding-2",
        contents: movie.Description
    );

    var values = response.Embeddings[0].Values;

    movie.Vector = new ReadOnlyMemory<float>(
        values.Select(x => (float)x).ToArray());

    await moviesStore.UpsertAsync(movie);

    Console.WriteLine(
        $"Stored: {movie.Title} " +
        $"({movie.Vector.Length} dimensions)");
}

Console.WriteLine("\nAll movies stored successfully.");


// Generate embedding for user's query
var query = "A science fiction movie about space travel";

var queryResponse = await client.Models.EmbedContentAsync(
    model: "gemini-embedding-2",
    contents: query);

var queryEmbedding = queryResponse.Embeddings[0].Values
    .Select(x => (float)x)
    .ToArray();


// Search vector store
var searchResults =
    moviesStore.SearchAsync(queryEmbedding, top: 2);


// Display results
await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}




Console.ReadKey();