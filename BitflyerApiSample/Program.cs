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
                ProductCode.FX_BTC_JPY,
                timeoutSec: 6
            );

            Task.Run(async () =>
            {
                try
                {
                    // 資産情報を取得・表示
                    // await ShowAssetInfo();

                    // 現在の板情報を取得・表示
                    //await ShowBoard();

                    // 自分の注文情報を取得・表示
                    // await ShowMyActiveOrders();

                    // 自分の注文を1個だけキャンセル
                    await CancelOneOrder();

                    // 実際の注文
                    // await SendSomeOrders();

                    // 注文の取り消し
                    //await client.CancelAllOrders();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AnyError: " + ex.Message);
                }
            }).Wait();
        }

        static async Task ShowAssetInfo()
        {
            Console.WriteLine("");
            Console.WriteLine("========================================");
            Console.WriteLine("asset");
            Console.WriteLine("========================================");
            
            // 資産情報を取得・表示
            var assetList = await client.GetMyAssetList();
            Console.WriteLine(assetList);
            Console.WriteLine("JPY = " + assetList.Jpy.Amount);
            Console.WriteLine("JPY(Available) = " + assetList.Jpy.Available);
            Console.WriteLine("BTC = " + assetList.Btc.Amount);

            // 証拠金情報
            var collateral = await client.GetMyCollateral();
            Console.WriteLine(collateral);

            // 建玉情報
            Console.WriteLine("----positions----");
            var positions = await client.GetMyPositions();
            foreach(var p in positions)
            {
                Console.WriteLine(p);
            }
            Console.WriteLine("----/positions----");


            Console.WriteLine("");
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


        static async Task CancelOneOrder()
        {
            Console.WriteLine("");
            Console.WriteLine("========================================");
            Console.WriteLine("my orders (for cancel)");
            Console.WriteLine("========================================");
            var orders = await client.GetMyActiveOrders();
            foreach (var order in orders)
            {
                Console.WriteLine(order.ToString());
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("========================================");
            Console.WriteLine("cancel one order");
            Console.WriteLine("========================================");
            if(orders.Count > 0)
            {
                await client.CancelOrder(orders[0]);
                Console.WriteLine("one order cancel done");
            }
            else
            {
                Console.WriteLine("no order to cancel");
            }
        }

    }
}
