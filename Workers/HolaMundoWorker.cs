namespace HolaMundoScheduler.Workers;

public sealed class HolaMundoWorker : BackgroundService
{
    private readonly ILogger<HolaMundoWorker> _logger;
    private const int IntervalSeconds = 20;

    public HolaMundoWorker(ILogger<HolaMundoWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HolaMundoWorker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var hora = DateTime.Now.ToString("HH:mm:ss");
            _logger.LogInformation("hola, ya son las {Hora}", hora);
            Console.WriteLine($"hola, ya son las {hora}");

            await Task.Delay(TimeSpan.FromSeconds(IntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("HolaMundoWorker detenido.");
    }
}
