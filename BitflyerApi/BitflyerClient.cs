using BitflyerApi.ApiBridge;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitflyerApi
{
    public enum ProductCode
    {
        BTC_JPY,
        FX_BTC_JPY,
        ETH_BTC
    }


    public class BitflyerClient
    {
        ApiClient m_apiClient;
        ProductCode PRODUCT_CODE;

        public BitflyerClient(string apiKey, string apiSecret, ProductCode productCode)
        {
            m_apiClient = new ApiClient(apiKey, apiSecret);
            PRODUCT_CODE = productCode;
        }

        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        // 注文
        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        // 指値で買い注文
        public async Task<OrderResult> Buy(double price, double amount)
        {
            if (price <= 0) throw new Exception("Invalid buy price: " + price);
            return await _BuySell(OrderSide.BUY, price, amount);
        }

        // 成行で買い注文
        public async Task<OrderResult> Buy(double amount)
        {
            return await _BuySell(OrderSide.BUY, 0, amount);
        }

        // 指値で売り注文
        public async Task<OrderResult> Sell(double price, double amount)
        {
            if (price <= 0) throw new Exception("Invalid sell price: " + price);
            return await _BuySell(OrderSide.SELL, price, amount);
        }

        // 成行で売り注文
        public async Task<OrderResult> Sell(double amount)
        {
            return await _BuySell(OrderSide.SELL, 0, amount);
        }

        // {"child_order_acceptance_id":"JRF20161212-104117-716911"}
        // {"status":-110,"error_message":"The minimum order size is 0.001 BTC.","data":null}
        private async Task<OrderResult> _BuySell(OrderSide side, double price, double amount)
        {
            // リクエスト構築
            string body = "";
            if(price > 0) // 指値
            {
                var reqobj = new
                {
                    product_code = PRODUCT_CODE.ToString(), // BTC_JPY, FX_BTC_JPY
                    child_order_type = "LIMIT", // 指値: LIMIT, 成行: MARKET
                    side = side.ToString(),
                    price = price,
                    size = amount
                };
                body = JsonConvert.SerializeObject(reqobj);
            }
            else // 成行
            {
                var reqobj = new
                {
                    product_code = PRODUCT_CODE.ToString(), // BTC_JPY, FX_BTC_JPY
                    child_order_type = "MARKET", // 指値: LIMIT, 成行: MARKET
                    side = side.ToString(),
                    size = amount
                };
                body = JsonConvert.SerializeObject(reqobj);
            }

            // リクエスト送信
            string json = await m_apiClient.Post("/v1/me/sendchildorder", body);

            // 応答パース
            try
            {
                RawOrderResult _result = JsonConvert.DeserializeObject<RawOrderResult>(json);
                if (_result.IsError()) throw new Exception(_result.error_message);
                return new OrderResult(_result);
            }
            catch(Exception ex)
            {
                throw new Exception("OrderError: " + ex.Message);
            }
        }

        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        // 建玉情報の取得
        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        public async Task<List<Position>> GetMyPositions()
        {
            // リクエスト送信
            string json = await m_apiClient.Get("/v1/me/getpositions?product_code=" + PRODUCT_CODE.ToString());

            // 応答パース
            try
            {
                List<RawPosition> _positions = JsonConvert.DeserializeObject<List<RawPosition>>(json);
                return _positions.Select(p => new Position(p)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("GetBoardError: " + ex.Message);
            }
        }


        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        // 注文取り消し
        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        public async Task CancelAllOrders()
        {
            // リクエスト構築
            var reqobj = new
            {
                product_code = PRODUCT_CODE.ToString(), // BTC_JPY, FX_BTC_JPY
            };
            string body = JsonConvert.SerializeObject(reqobj);

            // リクエスト送信
            await m_apiClient.Post("/v1/me/cancelallchildorders", body);
        }

        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        // 板情報を取得
        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        public async Task<Board> GetBoard()
        {
            // リクエスト送信
            string json = await m_apiClient.Get("/v1/board?product_code=" + PRODUCT_CODE.ToString());

            // 応答パース
            try
            {
                RawBoard _board = JsonConvert.DeserializeObject<RawBoard>(json);
                return new Board(_board);
            }
            catch (Exception ex)
            {
                throw new Exception("GetBoardError: " + ex.Message);
            }
        }


        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        // 資産情報を取得
        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        public async Task<AssetList> GetMyAssetList()
        {
            // リクエスト送信
            string json = await m_apiClient.Get("/v1/me/getbalance");

            // 応答パース
            try
            {
                List<RawAsset> _assets = JsonConvert.DeserializeObject<List<RawAsset>>(json);
                List<Asset> assets = _assets.Select(a => new Asset(a)).ToList();
                return new AssetList(assets);
            }
            catch (Exception ex)
            {
                throw new Exception("GetMyAssetsError: " + ex.Message);
            }
        }

        // FX証拠金の状態を取得
        public async Task<Collateral> GetMyCollateral()
        {
            // リクエスト送信
            string json = await m_apiClient.Get("/v1/me/getcollateral");

            // 応答パース
            try
            {
                RawCollateral _collateral = JsonConvert.DeserializeObject<RawCollateral>(json);
                return new Collateral(_collateral);
            }
            catch (Exception ex)
            {
                throw new Exception("GetMyAssetsError: " + ex.Message);
            }
        }

        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        // 自分の注文一覧を取得 (※最大100件まで)
        // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- //
        enum MyOrderState
        {
            ACTIVE,     // オープンな注文の一覧を取得します。
            COMPLETED,  // 全額が取引完了した注文の一覧を取得します。
            CANCELED,   // お客様がキャンセルした注文です。
            EXPIRED,    // 有効期限に到達したため取り消された注文の一覧を取得します。
            REJECTED,   // 失敗した注文です。
        }
        public async Task<List<Order>> GetMyActiveOrders()
        {
            return await _GetMyOrders(MyOrderState.ACTIVE);
        }
        private async Task<List<Order>> _GetMyOrders(MyOrderState orderState)
        {
            // リクエスト送信
            string json = await m_apiClient.Get(
                "/v1/me/getchildorders?product_code=" + PRODUCT_CODE.ToString()
                + "&child_order_state=" + orderState.ToString()
                + "&count=100"
            );

            // 応答パース
            try
            {
                // Debug.WriteLine(json);
                List<RawOrder> rawOrders = JsonConvert.DeserializeObject<List<RawOrder>>(json);
                List<Order> orders = rawOrders.Select(o => new Order(o)).ToList();
                return orders;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("JsonError: " + ex.Message + "\nJSON=" + json);
                return null;
            }
        }
    }
}
