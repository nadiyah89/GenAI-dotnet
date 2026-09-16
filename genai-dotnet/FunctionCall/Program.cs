using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Google.GenAI;

// STEP 1 — Get credential from User Secrets
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException("Gemini API key not found.");


// STEP 2 — Create Gemini client
var client = new Client(apiKey: apiKey);


// STEP 3 — Create IChatClient + Function Invocation
IChatClient chatClient =
    new ChatClientBuilder(
        client.AsIChatClient("gemini-3.6-flash"))
    .UseFunctionInvocation()
    .Build();


// STEP 4 — Create chat options with function/tool
var chatOptions = new ChatOptions
{
    Tools =
    [
        AIFunctionFactory.Create(
            (string location) =>
            {
                Console.WriteLine(
                    $"\n*** TOOL CALLED: get_current_weather({location}) ***");

                var temperature = Random.Shared.Next(5, 20);

                var conditions = Random.Shared.Next(0, 2) == 0
                    ? "sunny"
                    : "rainy";

                return $"The weather is {temperature} degrees C and {conditions}.";
            },
            "get_current_weather",
            "Get the current weather in a given location."
        )
    ]
};


// STEP 5 — Create conversation history
var chatHistory = new List<ChatMessage>
{
    new ChatMessage(
        ChatRole.System,
        """
        You are a hiking enthusiast who helps people discover fun hikes in their area.
        You are upbeat and friendly.

        When the user asks about current weather,
        use the get_current_weather tool.
        """)
};


// STEP 6 — Add user message
chatHistory.Add(
    new ChatMessage(
        ChatRole.User,
        """
        I live in Istanbul and I'm looking for a moderate intensity hike.
        What's the current weather like?
        """));


// STEP 7 — Display user message
Console.WriteLine(
    $"{chatHistory.Last().Role} >>> {chatHistory.Last().Text}");

Console.WriteLine("\nCalling Gemini...");


// STEP 8 — Send request
try
{
    ChatResponse response =
        await chatClient.GetResponseAsync(
            chatHistory,
            chatOptions);

    Console.WriteLine("\nAssistant >>>");
    Console.WriteLine(response.Text);

    Console.WriteLine(
        $"\nTokens used: in={response.Usage?.InputTokenCount}, " +
        $"out={response.Usage?.OutputTokenCount}");
}
catch (Exception ex)
{
    Console.WriteLine("\nERROR:");
    Console.WriteLine(ex);
}

//Console.ReadKey();