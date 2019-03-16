namespace BinaryDiff.API.ViewModels
{
    /// <summary>
    /// Represents the base 64 input sent to left and right to be diff-ed
    /// </summary>
    public class DiffInputViewModel
    {
        /// <summary>
        /// Base 64 data
        /// </summary>
        public string Data { get; set; }
    }
}
