using BinaryDiff.Result.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BinaryDiff.Result.WebApi.ViewModels
{
    public class NewDiffResultViewModel
    {
        /// <summary>
        /// Diff entity UUID for reference
        /// </summary>
        public Guid DiffId { get; set; }

        /// <summary>
        /// Result type
        /// </summary>
        [Range(0, 3, ErrorMessage = "Result informed is out of range")]
        public ResultType Result { get; set; }

        /// <summary>
        /// Dictionary of differences, where key is offset and value is length
        /// </summary>
        public IDictionary<int, int> Differences { get; set; }
    }
}
