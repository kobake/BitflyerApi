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
    public enum AssetCode
    {
        JPY,
        BTC,
        ETH
    }

    public class Asset
    {
        public Asset(RawAsset _asset)
        {
            Code = (AssetCode)Enum.Parse(typeof(AssetCode), _asset.currency_code, true);
            Amount = _asset.amount;
            Available = _asset.available;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public AssetCode Code { get; set; }
        public double Amount { get; set; }
        public double Available { get; set; }

        public override string ToString()
        {
            return "Asset" + JsonConvert.SerializeObject(this);
        }
    }

    public class AssetList
    {
        public AssetList(List<Asset> _assets)
        {
            foreach(var asset in _assets)
            {
                switch (asset.Code) {
                    case AssetCode.JPY:
                        Jpy = asset; break;
                    case AssetCode.BTC:
                        Btc = asset; break;
                    case AssetCode.ETH:
                        Eth = asset; break;
                }
            }
        }

        public Asset Jpy { get; set; }
        public Asset Btc { get; set; }
        public Asset Eth { get; set; }

        public override string ToString()
        {
            return "Asset" + JsonConvert.SerializeObject(this);
        }
    }

    // 証拠金情報
    public class Collateral
    {
        public Collateral(RawCollateral _c)
        {
            CollateralAmount = _c.collateral;
            OpenPositionProfitAndLoss = _c.open_position_pnl;
            RequiredCollateral = _c.require_collateral;
            KeepRate = _c.keep_rate;
        }

        public double CollateralAmount { get; set; } // 預け入れた日本円証拠金の額(円)
        public double OpenPositionProfitAndLoss { get; set; } // 建玉の評価損益(円)
        public double RequiredCollateral { get; set; }// 現在の必要証拠金(円)
        public double KeepRate { get; set; }// 現在の証拠金維持率

        public override string ToString()
        {
            return "Collateral" + JsonConvert.SerializeObject(this);
        }
    }

}
