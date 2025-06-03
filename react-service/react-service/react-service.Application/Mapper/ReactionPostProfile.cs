using AutoMapper;
using react_service.Domain.Entites;
using react_service.Application.DTO;
using react_service.Application.DTO.ReactionPost.Request;
using react_service.Application.DTO.ReactionPost.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Mapper
{
    public class ReactionPostProfile : Profile 
    {
        public ReactionPostProfile()
        {
            CreateMap<CreateReactionRequest ,ReactionPost>()
            .ForMember(dest => dest.PostCreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ReactionPost,ReactionResponseDTO>();
            CreateMap<List<ReactionPost>, PagedReactsResponse>()
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src));




        }
    }
}
