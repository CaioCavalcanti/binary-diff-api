namespace BinaryDiff.Domain.Enum
{
    /// <summary>
    /// Possible result types when comparing left/right data
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// Data provided on both left and right are equal
        /// </summary>
        Equal = 0,
        /// <summary>
        /// Left data is larger than right
        /// </summary>
        LeftIsLarger,
        /// <summary>
        /// Right data is larger than left
        /// </summary>
        RightIsLarger,
        /// <summary>
        /// Left and right have the same size, but there are differences between them
        /// </summary>
        Different
    }
}
