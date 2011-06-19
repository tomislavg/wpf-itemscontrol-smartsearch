// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    /// <summary>
    /// The strategy adapter.
    /// </summary>
    public class StrategyAdapter : INotifyPropertyChanged
    {
        private DateTime _sendTime;

        /// <summary>
        ///   Gets or sets SendTime.
        /// </summary>
        public DateTime SendTime
        {
            get { return _sendTime; }
            set
            {
                _sendTime = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("SendTime"));
            }
        }

        private StrategyStatus _stratStat;

        /// <summary>
        ///   Gets or sets StratStat.
        /// </summary>
        public StrategyStatus StratStat
        {
            get { return _stratStat; }
            set
            {
                _stratStat = value;

                InvokePropertyChanged(new PropertyChangedEventArgs("StratStat"));
            }
        }

        private StrategyType _stratType;

        /// <summary>
        ///   Gets or sets StratType.
        /// </summary>
        public StrategyType StratType
        {
            get { return _stratType; }
            set
            {
                _stratType = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("StratType"));
            }
        }

        private Direction _dir;

        /// <summary>
        ///   Gets or sets Dir.
        /// </summary>
        public Direction Dir
        {
            get { return _dir; }
            set
            {
                _dir = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Dir"));
            }
        }

        private string _message;

        /// <summary>
        ///   Gets or sets Message.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Message"));
            }
        }

        private string _product;

        /// <summary>
        ///   Gets or sets Product.
        /// </summary>
        public string Product
        {
            get { return _product; }
            set
            {
                _product = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Product"));
            }
        }

        private long _executedAmount;

        /// <summary>
        ///   Gets or sets ExecutedAmount.
        /// </summary>
        public long ExecutedAmount
        {
            get { return _executedAmount; }
            set
            {
                _executedAmount = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("ExecutedAmount"));
            }
        }

        private long _requestedAmount;

        /// <summary>
        ///   Gets or sets RequestedAmount.
        /// </summary>
        public long RequestedAmount
        {
            get { return _requestedAmount; }
            set
            {
                _requestedAmount = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("RequestedAmount"));
            }
        }

        private decimal _executedPrice;

        /// <summary>
        ///   Gets or sets ExecutedPrice.
        /// </summary>
        public decimal ExecutedPrice
        {
            get { return _executedPrice; }
            set
            {
                _executedPrice = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("ExecutedPrice"));
            }
        }

        private decimal _requestedPrice;

        /// <summary>
        ///   Gets or sets RequestedPrice.
        /// </summary>
        public decimal RequestedPrice
        {
            get { return _requestedPrice; }
            set
            {
                _requestedPrice = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("RequestedPrice"));
            }
        }

        private string _markets;

        /// <summary>
        ///   Gets or sets Markets.
        /// </summary>
        public string Markets
        {
            get { return _markets; }
            set
            {
                _markets = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Markets"));
            }
        }

        private bool _isManual;
        public bool IsManual
        {
            get { return _isManual; }
            set
            {
                _isManual = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("IsManual"));
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            return string.Format("SendTime: {0}, StratStat: {1}, StratType: {2}, Dir: {3}, Product: {4}, IsManuel : {5}", SendTime,
                                 StratStat, StratType, Dir, Product, IsManual);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        public StrategyAdapter Clone()
        {
            return new StrategyAdapter
                       {
                           Dir = Dir,
                           IsManual = IsManual,
                           SendTime = SendTime,
                           StratStat = StratStat,
                           ExecutedAmount = ExecutedAmount,
                           RequestedAmount = RequestedAmount,
                           RequestedPrice = RequestedPrice,
                           ExecutedPrice = ExecutedPrice,
                           Message = Message,
                           Product = Product,
                           Markets = Markets,

                           StratType = StratType
                       };
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
        InProgress = 0,

        /// <summary>
        ///   The done.
        /// </summary>
        Done = 1,

        /// <summary>
        ///   The in error.
        /// </summary>
        InError = 2,

        /// <summary>
        ///   The cancelled.
        /// </summary>
        Cancelled = 3,
    }

    /// <summary>
    /// The strategy type.
    /// </summary>
    public enum StrategyType
    {
        /// <summary>
        ///   The ioc sweep.
        /// </summary>
        IOCSweep = 0,

        /// <summary>
        ///   The gtc sweep.
        /// </summary>
        GTCSweep = 1,

        /// <summary>
        ///   The simple order.
        /// </summary>
        SimpleOrder = 2,
    }

    /// <summary>
    /// The direction.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        ///   The buy.
        /// </summary>
        Buy = 0,

        /// <summary>
        ///   The sell.
        /// </summary>
        Sell = 1,
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


    public enum ExecutionMarkets
    {
        EBS, REUTERS, AUTHOBAN, BLOOMBERG, HSBC
    }


}