using AutoMapper;
using BinaryDiff.Input.Domain.Enums;
using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Input.WebApi.ViewModels;
using System;

namespace BinaryDiff.Input.WebApi.Mappers
{
    public class InputMapper : Profile
    {
        public InputMapper()
        {
            CreateMap<InputData, InputViewModel>()
                .ForMember(
                    dest => dest.Id,
                    opts => opts.MapFrom(src => src.Id)
                )
                .ForMember(
                    dest => dest.Position,
                    opts => opts.MapFrom(
                        src => Enum.GetName(typeof(InputPosition), src.Position)
                    )
                );
        }
    }
}
