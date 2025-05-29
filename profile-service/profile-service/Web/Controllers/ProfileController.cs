using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("id/{userId}")]
        public async Task<IActionResult> GetByUserIdAsync(string userId)
        {   
            var profile = await _profileService.GetByUserIdAsync(userId);
            return Ok(profile);
        }

        [HttpGet("username/{userName}")]
        public async Task<IActionResult> GetByUserNameAsync(string userName)
        {
            var profile = await _profileService.GetByUserNameAsync(userName);
            return Ok(profile);
        }

        [HttpGet("min/id/{userId}")]
        public async Task<IActionResult> GetByUserIdMinAsync(string userId)
        {
            var profile = await _profileService.GetByUserIdMinAsync(userId);
            return Ok(profile);
        }

        [HttpGet("min/username/{userName}")]
        public async Task<IActionResult> GetByUserNameMinAsync(string userName)
        {
            var profile = await _profileService.GetByUserNameMinAsync(userName);
            return Ok(profile);
        }

        [HttpPost("batch-min")]
        public async Task<IActionResult> GetUsersByIdsAsync([FromBody] List<string> userIds)
        {
            var profiles = await _profileService.GetUsersByIdsAsync(userIds);
            return Ok(profiles);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] Profile profile)
        {
            var createdProfile = await _profileService.AddAsync(profile);
            return Ok(createdProfile);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAsync(string userId, [FromBody] Profile profile)
        {
            var updatedProfile = await _profileService.UpdateAsync(userId, profile);
            return Ok(updatedProfile);
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
