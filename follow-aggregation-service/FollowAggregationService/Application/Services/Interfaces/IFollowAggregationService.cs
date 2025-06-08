using Application.DTOs;
using Application.DTOs.Aggregation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IFollowAggregationService
    {
        Task<PaginationResponseWrapper<List<ProfileAggregation>>> GetFollowers(FollowListRequest request);
        Task<PaginationResponseWrapper<List<ProfileAggregation>>> GetFollowing(FollowListRequest request);
    }
}
