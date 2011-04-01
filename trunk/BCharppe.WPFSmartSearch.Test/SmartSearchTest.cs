//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Diagnostics;
//using XHedge.Client.Tests;
//using System.Linq;

//namespace WPFCommons.Test
//{
//    /// <summary>
//    /// Smart Search Custom control test class
//    /// In order to instantiate the custom control without getting an thread exception
//    /// test must be run under STA appartement and Nunit must be configured accordingly (See app.config
//    /// </summary>
//    [TestFixture]
//    public class SmartSearchTest
//    {
//        private SmartSearchCc sscc;
//        private ObservableCollection<ColumnBase> colcol;
//        private ObservableCollection<StrategyAdapterMock> source;
//        private ICollectionView view;
//        private int ITEM_NUMBER = 50000;

//        //You need do disable the delayed filter in the smart search compoenent in order for the following tests to work


//        [SetUp]
//        public void SmartSearchSetup()
//        {
//            //Simulate blotter columns collections
//            colcol = GetColumnsScopeOne();

//            //Simulate StrategyAdapter source collection
//            source = GetSourceScopeOne();

//            //Simulate datagrid collection view
//            view = CollectionViewSource.GetDefaultView(source);


//        }


//        [Test]
//        public void InstantiateComponent()
//        {
//            var runner = new CrossThreadTestRunner();
//            runner.RunInSta(InstantiateSmartSearch);
//        }
//        private void InstantiateSmartSearch()
//        {

//            Random rnd = new Random();
//            ObservableCollection<StrategyAdapterMock> source

//                = GetSourceScopeOne();

//            //    = new ObservableCollection<StrategyAdapterMock>();
//            //int i = 0;
//            //for (i = 0; i < ITEM_NUMBER; i++)
//            //{
//            //    source.Add(
//            //        new StrategyAdapterMock
//            //            {
//            //                Amount = rnd.Next() * 50000,
//            //                Dir = GetDirectionByRandomNumber(rnd.Next(0, 1)),
//            //                Markets = "A,B,C" + rnd.Next(),
//            //                Message = "Message " + rnd.Next(),
//            //                Price = (decimal)(rnd.NextDouble() + 1),
//            //                Product = ReturnProduct(rnd.Next(1, 5)),
//            //                RequestedAmount = rnd.Next() * 50000,
//            //                RequestedPrice = (decimal)(rnd.NextDouble() + 1),
//            //                SendTime = DateTime.Now.AddMilliseconds(2).ToString(),
//            //                StratStat = GetStrategyStatusByRandomNumber(rnd.Next(1, 4)),
//            //                StratType = GetStrategyTypeByRandomNumber(rnd.Next(1, 3))

//            //            });
//            //}
//            //Debug.WriteLine("Finished building " + i + " items. Each item contains 11 properties of different kinds.");
//            var dgc = new DataGridControl();
//            var cols = GetColumnsScopeOne();
//            Debug.WriteLine("Finished building datagridcontrol columns. " + cols.Count + " columns added. All visible.");
//            foreach (var columnBase in cols)
//            {
//                dgc.Columns.Add(columnBase);
//            }

//            var sw = Stopwatch.StartNew();
//            sw.Start();

//            SmartSearchScope sss1 = new SmartSearchScope { DataControl = dgc };
//            sss1.UnderlyingType = typeof(StrategyAdapterMock);
//            //sss1.PropertyFilters = GetPropertyFilters();
//            SmartSearchCc ssc = new SmartSearchCc();
//            ssc.Items.Add(sss1);
//            sss1.ItemsSource = GetPropertyFilters();
//            //int results = 0;

//            //Setting item source will trigger the cache initialization
//            dgc.ItemsSource = new DataGridCollectionView(source, typeof(StrategyAdapterMock));
//            //Assert.AreEqual(ITEM_NUMBER, results);

//            sw.Stop();
//            Debug.WriteLine("Cache initialization Duration : " + sw.Elapsed.TotalMilliseconds);

//            //results = 0;
//            string finalInput = "jpy";
//            string incommingInput = string.Empty;
//            var times = new List<int>();

//            //foreach (var c in finalInput)
//            //{

