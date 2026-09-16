using Microsoft.Extensions.Configuration;
using Google.GenAI;
using System.Numerics.Tensors;

// Get API key from User Secrets
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException("Gemini API key not found.");


// Create Gemini client
var client = new Client(apiKey: apiKey);


// ============================================================
// 1. Generate a single embedding
// ============================================================

var response = await client.Models.EmbedContentAsync(
    model: "gemini-embedding-2",
    contents: "Hello World"
);

var embedding = response.Embeddings[0].Values;

Console.WriteLine(
    $"Embedding dimension: {embedding.Count}");

foreach (var value in embedding)
{
    Console.WriteLine(value);
}


// ============================================================
// 2. Generate embeddings for comparison
// ============================================================

var catResponse = await client.Models.EmbedContentAsync(
    model: "gemini-embedding-2",
    contents: "cat"
);

var dogResponse = await client.Models.EmbedContentAsync(
    model: "gemini-embedding-2",
    contents: "dog"
);

var kittenResponse = await client.Models.EmbedContentAsync(
    model: "gemini-embedding-2",
    contents: "kitten"
);


// Gemini returns List<double>
// Convert them to float[] because TensorPrimitives
// CosineSimilarity expects float vectors.

var catVector = catResponse.Embeddings[0].Values
    .Select(x => (float)x)
    .ToArray();

var dogVector = dogResponse.Embeddings[0].Values
    .Select(x => (float)x)
    .ToArray();

var kittenVector = kittenResponse.Embeddings[0].Values
    .Select(x => (float)x)
    .ToArray();


// ============================================================
// 3. Calculate Cosine Similarity
// ============================================================

var catDog =
    TensorPrimitives.CosineSimilarity(
        catVector,
        dogVector);

var catKitten =
    TensorPrimitives.CosineSimilarity(
        catVector,
        kittenVector);

var dogKitten =
    TensorPrimitives.CosineSimilarity(
        dogVector,
        kittenVector);


// ============================================================
// 4. Display results
// ============================================================

Console.WriteLine($"\nCat ↔ Dog: {catDog:F2}");
Console.WriteLine($"Cat ↔ Kitten: {catKitten:F2}");
Console.WriteLine($"Dog ↔ Kitten: {dogKitten:F2}");


Console.ReadKey();