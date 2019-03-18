namespace BinaryDiff.Shared.Infrastructure.RabbitMQ
{
    /// <summary>
    /// RabbitMQ exchange types, mainly to avoid typo issues 
    /// when creating a new exchange using IModel.ExchangeDeclare()
    /// </summary>
    public enum RabbitMQExchangeType
    {
        Direct = 0,
        Topic,
        Fanout,
        Headers
    }
}
