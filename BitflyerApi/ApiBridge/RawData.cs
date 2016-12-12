using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitflyerApi.ApiBridge
{
    // 資産情報取得
    public class RawAsset
    {
        public string currency_code { get; set; } // JPY, BTC, ETH
        public double amount { get; set; }
        public double available { get; set; }
    }

    // 証拠金情報取得
    public class RawCollateral
    {
        public double collateral { get; set; } // 預け入れた日本円証拠金の額
        public double open_position_pnl { get; set; } // 建玉の評価損益
        public double require_collateral { get; set; } // 現在の必要証拠金
        public double keep_rate { get; set; } // 現在の証拠金維持率
    }

    // 注文取得
    public class RawOrder
    {
        public int id { get; set; }
        public string child_order_id { get; set; }
        public string product_code { get; set; }
        public string side { get; set; }
        public string child_order_type { get; set; }
        public double price { get; set; }
        public double average_price { get; set; }
        public double size { get; set; }
        public string child_order_state { get; set; }
        public string expire_date { get; set; }
        public string child_order_date { get; set; }
        public string child_order_acceptance_id { get; set; }
        public double outstanding_size { get; set; }
        public double cancel_size { get; set; }
        public double executed_size { get; set; }
        public double total_commission { get; set; }
    }

    // 板情報取得
    public class RawBoardOrder
    {
        // 価格 (BTCJPY等では常に整数だがETHJPY等では小数もあり得ることに注意)
        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }

        // 注文量
        [JsonProperty(PropertyName = "size")]
        public double Size { get; set; }
    }
    public class RawBoard
    {
        // 中間価格
        [JsonProperty(PropertyName = "mid_price")]
        public double MiddlePrice { get; set; }

        // 売り板
        [JsonProperty(PropertyName = "asks")]
        public List<BoardOrder> Asks { get; set; }

        // 買い板
        [JsonProperty(PropertyName = "bids")]
        public List<BoardOrder> Bids { get; set; }
    }

    // 注文結果
    // 例: {"child_order_acceptance_id":"JRF20161212-104117-716911"}
    // 例: {"status":-110,"error_message":"The minimum order size is 0.001 BTC.","data":null}
    public class RawOrderResult
    {
        public string child_order_acceptance_id { get; set; }
        public string error_message { get; set; }

        public bool IsError()
        {
            return !string.IsNullOrEmpty(error_message);
        }
    }
}
