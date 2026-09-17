using Microsoft.Extensions.AI;
using OllamaSharp;

// ============================================================
// Create a vision-capable Ollama chat client
// ============================================================

IChatClient client =
    new OllamaApiClient(
        new Uri("http://localhost:11434"),
        "llava");


// ============================================================
// User prompts
// ============================================================

var promptDescribe = "Describe the image";

var promptAnalyze =
    "How many red cars are in the picture? " +
    "and what other car colors are there?";


// ============================================================
// Create messages
// ============================================================

string systemPrompt =
    "You are a useful assistant that describes images using a direct style.";

var userPrompt = promptAnalyze;

List<ChatMessage> messages =
[
    new ChatMessage(ChatRole.System, systemPrompt),
    new ChatMessage(ChatRole.User, userPrompt),
];


// ============================================================
// Read image and add it to the message
// ============================================================

var imageFileName = "cars.png";

string image = Path.Combine(
    Directory.GetCurrentDirectory(),
    "images",
    imageFileName);

AIContent imageContent =
    new DataContent(
        File.ReadAllBytes(image),
        "image/png");

var imageMessage =
    new ChatMessage(ChatRole.User, [imageContent]);

messages.Add(imageMessage);


// ============================================================
// Send prompt + image to Ollama
// ============================================================

var response =
    await client.GetResponseAsync(messages);


// ============================================================
// Display response
// ============================================================

Console.WriteLine($"Prompt: {userPrompt}");
Console.WriteLine($"Image: {imageFileName}");
Console.WriteLine($"Response: {response.Text}");


Console.ReadKey();