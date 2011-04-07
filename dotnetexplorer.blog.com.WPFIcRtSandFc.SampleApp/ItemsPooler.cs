using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    public class ItemsPooler
    {

        public ObservableCollection<MarketAdapter> GetMarketsSourceScopeOne()
        {
            return new ObservableCollection<MarketAdapter>
                        {
                            new MarketAdapter{ Liquidity=525680.55, Name="EBS", Type= MarketType.Institutional},
                             new MarketAdapter{ Liquidity=1235056.8978, Name="REUTERS", Type= MarketType.Institutional},
                              new MarketAdapter{ Liquidity=12354.7, Name="AUTHOBAN", Type= MarketType.Corporate},
                               new MarketAdapter{ Liquidity=1000000.45, Name="ATS BROKER", Type= MarketType.Institutional},
                        };
        }

        public ObservableCollection<StrategyAdapter> GetStrategiesSourceScopeOne()
        {
            return new ObservableCollection<StrategyAdapter>
                       {
                           new StrategyAdapter()
                               {
                                   SendTime = "22:11:12",
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
                           new StrategyAdapter()
                               {
                                   SendTime = "12:05:25",
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
                           new StrategyAdapter()
                               {
                                   SendTime = "18:56:03",
                                   StratStat = StrategyStatus.InError,
                                   StratType = StrategyType.GTCSweep,
                                   Dir = Direction.Buy,
                                   Message = "",
                                   Product = "EUR/GBP",
                                   Amount = 500000,
                                   RequestedAmount = 500000,
                                   Price = 0.8968m,
                                   RequestedPrice = 0.8888m,
                                   Markets = "EBS,ATS BROKER"
                               },
                           new StrategyAdapter()
                               {
                                   SendTime = "08:41:23",
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

        public List<string> GetStringSourceScopeOne()
        {
            var strLst = new List<string>();
            Random rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                
                char[] chrs = new char[10];
                for (int j = 0; j < 10; j++)
                {
                    chrs[j] = Convert.ToChar(rnd.Next(97, 122));
                }
                string str = new string(chrs);
                strLst.Add(str);
            }

            return strLst;
        }

        public List<int> GetIntSourceScopeOne()
        {
            var intLst = new List<int>();
            Random rnd = new Random();

            for (int i = 0; i < 50; i++)
            {
                intLst.Add(i);

            }

            return intLst;
        }

        StrategyStatus GetStrategyStatusByRandomNumber(int rnd)
        {
            return (StrategyStatus)rnd;
        }
        StrategyType GetStrategyTypeByRandomNumber(int rnd)
        {
            return (StrategyType)rnd;
        }
        Direction GetDirectionByRandomNumber(int rnd)
        {
            return (Direction)rnd;
        }

        string ReturnProduct(int rnd)
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
