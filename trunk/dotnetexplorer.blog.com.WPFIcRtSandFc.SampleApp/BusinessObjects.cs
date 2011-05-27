// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    /// <summary>
    /// The strategy adapter.
    /// </summary>
    public class StrategyAdapter
    {
        /// <summary>
        ///   Gets or sets SendTime.
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        ///   Gets or sets StratStat.
        /// </summary>
        public StrategyStatus StratStat { get; set; }

        /// <summary>
        ///   Gets or sets StratType.
        /// </summary>
        public StrategyType StratType { get; set; }

        /// <summary>
        ///   Gets or sets Dir.
        /// </summary>
        public Direction Dir { get; set; }

        /// <summary>
        ///   Gets or sets Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///   Gets or sets Product.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        ///   Gets or sets Amount.
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        ///   Gets or sets RequestedAmount.
        /// </summary>
        public long RequestedAmount { get; set; }

        /// <summary>
        ///   Gets or sets Price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///   Gets or sets RequestedPrice.
        /// </summary>
        public decimal RequestedPrice { get; set; }

        /// <summary>
        ///   Gets or sets Markets.
        /// </summary>
        public string Markets { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            return string.Format("SendTime: {0}, StratStat: {1}, StratType: {2}, Dir: {3}, Product: {4}", SendTime, 
                                 StratStat, StratType, Dir, Product);
        }
    }


    /// <summary>
    /// The strategy status.
    /// </summary>
    public enum StrategyStatus
    {
        /// <summary>
        ///   The in progress.
        /// </summary>
        InProgress, 

        /// <summary>
        ///   The done.
        /// </summary>
        Done, 

        /// <summary>
        ///   The in error.
        /// </summary>
        InError, 

        /// <summary>
        ///   The cancelled.
        /// </summary>
        Cancelled, 
    }

    /// <summary>
    /// The strategy type.
    /// </summary>
    public enum StrategyType
    {
        /// <summary>
        ///   The ioc sweep.
        /// </summary>
        IOCSweep, 

        /// <summary>
        ///   The gtc sweep.
        /// </summary>
        GTCSweep, 

        /// <summary>
        ///   The simple order.
        /// </summary>
        SimpleOrder, 
    }

    /// <summary>
    /// The direction.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        ///   The buy.
        /// </summary>
        Buy, 

        /// <summary>
        ///   The sell.
        /// </summary>
        Sell
    }

    /// <summary>
    /// The market type.
    /// </summary>
    public enum MarketType
    {
        /// <summary>
        ///   The institutional.
        /// </summary>
        Institutional, 

        /// <summary>
        ///   The corporate.
        /// </summary>
        Corporate, 
    }

    /// <summary>
    /// The market adapter.
    /// </summary>
    public class MarketAdapter
    {
        /// <summary>
        ///   Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   Gets or sets Liquidity.
        /// </summary>
        public double Liquidity { get; set; }

        /// <summary>
        ///   Gets or sets Type.
        /// </summary>
        public MarketType Type { get; set; }
    }
}