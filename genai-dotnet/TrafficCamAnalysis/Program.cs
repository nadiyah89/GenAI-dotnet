using System.Text.Json;
using System.Text.Json.Serialization;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;


// ============================================================
// Get Gemini API key
// ============================================================

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException(
        "Gemini API key not found.");


// ============================================================
// Create Gemini client
// ============================================================

var client = new Client(apiKey: apiKey);


// ============================================================
// Process traffic camera images
// ============================================================

foreach (var imagePath in Directory.GetFiles("images", "*.jpg"))
{
    var name = Path.GetFileNameWithoutExtension(imagePath);

    Console.WriteLine($"\nProcessing camera: {name}");

    // --------------------------------------------------------
    // Read image
    // --------------------------------------------------------

    var imageBytes =
        await System.IO.File.ReadAllBytesAsync(imagePath);


    // --------------------------------------------------------
    // Create prompt
    // --------------------------------------------------------

    var prompt = $$"""
        Extract information from this image from camera {{name}}.

        Respond with a JSON object in this form:

        {
            "Status": "Clear",
            "NumCars": 0,
            "NumTrucks": 0
        }

        Status must be one of:
        "Clear", "Flowing", "Congested", "Blocked"

        NumCars must be the number of cars visible.
        NumTrucks must be the number of trucks visible.
        """;


    // --------------------------------------------------------
    // Send image + prompt to Gemini
    // --------------------------------------------------------

    var response = await client.Models.GenerateContentAsync(
        model: "gemini-3.6-flash",
        contents:
        [
            new Content
            {
                Parts =
                [
                    new Part
                    {
                        Text = prompt
                    },

                    new Part
                    {
                        InlineData = new Blob
                        {
                            MimeType = "image/jpeg",
                            Data = imageBytes
                        }
                    }
                ]
            }
        ],
        config: new GenerateContentConfig
        {
            ResponseMimeType = "application/json"
        });


    // --------------------------------------------------------
    // Get JSON response
    // --------------------------------------------------------

    var json = response.Text;

    Console.WriteLine($"Gemini JSON: {json}");


    // --------------------------------------------------------
    // Convert JSON → C# object
    // --------------------------------------------------------

    var result =
        JsonSerializer.Deserialize<TrafficCamResult>(json);


    // --------------------------------------------------------
    // Display result
    // --------------------------------------------------------

    if (result is not null)
    {
        Console.WriteLine(
            $"{name} status: {result.Status} " +
            $"(cars: {result.NumCars}, " +
            $"trucks: {result.NumTrucks})");
    }
}


Console.WriteLine("\nFinished processing all cameras.");
Console.ReadKey();


// ============================================================
// Result model
// ============================================================

class TrafficCamResult
{
    public TrafficStatus Status { get; set; }

    public int NumCars { get; set; }

    public int NumTrucks { get; set; }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrafficStatus
    {
        Clear,
        Flowing,
        Congested,
        Blocked
    }
}