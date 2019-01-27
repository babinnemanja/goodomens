using Rebus.Activation;
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

            using (var adapter = new BuiltinHandlerActivator())
            {
                adapter.Handle<string>(async str =>
                {
                    Console.WriteLine("Got message from Adam: {0}", str);
                });

                Console.WriteLine("Starting {0} bus", endpointName);

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseAzureServiceBus(connectionString, inputQueueName))
                    //.Subscriptions(s => s.StoreInRavenDb(Idoc, isCentralized: true))
                    .Start();

                Console.WriteLine(@"-------------------------------
                                    A) Subscribe to System.String
                                    B) Unsubscribe to System.String
                                    C) Publish System.String
                                    Q) Quit
                                    -------------------------------
                                    ");

                var keepRunning = true;

                while (keepRunning)
                {
                    var key = Console.ReadKey(true);

                    switch (char.ToLower(key.KeyChar))
                    {
                        case 'a':
                            Console.WriteLine("Subscribing!");
                            adapter.Bus.Subscribe<string>().Wait();
                            break;

                        case 'b':
                            Console.WriteLine("Unsubscribing!");
                            adapter.Bus.Unsubscribe<string>().Wait();
                            break;

                        case 'c':
                            Console.WriteLine("Publishing!");
                            adapter.Bus.Publish(string.Format("Greetings to subscribers from {0}", endpointName)).Wait();
                            break;

                        case 'q':
                            Console.WriteLine("Quitting!");
                            keepRunning = false;
                            break;
                    }
                }

                Console.WriteLine("Stopping the bus....");
            }
        }

        public void Stop() {
            _timer.Stop(); }
            }
}
