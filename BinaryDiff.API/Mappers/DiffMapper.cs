using AutoMapper;
using BinaryDiff.API.ViewModels;
using BinaryDiff.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinaryDiff.API.Mappers
{
    public class DiffMapper : Profile
    {
        public DiffMapper()
        {
            CreateMap<Diff, DiffViewModel>();
        }
    }
}
