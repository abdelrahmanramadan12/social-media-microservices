using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.DTOs.Requests;
using Service.DTOs.Responses;
using Service.Interfaces.ProfileServices;

namespace Web.Controllers.Public
{
    [Route("api/public/profile")]
    [ApiController]
    public class PublicProfileController : BaseController
    {
        private readonly IProfileService _profileService;

        public PublicProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("id/{userId}")]
        public async Task<IActionResult> GetByUserIdAsync(string userId)
        {
            var response = await _profileService.GetByUserIdAsync(userId);
            return HandleServiceResponse(response);
        }

        [HttpGet("username/{userName}")]
        public async Task<IActionResult> GetByUserNameAsync(string userName)
        {
            var response = await _profileService.GetByUserNameAsync(userName);
            return HandleServiceResponse(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByUserName([FromQuery] string query,[FromQuery]int pageNumber=1)
        {
            var response = await _profileService.SearchByUserName(query,pageNumber);
            return HandlePaginatedResponse(response);
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

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromHeader(Name = "userId")] string userId, [FromForm] ProfileRequestDto profile)
        {
            var response = await _profileService.AddAsync(userId, profile);
            return HandleServiceResponse(response);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAsync( [FromHeader(Name = "userId")] string userId, [FromForm] ProfileRequestDto profile)
        {
            var response = await _profileService.UpdateAsync(userId, profile);
            return HandleServiceResponse(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromHeader(Name = "userId")] string userId)
        {
            var response = await _profileService.DeleteAsync(userId);
            return HandleServiceResponse(response);
        }
    }
}
