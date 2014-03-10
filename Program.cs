using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netbar_client_console {
    class Program {
        static void Main(string[] args) {
            Client client = new Client();
            client.handle_command();

         }
    }
}
