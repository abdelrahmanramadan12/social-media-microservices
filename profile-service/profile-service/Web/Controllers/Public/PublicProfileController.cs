using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces.ProfileServices;

namespace Web.Controllers.Public
{
    [Route("api/public/profile")]
    [ApiController]
    public class PublicProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public PublicProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("id/{userId}")]
        public async Task<IActionResult> GetByUserIdAsync(string userId)
        {
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null || profile.Success == false)
            {
                return NotFound(profile?.Errors ?? new List<string> { "Profile not found." });
            }
            return Ok(profile);
        }

        [HttpGet("username/{userName}")]
        public async Task<IActionResult> GetByUserNameAsync(string userName)
        {
            var profile = await _profileService.GetByUserNameAsync(userName);
            if (profile == null || profile.Success == false)
            {
                return NotFound(profile?.Errors ?? new List<string> { "Profile not found." });
            }
            return Ok(profile);
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

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromHeader(Name = "userId")] string userId, [FromForm] ProfileRequestDto profile)
        {
            try
            {
                ProfileResponseDto? createdProfile = await _profileService.AddAsync(userId, profile);
                if (createdProfile == null || !createdProfile.Success)
                {
                    return BadRequest(createdProfile?.Errors ?? new List<string> { "Failed to create profile." });
                }

                return Created($"api/public/profile/id/{createdProfile.Data.UserId}", createdProfile.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", detail = ex.Message });
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAsync(string userId, [FromBody] ProfileRequestDto profile)
        {
            try
            {
                var updatedProfile = await _profileService.UpdateAsync(userId, profile);
                if (updatedProfile == null || !updatedProfile.Success)
                {
                    return BadRequest(updatedProfile?.Errors ?? new List<string> { "Failed to update profile." });
                }
                return Ok(updatedProfile.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", detail = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAsync(string userId)
        {
            var success = await _profileService.DeleteAsync(userId);
            if (success)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
