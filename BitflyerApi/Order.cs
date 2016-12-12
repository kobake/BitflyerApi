using BitflyerApi.ApiBridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitflyerApi
{
    public enum OrderSide
    {
        BUY,
        SELL
    }

    public class Order
    {
        public Order(RawOrder _order)
        {
            // Id = _order.id; // これはただのインデックスで意味ないので消す
            OrderId = _order.child_order_acceptance_id; // 注文受付ID
            ChildOrderId = _order.child_order_id; // 約定ID
            Date = DateTime.Parse(_order.child_order_date + "+00:00"); // 注文日時
            Side = (OrderSide)Enum.Parse(typeof(OrderSide), _order.side, true);
            Price = (int)_order.price;
            Size = _order.size;
            ExecutedSize = _order.executed_size;
            OutstandingSize = _order.outstanding_size;
        }

        // public int Id { get; set; } これはただのインデックスで意味ないので消す
        public string OrderId { get; set; } // 注文受付ID
        public string ChildOrderId { get; set; } // 約定ID
        public DateTime Date { get; set; } // 注文日時
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderSide Side { get; set; } // 売・買
        public int Price { get; set; } // 価格
        public double Size { get; set; } // 数量
        public double ExecutedSize { get; set; } // 約定済数量
        public double OutstandingSize { get; set; } // 残数量

        public override string ToString()
        {
            return "Order" + JsonConvert.SerializeObject(this);
        }
    }

    public class OrderResult
    {
        public OrderResult(RawOrderResult _result)
        {
            this.OrderId = _result.child_order_acceptance_id;
        }

        public string OrderId { get; set; } // 注文受付ID

        public override string ToString()
        {
            return "OrderResult" + JsonConvert.SerializeObject(this);
        }
    }
}
