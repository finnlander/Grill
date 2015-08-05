using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Grill.Lib.Helpers;
using Nancy;
using Nancy.Hosting.Self;

namespace Grill.Lib.ServerSpace
{
    public class GrillServer : IDisposable
    {
        private const int DefaultPort = 8090;
        private const string HostAddress = @"http://localhost";

        private readonly object _lckStarted = new object();
        private bool _started = false;
        private int _hostPort;
        private string _ip;
        private readonly bool _useEmbeddedResources;

        private string ServerAddress => $"{HostAddress}:{_hostPort}";
        

        NancyHost _host;

        public GrillServer()
        : this(DefaultPort)
        {
            
        }

        public GrillServer(int port, bool useEmbeddedResources=true)
        {

            _useEmbeddedResources = useEmbeddedResources;
            try
            {
                Start(port);
            }
            catch (AutomaticUrlReservationCreationFailureException)
            {
                // This should only occur, if Nancy is not run as administrator and it cannot add the requred ACL entry
                if (TryResolveUrlReservationError())
                {
                    ConsoleWriter.ImportantInfo("[resolved server start problem] Added server URL to Windows ACL");
                    Start(port);
                    return;
                }
                
                PrintUrlReservartionError();
                Environment.Exit(-1);
            }

        }

        

        public void Dispose()
        {
            Stop();
            _host.Dispose();
            _host = null;
            ConsoleWriter.Info($"Server stopped on :: {ServerAddress} ({_ip})");
        }

        public void Restart(int port)
        {
            lock (_lckStarted)
            {
                Stop();
                Start(port);
            }
            
        }

        public bool IsStarted()
        {
            lock (_lckStarted)
            {
                return _started;
                
            }
        }

        /* Private Helpers */
        private void Start(int port)
        {
            lock (_lckStarted)
            {
                if (_started)
                {
                    ConsoleWriter.Warn("Server was already running -> start request ignored!");
                    return;
                }
                _started = true;
            }
            

            _hostPort = port;
            _ip = GetLocalhostIp() ?? string.Empty;

            var conf = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() {CreateAutomatically = true}
            };

            _host = _useEmbeddedResources
                ? new NancyHost(new Uri(ServerAddress), new EmbeddedResourceBootstrapper(), conf)
                : new NancyHost(new Uri(ServerAddress), new DefaultNancyBootstrapper(), conf);
            
            _host.Start();
            ConsoleWriter.Info($"Server running on :: {ServerAddress} ({_ip})");
        }

        private void Stop()
        {
            lock (_lckStarted)
            {
                if (!_started)
                {
                    ConsoleWriter.Warn("Server was not running -> stop request ignored!");
                    return;
                }
                   
                _host.Stop();
                _started = false;
            }
            
        }

        private static string GetLocalhostIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            return ip?.ToString();
        }

        private string GetUrlReservationCommandArgs() => $"http add urlacl url=http://+:{_hostPort}/ user=Everyone";

        private bool TryResolveUrlReservationError()
        {
            var psi = new ProcessStartInfo("netsh", GetUrlReservationCommandArgs()) {Verb = "runas"};
            var prc = Process.Start(psi);
            if (prc == null)
                return false;

            prc.WaitForExit();
            return (prc.ExitCode == 0);
        }

        private void PrintUrlReservartionError()
        {
            var instructionCommand = $"netsh {GetUrlReservationCommandArgs()}";
            ConsoleWriter.Error("ERROR: Grill server was unable to start. You must add server URL namespace to Windows access control list.");
            ConsoleWriter.Info("Run the following command as administrator: ");
            ConsoleWriter.ImportantInfo(instructionCommand);

        }
    }
}
