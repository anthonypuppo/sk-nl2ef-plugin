# Semantic Kernel Plugin Hackathon Entry

This project is my entry to the Semantic Kernel Plugin Hackathon. It's designed to be a drop-in plugin service that can expose an existing database to be queried via natural language. It accomplishes this by leveraging the power of Entity Framework Core and OpenAI's embedding/GPT models to generate and construct SQL queries that retrieve relevant information for RAG based responses.

The demo connects to a modified version of the SQLite movies database available [here](https://www.kaggle.com/datasets/luizpaulodeoliveira/imdb-project-sql).

## Built With

- .NET 7 Minimal Web APIs
- Visual Studio Code (C# and C# Dev Kit Extensions)

## How It Works

1. **Database Creation Script Generation**: Use Entity Framework Core to generate the database creation script.
2. **Embedding Creation**: Create embeddings for each part of the database creation script.
3. **User Input Processing**: Take a user's input and get the most relevant parts of the database creation script that will help build a SQL query.
4. **SQL Query Construction**: Build the SQL query using a GPT model.
5. **Query Execution**: Run the query and attempt to retry and have the model fix its query if it fails.
6. **Response Formatting**: Format the response data as a CSV which the model can easily parse.
7. **Answer Generation**: Answer the user's question using the retrieved data for grounding (RAG).

## Future Enhancements

- Code cleanup
- Modifying the prompts to produce better results
- Moving hardcoded options to be environment configurable

## Pitfalls Before Moving to Production

- Ensure the user connecting to the database has the appropriate permissions (or lack thereof) to prevent SQL injection or users viewing data they shouldn't.
- Seed the kernel database schema memories as part of a preprocessing pipeline instead of every run.
- Be aware of responses overloading the model token window.

## Getting Started

To get a local copy up and running follow these simple steps.

### Prerequisites

- .NET 7
- Visual Studio Code
- C# and C# Dev Kit Extensions

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/anthonypuppo/sk-nl2ef-plugin.git
   ```
2. Install .NET packages
   ```sh
   dotnet restore
   ```
3. Run the project
   ```sh
   dotnet run
   ```

## Usage

Configure the DB context to expose the relevant parts of the database to the model. The service will automatically seed the database creation script embeddings at startup.

## Plugin Manifest

When running locally the plugin will be exposed at https://localhost:7012/.well-known/ai-plugin.json. CORS defaults to allowing ChatGPT as well as https://localhost:7012.
