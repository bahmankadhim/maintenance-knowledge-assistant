using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace Mka.Rag;

public class KnowledgeIndex(SearchIndexClient indexClient, string indexName)
{
    public const string VectorProfile = "default-profile";
    private const string Algorithm = "default-hnsw";

    private readonly SearchClient _search = indexClient.GetSearchClient(indexName);

    // Safe to call every time: creates the index once, then only updates it if the schema changed.
    public async Task EnsureIndexAsync(CancellationToken ct = default)
    {
        var index = new SearchIndex(indexName, new FieldBuilder().Build(typeof(DocumentChunk)))
        {
            VectorSearch = new VectorSearch
            {
                Algorithms = { new HnswAlgorithmConfiguration(Algorithm) },
                Profiles = { new VectorSearchProfile(VectorProfile, Algorithm) }
            }
        };
        await indexClient.CreateOrUpdateIndexAsync(index, cancellationToken: ct);
    }

    // MergeOrUpload: same Id overwrites, so a retried run never creates duplicates (idempotent).
    public async Task UploadAsync(IEnumerable<DocumentChunk> chunks, CancellationToken ct = default)
    {
        var options = new IndexDocumentsOptions { ThrowOnAnyError = true }; // default is false: failures would be silent
        await _search.MergeOrUploadDocumentsAsync(chunks, options, ct);
    }
}