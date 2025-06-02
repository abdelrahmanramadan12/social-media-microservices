using Microsoft.AspNetCore.Mvc;
using Service.Interfaces.ProfileServices;

namespace Web.Controllers.Internal
{
    [Route("api/internal/profile")]
    [ApiController]
    public class InternalProfileController : BaseController
    {
        private readonly IProfileService _profileService;

        public InternalProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("min/id/{userId}")]
        public async Task<IActionResult> GetByUserIdMinAsync(string userId)
        {
            var response = await _profileService.GetByUserIdMinAsync(userId);
            return HandleServiceResponse(response);
        }

        [HttpGet("min/username/{userName}")]
        public async Task<IActionResult> GetByUserNameMinAsync(string userName)
        {
            var response = await _profileService.GetByUserNameMinAsync(userName);
            return HandleServiceResponse(response);
        }

        [HttpPost("batch-min")]
        public async Task<IActionResult> GetUsersByIdsAsync([FromBody] List<string> userIds)
        {
            var response = await _profileService.GetUsersByIdsAsync(userIds);
            return HandleServiceResponse(response);
        }
    }
}
