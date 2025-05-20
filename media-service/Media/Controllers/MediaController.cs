using Application.Services;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Media.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController(IUploadMediaService uploadMediaService) : ControllerBase
    {
        private readonly IUploadMediaService _uploadMediaService = uploadMediaService;

        [HttpPost("upload")]
        public async Task<IActionResult> UploadMedia([FromForm] IFormFile file, [FromForm] MediaType mediaType = MediaType.Image)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            // Save the file temporarily
            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                var result = await _uploadMediaService.UploadAsync(filePath, mediaType);
                return Ok(new { Url = result });
            }
            finally
            {
                // Clean up the temp file
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit(string ImageUrl, string filePath, string? folder = null)
                                            => Ok(await _uploadMediaService.EditMediaAsync(ImageUrl, filePath, folder));

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id) => Ok(await _uploadMediaService.DeleteMediaAsync(id));

    }
}
