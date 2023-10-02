using Microsoft.Extensions.DependencyInjection;
using org.bidib.Net.Core.Message;
using org.bidib.Net.Core.Services;
using org.bidib.Net.Core.Services.ConnectionStrategies;
using org.bidib.Net.Core.Services.Interfaces;

namespace org.bidib.Net.Core;

public static class DependencyExtensions
{
    public static void AddBiDiBCore(this IServiceCollection services)
    {
        services.AddSingleton<IIoService, IoService>();
        services.AddSingleton<IJsonService, JsonService>();
        services.AddSingleton<IBiDiBInterface, BiDiBInterface>();
        services.AddSingleton<IConnectionService, ConnectionService>();
        services.AddSingleton<IBiDiBNodesFactory, BiDiBNodesFactory>();
        services.AddSingleton<IMessageReceiver, MainMessageReceiver>();
        services.AddSingleton<IMessageReceiver, AccessoryMessageReceiver>();
        services.AddSingleton<IMessageReceiver, BoosterMessageReceiver>();
        services.AddSingleton<IMessageReceiver, FeatureMessageReceiver>();
        services.AddSingleton<IMessageReceiver, StringMessageReceiver>();
        services.AddSingleton<IBiDiBMessageProcessor, BiDiBMessageProcessor>();
        services.AddTransient<IBiDiBMessageExtractor, BiDiBMessageExtractor>();
        services.AddSingleton<IBiDiBBoosterNodesManager, BiDiBBoosterNodesManager>();
        services.AddSingleton<IConnectionStrategy, DefaultStrategy>();
        services.AddSingleton<IConnectionStrategy, ConnectOnlyStrategy>();
        services.AddSingleton<IMessageFactory, MessageFactory>();
    }
}