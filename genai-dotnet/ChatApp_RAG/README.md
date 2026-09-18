# AI Chat with Custom Data

This project is an AI chat application that demonstrates how to chat with custom data using an AI language model.

>[!NOTE]
> Before running this project you need to configure your Gemini API key. See below for details.

### Prerequisites

To use Google Gemini, you need a Google AI Studio API key.

# Configure the AI Model Provider

## Using Google Gemini

To use Gemini, you will need a Gemini API key from Google AI Studio.

### 1. Create a Gemini API Key

Go to Google AI Studio and create an API key for your project.

### 2. Configure Gemini for Local Development

This project uses .NET User Secrets to store the Gemini API key so that the key is not tracked in source control.

In Visual Studio:

1. Right-click the project in **Solution Explorer**.
2. Select **Manage User Secrets**.
3. This will open the `secrets.json` file.
4. Add the following:

   ```json
   {
     "Gemini:ApiKey": "YOUR-GEMINI-API-KEY"
   }