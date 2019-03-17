namespace BinaryDiff.Result.WebApi.ViewModels
{
    public class InputDifferenceViewModel
    {
        public InputDifferenceViewModel()
        {
        }

        public InputDifferenceViewModel(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }

        public int Offset { get; set; }

        public int Length { get; set; }
    }
}
