using static Grill.Lib.Helpers.ConsoleWriter;
using static System.Console;
using static Grill.Lib.Helpers.ConsoleParameters;

namespace Grill.Lib.ServerSpace
{
    /// <summary>
    /// Provides single instance of Grill Server
    /// </summary>
    public static class Server
    {
        private static GrillServer _grill;
        private static int _port = 8090; // Default value 8090

        public static void Run(int? port)
        {
            if (port != null)
                SetPort(port.Value);

            using (Grill)
            {
                Info("-- Press any key to exit --", true);
                ReadKey();
            }
        }

        /// <summary>
        /// Set port value
        /// </summary>
        /// <param name="port"></param>
        /// <param name="restartIfStarted"></param>
        public static void SetPort(int port, bool restartIfStarted=true)
        {
            _port = port;
            if (_grill == null)
                return;

            if (restartIfStarted)
            {
                Grill.Restart(_port);
            }
        }
        /// <summary>
        ///Grill Server instance
        /// </summary>
        public static GrillServer Grill => _grill ?? (_grill = new GrillServer(_port));
    }
}
