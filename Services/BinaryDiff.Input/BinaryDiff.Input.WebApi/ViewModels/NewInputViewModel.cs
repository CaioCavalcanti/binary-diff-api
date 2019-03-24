namespace BinaryDiff.Input.WebApi.ViewModels
{
    public class NewInputViewModel
    {
        public NewInputViewModel() { }

        public NewInputViewModel(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
    }
}