//            incommingInput += finalInput;
//            sw.Start();
//            ssc.FilterTextChanged(incommingInput);
//            sw.Stop();
//            times.Add(sw.Elapsed.Milliseconds);
//            Debug.WriteLine("|_> " + incommingInput + " => Filter Input duration: " + sw.Elapsed.Milliseconds + " for " + sss1.Res + " results" + Environment.NewLine);
//            //results = 0;
//            //}

//            Debug.WriteLine("Mean filter execution time : " + times.Average());
//            //times.Clear();
//            //results = 0;
//            //finalInput = "done";
//            //incommingInput = string.Empty;


//            ////foreach (var c in finalInput)
//            ////{

//            //incommingInput += finalInput;
//            //sw.Start();
//            //ssc.FilterTextChanged(incommingInput);
//            //sw.Stop();
//            //times.Add(sw.Elapsed.Milliseconds);
//            //Debug.WriteLine(incommingInput + " => Filter Input duration: " + sw.Elapsed.Milliseconds + " for " + results + " results");
//            //results = 0;
//            ////}
//            //Debug.WriteLine("Mean filter execution time : " + times.Average());

//            //Timer t = new Timer(0.200);

//            //t.Elapsed += delegate
//            //                 {
//            //                     sw.Start();
//            //                     source.Add(
//            //                                         new StrategyAdapterMock
//            //                                         {
//            //                                             Amount = rnd.Next() * 50000,
//            //                                             Dir = GetDirectionByRandomNumber(rnd.Next(0, 1)),
//            //                                             Markets = "A,B,C" + rnd.Next(),
//            //                                             Message = "Message " + rnd.Next(),
//            //                                             Price = (decimal)(rnd.NextDouble() + 1),
//            //                                             Product = ReturnProduct(rnd.Next(1, 5)),
//            //                                             RequestedAmount = rnd.Next() * 50000,
//            //                                             RequestedPrice = (decimal)(rnd.NextDouble() + 1),
//            //                                             SendTime = DateTime.Now.AddMilliseconds(2).ToString(),
//            //                                             StratStat = GetStrategyStatusByRandomNumber(rnd.Next(1, 4)),
//            //                                             StratType = GetStrategyTypeByRandomNumber(rnd.Next(1, 3))

//            //                                         });
//            //                     sw.Stop();
//            //                     Debug.WriteLine(finalInput + " => Filter Input duration when adding new item: " + sw.Elapsed.Milliseconds + " for " + results + " results");

//            //                     if (source.Count > ITEM_NUMBER + 80)
//            //                         t.Stop();
//            //                 };
//            //t.Start();
//        }



//        private static ObservableCollection<ColumnBase> GetColumnsScopeOne()
//        {
//            return new ObservableCollection<ColumnBase>
//                       {
//                           new Column("SendTime", "Send time", null),
//                           new Column("StratStat", "Status", null),
//                           new Column("StratType", "Strategy type", null),
//                           new Column("Dir", "Direction", null),
//                           new Column("Message", "Message", null),
//                           new Column("Product", "Product", null),
//                           new Column("Amount", "Amount", null),
//                           new Column("RequestedAmount", "Requested amount", null),
//                           new Column("Price", "Price", null),
//                           new Column("RequestedPrice", "Requested Price", null),
//                           new Column("Markets", "Markets", null),
//                       };
//        }

//        private List<PropertyFilter> GetPropertyFilters()
//        {
//            var lst = new List<PropertyFilter>
//                       {
//                           new PropertyFilter("SendTime"),
//                           new PropertyFilter("StratStat",true),
//                           new PropertyFilter("StratType"),
//                           new PropertyFilter("Dir"),
//                           new PropertyFilter("Message"),
//                           new PropertyFilter("Product"),
//                           new PropertyFilter("Amount",true),
//                           new PropertyFilter("RequestedAmount"),
//                           new PropertyFilter("Price",true),
//                           new PropertyFilter("RequestedPrice"),
//                           new PropertyFilter("Markets"),
//                       };

//            return lst;
//        }

