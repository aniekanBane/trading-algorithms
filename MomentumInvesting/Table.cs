using System;
using AlgorithmicTrading;

namespace MomentumInvesting
{
    public class HQM : StockData
    {
        public double OneYearPriceReturn { get; set; }

        public double OneYearPercentile { get; set; }

        public double SixMonthPriceReturn { get; set; }

        public double SixMonthPercentile { get; set; }

        public double ThreeMonthPriceReturn { get; set; }

        public double ThreeMonthPercentile { get; set; }

        public double OneMonthPriceReturn { get; set; }

        public double OneMonthPercentile { get; set; }

        public HQM(string ticker) : base(ticker) { }

        /// <summary>Average percentile score</summary>
        public double HQMScore => new List<double>
        {
            OneMonthPercentile,
            OneYearPercentile,
            SixMonthPercentile,
            ThreeMonthPercentile
        }.Average();

        public override string ToString()
        {
            return $"Ticker: {Ticker} | Price: ${Price} | Performance: {HQMScore:F2}th percentile | (Shares to buy): {SharesHodl}";
        }
    }
}

