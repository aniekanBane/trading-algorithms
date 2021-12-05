using CsvHelper;
using DotNetEnv;
using System.Globalization;

namespace AlgorithmicTrading
{
    /// <summary>CSV data extraction record</summary>
    /// <param name="Symbol">Stock Symbol</param>
    /// <param name="Name">Company name</param>
    /// <param name="Sector">Main Industry</param>
    public record SP500(string Symbol, string Name, string Sector);

    public class StockData
    {
        /// <summary>Stock Symbol </summary>
        public string Ticker { get; set; }

        public double Price { get; set; }

        /// <summary> Suggested Number of shares to buy</summary>
        public double SharesHodl { get; set; }

        public StockData(string ticker) => Ticker = ticker;

        /// <returns>A string that represents current object</returns>
        public override string ToString()
        {
            return $"Ticker: {Ticker} | Price: ${Price} | (Shares To Buy): {SharesHodl}";
        }
    }

    public class Utils
    {
        private static readonly HttpClient client = new();

        /// <summary>Load csv data</summary>
        /// <returns>List of records</returns>
        public static async Task<IEnumerable<SP500>> GetListAsync()
        {
            string url = "https://datahub.io/core/s-and-p-500-companies/r/constituents.csv";
            using var resp = await client.GetStreamAsync(url);

            using var reader = new StreamReader(resp);
            using var file = new CsvReader(reader, CultureInfo.InvariantCulture);

            return file.GetRecords<SP500>().ToList();
        }

        /// <summary>Configure portfolio size</summary>
        /// <returns>Entered value</returns>
        public static double Portfolio()
        {
            Console.WriteLine("Enter Portfolio size:");
            var ans = double.TryParse(Console.ReadLine(), out double size);

            while (!ans)
            {
                Console.WriteLine("Please enter a valid number:");
                ans = double.TryParse(Console.ReadLine(), out size);
            }

            return size;
        }

        public static string? Secret()
        {
            using var file = File.OpenRead("../../../../tk.env");
            Env.Load(file);
            return Environment.GetEnvironmentVariable("IEX_CLOUD_TOKEN");
        }

        static void Main() { }
    }
}