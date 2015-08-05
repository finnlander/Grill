using Nancy;

namespace Grill.Lib.ServerSpace.Modules
{
    /// <summary>
    /// Module that provides root path "/"
    /// </summary>
    public class RootModule : NancyModule
    {
        private const string HelloMessage = "Grill server is up and running";

        public RootModule()
        {
            Get["/"] = parameters => HelloMessage;
        }
    }
}