//        private ObservableCollection<StrategyAdapterMock> GetSourceScopeOne()
//        {
//            return new ObservableCollection<StrategyAdapterMock>
//                       {
//                           new StrategyAdapterMock()
//                               {
//                                   SendTime = "22:11:12",
//                                   StratStat = StrategyStatus.Done,
//                                   StratType = StrategyType.IOCSweep,
//                                   Dir = Direction.Sell,
//                                   Message = "Empty message",
//                                   Product = "USD/JPY",
//                                   Amount = 458000,
//                                   RequestedAmount = 500000,
//                                   Price = 1.5m,
//                                   RequestedPrice = 1.3m,
//                                   Markets = "EBS,AUTHOBAN"
//                               },
//                           new StrategyAdapterMock()
//                               {
//                                   SendTime = "12:05:25",
//                                   StratStat = StrategyStatus.Cancelled,
//                                   StratType = StrategyType.SimpleOrder,
//                                   Dir = Direction.Buy,
//                                   Message = "Custom test message",
//                                   Product = "EUR/USD",
//                                   Amount = 999099,
//                                   RequestedAmount = 1000001,
//                                   Price = 0.568m,
//                                   RequestedPrice = 0.8956m,
//                                   Markets = "EBS"
//                               },
//                           new StrategyAdapterMock()
//                               {
//                                   SendTime = "18:56:03",
//                                   StratStat = StrategyStatus.InError,
//                                   StratType = StrategyType.GTCSweep,
//                                   Dir = Direction.Buy,
//                                   Message = "",
//                                   Product = "EUR/GBP",
//                                   Amount = 500000,
//                                   RequestedAmount = 500000,
//                                   Price = 0.8968m,
//                                   RequestedPrice = 0.8888m,
//                                   Markets = "EBS,ATS BROKER"
//                               },
//                           new StrategyAdapterMock()
//                               {
//                                   SendTime = "08:41:23",
//                                   StratStat = StrategyStatus.Done,
//                                   StratType = StrategyType.IOCSweep,
//                                   Dir = Direction.Buy,
//                                   Message = "Alert message from strategy",
//                                   Product = "EUR/USD",
//                                   Amount = 800000,
//                                   RequestedAmount = 750000,
//                                   Price = 1.32m,
//                                   RequestedPrice = 1.38m,
//                                   Markets = "AUTHOBAN"
//                               },
//                       };
//        }

//        StrategyStatus GetStrategyStatusByRandomNumber(int rnd)
//        {
//            return (StrategyStatus)rnd;
//        }
//        StrategyType GetStrategyTypeByRandomNumber(int rnd)
//        {
//            return (StrategyType)rnd;
//        }
//        Direction GetDirectionByRandomNumber(int rnd)
//        {
//            return (Direction)rnd;
//        }

//        string ReturnProduct(int rnd)
//        {
//            if (rnd == 1)
//            {
//                return "EUR/USD";
//            }
//            if (rnd == 2)
//            {
//                return "EUR/CHF";
//            }
//            if (rnd == 3)
//            {
//                return "EUR/GBP";
//            }
//            if (rnd == 4)
//            {
//                return "EUR/NOK";
//            }

//            return "EUR/JPY";

//        }

//    }


//    internal class StrategyAdapterMock
//    {
//        public string SendTime { get; set; }
//        public StrategyStatus StratStat { get; set; }
//        public StrategyType StratType { get; set; }
//        public Direction Dir { get; set; }
//        public string Message { get; set; }
//        public string Product { get; set; }
//        public long Amount { get; set; }
//        public long RequestedAmount { get; set; }
//        public decimal Price { get; set; }
//        public decimal RequestedPrice { get; set; }
//        public string Markets { get; set; }

//        public override string ToString()
//        {
//            return string.Format("SendTime: {0}, StratStat: {1}, StratType: {2}, Dir: {3}, Product: {4}", SendTime, StratStat, StratType, Dir, Product);
//        }
//    }



//    enum StrategyStatus
//    {
//        InProgress,
//        Done,
//        InError,
//        Cancelled,
//    }

//    enum StrategyType
//    {
//        IOCSweep,
//        GTCSweep,
//        SimpleOrder,
//    }

//    enum Direction
//    {
//        Buy,
//        Sell
//    }
//}