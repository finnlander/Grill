using System.Configuration;
using System.IO;
using GrillSrv.Properties;
using Topshelf;
using static Grill.Lib.Helpers.ConsoleWriter;

namespace GrillSrv
{
    public class GrillServerService : ServiceControl
    {
        private const string Description = "Grill Web Server host service";
        private const string DisplayName = "Grill Web Server";
        private const string ServiceName = "Grill_web_server";
        private const string ConfNameSpecifiedPort = "specifiedPort";

        private readonly int _port;
        private Grill.Lib.ServerSpace.GrillServer _grillServer;



        public GrillServerService(int port)
        {
            _port = port;
        }

        public bool Start(HostControl hostControl=null)
        {
            if (_grillServer != null)
                return false;

            _grillServer = new Grill.Lib.ServerSpace.GrillServer(_port);
            return true;
        }

        public bool Stop(HostControl hostControl=null)
        {
            if (_grillServer == null)
                return false;
            
            _grillServer.Dispose();
            _grillServer = null;
            return true;
        }


        /// <summary>
        /// Service initialization for TopShelf library
        /// </summary>
        public static void Run(bool startAutomatically=true)
        {
            HostFactory.Run(conf =>
            {
                
                conf.AddCommandLineDefinition("port", value =>
                {
                    int definedPort;
                    
                    if (int.TryParse(value, out definedPort))
                    {
                        var port = definedPort;
                        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                        var specifiedPortConf = config.AppSettings.Settings[ConfNameSpecifiedPort];
                        if (specifiedPortConf != null)
                            specifiedPortConf.Value = definedPort.ToString();
                        else
                            config.AppSettings.Settings.Add(ConfNameSpecifiedPort, definedPort.ToString());

                        config.Save(ConfigurationSaveMode.Modified, true);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                       
                });

                conf.ApplyCommandLine();
                conf.Service<GrillServerService>(srv =>
                {
                    srv.ConstructUsing(name =>
                    {
                        
                        ImportantInfo("Loading settings from config file...");
                        var value = ConfigurationManager.AppSettings[ConfNameSpecifiedPort];
                        int port;
                        if (value != null)
                        {
                            ImportantInfo($"[Settings] Using specified port value '{value}' from settings");
                            if (!int.TryParse(value, out port))
                                port = Settings.Default.defaultPort;
                        }
                        else
                        {
                           
                            port = Settings.Default.defaultPort;
                            ImportantInfo("Using defaultPort value from settings: " + port);
                        }

                        ImportantInfo("[Settings] Settings loaded");
                        return new GrillServerService(port);
                    });
                    srv.WhenStarted((s, hostControl) => s.Start(hostControl));
                    srv.WhenStopped((s, hostControl) => s.Stop(hostControl));

                });

                conf.RunAsNetworkService();
                conf.SetDescription(Description);
                conf.SetDisplayName(DisplayName);
                conf.SetServiceName(ServiceName);
                if (startAutomatically)
                    conf.StartAutomatically();
                else
                    conf.StartManually();
                
            });
        }
    }
}