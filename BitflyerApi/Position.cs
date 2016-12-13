using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitflyerApi
{
    /*
    例
    {
        "product_code": "FX_BTC_JPY",
        "side": "BUY",
        "price": 36000,
        "size": 10,
        "commission": 0,
        "swap_point_accumulate": -35,
        "require_collateral": 120000,
        "open_date": "2015-11-03T10:04:45.011",
        "leverage": 3,
        "pnl": 965
    }
    */
    public class RawPosition
    {
        public string product_code { get; set; }
        public string side { get; set; }
        public double price { get; set; }
        public double size { get; set; }
        public double commission { get; set; }
        public double swap_point_accumulate { get; set; }
        public double require_collateral { get; set; }
        public string open_date { get; set; }
        public double leverage { get; set; }
        public double pnl { get; set; }
    }
    public class Position
    {
        public Position(RawPosition p)
        {
            ProductCode = (ProductCode)Enum.Parse(typeof(ProductCode), p.product_code, true);
            Side = (OrderSide)Enum.Parse(typeof(OrderSide), p.side, true);
            Price = p.price;
            Size = p.size;
            Comission = p.commission;
            SwapPointAccumulate = p.swap_point_accumulate;
            RequiredCollateral = p.require_collateral;
            OpenDate = DateTime.Parse(p.open_date + "+00:00");
            Leverage = p.leverage;
            ProfitAndLoss = p.pnl;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProductCode ProductCode { get; set; } // 今のところは常に FX_BTC_JPY が入る想定
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderSide Side { get; set; } // 売り・買い
        public double Price { get; set; } // 約定価格
        public double Size { get; set; } // 数量
        public double Comission { get; set; } // 手数料
        public double SwapPointAccumulate { get; set; } // スワップポイント蓄積額
        public double RequiredCollateral { get; set; } // 取引証拠金
        public DateTime OpenDate { get; set; } // 注文日時
        public double Leverage { get; set; } // レバレッジ
        public double ProfitAndLoss { get; set; } // 評価損益

        public override string ToString()
        {
            return "Position" + JsonConvert.SerializeObject(this);
        }
    }
}
