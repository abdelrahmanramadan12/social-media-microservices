using Application.DTOs;
using Application.DTOs.Reaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IReactionServiceClient
    {
        public Task<ResponseWrapper<List<string>>> FilterCommentsReactedByUserAsync(FilterCommentsReactedByUserRequest request);

        
    }
}
