#pragma warning disable CA1506

using PassengerService.Application.BackgroundServices;
using PassengerService.Application.Extensions;
using PassengerService.Infrastructure.Persistence.Extensions;
using PassengerService.Infrastructure.Persistence.Options;
using PassengerService.Presentation.Grpc.Controllers;
using PassengerService.Presentation.Grpc.Interceptors;
using PassengerService.Presentation.Kafka.Extensions;
using PassengerService.Presentation.Kafka.Models;
using PassengerService.Presentation.TicketGrpcClient.Extensions;
using PassengerService.Presentation.TicketGrpcClient.Options;
using ServiceManagement.Presentation.Kafka.Configuration;
using Tasks.Kafka.Contracts;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

builder.Services.Configure<PostgresOptions>(builder.Configuration.GetSection("Persistence:Postgres"));
builder.Services.Configure<TicketServiceClientOptions>(builder.Configuration.GetSection("TicketClient"));

builder.Services.Configure<ConsumerOptions>("PassengerNotifications", builder.Configuration.GetSection("Kafka:Consumers:PassengerNotifications"));
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services
    .AddMigration()
    .AddInfrastructureDataAccess()
    .AddApplication();

builder.Services.AddHostedService<MigrationService>();

builder.Services.AddGrpc(o => o.Interceptors.Add<ExceptionInterceptor>());

builder.Services.AddHandlers();
builder.Services.AddKafkaConsumer<PassengerNotificationsKey, PassengerNotificationsValue>("PassengerNotifications");

builder.Services.AddTicketGrpcClient();
WebApplication app = builder.Build();

app.MapGrpcService<PassengerController>();
app.Run();