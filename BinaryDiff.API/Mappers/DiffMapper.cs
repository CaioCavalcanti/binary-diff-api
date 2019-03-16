using AutoMapper;
using BinaryDiff.API.ViewModels;
using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Models;
using System;

namespace BinaryDiff.API.Mappers
{
    /// <summary>
    /// Defines the ViewModels/Models mapping rules for Diff domain.
    /// Map configuration profile for AutoMapper (auto discovered on Startup).
    /// </summary>
    public class DiffMapper : Profile
    {
        /// <summary>
        /// Declares the available mappings
        /// </summary>
        public DiffMapper()
        {
            CreateMap<Diff, DiffViewModel>();

            CreateMap<DiffResult, DiffResultViewModel>()
                .ForMember(dest => dest.Result, opts => opts.MapFrom(src => Enum.GetName(typeof(ResultType), src.Result)));
        }
    }
}
