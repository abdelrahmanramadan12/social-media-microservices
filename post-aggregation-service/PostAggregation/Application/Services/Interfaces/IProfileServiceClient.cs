using Application.DTOs;
using Application.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IProfileServiceClient
    {
        Task<ResponseWrapper<SimpleUserProfile>> GetByUserIdMinAsync(string userId);
        Task<ResponseWrapper<List<SimpleUserProfile>>> GetUsersByIdsAsync(GetUsersProfileByIdsRequest request);
    }
}
