using Azure.Search.Documents.Indexes;

namespace Mka.Rag;

public class DocumentChunk
{
    [SimpleField(IsKey = true)] // unique id: "{documentId}_{chunkIndex}"
    public string Id {get;set;} = "";

    [SimpleField(IsFilterable = true)] // lets us delete or filter one later
    public string DocumentId {get;set;} = "";

    [SimpleField(IsFilterable = true)]
    public string FileName {get;set;} = "";

    [SimpleField]
    public int ChunkIndex {get;set;}

    [SimpleField] // null for .txt/.md/.docx page
    public int? PageNumber {get;set;}

    [SearchableField] // full-text (keyword) search on the chunk text
    public string Content {get; set;} ="";

    [VectorSearchField(VectorSearchDimensions = 1536, VectorSearchProfileName = KnowledgeIndex.VectorProfile)] // 1536 = size of text-embedding-3-small
    public ReadOnlyMemory<float> ContentVector {get;set;} 
}