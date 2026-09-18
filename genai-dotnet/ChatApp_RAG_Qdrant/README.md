
# AI Chat with Custom Data — Gemini + Qdrant + .NET Aspire
This project is an AI chat application that demonstrates how to chat with custom data using a **Google Gemini language model** and **Qdrant vector database**.

The application uses Retrieval-Augmented Generation (RAG) to retrieve relevant information from your own documents and provide grounded answers using Gemini.

> [!NOTE]
> Before running this project, configure your **Gemini API key**. Qdrant is automatically started in Docker when using .NET Aspire orchestration.

---

## Architecture

This project uses the following architecture:

```text
                    .NET Aspire
                         │
          ┌──────────────┼──────────────┐
          │              │              │
          ▼              ▼              ▼
   Chat Web App       Backend/API     Qdrant
          │                             │
          │                             │
          ▼                             ▼
      Gemini API                  Vector Database
          │
          ├── Chat / Generation
          └── Embeddings
````

The RAG flow is:

```text
Documents
    ↓
Document Reader
    ↓
Chunking
    ↓
Gemini Embeddings
    ↓
Qdrant Vector Database
    ↓
Semantic Search
    ↓
Relevant Chunks
    ↓
Gemini
    ↓
Grounded Answer + Citations
```

---

# Prerequisites

Before running the application, make sure you have:

* .NET 10 SDK
* Visual Studio 2022 or later
* Docker Desktop
* Google Gemini API key
* Internet connection for Gemini API access

You also need the following NuGet packages configured by the template/project:

* `Google.GenAI`
* `Microsoft.Extensions.AI`
* `Microsoft.Extensions.DataIngestion`
* Qdrant vector store integration
* .NET Aspire packages

---

# Configure Gemini

This project uses **Google Gemini** instead of GitHub Models or OpenAI.

Store your Gemini API key securely using **User Secrets**.

From the project directory, you can initialize User Secrets with:

```bash
dotnet user-secrets init
```

Then set the Gemini API key:

```bash
dotnet user-secrets set "Gemini:ApiKey" "YOUR_GEMINI_API_KEY"
```

Do **not** hard-code the API key inside `Program.cs`.

The application reads the key from configuration:

```csharp
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = configuration["Gemini:ApiKey"]
    ?? throw new InvalidOperationException("Gemini API key not found.");
```

The Gemini client can then be created using:

```csharp
var client = new Client(apiKey: apiKey);
```

---

# Configure Gemini Chat Model

The application uses Gemini as the chat/generation model.

For example:

```csharp
IChatClient chatClient =
    client.AsIChatClient("gemini-3.6-flash");
```

This gives the application an `IChatClient` abstraction through `Microsoft.Extensions.AI`.

The rest of the application can therefore work with:

```csharp
IChatClient
```

instead of being tightly coupled to provider-specific chat APIs.

---

# Configure Gemini Embeddings

Gemini is also used for generating document embeddings.

The application registers an embedding generator:

```csharp
builder.Services.AddEmbeddingGenerator(embeddingGenerator);
```

The embedding generator converts document chunks into vectors that can be stored in Qdrant.

Conceptually:

```text
Document chunk
      ↓
Gemini Embedding Model
      ↓
Vector
      ↓
Qdrant
```

---

# Setting up a local environment for Qdrant

This project is configured to run **Qdrant in a Docker container**.

Docker Desktop must be installed and running for the project to run successfully.

When using .NET Aspire orchestration, the Qdrant container is automatically started when the application runs.

You do not need to manually start Qdrant separately.

## Docker Desktop

Download, install, and run Docker Desktop from the official Docker website.

> [!NOTE]
> Qdrant and Docker are open-source products and are not maintained by Microsoft.

---

# .NET Aspire Orchestration

This project uses **.NET Aspire** to orchestrate the application's services.

The Aspire AppHost defines the application's infrastructure and dependencies.

The main services are:

```text
AppHost
   │
   ├── Chat Web Application
   │
   ├── Backend/API
   │
   └── Qdrant Container
```

Aspire automatically manages the local development environment and provides a unified dashboard.

The Aspire Dashboard can be used to inspect:

* Application resources
* Service status
* Logs
* Endpoints
* Environment variables
* Traces

---

# AppHost

The `AppHost` project is the orchestration layer.

It defines which services are required by the application.

Conceptually:

```text
AppHost
   │
   ├── Add Qdrant
   │
   └── Add Chat Web Application
          │
          └── Depends on Qdrant
```

When the AppHost starts, .NET Aspire launches the required resources and connects them together.

---

# Qdrant Vector Database

Qdrant is used as the application's vector database.

During document ingestion:

```text
PDF / Markdown
      ↓
DocumentReader
      ↓
Chunks
      ↓
Gemini Embeddings
      ↓
Qdrant
```

Each document chunk is represented by a vector and associated metadata.

Qdrant then allows the application to perform semantic similarity searches.

---

# Document Ingestion

Documents placed inside the application's data directory are processed by the ingestion pipeline.

For example:

```text
wwwroot/
    Data/
        Example_Emergency_Survival_Kit.pdf
        Example_GPS_Watch.md
```

The ingestion pipeline performs:

1. Read the document
2. Extract the content
3. Split the content into chunks
4. Generate embeddings using Gemini
5. Store the chunks and embeddings in Qdrant

Conceptually:

```text
Document
   ↓
Read
   ↓
Chunk
   ↓
Embed
   ↓
Store in Qdrant
```

---

# Semantic Search

When the user asks a question, the application performs semantic search.

For example:

```text
"What is included in the survival kit?"
```

The application:

```text
User Question
      ↓
Gemini Embedding
      ↓
Query Vector
      ↓
