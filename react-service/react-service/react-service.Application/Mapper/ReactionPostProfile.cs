using AutoMapper;
using react_service.Domain.Entites;
using react_service.Application.DTO;
using react_service.Application.DTO.Reaction.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using react_service.Application.DTO.Reaction.Request.Post;

namespace react_service.Application.Mapper
{
    public class ReactionPostProfile : Profile 
    {
        public ReactionPostProfile()
        {
            CreateMap<CreatePostReactionRequest ,PostReaction>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<PostReaction,ReactionResponseDTO>();
            CreateMap<List<PostReaction>, PagedReactsResponse>()
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src));
        }
    }
}
