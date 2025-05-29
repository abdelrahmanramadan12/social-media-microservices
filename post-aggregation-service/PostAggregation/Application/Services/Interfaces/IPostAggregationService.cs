using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IPostAggregationService
    {
        public Task<ServiceResponseDTO<PostAggregationDTO>> GetProfilePosts(string userId, string targetUser, string nextPostHashId);
    }
}
