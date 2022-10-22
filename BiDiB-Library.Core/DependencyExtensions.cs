using Microsoft.Extensions.DependencyInjection;
using org.bidib.netbidibc.core.Message;
using org.bidib.netbidibc.core.Services;
using org.bidib.netbidibc.core.Services.Interfaces;

namespace org.bidib.netbidibc.core
{
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
            services.AddSingleton<IBiDiBMessageProcessor, BiDiBMessageProcessor>();
            services.AddTransient<IBiDiBMessageExtractor, BiDiBMessageExtractor>();
            services.AddSingleton<IBiDiBBoosterNodesManager, BiDiBBoosterNodesManager>();
        }
    }
}
