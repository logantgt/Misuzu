using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Misuzu.Controllers;

[ApiController]
[Route("images")]
public class ImagesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;

    public ImagesController()
    {
        _contentTypeProvider = new FileExtensionContentTypeProvider();
    }

    [HttpGet("{uuid}")]
    public IActionResult GetImage(string uuid)
    {
        // Search for any file in Images folder that matches the filename (ignore case)
        var imagesFolder = Path.Combine(Environment.CurrentDirectory, "store/images");

        if (!Directory.Exists(imagesFolder))
            Directory.CreateDirectory("store/images");

        var file = Directory.EnumerateFiles(imagesFolder)
                            .FirstOrDefault(f =>
                                string.Equals(Path.GetFileNameWithoutExtension(f), uuid, StringComparison.OrdinalIgnoreCase));

        if (file == null)
            return NotFound();

        // Auto-detect content type
        if (!_contentTypeProvider.TryGetContentType(file, out var contentType))
        {
           
        }

        var bytes = System.IO.File.ReadAllBytes(file);
        return File(bytes, contentType);
    }
}
