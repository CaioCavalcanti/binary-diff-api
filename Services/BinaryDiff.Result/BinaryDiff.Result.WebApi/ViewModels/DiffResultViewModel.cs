﻿using System;

namespace BinaryDiff.Result.WebApi.ViewModels
{
    /// <summary>
    /// View model for a diff result
    /// </summary>
    public class DiffResultViewModel
    {
        /// <summary>
        /// User friendly result description
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// UTC date and time when the result was created
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// List of differences
        /// </summary>
        public InputDifferenceViewModel[] Differences { get; set; }
    }
}
