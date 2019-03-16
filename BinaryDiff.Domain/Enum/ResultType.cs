using System.ComponentModel;

namespace BinaryDiff.Domain.Enum
{
    public enum ResultType
    {
        [DisplayName("Data provided on both left and right are equal")]
        AreEqual = 0,

        [DisplayName("Left data is larger than right")]
        LeftIsLarger,

        [DisplayName("Right data is larger than left")]
        RightIsLarger,

        [DisplayName("Left and right have the same size, but there are differences between them.")]
        DifferentContent
    }
}
