using System;
using Grill.Lib.ServerSpace;
using static Grill.Lib.Helpers.ConsoleParameters;
using static Grill.Lib.Helpers.ConsoleWriter;

namespace GrillSrv
{
    internal class Program
    {
        const string StartMsg = "[Started] Grill web server started";
        const string StopMsg = "[Stopped] Grill web server stopped";


        private static void Main(string[] args)
        {
            
            ImportantInfo("=== Grill web server ===");

            if (HasHelpSynonym(args))
            {
                Warn($"Did you mean \"{GetDefaultHelpParameter}\"?");
                return;
            }
            if (HasPortSynonym(args))
            {
                Warn($"Did you mean \"{GetDefaultPortParameter}\"?");
                return;
            }

            if (HasHelpParameter(args))
            {
                
                PrintHelpHeader();
                GrillServerService.Run();
                return;
            }

            var serviceMode = HasServiceStartVerb(args);
            string startInfo = string.Empty;
            if (serviceMode)
            {
                startInfo = "[INFO] Executing Grill server as Windows service";
                ImportantInfo(StartMsg);
            }
               

            else if (!HasServiceVerbs(args))
            {
                ImportantInfo(StartMsg);
                startInfo = "[INFO] Executing Grill server as console application";
            }
                
            else
                startInfo = "[INFO] Executing Grill service configurations";
            

           
            // Run as service or console
            ImportantInfo(startInfo);
            GrillServerService.Run();
            if (!serviceMode)
                ImportantInfo(StopMsg);

        }
    }
}