Qdrant Semantic Search
      ↓
Relevant Document Chunks
```

The retrieved chunks are then supplied to Gemini as context.

---

# Retrieval-Augmented Generation

The complete RAG process is:

```text
User Question
      ↓
Semantic Search
      ↓
Qdrant
      ↓
Relevant Chunks
      ↓
Context
      ↓
Gemini
      ↓
Final Answer
```

Gemini doesn't have to rely only on its general training knowledge.

Instead, the application provides relevant information retrieved from your documents.

This is the core idea of **Retrieval-Augmented Generation (RAG)**.

---

# Citations

The application can return citations pointing back to the source documents.

For example:

```text
User
 ↓
Question
 ↓
Qdrant
 ↓
Relevant chunk
 ↓
Gemini
 ↓
Answer + Citation
```

The user can click the citation to inspect the relevant section of the original document.

This helps users verify where the answer came from.

---

# Running the Application

## Using Visual Studio

1. Open the `.sln` file in Visual Studio.
2. Make sure Docker Desktop is running.
3. Set the `AppHost` project as the startup project.
4. Press `Ctrl+F5` or click **Start**.

The .NET Aspire Dashboard should open.

---

## Using Visual Studio Code

1. Open the project folder in Visual Studio Code.
2. Install the **C# Dev Kit** extension.
3. Open:

```text
ChatApp_RAG_Qdrant.AppHost/Program.cs
```

4. Run the AppHost project using the Run/Debug interface.

---

# Aspire Dashboard

When the application starts, the Aspire Dashboard acts as the central monitoring interface.

You should see resources similar to:

```text
Resources

Chat Web Application
Backend/API
Qdrant
```

You can select individual resources to inspect:

* Logs
* Endpoints
* Environment variables
* Resource status
* Traces

---

# Verify Qdrant

After ingestion completes, open the Qdrant endpoint from the Aspire Dashboard.

The Qdrant dashboard allows you to inspect the collections created by the application.

You should be able to see:

```text
Collection
    ↓
Points
    ↓
Document chunks
    ↓
Vector embeddings
    ↓
Metadata
```

This provides a visual confirmation that document ingestion successfully stored the vectors in Qdrant.

---

# Trust the localhost certificate

Several .NET Aspire applications use HTTPS by default.

If this is the first time you are running the application, you may encounter an error related to the local development certificate.

You can trust the ASP.NET Core development certificate using:

```bash
dotnet dev-certs https --trust
```

Then restart the application.

---

# Testing the Application

Once the Aspire Dashboard and chat application are running, ask a question about the provided documents.

For example:

```text
What is included in the survival kit?
```

The expected flow is:

```text
Question
   ↓
Gemini
   ↓
Semantic Search
   ↓
Qdrant
   ↓
Relevant survival-kit chunks
   ↓
Gemini
   ↓
Answer + Citation
```

The response should contain information retrieved from the survival-kit document.

---

# Observability with Aspire Traces

.NET Aspire provides distributed tracing for the application.

For example:

```text
User
 ↓
Chat Web Application
 ↓
Backend/API
 ↓
Qdrant
 ↓
Retrieved Context
 ↓
Gemini
 ↓
Response
```

The Aspire Dashboard can show the spans involved in these operations.

This makes it easier to understand how the different services communicate and helps with debugging performance or connectivity issues.

---

# Local Qdrant vs SQLite

In the earlier RAG application, we used SQLite as the vector store.

```text
Gemini
   ↓
SQLite
```

This project replaces SQLite with Qdrant:

```text
Gemini
   ↓
Qdrant
```

The RAG concept remains the same.

The main change is the vector database used to store and search embeddings.

---

# Gemini + Qdrant Architecture

The final architecture is:

```text
                    .NET Aspire
                         │
             ┌───────────┴───────────┐
             │                       │
             ▼                       ▼
      Chat Application             Qdrant
             │                       │
             ▼                       │
          Gemini                     │
       ┌─────┴─────┐                 │
       │           │                 │
      Chat     Embeddings            │
       │           │                 │
       │           └──────→ Qdrant ←─┘
       │
       ▼
   RAG Answer
       │
       ▼
   Citations
```

---

# Updating JavaScript Dependencies

This template may use JavaScript libraries located in:

```text
wwwroot/lib
```

For instructions on updating individual dependencies, refer to the README files provided with the respective libraries.

---

# Known Issues

## Gemini API availability

Gemini API requests can occasionally fail because of:

* Temporary service availability
* Rate limits
* Quota limits
* Model availability

If a temporary server-side error occurs, retry after some time.

For quota-related errors, check the Gemini API project and its available quota.

---

## Ollama and Docker

This project uses Gemini, so Ollama is **not required** for this version.

If you are also working with the separate Ollama RAG project, Docker/Ollama compatibility issues may affect that project independently.

---

# Learn More

To learn more about the technologies used in this project, explore:

* .NET AI
* Microsoft.Extensions.AI
* Google Gemini API
* Retrieval-Augmented Generation
* Qdrant
* .NET Aspire
* Docker
* Vector databases

---

# Project Summary

This project demonstrates how to build a RAG application using:

```text
.NET
+
Gemini
+
Gemini Embeddings
+
Qdrant
+
.NET Aspire
+
Docker
```

The application can:

* Ingest custom documents
* Generate embeddings
* Store vectors in Qdrant
* Perform semantic search
* Retrieve relevant document chunks
* Send context to Gemini
* Generate grounded answers
* Display citations
* Monitor the distributed application through the Aspire Dashboard

The result is a modern RAG architecture where **Gemini handles AI processing, Qdrant handles vector search, and .NET Aspire manages the local multi-service environment**.


