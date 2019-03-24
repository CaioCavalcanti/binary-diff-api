using System;

namespace BinaryDiff.Input.WebApi.ViewModels
{
    public class InputViewModel
    {
        public string Id { get; set; }

        public string Position { get; set; }

        public Guid DiffId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
