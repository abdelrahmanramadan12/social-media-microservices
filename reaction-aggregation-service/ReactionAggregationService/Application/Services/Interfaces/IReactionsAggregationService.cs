using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Profile;
using Application.DTOs.Reactions;

namespace Application.Services.Interfaces
{
    public interface IReactionsAggregationService
    {
        Task<PaginationResponseWrapper<List<SimpleUserProfile>>> GetReactionsOfPostAsync(GetReactsOfPostRequest request);
    }
}
