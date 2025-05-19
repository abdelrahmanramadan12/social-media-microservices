using Application.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        //[ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetImage(string ImageUrl) => Ok(await _getMediaService.GetMediaAsync(ImageUrl));

        [HttpGet]
        //[ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]

        public async Task<ActionResult> GetImageInfo(Guid ImageId) => Ok(await _getMediaService.GetMediaURL(ImageId));

        [HttpPost]
        //[ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]

        public async Task<ActionResult> UploadImage(IFormFile FormFile, string description)
                                                => Ok(await _uploadMediaService.UploadMediaAsync(FormFile, description));

        [HttpPut]
        //[ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]

        public async Task<ActionResult> Edit(string ImageUrl) => Ok(await _getMediaService.EditMediaAsync(ImageUrl));

        [HttpDelete]
        // GET: MediaController/Delete/5
        //[ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]

        public async Task<ActionResult> Delete(Guid id) => Ok(await _uploadMediaService.DeleteMediaAsync(id));

    }
}
