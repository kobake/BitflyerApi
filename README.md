# BitflyerApi
[bitFlyer](https://bitflyer.jp/) の API を扱うための NuGet パッケージです。

このパッケージは kobake が個人的に作ったものであり、bitFlyer 公式パッケージではありません。

## bitFlyer について
- https://bitflyer.jp/ … 仮想通貨(ビットコイン等)取引所のひとつです。
- https://lightning.bitflyer.jp/docs?lang=ja … 公式に公開されている API 仕様です。当パッケージはこの API アクセスをラップしています。（全APIではなく主要と判断したAPIのみラップしています）

## Install
当パッケージは Microsoft の [NuGet Gallery](https://www.nuget.org/packages/BitflyerApi) に登録されています。
Visual Studio の Package Manager Console にて以下を実行すればパッケージが導入されます。
```
PM> Install-Package BitflyerApi
```
### APIキーの入手
bitFlyer の API にアクセスするためには、以下の開発者ページで API Key, API Secret を入手する必要があります。
https://lightning.bitflyer.jp/developer

## Usage
### 初期化
API Key, API Secret, および取引種を指定した上で BitflyerClient インスタンスを作成してください。一般的にこのインスタンスはアプリケーションに1つあれば十分です。（必要であれば複数作っても構いません）
```cs
using BitflyerApi;
...
client = new BitflyerClient(
    "xxxxxxxxxxxxx", // API Key を指定
    "xxxxxxxxxxxxx", // API Secret を指定
    BitflyerProductCode.BTC_JPY // 取引の種類を指定。BTC_JPY, FX_BTC_JPY, ETH_BTC のいずれか。
);
```

### APIアクセス用メソッドについて
BitflyerClient インスタンスのメソッドにより API アクセスを行います。
各 API アクセスは async メソッド内で使われることを想定しています。

なお、各 API アクセスが失敗した場合には例外 Exception が投げられます。適切に try, catch してエラー対処を行ってください。

### 資産情報の取得
```cs
static async Task ShowAssetInfo()
{    
    // 資産情報
    var assetList = await client.GetMyAssetList();
    Console.WriteLine(assetList); // 諸情報
    
    // JPY残高
    Console.WriteLine("JPY = " + assetList.Jpy.Amount);

    // JPY有効残高 (たとえば約定してない買い注文が残っている場合、この数値はAmountより小さくなる)
    Console.WriteLine("JPY(Available) = " + assetList.Jpy.Available); 
    
    // BTC残高
    Console.WriteLine("BTC = " + assetList.Btc.Amount);
    
    // 証拠金情報
    var collateral = await client.GetMyCollateral();
    Console.WriteLine(collateral); // 諸情報
}
```

### 板情報の取得
```cs
async Task ShowBoard()
{
    // 板情報の取得
    Board board = await client.GetBoard();

    // 売り板 (数が多すぎるので Take で 10 件に絞っています)
    foreach (var ask in Enumerable.Reverse(board.Asks.Take(10)))
    {
        Console.WriteLine(ask.ToString());
    }

    // 買い板 (数が多すぎるので Take で 10 件に絞っています)
    foreach (var bid in board.Bids.Take(10))
    {
        Console.WriteLine(bid.ToString());
    }

    // 中間価格
    Console.WriteLine("MiddlePrice = " + board.MiddlePrice);
}
```

### 注文の実行
```cs
async Task SomeOrders()
{
    await client.Buy(60000, 0.01);  // 6万円の指値で0.01BTC買い注文
    await client.Sell(95000, 0.01); // 9万5千円の指値で0.01BTC売り注文
    await client.Buy(0.01);  // 成行で0.01BTC買い注文
    await client.Sell(0.01); // 成行で0.01BTC売り注文
}
```

### アクティブな注文 (未約定が残っている注文) の取得
```cs
static async Task ShowMyActiveOrders()
{
    var orders = await client.GetMyActiveOrders();
    foreach(var order in orders)
    {
        Console.WriteLine(order.ToString());
    }
}
```

### 注文の取り消し
```
static async Task ShowMyActiveOrders()
{
    // 全注文の取り消し
    await client.CancelAllOrders();
}
```

※個別注文の取り消し機能は今のところ（このライブラリでは）提供していません。
そのうち追加すると思いますが個人的にあまり使わないので優先度低めです。
