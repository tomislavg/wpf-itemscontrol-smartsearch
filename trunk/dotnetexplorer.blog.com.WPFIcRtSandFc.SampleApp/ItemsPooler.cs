// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    public delegate void AddItemHandler(StrategyAdapter obj);

    public delegate void RemoveItemHandler(StrategyAdapter obj);

    public delegate void AddLogEntry(string message);

    /// <summary>
    /// The items pooler class. This is just a utility class thus code could not be optimal
    /// </summary>
    public class ItemsPooler
    {
        private readonly List<StrategyAdapter> _objectCache;
        private readonly Random _random = new Random();
        private readonly object _sync = new object();
        private readonly Dispatcher _uiDispatcher;
        private readonly List<string> _productsReferential;
        private Timer _objectAddTimer;
        private Timer _objectPropertyChangedTimer;
        private Timer _objectRemoveTimer;
        private readonly List<string> _lstMarkets;
        private List<StrategyStatus> strategyStatusReferential;
        private List<StrategyType> strategyTypesReferential;
        private List<Direction> directionReferential;

        public ItemsPooler(Dispatcher uiDispatcher)
        {
            _uiDispatcher = uiDispatcher;
            _objectCache = new List<StrategyAdapter>();
            _productsReferential = new List<string>
                                      {
                                          "EUR/USD",
                                          "EUR/CHF",
                                          "EUR/GBP",
                                          "EUR/NOK",
                                          "EUR/JPY",
                                      };

            _lstMarkets = new List<string>
                                 {
                                     "EBS",
                                     "REUTERS",
                                     "AUTHOBAN",
                                     "BLOOMBERG",
                                     "HSBC"
                                 };

            strategyStatusReferential = new List<StrategyStatus>();

            var ssrtmp = Enum.GetNames(typeof(StrategyStatus));

            foreach (var s in ssrtmp)
            {
                strategyStatusReferential.Add((StrategyStatus)Enum.Parse(typeof(StrategyStatus), s));
            }

            strategyTypesReferential = new List<StrategyType>();

            var sttmp = Enum.GetNames(typeof (StrategyType));

            foreach (var s in sttmp)
            {
                strategyTypesReferential.Add((StrategyType)Enum.Parse(typeof(StrategyType), s));
            }

            directionReferential = new List<Direction>();

            var dirtmp = Enum.GetNames(typeof(Direction));

            foreach (var s in dirtmp)
            {
                directionReferential.Add((Direction)Enum.Parse(typeof(Direction), s));
            }
        }

        public List<string> LstMarkets
        {
            get { return _lstMarkets; }
        }

        public List<string> ProductsReferential
        {
            get { return _productsReferential; }
        }

        public List<StrategyStatus> StrategyStatusReferential
        {
            get { return strategyStatusReferential; }
        }

        public List<StrategyType> StrategyTypesReferential
        {
            get { return strategyTypesReferential; }
        }

        public List<Direction> DirectionReferential
        {
            get { return directionReferential; }
        }

        public event AddLogEntry AddLog;

        public void InvokeAddLog(string message)
        {
            AddLogEntry handler = AddLog;
            if (handler != null) handler(message);
        }

        public event AddItemHandler AddItem;
        public event RemoveItemHandler RemoveItem;

        public void InvokeRemoveItem(StrategyAdapter obj)
        {
            lock (_sync)
            {
                _objectCache.Remove(obj);
                Debug.WriteLine("Removing item");
                RemoveItemHandler handler = RemoveItem;
                if (handler != null) handler(obj);
            }
        }

        public void InvokeAddItem(StrategyAdapter obj)
        {
            lock (_sync)
            {
                _objectCache.Add(obj);
                Debug.WriteLine("Adding item");
                AddItemHandler handler = AddItem;
                if (handler != null) handler(obj);
            }
        }


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
        public IEnumerable<StrategyAdapter> GetStrategiesSourceScopeOne()
        {
            return new ObservableCollection<StrategyAdapter>
                       {
                           new StrategyAdapter
                               {
                                   SendTime = new DateTime(11, 11, 12, 14, 23, 56, 896),
                                   StratStat = StrategyStatus.Done,
                                   StratType = StrategyType.IOCSweep,
                                   Dir = Direction.Sell,
                                   Message = "Empty message",
                                   Product = "USD/JPY",
                                   ExecutedAmount = 458000,
                                   RequestedAmount = 500000,
                                   ExecutedPrice = 1.5m,
                                   RequestedPrice = 1.3m,
                                   Markets = "EBS,AUTHOBAN"
                               },
                           new StrategyAdapter
                               {
                                   SendTime = new DateTime(11, 11, 12, 14, 28, 36, 196),
                                   StratStat = StrategyStatus.Cancelled,
                                   StratType = StrategyType.SimpleOrder,
                                   Dir = Direction.Buy,
                                   Message = "Custom test message",
                                   Product = "EUR/USD",
                                   ExecutedAmount = 999099,
                                   RequestedAmount = 1000001,
                                   ExecutedPrice = 0.568m,
                                   RequestedPrice = 0.8956m,
                                   Markets = "EBS"
                               },
                           new StrategyAdapter
                               {
                                   SendTime = new DateTime(11, 12, 2, 9, 13, 0, 0),
                                   StratStat = StrategyStatus.InError,
                                   StratType = StrategyType.GTCSweep,
                                   Dir = Direction.Buy,
                                   Message = string.Empty,
                                   Product = "EUR/GBP",
                                   ExecutedAmount = 500000,
                                   RequestedAmount = 500000,
                                   ExecutedPrice = 0.8968m,
                                   RequestedPrice = 0.8888m,
                                   Markets = "EBS,ATS BROKER"
                               },
                           new StrategyAdapter
                               {
                                   SendTime = new DateTime(11, 10, 23, 19, 28, 56, 596),
                                   StratStat = StrategyStatus.Done,
                                   StratType = StrategyType.IOCSweep,
                                   Dir = Direction.Buy,
                                   Message = "Alert message from strategy",
                                   Product = "EUR/USD",
                                   ExecutedAmount = 800000,
                                   RequestedAmount = 750000,
                                   ExecutedPrice = 1.32m,
                                   RequestedPrice = 1.38m,
                                   Markets = "AUTHOBAN"
                               },
                       };
        }

        private StrategyAdapter CreateObject()
        {
            var a = new StrategyAdapter
                        {
                            SendTime = DateTime.Now,
                            StratStat = StrategyStatus.InProgress,
                            StratType = GetStrategyTypeByRandomNumber(_random.Next(0, 3)),
                            Dir = GetDirectionByRandomNumber(_random.Next(0, 2)),
                            Message = "message " + _random.Next(0, 1000),
                            Product = GetProduct(_random.Next(0, 5)),
                            ExecutedAmount = 0,
                            RequestedAmount = _random.Next(100000, 50000000),
                            ExecutedPrice = 0,
                            RequestedPrice = (decimal)_random.NextDouble() + _random.Next(0, 24),
                            Markets = GetMarkets(_random.Next(0, 5))
                        };
            return a;
        }

        private string GetMarkets(int random)
        {
            return _lstMarkets[random];
        }

        /// <summary>
        /// The get string source scope one.
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerable<string> GetStringSourceScopeOne()
        {
            var strLst = new List<string>();

            for (int i = 0; i < 10000; i++)
            {
                var chrs = new char[10];
                for (int j = 0; j < 10; j++)
                {
                    chrs[j] = Convert.ToChar(_random.Next(97, 122));
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
        public IEnumerable<int> GetIntSourceScopeOne()
        {
            var intLst = new List<int>();


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
        private string GetProduct(int rnd)
        {
            return _productsReferential[rnd];
        }


        public void StopPooling()
        {
            if (_objectAddTimer != null) _objectAddTimer.Dispose();
            if (_objectRemoveTimer != null) _objectRemoveTimer.Dispose();
            if (_objectPropertyChangedTimer != null) _objectPropertyChangedTimer.Dispose();

            InvokeAddLog("----- ITEM POOLING STOPED -----");
        }

        private void AddObject(object state)
        {
            StrategyAdapter obj = CreateObject();
            InvokeAddItem(obj);
            InvokeAddLog(string.Format("Item added : {0}", obj));
            _objectAddTimer.Change(GetAddTimerDelay(), Timeout.Infinite);
        }

        public void StartPooling()
        {
            _objectAddTimer = new Timer(AddObject, null, GetAddTimerDelay(), Timeout.Infinite);

            _objectRemoveTimer = new Timer(RemoveObject, null, GetRemoveTimerDelay(), Timeout.Infinite);

            _objectPropertyChangedTimer = new Timer(ObjectPropertyChanged, null, GetPropertyChangedTimerDelay(),
                                                    Timeout.Infinite);

            InvokeAddLog("----- ITEM POOLING STARTED -----");
        }

        private void ObjectPropertyChanged(object state)
        {
            lock (_sync)
            {
                List<StrategyAdapter> progressItems =
                    _objectCache.Where(p => p.StratStat == StrategyStatus.InProgress).ToList();

                var newStatus = (StrategyStatus)_random.Next(1, 4);

                int count = progressItems.Count();
                if (count > 0)
                {
                    _uiDispatcher.Invoke(new Action(delegate
                                                        {
                                                            int propChangedIndex = _random.Next(0,
                                                                                                count -
                                                                                                1);
                                                            Debug.WriteLine("Changing item properties");
                                                            StrategyAdapter changedItem =
                                                                progressItems[propChangedIndex];
                                                            InvokeAddLog(
                                                                string.Format("Item status changed from {0} to {1}",
                                                                              changedItem.StratStat, newStatus));
                                                            changedItem.StratStat = newStatus;
                                                            changedItem.Message = "Status changed !";
                                                            if (newStatus == StrategyStatus.Done)
                                                            {
                                                                int sign01 = _random.Next(1, 2) == 1
                                                                                 ? -1
                                                                                 : 1;
                                                                int sign02 = _random.Next(1, 2) == 1
                                                                                 ? -1
                                                                                 : 1;
                                                                changedItem.ExecutedAmount =
                                                                    changedItem.RequestedAmount +
                                                                    (sign01 * _random.Next(1000, 50000));
                                                                changedItem.ExecutedPrice =
                                                                    changedItem.RequestedPrice +
                                                                    (sign02 *
                                                                     (decimal)_random.NextDouble());
                                                            }
                                                        }), null);
                }


                _objectPropertyChangedTimer.Change(GetPropertyChangedTimerDelay(), Timeout.Infinite);
            }
        }

        private int GetRemoveTimerDelay()
        {
            return _random.Next(15000, 25000);
        }

        private int GetPropertyChangedTimerDelay()
        {
            return _random.Next(500, 3000);
        }

        private void RemoveObject(object state)
        {
            lock (_sync)
            {
                List<StrategyAdapter> nonProgressItems =
                    _objectCache.Where(p => p.StratStat != StrategyStatus.InProgress).ToList();

                int count = nonProgressItems.Count();
                if (count > 0)
                {
                    int removeIndex = _random.Next(0, count - 1);

                    StrategyAdapter objToRemove = nonProgressItems[removeIndex];
                    InvokeRemoveItem(objToRemove);
                    InvokeAddLog(string.Format("Item removed : {0}", objToRemove));
                }
                _objectRemoveTimer.Change(GetRemoveTimerDelay(), Timeout.Infinite);
            }
        }

        private int GetAddTimerDelay()
        {
            return _random.Next(100, 5000);
        }

        internal void InsertManualItem(StrategyAdapter manualItem)
        {
            if (manualItem == null) throw new ArgumentNullException("manualItem");
            manualItem.SendTime = DateTime.Now;
            manualItem.IsManual = true;

            InvokeAddItem(manualItem.Clone());
            InvokeAddLog(string.Format("Item added : {0}", manualItem));
        }

        public void Clear()
        {
            lock (_sync)
            {
                _objectCache.Clear(); 
            }
        }
    }
}