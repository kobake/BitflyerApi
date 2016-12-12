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
    public enum BoardOrderSide
    {
        ASK, // 売り
        BID // 買い
    }

    public class BoardOrder
    {
        // 売りまたは買い
        [JsonConverter(typeof(StringEnumConverter))]
        public BoardOrderSide Side { get; set; }

        // 価格 (BTCJPY等では常に整数だがETHJPY等では小数もあり得ることに注意)
        public double Price { get; set; }

        // 注文量
        public double Size { get; set; }

        public override string ToString()
        {
            return "BoardOrder" + JsonConvert.SerializeObject(this);
        }
    }

    public class Board
    {
        public Board(RawBoard _board)
        {
            MiddlePrice = _board.MiddlePrice;
            Asks = _board.Asks.Select(e => new BoardOrder { Side = BoardOrderSide.ASK, Price = e.Price, Size = e.Size }).ToList();
            Bids = _board.Bids.Select(e => new BoardOrder { Side = BoardOrderSide.BID, Price = e.Price, Size = e.Size }).ToList();
        }

        // 中間価格 (これ意味あんのかな…)
        public double MiddlePrice { get; set; }

        // 売り板
        public List<BoardOrder> Asks { get; set; }

        // 買い板
        public List<BoardOrder> Bids { get; set; }
    }
}
