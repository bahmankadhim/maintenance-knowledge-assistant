using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mka.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DocumentController(BlobContainerClient container) : ControllerBase
{
    private const long MaxBytes = 10 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".pdf", ".txt", ".md", ".docx"];

    [HttpPost]
    [Authorize(Roles = "Supervisor")]
    [RequestSizeLimit(MaxBytes)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) return BadRequest("File is empty.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest($"Allowed types: {string.Join(", ", AllowedExtensions)}");

        var safeName = Path.GetFileName(file.FileName);// removes any folder parts like "../"
        var blobName = $"{Guid.NewGuid()}/{safeName}";
        var blob = container.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blob.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            },
            ct
        );

        return Created(blob.Uri, new { name = blobName, originalName = file.FileName, size = file.Length });
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var items = new List<DocumentResponse>();
        await foreach (var b in container.GetBlobsAsync(cancellationToken: ct))
            items.Add(new DocumentResponse(b.Name, Path.GetFileName(b.Name), b.Properties.ContentLength, b.Properties.CreatedOn));

        return Ok(items);
    }

    public record DocumentResponse(string Id, string FileName, long? size, DateTimeOffset? UploadedOn);
}