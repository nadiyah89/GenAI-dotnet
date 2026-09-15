using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Google.GenAI;

// STEP 1 — Get credential from User Secrets
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["Gemini:ApiKey"];

// STEP 2 — Create Gemini client
var client = new Client(apiKey: apiKey);

// STEP 3 — Create IChatClient
IChatClient chatClient =
    client.AsIChatClient("gemini-3.6-flash");


// Create chat options with function/tool

var chatOptions = new ChatOptions
{
    Tools =
    [
        AIFunctionFactory.Create(
            (string location, string unit) =>    //parameters
            {
                // Here you would call a weather API
                var temperature = Random.Shared.Next(5, 20);

                var conditions = Random.Shared.Next(0, 2) == 0
                    ? "sunny"
                    : "rainy";

                return $"The weather is {temperature} degrees C and {conditions}.";
            },
            "get_current_weather",  //name
            "Get the current weather in a given location" //description
        )
    ]
};

// Create conversation history

var chatHistory = new List<ChatMessage>
{
    new ChatMessage(
        ChatRole.System,
        """
        You are a hiking enthusiast who helps people discover fun hikes in their area.
        You are upbeat and friendly.
        """)
};

// Add user message

chatHistory.Add(
    new ChatMessage(
        ChatRole.User,
        """
        I live in Istanbul and I'm looking for a moderate intensity hike.
        What's the current weather like?
        """));

Console.WriteLine(
    $"{chatHistory.Last().Role} >>> {chatHistory.Last().Text}");

// Send request
ChatResponse response =
    await chatClient.GetResponseAsync(chatHistory, chatOptions);

// Add assistant response to history
chatHistory.Add(new(ChatRole.Assistant, response.Text));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

Console.ReadLine();