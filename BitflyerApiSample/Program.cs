using BitflyerApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitflyerApiSample
{
    class Program
    {
        static BitflyerClient client;
        
        static void Main(string[] args)
        {
            // 通信用クライアント生成 (apiKey, apiSecret はご自身のもので差し替えてください)
            client = new BitflyerClient(
                "xxxxxxxxxxxxx",
                "xxxxxxxxxxxxx",
                BitflyerProductCode.BTC_JPY
            );

            Task.Run(async () =>
            {
                try
                {
                    // 資産情報を取得・表示
                    var assetList = await client.GetMyAssetList();
                    Console.WriteLine(assetList);

                    // 証拠金情報
                    var collateral = await client.GetMyCollateral();
                    Console.WriteLine(collateral);

                    // 現在の板情報を取得・表示
                    await ShowBoard();

                    // 自分の注文情報を取得・表示
                    await ShowMyActiveOrders();

                    // 実際の注文
                    // await SendSomeOrders();

                    // 注文の取り消し
                    await client.CancelAllOrders();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("AnyError: " + ex.Message);
                }
            }).Wait();
        }

        // 実際の注文
        static async Task SendSomeOrders()
        {
            try
            {
                //var b = await client.Buy(0.001f);
                //var b = await client.Sell(0.001f);
                //Console.WriteLine(b);
                OrderResult hoge = await client.Buy(60002, 0.01);
                Console.WriteLine(hoge);
                //OrderResult sell = await client.Sell(100000, 0.00001f);
                //Console.WriteLine(sell);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // 現在の板情報を取得・表示
        static async Task ShowBoard()
        {
            Console.WriteLine("");
            Console.WriteLine("========================================");
            Console.WriteLine("board");
            Console.WriteLine("========================================");

            // 板情報の表示 (売り板、買い板を10件ずつ)
            Board board = await client.GetBoard();
            foreach (var ask in Enumerable.Reverse(board.Asks.Take(10)))
            {
                Console.WriteLine(ask.ToString());
            }
            Console.WriteLine("------------------");
            foreach (var bid in board.Bids.Take(10))
            {
                Console.WriteLine(bid.ToString());
            }

            // 中間価格
            Console.WriteLine("");
            Console.WriteLine("MiddlePrice = " + board.MiddlePrice);
            Console.WriteLine("");
        }

        static async Task ShowMyActiveOrders()
        {
            Console.WriteLine("");
            Console.WriteLine("========================================");
            Console.WriteLine("my orders");
            Console.WriteLine("========================================");
            var orders = await client.GetMyActiveOrders();
            foreach(var order in orders)
            {
                Console.WriteLine(order.ToString());
            }
            Console.WriteLine("");
        }

    }
}
