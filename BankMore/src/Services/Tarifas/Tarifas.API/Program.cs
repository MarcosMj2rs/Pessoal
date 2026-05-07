using Tarifas.Application.Interfaces;
using Tarifas.Infrastructure.Messaging;
using Tarifas.Infrastructure.Persistence;
using Tarifas.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

var bootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    ?? throw new InvalidOperationException("Kafka BootstrapServers not found.");

var topicoConsumer = builder.Configuration["Kafka:TopicoConsumer"]
    ?? throw new InvalidOperationException("Kafka TopicoConsumer not found.");

var valorTarifa = builder.Configuration.GetValue<decimal>("Tarifas:ValorTransferencia");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));
builder.Services.AddScoped<ITarifaRepository, TarifaRepository>();
builder.Services.AddSingleton<IKafkaProducer>(sp => new KafkaProducer(bootstrapServers));

builder.Services.AddHostedService(sp => new KafkaConsumer(
    bootstrapServers,
    topicoConsumer,
    valorTarifa,
    sp.GetRequiredService<IServiceScopeFactory>(),
    sp.GetRequiredService<ILogger<KafkaConsumer>>()
));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();