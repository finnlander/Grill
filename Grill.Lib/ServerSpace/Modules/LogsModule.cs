using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;

namespace Grill.Lib.ServerSpace.Modules
{
    public class LogsModule : NancyModule
    {
        private const string LogFilePath = @"Logs\Test.log";

       public LogsModule()
            : base("/logs")
        {
            Get["/"] = (parameters) =>
            {
                var model = new LogsModel() {LogMessages = GetLatestLogs(500)};
                return View["logs.html", model];
            };
            

        }

        private static List<string> GetLatestLogs(int lines = -1)
        {
            using (var reader = new StreamReader(LogFilePath))
            {
                var lineList = new List<string>();
                while (reader.Peek() > 0)
                {
                    lineList.Add(reader.ReadLine());
                }
                if (lines < 0)
                    return lineList;

                var count = lineList.Count;
                if (count > lines)
                {
                    var toSkip = count - lines - 1;
                    lineList = lineList.Skip(toSkip).ToList();
                }
                return lineList;
            }
        }

        public class LogsModel
        {
            public List<string> LogMessages;
        }
    }
}
