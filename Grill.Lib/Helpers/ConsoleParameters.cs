using System;
using System.Collections.Generic;
using System.Linq;
using Grill.Lib.Properties;

namespace Grill.Lib.Helpers
{
    public static class ConsoleParameters
    {
        private static readonly string ServiceStartVerb = "start";
        private static readonly string[] ServiceVerbs=
        {
            ServiceStartVerb,
            "stop",
            "install",
            "uninstall"
        };


        private static readonly string[] HelpParams =
        {
            "--help",
            "help"
        };
        private static readonly string[] HelpSynonyms =
        {
            "-help",
            "-?",
            "--?",
            "/?",
            "/help"
        };

        private static readonly string[] PortParams =
        {
            "-port"
        };

        private static readonly string[] PortSynonyms =
        {
            "-p",
            "--port",
            "/p"
        };

        public static string GetDefaultHelpParameter => HelpParams[0];


        public static string GetDefaultPortParameter => PortParams[0];


        public static bool HasServiceStartVerb(IReadOnlyList<string> args)
        {
            return args.Any(arg => string.Equals(ServiceStartVerb, arg, StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasServiceVerbs(IReadOnlyList<string> args)
        {
            return args.Select(x => x.ToLowerInvariant()).Any(arg => ServiceVerbs.Contains(arg));
        }

        /// <summary>
        /// Check if arguments has help defined
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool HasHelpParameter(IReadOnlyList<string> args)
        {
            return args.Select(t => t.ToLowerInvariant()).Any(arg => HelpParams.Contains(arg));
        }

        /// <summary>
        /// Check if arguments has invalid "help" defined 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool HasHelpSynonym(IReadOnlyList<string> args)
        {
            
            return args.Select(t => t.ToLowerInvariant()).Any(arg => HelpSynonyms.Contains(arg));
        }

        /// <summary>
        /// Check if arguments has invalid "port" defined 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool HasPortSynonym(IReadOnlyList<string> args)
        {
            var paramNames = args.Select(t => string.Join(string.Empty, t.ToLowerInvariant().ToCharArray().TakeWhile(chr => chr != ':'))).ToList();
            return paramNames.Any(arg => PortSynonyms.Contains(arg));
        }

        public static void PrintHelpHeader()
        {
            const ConsoleColor argumentColor = ConsoleColor.Green;

            ConsoleWriter.ImportantInfo("-- Grill :: Help --");
            ConsoleWriter.Info("The following arguments are extensions for the built-in argument list.");
            ConsoleWriter.Info(" Syntax: ", "GrillSrv.exe [argument(s)].", argumentColor);
            ConsoleWriter.ImportantInfo("Argument\t\t\t\tDescription", ConsoleColor.White, false);

            var portParams = String.Join(" | ", PortParams);
            ConsoleWriter.Info($"{portParams}", "\t\t\tSpecify port number for Grill server", argumentColor);
            ConsoleWriter.ImportantInfo(String.Empty);
        }

        /// <summary>
        /// Get port number from arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int GetPort(IReadOnlyList<string> args)
        {
            foreach (string arg in args)
            {
                var port = GetPort(arg);
                if (port != null)
                    return port.Value;
            }

            return Settings.Default.defaultPort;
        }

        public static int? GetPort(string arg)
        {
            if (arg == null)
                return null;

            arg = arg.ToLowerInvariant();
            var sep = arg.IndexOf(":", StringComparison.Ordinal);
            if (sep < 0)
            {
                if (PortParams.Contains(arg))
                {
                    ConsoleWriter.Error("Expected port value after '-p'. Syntax: '-p:[port]'");
                    Environment.Exit(1);
                }
                return null;
            }

            var paramName = arg.Substring(0, sep);

            if (!PortParams.Contains(paramName))
                return null;

            if (arg.Length <= sep + 1)
            {
                ConsoleWriter.Error("Expected port value after '-p'. Syntax: '-p:[port]'");
                Environment.Exit(1);
            }

            var paramValue = arg.Substring(sep + 1);
            int port;
            if (!int.TryParse(paramValue, out port))
            {
                ConsoleWriter.Error($"Invalid value for port: {paramValue}");
                Environment.Exit(1);
            }

            

            return port;
        }
    }
}
