using Newtonsoft.Json;
using AlgorithmicTrading;

//Load token
var key = Utils.Secret();
//var key = Environment.GetEnvironmentVariable("IEX_CLOUD_TOKEN");

// Extract tickers form records.
var tickers = from s in await Utils.GetListAsync()
              select s.Symbol;

// Divide lists of securities into chunks of 100 for batch API calls.
var chunks = tickers.Chunk(100)
    .Select(x => string.Join(',', x));

List<StockData> stocks = new();

using var client = new HttpClient();

foreach (var symbols in chunks)
{
    string apiUri = $"https://sandbox.iexapis.com/stable/stock/market/batch?symbols={symbols}&types=quote&token={key}";
    dynamic? data = JsonConvert.DeserializeObject<dynamic>(await client.GetStringAsync(apiUri));
    foreach (var ticker in symbols.Split(','))
    {
        stocks.Add(new(ticker) { Price = data?[ticker]["quote"]["latestPrice"] });
    }
}

var share = Utils.Portfolio() / stocks.Count;

foreach (var stock in stocks)
    stock.SharesHodl = Math.Floor(share / stock.Price);

stocks.ForEach(x => Console.WriteLine(x.ToString()));