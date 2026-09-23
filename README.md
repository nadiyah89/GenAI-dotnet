# Generative AI with .NET / C#

This repository documents my learning journey through a **Generative AI with .NET / C#** course. Throughout the course, I built multiple hands-on applications while progressing from basic LLM integration to embeddings, RAG, vector databases, MCP, multimodal AI, and AI-powered applications.


## Course Overview

Throughout the course, I learned how to integrate Generative AI into **.NET and C# applications** using different AI providers, SDKs, abstractions, and supporting technologies.

The course covered:

* LLM integration with .NET
* `Microsoft.Extensions.AI`
* Chat applications
* Local AI with Ollama
* Function / Tool Calling
* Structured Output
* Embeddings
* Vector Search
* Document Ingestion
* Chunking
* Retrieval-Augmented Generation (RAG)
* Qdrant Vector Database
* .NET Aspire
* Multimodal / Image Analysis
* MCP and MarkItDown
* AI-powered distributed applications



# What I Learned

## 1. AI Chat Applications

I learned how to connect .NET applications with Large Language Models and build conversational AI functionality using C#.

I worked with:

* `Microsoft.Extensions.AI`
* `IChatClient`
* Chat messages
* System instructions
* Prompt handling
* AI response generation

I also learned how AI abstractions can make applications less dependent on a specific AI provider.


## 2. AI Providers

During the course, I explored different AI providers and approaches, including:

* Google Gemini
* OpenAI
* GitHub Models
* Azure AI
* Ollama
* Local LLMs

For my implementations, I primarily worked with **Google Gemini** using the `Google.GenAI` SDK.


## 3. Local AI with Ollama

I learned how to run AI models locally using **Ollama** and Docker.

I worked with:

* Local LLMs
* Local embedding models
* Docker-based AI services
* AI integration with .NET applications

This helped me understand how locally hosted AI models can be integrated into applications.



## 4. Function / Tool Calling

I learned how LLMs can interact with application-defined tools to retrieve information or perform specific operations.

I explored concepts such as:

* Tool definitions
* Function calls
* Tool execution
* Returning tool results to the model
* AI orchestration

This helped me understand how AI applications can interact with real application data and functionality.


## 5. Structured Output

I learned how to make AI models return structured information instead of only natural-language responses.

I worked with:

* JSON output
* Structured data
* Strongly typed C# models
* AI-generated application data

This is useful when AI output needs to be consumed directly by application code.


# 6. Embeddings

I learned how text can be converted into numerical representations called **embeddings**.

I explored embeddings for:

* Documents
* User queries
* Product information
* Semantic similarity

I also implemented embedding generation using **Gemini**.


# 7. Vector Search

I learned how vector databases can store embeddings and perform similarity searches.

The concepts I worked with included:

* Vector embeddings
* Vector stores
* Similarity search
* Cosine distance
* Qdrant

Vector search became an important foundation for the RAG applications and later projects in the course.


# 8. Document Ingestion and Chunking

I learned how documents can be prepared for AI applications.

The general process was:

```text
Document
   ↓
Text Extraction
   ↓
Chunking
   ↓
Embedding Generation
   ↓
Vector Storage
   ↓
Semantic Search
```

I worked with documents such as PDF and Markdown files and learned why document chunking is important for retrieval-based AI systems.



# 9. Retrieval-Augmented Generation (RAG)

I learned how to build applications where an AI model retrieves relevant information before generating an answer.

The general workflow was:

```text
User Question
      ↓
Query Embedding
      ↓
Vector Search
      ↓
Relevant Context
      ↓
LLM
      ↓
Generated Answer
```

I learned how RAG can allow AI applications to work with external knowledge provided by the application.



# 10. RAG with SQLite

I built a RAG application using a local SQLite-based vector store.

### Project

`ChatApp_RAG`

The application demonstrated:

* Document ingestion
* Chunking
* Embedding generation
* Vector storage
* Semantic retrieval
* AI-generated responses


# 11. RAG with Ollama

I then built another RAG application using **Ollama** for local AI.

### Project

`ChatApp_RAG_Ollama`

The project involved:

* Ollama
* Local LLM
* Local embeddings
* SQLite vector storage
* Docker

I tested the application using example documents to understand the complete local RAG workflow.



# 12. RAG with Qdrant and .NET Aspire

I also worked with **Qdrant** as a vector database and **.NET Aspire** for application orchestration.

### Project

`ChatApp_RAG_Qdrant`

I learned how Qdrant can be integrated into a .NET application for:

* Vector storage
* Embedding retrieval
* Similarity search
* RAG workflows

I also learned how .NET Aspire can orchestrate supporting infrastructure such as Qdrant and other application services.


# 13. Multimodal / Image Analysis

I learned how Generative AI models can work with images in addition to text.

I explored:

* Image input
* Image analysis
* Multimodal prompts
* Combining visual and textual information

This introduced me to AI applications that are not limited to text-only interactions.



#  14. MCP and MarkItDown

I learned about **Model Context Protocol (MCP)** and how AI applications can connect models with external tools and data sources.

I also explored **MarkItDown** for converting documents into AI-friendly text.

This helped me understand how modern AI applications can extend the capabilities of an AI model through external tools and services.


# Final Project

As the final project of the course, I extended a **.NET Aspire-based distributed eShop application** with Generative AI capabilities.

### Project Repository

 [**AI-Powered Semantic Product Search & Chatbot**](YOUR_GITHUB_REPO_URL)


# Course Outcome

This course gave me practical experience moving from individual AI concepts to complete AI-enabled .NET applications.

I progressed from simple LLM interactions to building a distributed application where Gemini, embeddings, Qdrant, PostgreSQL, .NET Aspire, ASP.NET Core, and Blazor work together to provide semantic product search.

The final project brought together the major concepts I learned throughout the course into one end-to-end application.
