using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFCommons.Test
{
    public class StrategyAdapterMock
    {
        public StrategyAdapterMock()
        {
            
        }
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
        public bool Agressive { get; set; }
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
}
