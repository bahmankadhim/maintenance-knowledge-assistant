using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Mka.Functions;

public class ProcessDocument(ILogger<ProcessDocument> logger)
{
    [Function(nameof(ProcessDocument))]
    public void Run(
        [BlobTrigger("documents/{name}", Connection = "DocsStorage")] byte[] content,
        string name)
    {
        logger.LogInformation("New document {Name} ({Size} bytes)", name, content.Length);

        // Module 3 (RAG): extract text, split into chunks, create embeddings, save to the index.
    }
}