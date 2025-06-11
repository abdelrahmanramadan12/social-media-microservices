using Application.DTOs;
using Application.Services;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Media.Controllers
{
    [Route("api/internal/[controller]")]
    [ApiController]
    public class MediaController(IUploadMediaService uploadMediaService) : ControllerBase
    {
        private readonly IUploadMediaService _uploadMediaService = uploadMediaService;

        [HttpPost]
        public async Task<IActionResult> UploadMedia([FromForm] ReceivedMediaDto mediaDto)
        {
            if (mediaDto.Files == null || !mediaDto.Files.Any())
                return BadRequest("No files were uploaded.");

            if (!AreFilesValid(mediaDto.Files, mediaDto.MediaType, out var validationError))
                return BadRequest(validationError);

            var uploadResult = await ProcessFilesAsync(mediaDto);
            return Ok(new { Success = true, Uploaded = uploadResult.Count, Urls = uploadResult });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMedia([FromBody] IEnumerable<string> Urls)
            => Ok(await _uploadMediaService.DeleteMediaAsync(Urls));

        private static bool AreFilesValid(IEnumerable<IFormFile> files, MediaType mediaType, out string? error)
        {
            error = null;

            var allowedContentTypes = AllowedContentTypes;
            if (!allowedContentTypes.TryGetValue(mediaType, out var validTypes))
            {
                error = "Unsupported media type.";
                return false;
            }

            foreach (var file in files)
            {
                if (!validTypes.Contains(file.ContentType))
                {
                    error = $"File '{file.FileName}' is not a valid {mediaType} type.";
                    return false;
                }
            }

            return true;
        }

        private static Dictionary<MediaType, string[]> AllowedContentTypes => new()
        {
            [MediaType.IMAGE] = ["image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp"],
            [MediaType.VIDEO] = ["video/mp4", "video/quicktime", "video/x-msvideo", "video/x-matroska", "video/webm"],
            [MediaType.AUDIO] = ["audio/mpeg", "audio/wav", "audio/ogg", "audio/mp3", "audio/x-wav"],
            [MediaType.DOCUMENT] = [
                                "application/pdf", "application/msword",
                                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                                "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "text/plain"]
        };

        private async Task<List<string>> ProcessFilesAsync(ReceivedMediaDto mediaDto)
        {
            var results = new List<string>();
            var tempFiles = new List<string>();

            try
            {
                foreach (var file in mediaDto.Files)
                {
                    var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(file.FileName));
                    tempFiles.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var result = await _uploadMediaService.UploadAsync(filePath, mediaDto.MediaType, mediaDto.UsageCategory);
                    results.Add(result);
                }
            }
            finally
            {
                foreach (var tempFile in tempFiles)
                {
                    if (System.IO.File.Exists(tempFile))
                        System.IO.File.Delete(tempFile);
                }
            }

            return results;
        }

    }
}
