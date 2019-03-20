using AutoMapper;
using BinaryDiff.Result.Domain.Enums;
using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.WebApi.Events.IntegrationEvents;
using BinaryDiff.Result.WebApi.ViewModels;
using System;
using System.Linq;

namespace BinaryDiff.Result.WebApi.Mappers
{
    public class DiffResultMapper : Profile
    {
        public DiffResultMapper()
        {
            CreateMap<DiffResult, DiffResultViewModel>()
                .ForMember(
                    dest => dest.Result,
                    opts => opts.MapFrom(src => Enum.GetName(typeof(ResultType), src.Result)))
                .ForMember(
                    dest => dest.Differences,
                    opts => opts.MapFrom(
                        src => src.Differences
                            .Select(i => new InputDifferenceViewModel(i.Offset, i.Length))
                    )
                );

            CreateMap<NewResultIntegrationEvent, DiffResult>()
                .ForMember(
                    dest => dest.Timestamp,
                    opts => opts.MapFrom(_ => DateTime.UtcNow))
                .ForMember(
                    dest => dest.OpposingInputId,
                    opts => opts.MapFrom(src => src.OpposingId)
                )
                .ForMember(
                    dest => dest.TriggerInputId,
                    opts => opts.MapFrom(src => src.InputId)
                )
                .ForMember(
                    dest => dest.TriggerInputPosition,
                    opts => opts.MapFrom(src => src.InputPosition)
                )
                .ForMember(
                    dest => dest.Differences,
                    opts => opts.MapFrom(src =>
                        src.Differences
                            .Select(s => new InputDifference(s.Key, s.Value))
                    )
                );

        }
    }
}
