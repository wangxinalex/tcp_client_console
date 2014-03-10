using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Netbar_client_console{
    class Client {
        string ipAddress = "127.0.0.1";
        int port = 2500;
        private TcpClient tcp_client;
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        bool is_connected;
        string menu_path= ".\\menu.xml";
        string menu_md5 = null;

        public Client(){
            try {
                tcp_client = new TcpClient(ipAddress, port);
                ns = tcp_client.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns) { AutoFlush = true};
                is_connected = true;
                check_menu_file();
            } catch (Exception ex) {
                is_connected = false;
                Util.error_info(ex.Message);
                Console.ReadLine();
            }
        }

        public void check_menu_file() {
            menu_md5 = Util.getFileMD5(menu_path);
            sw.WriteLine("SYNC|"+menu_md5);
            string response  = sr.ReadLine();
            if (response == "NEED_SYNC") {
                Util.info("NEED_SYNC");
                sw.WriteLine("PREPARED");
                receive_menu();
            } else if (response == "NOT_NEED_SYNC") {
                Util.info("NOT_NEED_SYNC");
            }
        }

        private void receive_menu() {
            if (File.Exists(menu_path + ".old")) {
                File.Delete(menu_path+".old");
            }
            File.Move(menu_path, menu_path+".old");
            FileStream fs = new FileStream(menu_path, FileMode.Create, FileAccess.Write);
            int size = 0, len = 0;
            NetworkStream stream = ns;
            byte[] buffer = new byte[512];
            while(stream.DataAvailable){
                size = stream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, size);
                len += size;
            } 
            fs.Flush();
            stream.Flush();
            fs.Close();
            Util.info("Menu Received. Length = "+len);
        }

        public void handle_command() {
            while (is_connected) {
                String command = Console.ReadLine();
                sw.WriteLine(command);
                string response = sr.ReadLine();
                Util.response_info(response);

            }
                
        }
    }
}
