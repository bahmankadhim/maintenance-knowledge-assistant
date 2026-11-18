using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Azure.Search.Documents.Indexes;
using Mka.Rag;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
var config = builder.Configuration;

// On Azure, App Service sets WEBSITE_INSTANCE_ID. Locally it is missing.
TokenCredential credential = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") is null
    ? new AzureCliCredential()
    : new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);

builder.Services.AddSingleton(_ =>
    new AzureOpenAIClient(new Uri(config["AzureOpenAI:Endpoint"]!), credential)
        .GetEmbeddingClient(config["AzureOpenAI:EmbeddingDeployment"]));

builder.Services.AddSingleton(_ =>
    new KnowledgeIndex(
        new SearchIndexClient(new Uri(config["Search:Endpoint"]!), credential),
        config["Search:IndexName"]!));

builder.Build().Run();