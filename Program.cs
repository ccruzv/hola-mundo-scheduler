using HolaMundoScheduler.Messaging;
using HolaMundoScheduler.Workers;

var builder = Host.CreateApplicationBuilder(args);

// Health checks
builder.Services.AddHealthChecks();

// RabbitMQ
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection(RabbitMQSettings.SectionName));
builder.Services.AddSingleton<RabbitMQPublisher>();

// Register workers
builder.Services.AddHostedService<HolaMundoWorker>();

// Graceful shutdown timeout
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

var host = builder.Build();
await host.RunAsync();
