/*
 *Title: Quantitative Momentum Strategy 
 *
 *Description: Build an Investing strategy that selects 50 stocks with the best momentum.
 */

using AlgorithmicTrading;
using MomentumInvesting;
using Newtonsoft.Json;

// Load token
var key = Utils.Secret();
//var key = Environment.GetEnvironmentVariable("IEX_CLOUD_TOKEN");

// Extract tickers form records.
var tickers = from s in await Utils.GetListAsync()
              select s.Symbol;

// Divide lists of securities into chunks of 100 for batch API calls.
var chunks = tickers.Chunk(100)
    .Select(x => string.Join(',', x));

List<HQM> stocks = new();

using var client = new HttpClient();

/*
 * Build high-quality momentum stock strategy by selecting the highest percentiles these
 * time-series momentum metrics:
 * 1-month price return
 * 3-month price return
 * 6-month price return
 * 1-year price return
 */
foreach (var symbols in chunks)
{
    string apiUri = $"https://sandbox.iexapis.com/stable/stock/market/batch?symbols={symbols}&types=stats,price&token={key}";
    var data = JsonConvert.DeserializeObject<dynamic>(await client.GetStringAsync(apiUri));
    foreach (var ticker in symbols.Split(','))
    {
        stocks.Add(new HQM(ticker)
        {
            Price = (double)data?[ticker]["price"],
            OneYearPriceReturn = (double)data?[ticker]["stats"]["year1ChangePercent"],
            SixMonthPriceReturn = (double)data?[ticker]["stats"]["month6ChangePercent"],
            ThreeMonthPriceReturn = (double)data?[ticker]["stats"]["month3ChangePercent"],
            OneMonthPriceReturn = (double)data?[ticker]["stats"]["month1ChangePercent"]
        });
    }
}

// Calculate the momentum percentiles for each stock in the list.
var count = stocks.Count;
var oneYear = (from num in stocks select num.OneYearPriceReturn).ToArray();
var sixM = (from num in stocks select num.SixMonthPriceReturn).ToArray();
var threeM = (from num in stocks select num.ThreeMonthPriceReturn).ToArray();
var oneM = (from num in stocks select num.OneMonthPriceReturn).ToArray();

int i = 0;
while (i < count)
{
    var stock = stocks[i];
    stock.OneYearPercentile = 100 * ((double)stocks.Count(o => o.OneYearPriceReturn < oneYear[i]) / count);
    stock.SixMonthPercentile = 100 * ((double)stocks.Count(o => o.SixMonthPriceReturn < sixM[i]) / count);
    stock.ThreeMonthPercentile = 100 * ((double)stocks.Count(o => o.ThreeMonthPriceReturn < threeM[i]) / count);
    stock.OneMonthPercentile = 100 * ((double)stocks.Count(o => o.OneMonthPriceReturn < oneM[i]) / count);
    i++;
}

// Select the 50 best momentum stocks
stocks = stocks.OrderByDescending(x => x.HQMScore).Take(50).ToList();

var share = Utils.Portfolio() / stocks.Count;

foreach (var stock in stocks)
    stock.SharesHodl = Math.Floor(share / stock.Price);

//Display results
stocks.ForEach(x => Console.WriteLine(x.ToString()));