using Agile.AServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vaporwave_core.api
{
    internal class Core
    {
        public Core() => ServerSing.AddHandler(
                 new HttpHandler()
                 {
                     Method = "GET",
                     Path = "/vaporwave_core",
                     Handler = async (req, resp) =>
                     {
                         await resp.WriteJson("vaporwave_core");
                     }
                 }
             );
    }
}
