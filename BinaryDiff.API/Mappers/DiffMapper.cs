using AutoMapper;
using BinaryDiff.API.ViewModels;
using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Models;
using System;

namespace BinaryDiff.API.Mappers
{
    public class DiffMapper : Profile
    {
        public DiffMapper()
        {
            CreateMap<Diff, DiffViewModel>();

            CreateMap<DiffResult, DiffResultViewModel>()
                .ForMember(dest => dest.Result, opts => opts.MapFrom(src => Enum.GetName(typeof(ResultType), src.Result)));
        }
    }
}
