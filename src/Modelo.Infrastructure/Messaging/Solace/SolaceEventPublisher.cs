using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Modelo.Application.Interfaces;
using SolaceSystems.Solclient.Messaging;

namespace Modelo.Infrastructure.Messaging.Solace;

public sealed class SolaceEventPublisher : IEventPublisher, IDisposable
{
    private readonly ISession _session;
    private readonly IContext _context;
    private readonly ILogger<SolaceEventPublisher> _logger;

    public SolaceEventPublisher(IConfiguration configuration, ILogger<SolaceEventPublisher> logger)
    {
        _logger = logger;

        var solaceSection = configuration.GetSection("Solace");
        var host = solaceSection["Host"];
        var vpnName = solaceSection["VpnName"];
        var username = solaceSection["Username"];
        var password = solaceSection["Password"];

        var cfp = new ContextFactoryProperties();
        ContextFactory.Instance.Init(cfp);

        _context = ContextFactory.Instance.CreateContext(new ContextProperties(), null);

        var sessionProps = new SessionProperties
        {
            Host = host,
            VPNName = vpnName,
            UserName = username,
            Password = password,
            ReconnectRetries = 3
        };

        _session = _context.CreateSession(sessionProps, null, null);
        var returnCode = _session.Connect();

        if (returnCode != ReturnCode.SOLCLIENT_OK)
        {
            throw new InvalidOperationException($"Falha ao conectar no Solace: {returnCode}");
        }
    }

    public Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken ct = default)
    {
        var destination = ContextFactory.Instance.CreateTopic(topic);
        using var message = ContextFactory.Instance.CreateMessage();

        var payload = JsonSerializer.Serialize(@event);
        message.Destination = destination;
        message.DeliveryMode = MessageDeliveryMode.Direct;
        message.BinaryAttachment = Encoding.UTF8.GetBytes(payload);

        var returnCode = _session.Send(message);
        if (returnCode != ReturnCode.SOLCLIENT_OK)
        {
            _logger.LogError("Falha ao publicar mensagem no Solace. ReturnCode={ReturnCode}", returnCode);
            throw new InvalidOperationException($"Falha ao publicar mensagem no Solace: {returnCode}");
        }

        _logger.LogInformation("Mensagem enviada para Solace. Topic={Topic}", topic);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        try
        {
            _session?.Dispose();
            _context?.Dispose();
        }
        catch
        {
        }
    }
}
