namespace BinaryDiff.API.Helpers.Messages
{
    /// <summary>
    /// Base message to be returned on any other result than 2XX.
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        /// Classes that inherits it should set their own default message
        /// </summary>
        /// <param name="message"></param>
        public BaseMessage(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Message to return
        /// </summary>
        public string Message { get; }
    }
}
