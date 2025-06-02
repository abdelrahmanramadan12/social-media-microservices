using Microsoft.AspNetCore.Mvc;
using Service.Interfaces.ProfileServices;

namespace Web.Controllers.Internal
{
    [Route("api/internal/profile")]
    [ApiController]
    public class InternalProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public InternalProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("min/id/{userId}")]
        public async Task<IActionResult> GetByUserIdMinAsync(string userId)
        {
            var profile = await _profileService.GetByUserIdMinAsync(userId);
            if (profile == null || profile.Success == false)
            {
                return NotFound(profile?.Errors ?? new List<string> { "Profile not found." });
            }
            return Ok(profile);
        }

        [HttpGet("min/username/{userName}")]
        public async Task<IActionResult> GetByUserNameMinAsync(string userName)
        {
            var profile = await _profileService.GetByUserNameMinAsync(userName);
            if (profile == null || profile.Success == false)
            {
                return NotFound(profile?.Errors ?? new List<string> { "Profile not found." });
            }
            return Ok(profile);
        }

        [HttpPost("batch-min")]
        public async Task<IActionResult> GetUsersByIdsAsync([FromBody] List<string> userIds)
        {
            var profiles = await _profileService.GetUsersByIdsAsync(userIds);
            if (profiles == null || profiles.Data == null || !profiles.Data.Any())
            {
                return NotFound("No profiles found for the provided user IDs.");
            }
            if (profiles.Success == false)
            {
                return BadRequest(profiles.Errors);
            }
            return Ok(profiles);
        }
    }
}
