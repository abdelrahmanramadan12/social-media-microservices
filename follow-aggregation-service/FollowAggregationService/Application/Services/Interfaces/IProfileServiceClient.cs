using Application.DTOs;
using Application.DTOs.Profile;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IProfileServiceClient
    {
        Task<ResponseWrapper<List<SimpleUserProfile>>> GetUsersByIdsAsync(GetUsersProfileByIdsRequest request);
    }
}
