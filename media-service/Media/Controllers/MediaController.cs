using Application.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Media.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController(IUploadMediaService uploadMediaService, IGetMediaService getMediaService) : ControllerBase
    {
        private readonly IGetMediaService _getMediaService = getMediaService;
        private readonly IUploadMediaService _uploadMediaService = uploadMediaService;

        // GET: MediaController
        [HttpGet]
        public async Task<ActionResult> GetImage(string ImageUrl)
        {
            return Ok(await _getMediaService.GetMediaAsync(ImageUrl));
        }

        [HttpGet]
        public async Task<ActionResult> GetImageInfo(Guid ImageId)
        {
            return Ok(await _getMediaService.GetMediaURL(ImageId));
        }

        [HttpGet]
        public async Task<ActionResult> UploadImage(IFormFile FormFile, string description)
        {
            return Ok(await _uploadMediaService.UploadMediaAsync(FormFile, description));
        }

        // GET: MediaController/Edit/5
        public ActionResult Edit(Guid id)
        {
            return Ok();
        }

        // GET: MediaController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            return Ok(await _uploadMediaService.DeleteMediaAsync(id));
        }
    }
}
