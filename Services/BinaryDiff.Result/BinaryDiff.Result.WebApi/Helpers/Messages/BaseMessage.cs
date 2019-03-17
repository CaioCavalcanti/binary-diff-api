namespace BinaryDiff.Result.WebApi.Helpers.Messages
{
    public class BaseMessage
    {
        public BaseMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
