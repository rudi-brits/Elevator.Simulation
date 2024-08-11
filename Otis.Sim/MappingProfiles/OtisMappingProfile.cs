using AutoMapper;
using Otis.Sim.Elevator.Models;

namespace Otis.Sim.MappingProfiles;

/// <summary>
/// Class OtisMappingProfile extends the <see cref="Profile" /> class.
/// </summary>
public class OtisMappingProfile : Profile
{
    /// <summary>
    /// The OtisMappingProfile constructor
    /// </summary>
    public OtisMappingProfile()
    {
        CreateMap<ElevatorModel, ElevatorDataRow>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CurrentStatus));

        CreateMap<ElevatorRequest, ElevatorAcceptedRequest>()
            .ForMember(dest => dest.RequestDirection, opt => opt.MapFrom(src => src.Direction))
            .ForMember(dest => dest.ElevatorName, opt => opt.Ignore())
            .ForMember(dest => dest.OriginFloorServiced, opt => opt.Ignore())
            .ForMember(dest => dest.DestinationFloorServiced, opt => opt.Ignore());
    }
}
