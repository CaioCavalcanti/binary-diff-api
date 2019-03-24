namespace BinaryDiff.Shared.Infrastructure.Configuration
{
    public class RabbitMQConfiguration
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public int? RetryCount { get; set; }

        public bool IsValid()
        {
            return
                !string.IsNullOrEmpty(Host) &&
                Port != 0 &&
                !string.IsNullOrEmpty(User) &&
                !string.IsNullOrEmpty(Password);
        }
    }
}
