using AutoMapper;
using Otis.Sim.Elevator.Models;

namespace Otis.Sim.MappingProfiles
{
    public class OtisMappingProfile : Profile
    {
        public OtisMappingProfile()
        {
            CreateMap<ElevatorModel, ElevatorDataRow>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CurrentStatus));

            CreateMap<ElevatorRequest, ElevatorAcceptedRequest>();
        }
    }
}
