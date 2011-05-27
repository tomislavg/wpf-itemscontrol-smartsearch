// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    /// <summary>
    /// The items pooler.
    /// </summary>
    public class ItemsPooler
    {
        /// <summary>
        /// The get markets source scope one.
        /// </summary>
        /// <returns>
        /// </returns>
        public ObservableCollection<MarketAdapter> GetMarketsSourceScopeOne()
        {
            return new ObservableCollection<MarketAdapter>
                       {
                           new MarketAdapter {Liquidity = 525680.55, Name = "EBS", Type = MarketType.Institutional}, 
                           new MarketAdapter
                               {
                                   Liquidity = 1235056.8978, 
                                   Name = "REUTERS", 
                                   Type = MarketType.Institutional
                               }, 
                           new MarketAdapter {Liquidity = 12354.7, Name = "AUTHOBAN", Type = MarketType.Corporate}, 
                           new MarketAdapter
                               {
                                   Liquidity = 1000000.45, 
                                   Name = "ATS BROKER", 
                                   Type = MarketType.Institutional
                               }, 
                       };
        }

        /// <summary>
        /// The get strategies source scope one.
        /// </summary>
        /// <returns>
        /// </returns>
        public ObservableCollection<StrategyAdapter> GetStrategiesSourceScopeOne()
        {
            return new ObservableCollection<StrategyAdapter>
                       {
                           new StrategyAdapter
                               {
                                   SendTime =new DateTime(11,11,12,14,23,56,896), 
                                   StratStat = StrategyStatus.Done, 
                                   StratType = StrategyType.IOCSweep, 
                                   Dir = Direction.Sell, 
                                   Message = "Empty message", 
                                   Product = "USD/JPY", 
                                   Amount = 458000, 
                                   RequestedAmount = 500000, 
                                   Price = 1.5m, 
                                   RequestedPrice = 1.3m, 
                                   Markets = "EBS,AUTHOBAN"
                               }, 
                           new StrategyAdapter
                               {
                                   SendTime  =new DateTime(11,11,12,14,28,36,196), 
                                   StratStat = StrategyStatus.Cancelled, 
                                   StratType = StrategyType.SimpleOrder, 
                                   Dir = Direction.Buy, 
                                   Message = "Custom test message", 
                                   Product = "EUR/USD", 
                                   Amount = 999099, 
                                   RequestedAmount = 1000001, 
                                   Price = 0.568m, 
                                   RequestedPrice = 0.8956m, 
                                   Markets = "EBS"
                               }, 
                           new StrategyAdapter
                               {
                                   SendTime  =new DateTime(11,12,2,9,13,0,0), 
                                   StratStat = StrategyStatus.InError, 
                                   StratType = StrategyType.GTCSweep, 
                                   Dir = Direction.Buy, 
                                   Message = string.Empty, 
                                   Product = "EUR/GBP", 
                                   Amount = 500000, 
                                   RequestedAmount = 500000, 
                                   Price = 0.8968m, 
                                   RequestedPrice = 0.8888m, 
                                   Markets = "EBS,ATS BROKER"
                               }, 
                           new StrategyAdapter
                               {
                                   SendTime  =new DateTime(11,10,23,19,28,56,596), 
                                   StratStat = StrategyStatus.Done, 
                                   StratType = StrategyType.IOCSweep, 
                                   Dir = Direction.Buy, 
                                   Message = "Alert message from strategy", 
                                   Product = "EUR/USD", 
                                   Amount = 800000, 
                                   RequestedAmount = 750000, 
                                   Price = 1.32m, 
                                   RequestedPrice = 1.38m, 
                                   Markets = "AUTHOBAN"
                               }, 
                       };
        }

        /// <summary>
        /// The get string source scope one.
        /// </summary>
        /// <returns>
        /// </returns>
        public List<string> GetStringSourceScopeOne()
        {
            var strLst = new List<string>();
            var rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {
                var chrs = new char[10];
                for (int j = 0; j < 10; j++)
                {
                    chrs[j] = Convert.ToChar(rnd.Next(97, 122));
                }

                var str = new string(chrs);
                strLst.Add(str);
            }

            return strLst;
        }

        /// <summary>
        /// The get int source scope one.
        /// </summary>
        /// <returns>
        /// </returns>
        public List<int> GetIntSourceScopeOne()
        {
            var intLst = new List<int>();
            var rnd = new Random();

            for (int i = 0; i < 3000; i++)
            {
                intLst.Add(i);
            }

            return intLst;
        }

        /// <summary>
        /// The get strategy status by random number.
        /// </summary>
        /// <param name="rnd">
        /// The rnd.
        /// </param>
        /// <returns>
        /// </returns>
        private StrategyStatus GetStrategyStatusByRandomNumber(int rnd)
        {
            return (StrategyStatus)rnd;
        }

        /// <summary>
        /// The get strategy type by random number.
        /// </summary>
        /// <param name="rnd">
        /// The rnd.
        /// </param>
        /// <returns>
        /// </returns>
        private StrategyType GetStrategyTypeByRandomNumber(int rnd)
        {
            return (StrategyType)rnd;
        }

        /// <summary>
        /// The get direction by random number.
        /// </summary>
        /// <param name="rnd">
        /// The rnd.
        /// </param>
        /// <returns>
        /// </returns>
        private Direction GetDirectionByRandomNumber(int rnd)
        {
            return (Direction)rnd;
        }

        /// <summary>
        /// The return product.
        /// </summary>
        /// <param name="rnd">
        /// The rnd.
        /// </param>
        /// <returns>
        /// The return product.
        /// </returns>
        private string ReturnProduct(int rnd)
        {
            if (rnd == 1)
            {
                return "EUR/USD";
            }

            if (rnd == 2)
            {
                return "EUR/CHF";
            }

            if (rnd == 3)
            {
                return "EUR/GBP";
            }

            if (rnd == 4)
            {
                return "EUR/NOK";
            }

            return "EUR/JPY";
        }
    }
}