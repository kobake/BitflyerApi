using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitflyerApi.ApiBridge
{
    public class ApiClient
    {
        string API_SECRET;
        string API_KEY;

        public ApiClient(string apiKey, string apiSecret)
        {
            API_KEY = apiKey;
            API_SECRET = apiSecret;
        }

        public async Task<string> Get(string path)
        {
            return await _Send("GET", path, "");
        }

        public async Task<string> Post(string path, string body)
        {
            return await _Send("POST", path, body);
        }

        private async Task<string> _Send(string httpMethod, string path, string body)
        {
            Int64 timestamp_ = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            string timestamp = "" + timestamp_;

            // 例: 1POST/v1/me/sendchildorder{"product_code":"FX_BTC_JPY","child_order_type":"LIMIT","side":"BUY","price":30000,"size":0.1}
            string text = timestamp + httpMethod + path + body;

            // ハッシュ計算
            // 例: 008f86d0c56d36f1231596dfddd31cef85452fd2189357e2837f9cb3791d7144
            HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(API_SECRET));
            byte[] sign_ = hmac.ComputeHash(Encoding.ASCII.GetBytes(text));
            string sign = "";
            for (int i = 0; i < sign_.Length; i++)
            {
                sign += string.Format("{0:x2}", sign_[i]);
            }

            // リクエスト送信
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(2); // タイムアウトは仮で2秒にしておく
            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("ACCESS-KEY", API_KEY);
            client.DefaultRequestHeaders.Add("ACCESS-TIMESTAMP", timestamp);
            client.DefaultRequestHeaders.Add("ACCESS-SIGN", sign);

            // リクエスト送信
            HttpResponseMessage response;
            if (httpMethod == "POST")
            {
                response = await client.PostAsync("https://api.bitflyer.jp" + path, content);
            }
            else
            {
                response = await client.GetAsync("https://api.bitflyer.jp" + path);
            }

            // 応答受け取り
            var responseText = await response.Content.ReadAsStringAsync();
            return responseText;
        }
    }
}
