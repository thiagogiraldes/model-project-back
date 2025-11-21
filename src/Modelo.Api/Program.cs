using MediatR;
using Modelo.Application.Commands.Orders;
using Modelo.Infrastructure.IoC;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(typeof(CreateOrderCommand).Assembly);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Modelo.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint = new Uri("http://localhost:4317");
            });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
