using AutoMapper;
using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Input.WebApi.ViewModels;

namespace BinaryDiff.Input.WebApi.Mappers
{
    public class DiffMapper : Profile
    {
        public DiffMapper()
        {
            CreateMap<Diff, DiffViewModel>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.UUID));
        }
    }
}
