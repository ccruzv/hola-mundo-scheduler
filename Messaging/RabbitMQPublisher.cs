using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace HolaMundoScheduler.Messaging;

public sealed class RabbitMQPublisher : IAsyncDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMQPublisher(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            VirtualHost = _settings.VirtualHost,
            UserName = _settings.Username,
            Password = _settings.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogInformation("RabbitMQ conectado. Cola: {Queue}", _settings.QueueName);
    }

    public async Task PublishAsync(string message, CancellationToken cancellationToken = default)
    {
        if (_channel is null)
            throw new InvalidOperationException("El publisher no ha sido inicializado. Llama InitializeAsync primero.");

        var body = Encoding.UTF8.GetBytes(message);

        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "text/plain",
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _settings.QueueName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Mensaje encolado en '{Queue}': {Message}", _settings.QueueName, message);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
