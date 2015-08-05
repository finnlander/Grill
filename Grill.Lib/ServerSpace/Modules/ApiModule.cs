using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace Grill.Lib.ServerSpace.Modules
{
    public class ApiModule : NancyModule
    {

        public ApiModule()
            : base("/api")
        {
            Get["/test"] = parameters => Response.AsJson<object>( new {Id = "test-id", TimeStamp = DateTime.Now, Value = "OK"});
        }
    }
}
