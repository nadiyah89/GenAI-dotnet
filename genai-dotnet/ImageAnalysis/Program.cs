using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

// ============================================================
// 1. Get Gemini API key from User Secrets
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
// 3. User prompts
// ============================================================

var promptDescribe =
    "Describe the image.";

var promptAnalyze =
    "How many red cars are in the picture? " +
    "What other car colors are there?";

string systemPrompt =
"You are a useful assistant that describes images using a direct style.";

var userPrompt = promptAnalyze;

List<ChatMessage> messages =
[
    new ChatMessage(ChatRole.System, systemPrompt),
    new ChatMessage(ChatRole.User, userPrompt),
];


// ============================================================
// 4. Image path
// ============================================================

var imageFileName = "cars.png";

var imagePath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "images",
    imageFileName);


// ============================================================
// 5. Read image
// ============================================================

var imageBytes =
    await System.IO.File.ReadAllBytesAsync(imagePath);


// ============================================================
// 6. Send image + prompt to Gemini
// ============================================================

var response =
    await client.Models.GenerateContentAsync(
        model: "gemini-3.6-flash",
        contents:
        [
            new Content
            {
                Parts =
                [
                    new Part
                    {
                        Text = userPrompt
                    },
                    new Part
                    {
                        InlineData = new Blob
                        {
                            MimeType = "image/png",
                            Data = imageBytes
                        }
                    }
                ]
            }
        ]);


// ============================================================
// 7. Display response
// ============================================================

Console.WriteLine($"Prompt: {userPrompt}");
Console.WriteLine($"Image: {imageFileName}");
Console.WriteLine($"Response: {response.Text}");

Console.ReadKey();