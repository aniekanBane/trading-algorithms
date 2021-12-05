/*
 * Title: Quantitative Value Strategy
 * 
 * Description: Build an Investing strategy that selects 50 stocks with the best value metrics.
 */

using Newtonsoft.Json;
using AlgorithmicTrading;
using ValueInvesting;

//Load token
var key = Utils.Secret();
//var key = Environment.GetEnvironmentVariable("IEX_CLOUD_KEY");

// Extract tickers form records.
var tickers = from s in await Utils.GetListAsync()
              select s.Symbol;

// Divide lists of securities into chunks of 100 for batch API calls.
var chunks = tickers.Chunk(100)
    .Select(x => string.Join(',', x));

List<RQV> stocks = new();

using var client = new HttpClient();

/*
 * Build a Robust quantitative value investing strategy using a comopsite baskets of valuation metrics:
 * Price-to-earn ratio
 * Price-to-book ratio
 * Price-to-sales ratio
 * EV / EBITDA
 * EV / GP
 * EV-to-revenue
 */
foreach (var symbols in chunks)
{
    string apiUri = $"https://sandbox.iexapis.com/stable/stock/market/batch?symbols={symbols}&types=quote,advanced-stats&token={key}";
    dynamic? data = JsonConvert.DeserializeObject<dynamic>(await client.GetStringAsync(apiUri));
    foreach (var ticker in symbols.Split(','))
    {
        double? ev = (double?)data?[ticker]["advanced-stats"]["enterpriseValue"];
        double? ebitda = (double?)data?[ticker]["advanced-stats"]["EBITDA"];
        double? gp = (double?)data?[ticker]["advanced-stats"]["grossProfit"];

        stocks.Add(new(ticker)
        {
            Price = (double)data?[ticker]["quote"]["latestPrice"],
            PERatio = (double?)data?[ticker]["quote"]["peRatio"],
            PBRatio = (double?)data?[ticker]["advanced-stats"]["priceToBook"],
            PSRatio = (double?)data?[ticker]["advanced-stats"]["priceToSales"],
            EVtoEBITDA = (double?)(ev / ebitda),
            EVtoGP = (double?)(ev / gp),
            EVtoRevenue = (double?)data?[ticker]["advanced-stats"]["enterpriseValueToRevenue"]
        });
    }
}

// Handle missing data
//var n = (from stock in stocks
//         from prop in stock.GetType().GetProperties()
//         where prop.CanRead
//         let vals = prop.GetValue(stock)
//         where vals == null
//         select stock).Distinct();

// Replace missing data with average non-null data point from that property
var avg_PE = stocks.Select(x => x.PERatio).Average();
var avg_PB = stocks.Select(x => x.PBRatio).Average();
var avg_PS = stocks.Select(x => x.PSRatio).Average();
var avg_EVEB = stocks.Select(x => x.EVtoEBITDA).Average();
var avg_EVGP = stocks.Select(x => x.EVtoGP).Average();
var avg_EVR = stocks.Select(x => x.EVtoRevenue).Average();
foreach (var stock in stocks)
{
    stock.PERatio ??= avg_PE;
    stock.PBRatio ??= avg_PB;
    stock.PSRatio ??= avg_PS;
    stock.EVtoEBITDA ??= avg_EVEB;
    stock.EVtoGP ??= avg_EVGP;
    stock.EVtoRevenue ??= avg_EVR;
}

var pCount = stocks.Count;
var avgPE = stocks.Select(x => x.PERatio).Average();
var avgPB = stocks.Select(x => x.PBRatio).Average();
var avgPS = stocks.Select(x => x.PSRatio).Average();
var avgEVEB = stocks.Select(x => x.EVtoEBITDA).Average();
var avgEVGP = stocks.Select(x => x.EVtoGP).Average();
var avgEVR = stocks.Select(x => x.EVtoRevenue).Average();
foreach (var stock in stocks)
{
    stock.PEPercentile = 100 * ((double)stocks.Count(x => x.PERatio < stock.PERatio) / pCount);
    stock.PBPercentile = 100 * ((double)stocks.Count(x => x.PBRatio < stock.PBRatio) / pCount);
    stock.PSPercentile = 100 * ((double)stocks.Count(x => x.PSRatio < stock.PSRatio) / pCount);
    stock.EVtoEBITDAPercentile = 100 * ((double)stocks.Count(x => x.EVtoEBITDA < stock.EVtoEBITDA) / pCount);
    stock.EVtoGPPercentile = 100 * ((double)stocks.Count(x => x.EVtoGP < stock.EVtoGP) / pCount);
    stock.EvtoRevenuePercentile = 100 * ((double)stocks.Count(x => x.EVtoRevenue < stock.EVtoRevenue) / pCount);
}

// select the 50 best stocks
stocks = stocks.OrderBy(rv => rv.RVScore).Take(50).ToList();

var share = Utils.Portfolio() / stocks.Count;

foreach (var stock in stocks)
    stock.SharesHodl = Math.Floor(share / stock.Price);

stocks.ForEach(x => Console.WriteLine(x.ToString()));