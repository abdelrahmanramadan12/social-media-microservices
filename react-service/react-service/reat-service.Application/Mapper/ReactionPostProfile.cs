using AutoMapper;
using react_service.Domain.Entites;
using reat_service.Application.DTO;
using reat_service.Application.DTO.ReactionPost.Request;
using reat_service.Application.DTO.ReactionPost.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reat_service.Application.Mapper
{
    public class ReactionPostProfile : Profile 
    {
        public ReactionPostProfile()
        {
            CreateMap<CreateReactionRequest ,ReactionPost>()
            .ForMember(dest => dest.PostCreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ReactionPost,ReactDto>();
            CreateMap<List<ReactionPost>, PagedReactsResponse>()
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src));




        }
    }
}
