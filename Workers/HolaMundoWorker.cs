using HolaMundoScheduler.Messaging;

namespace HolaMundoScheduler.Workers;

public sealed class HolaMundoWorker : BackgroundService
{
    private readonly ILogger<HolaMundoWorker> _logger;
    private readonly RabbitMQPublisher _publisher;
    private const int IntervalSeconds = 20;

    public HolaMundoWorker(ILogger<HolaMundoWorker> logger, RabbitMQPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HolaMundoWorker iniciado.");

        await _publisher.InitializeAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var hora = DateTime.Now.ToString("HH:mm:ss");
            var mensaje = $"hola, ya son las {hora}";

            await _publisher.PublishAsync(mensaje, stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(IntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("HolaMundoWorker detenido.");
    }
}
