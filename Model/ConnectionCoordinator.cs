using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineTableGamePlayer.Model
{
    public class ConnectionCoordinator
    {
        static string SEVER_URL = AnonymousData.SERVER_URL_ANONYMOUS;//azure functionsのURL
        public bool getIP;
        public string yourIP;
        public int port;
        private string passStr;

        public ConnectionCoordinator(int my_port,string pass)
        {
            port = my_port;
            passStr = pass;
        }

        //サーバーと応答ができればtrueが返る
        public async Task<bool> AskHTTPServerAsync()
        {
            bool status_bool = false;
            using (var client = new HttpClient())
            {
                
                var json = JsonSerializer.Serialize<SendBodyJson>(new SendBodyJson() { name = "Tester1"});
                var content = new StringContent(json, Encoding.UTF8,"application/json");
                var response = await client.PostAsync(SEVER_URL, content); // POST
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    status_bool = true;
                    var str =await response.Content.ReadAsStringAsync();

                }
            }

            return status_bool;
        }

        private class SendBodyJson
        {
            public string name { get; set; }
        }
    }
}
