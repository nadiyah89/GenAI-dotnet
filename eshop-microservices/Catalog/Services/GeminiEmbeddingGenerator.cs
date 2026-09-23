using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.AI;

namespace Catalog.Services;

public sealed class GeminiEmbeddingGenerator(Client client)
    : IEmbeddingGenerator<string, Embedding<float>>
{
    private const string Model = "gemini-embedding-2";
    private const int Dimensions = 1536;

    private readonly EmbeddingGeneratorMetadata _metadata =
        new(
            providerName: "Google",
            providerUri: new Uri("https://ai.google.dev/"),
            defaultModelId: Model);

    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<Embedding<float>>();

        foreach (var value in values)
        {
            var response = await client.Models.EmbedContentAsync(
                model: Model,
                contents: value,
                config: new EmbedContentConfig
                {
                    OutputDimensionality = Dimensions,
                    TaskType = "RETRIEVAL_DOCUMENT"
                });

            var embedding = response.Embeddings?.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "Gemini returned no embedding.");

            if (embedding.Values is null)
            {
                throw new InvalidOperationException(
                    "Gemini returned an embedding without values.");
            }

            results.Add(
                new Embedding<float>(
                    embedding.Values.Select(v => (float)v).ToArray()));
        }

        return new GeneratedEmbeddings<Embedding<float>>(results);
    }

    public object? GetService(
        System.Type serviceType,
        object? serviceKey)
    {
        if (serviceKey is not null)
            return null;

        if (serviceType == typeof(EmbeddingGeneratorMetadata))
            return _metadata;

        return serviceType.IsInstanceOfType(this)
            ? this
            : null;
    }

    public void Dispose()
    {
    }
}