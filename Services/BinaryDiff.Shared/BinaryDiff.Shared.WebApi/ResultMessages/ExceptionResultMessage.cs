using System;

namespace BinaryDiff.Shared.WebApi.ResultMessages
{
    public class ExceptionResultMessage : BaseResultMessage
    {
        public ExceptionResultMessage(Guid errorId)
            : base("An error occurred and your message couldn't be processed. If the error persists, contact the admin informing the error id provided.")
        {
            ErrorId = errorId;
        }

        public Guid ErrorId { get; set; }
    }
}
