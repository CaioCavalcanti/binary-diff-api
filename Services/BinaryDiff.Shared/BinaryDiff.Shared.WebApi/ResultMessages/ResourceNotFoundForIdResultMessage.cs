using System;

namespace BinaryDiff.Shared.WebApi.ResultMessages
{
    public class ResourceNotFoundForIdResultMessage<TResource> : BaseResultMessage
    {
        public ResourceNotFoundForIdResultMessage(Guid id)
            : base($"{typeof(TResource).Name} with ID '{id}' not found")
        {
        }
    }
}
