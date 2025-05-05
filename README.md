# BiDiB-Library.Core
BiDiB Infrastructure Library for connecting tools to BiDiB Bus

# Build and Test
Open BiDiB-Library.Core.sln with Visual Studio, VS Code or Rider.
Build with Debug configuration for development.
Build with Release configuration for release usage.

or build via console from root folder:
dotnet build --configuration debug

# Getting Started

## register library at IOC
- with MEF: scan assembly
- with MS DI: use extension -> (builder.)services.AddBiDiBCore();

## get from IOC and connect

var bidibInterface = IServiceProvider.GetService<IBiDiBInterface>();

bidibInterface.Initialize();

var connectionConfig = new IConnectionConfig(); 

await bidibInterface.ConnectAsync(connectionConfig);

## consume messages
To avoid handling with all the bits and bytes, the internal message receiver converts all data into handy message objects.
To consume the messages a custom implementation of IMessageReceiver is needed.
This receiver has to be registered at the interface.

bidibInterface.Register(receiver);

Messages can be send via the provided messages.
bidibInterface.SendMessage(message);
bidibInterface.SendMessage<TResponseType>(message);

