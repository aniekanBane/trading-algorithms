using AlgorithmicTrading;

namespace ValueInvesting
{
    public class RQV : StockData
    {
        /// <summary>Price-to-Earning Ratio </summary>
        public double? PERatio { get; set; }

        public double PEPercentile { get; set; }

        /// <summary>Price-to-Book Ratio </summary>
        public double? PBRatio { get; set; }

        public double PBPercentile { get; set; }

        /// <summary>Price-to-Sales Ratio </summary>
        public double? PSRatio { get; set; }

        public double PSPercentile { get; set; }

        public double? EVtoEBITDA { get; set; }

        public double EVtoEBITDAPercentile { get; set; }

        public double? EVtoGP { get; set; }

        public double EVtoGPPercentile { get; set; }

        public double? EVtoRevenue { get; set; }

        public double EvtoRevenuePercentile { get; set; }

        public RQV(string ticker) : base(ticker) { }

        /// <summary>Robust value score </summary>
        public double RVScore => new List<double>()
        {
            PEPercentile,
            PBPercentile,
            PSPercentile,
            EVtoEBITDAPercentile,
            EVtoGPPercentile,
            EvtoRevenuePercentile

        }.Average();

        public override string ToString()
        {
            return $"Ticker: {Ticker} | Price: ${Price} | Performance: {RVScore:F2}th percentile | (Shares to buy): {SharesHodl}";
        }
    }
}