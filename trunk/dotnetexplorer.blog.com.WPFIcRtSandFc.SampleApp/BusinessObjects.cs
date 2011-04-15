using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    public class StrategyAdapter
    {
        public string SendTime { get; set; }
        public StrategyStatus StratStat { get; set; }
        public StrategyType StratType { get; set; }
        public Direction Dir { get; set; }
        public string Message { get; set; }
        public string Product { get; set; }
        public long Amount { get; set; }
        public long RequestedAmount { get; set; }
        public decimal Price { get; set; }
        public decimal RequestedPrice { get; set; }
        public string Markets { get; set; }

        public override string ToString()
        {
            return string.Format("SendTime: {0}, StratStat: {1}, StratType: {2}, Dir: {3}, Product: {4}", SendTime, StratStat, StratType, Dir, Product);
        }
    }


    public enum StrategyStatus
    {
        InProgress,
        Done,
        InError,
        Cancelled,
    }

    public enum StrategyType
    {
        IOCSweep,
        GTCSweep,
        SimpleOrder,
    }

    public enum Direction
    {
        Buy,
        Sell
    }

    public enum MarketType
    {
        Institutional,
        Corporate,
    }
    public class MarketAdapter
    {
        public string Name { get; set; }
        public double Liquidity { get; set; }
        public MarketType Type { get; set; }
    }
}
