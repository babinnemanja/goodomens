using Autofac;
using GoodOmens.Data;
using GoodOmens.Host.CommandHandlers;
using GoodOmens.Messages.Commands;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using System;
using System.Reflection;
using System.Timers;

namespace GoodOmens.Host
{
    public class GoodOmensService
    {
        readonly Timer _timer;
        private ContainerBuilder _builder;
        private IContainer _container;

        public GoodOmensService()
        {
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and armagedon has started", DateTime.Now);
        }

        public void Start()
        {
            _timer.Start();

            var inputQueueName = "GoodOmens.Host";
            var connectionString = "Endpoint=sb://goodomens.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dRyijrdoeicJ5jqTuUs46mSMSXLgaTkPhCoR7TIBWIE=";

            var endpointName = string.Format("{0} ({1})", Assembly.GetEntryAssembly().GetName().Name, inputQueueName);

            _builder = new ContainerBuilder();

            _builder.RegisterHandlersFromAssemblyOf<CreateBookHandler>();

            // register all implementations of IHandleMessage<'T> within Autofac
            _builder.RegisterRebus((configurer, context) => configurer
            .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
            .Transport(t => t.UseAzureServiceBus(connectionString, inputQueueName))
            //.Subscriptions(s => s.StoreInRavenDb(DocumentStoreHolder.Store, isCentralized: true))
            .Options(o =>
            {
                o.SetNumberOfWorkers(2);
                o.SetMaxParallelism(30);
              
            }));

            // the bus is registered now, but it has not been started.... make all your other registrations, and then:
            var _container = _builder.Build();

            Console.WriteLine(@"-------------------------------
                                    A) Subscribe to System.String
                                    B) Unsubscribe to System.String
                                    C) Publish System.String
                                    Q) Quit
                                    -------------------------------
                                    ");

            var keepRunning = true;
            var bus = _container.Resolve<IBus>();
            while (keepRunning)
            {
                var key = Console.ReadKey(true);

                switch (char.ToLower(key.KeyChar))
                {
                    case 'a':
                        Console.WriteLine("Subscribing!");
                        bus.Subscribe<CreateBook>().Wait();
                        break;

                    case 'b':
                        Console.WriteLine("Unsubscribing!");
                        bus.Unsubscribe<string>().Wait();
                        break;

                    case 'c':
                        Console.WriteLine("Publishing!");

                        var createBook = new CreateBook
                        {
                            Title = "Good omens"
                        };
                        Create.Omen();
                        bus.Publish(createBook).Wait();

                        break;

                    case 'q':
                        Console.WriteLine("Quitting!");
                        keepRunning = false;
                        bus.Dispose();
                        break;
                }
            }

            Console.WriteLine("Stopping the bus....");
        }

        public void Stop()
        {
            _container.Dispose();
            _timer.Stop();
            
        }
    }
}
