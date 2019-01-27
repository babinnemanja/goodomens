using System;
using Topshelf;

namespace GoodOmens.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>                                   //1
            {
                x.Service<GoodOmensService>(s =>                                   //2
                {
                    s.ConstructUsing(name => new GoodOmensService());                //3
                    s.WhenStarted(tc => tc.Start());                         //4
                    s.WhenStopped(tc => tc.Stop());                          //5
                });
                x.RunAsLocalSystem();                                       //6

                x.SetDescription("An Antichrist service");                   //7
                x.SetDisplayName("GoodOmens");                                  //8
                x.SetServiceName("GoodOmens");                                  //9
            });                                                             //10

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  //11
            Environment.ExitCode = exitCode;
        }
    }
}
