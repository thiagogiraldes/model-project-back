using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;

namespace Modelo.Infrastructure.Messaging.Solace;

public sealed class SolaceOrderCreatedConsumer : BackgroundService
{
    private readonly ILogger<SolaceOrderCreatedConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    private IContext? _context;
    private ISession? _session;

    private const string TopicName = "corp/modelo/orders/order-created/v1";

    public SolaceOrderCreatedConsumer(
        ILogger<SolaceOrderCreatedConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            try
            {
                InitSolace();

                _logger.LogInformation(
                    "SolaceOrderCreatedConsumer conectado. (Exemplo de consumo está como TODO)");


                while (!stoppingToken.IsCancellationRequested)
                {
                    // Aqui adicionar a lógica de consumo

                    Task.Delay(1000, stoppingToken).Wait(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // cancel normal
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no SolaceOrderCreatedConsumer");
            }
        }, stoppingToken);
    }

    private void InitSolace()
    {
        var solaceSection = _configuration.GetSection("Solace");
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
        var rc = _session.Connect();
        if (rc != ReturnCode.SOLCLIENT_OK)
        {
            throw new Exception($"Falha ao conectar ao Solace: {rc}");
        }

        _logger.LogInformation("Conectado ao Solace. Host={Host}, Vpn={Vpn}", host, vpnName);

        // subscribe comentado, mas que deve ser descomentado na solução real
        // var topic = ContextFactory.Instance.CreateTopic(TopicName);
        // _session.Subscribe(topic, true);
    }

    public override void Dispose()
    {
        try
        {
            _session?.Dispose();
            _context?.Dispose();
        }
        catch
        {
        }

        base.Dispose();
    }
}
