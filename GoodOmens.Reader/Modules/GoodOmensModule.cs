using Nancy;

namespace GoodOmens.Reader.Modules
{
    public class GoodOmensModule : NancyModule
    {
        public GoodOmensModule()
        {
            Get("/", args => "Hello World, it's armagedon time");
        }
    }
}
