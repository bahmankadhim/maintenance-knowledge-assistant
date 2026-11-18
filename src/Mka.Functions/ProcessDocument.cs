using Mka.Rag;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Embeddings;

namespace Mka.Functions;

public class ProcessDocument(
    EmbeddingClient embeddings,
    KnowledgeIndex index,
    ILogger<ProcessDocument> logger)
{
    private const int BatchSize = 16;

    [Function(nameof(ProcessDocument))]
    public async Task Run(
        [BlobTrigger("documents/{name}", Connection = "DocsStorage")] byte[] content,
        string name,
        CancellationToken ct)
    {
        var documentId = name.Split('/')[0];
        var fileName = Path.GetFileName(name);

        var pages = TextExtractor.Extract(fileName, content);
        var chunks = TextChunker.Split(pages);
        if (chunks.Count == 0)
        {
            logger.LogWarning("No text found in {Name}", name);
            return;
        }

        await index.EnsureIndexAsync(ct);

        var totalTokens = 0;
        foreach (var batch in chunks.Chunk(BatchSize))
        {
            var result = await embeddings.GenerateEmbeddingsAsync(
                batch.Select(c => c.Content), cancellationToken: ct);
            totalTokens += result.Value.Usage.InputTokenCount;

            var docs = batch.Zip(result.Value, (chunk, embedding) => new DocumentChunk
            {
                Id = $"{documentId}_{chunk.Index}",
                DocumentId = documentId,
                FileName = fileName,
                ChunkIndex = chunk.Index,
                PageNumber = chunk.PageNumber,
                Content = chunk.Content,
                ContentVector = embedding.ToFloats()
            });

            await index.UploadAsync(docs, ct);
        }

        logger.LogInformation(
            "Indexed {Name}: {Chunks} chunks, {Tokens} embedding tokens",
            name, chunks.Count, totalTokens);
    }
}