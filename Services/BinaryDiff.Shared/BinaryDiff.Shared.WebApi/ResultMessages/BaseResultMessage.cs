namespace BinaryDiff.Shared.WebApi.ResultMessages
{
    public class BaseResultMessage
    {
        public BaseResultMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
