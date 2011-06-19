// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    /// <summary>
    /// The sample view model.
    /// </summary>
    internal class SampleViewModel : INotifyPropertyChanged
    {
        private readonly RelayCommand startPooling;
        private readonly RelayCommand _stopPooling;
        /// <summary>
        ///   The items pooler.
        /// </summary>
        private readonly ItemsPooler itemsPooler;

        private Dispatcher _uiDispatcher;
        private RelayCommand _clearData;
        private bool isPooling;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SampleViewModel" /> class.
        /// </summary>
        public SampleViewModel(Dispatcher dispatcher)
        {
            _uiDispatcher = dispatcher;
            itemsPooler = new ItemsPooler(_uiDispatcher);
            itemsPooler.AddItem += ItemsPoolerAddItem;
            itemsPooler.RemoveItem += ItemsPoolerRemoveItem;
            itemsPooler.AddLog += ItemsPoolerAddLog;
            DataSourceStrategies = new List<StrategyAdapter>(itemsPooler.GetStrategiesSourceScopeOne());
            DataSourceMarkets = itemsPooler.GetMarketsSourceScopeOne();
            DataSourceString = itemsPooler.GetStringSourceScopeOne();
            DataSourceInt = itemsPooler.GetIntSourceScopeOne();
            startPooling = new RelayCommand(ExecuteStartPooling);
            _stopPooling = new RelayCommand(ExecuteStopPooling);
            _clearData = new RelayCommand(ExecuteClearData);
            _insertItem = new RelayCommand(ExecuteInsertItem);
            RealTimeDataSourceStrategies = new ObservableCollection<StrategyAdapter>();
            InvokePropertyChanged(new PropertyChangedEventArgs("LstMarkets"));
            InvokePropertyChanged(new PropertyChangedEventArgs("ProductsReferential"));
            InvokePropertyChanged(new PropertyChangedEventArgs("StrategyStatusReferential"));
            InvokePropertyChanged(new PropertyChangedEventArgs("StrategyTypesReferential"));
            InvokePropertyChanged(new PropertyChangedEventArgs("DirectionReferential"));
            InvokePropertyChanged(new PropertyChangedEventArgs("ManualItem"));


        }

        private void ExecuteInsertItem(object obj)
        {
            itemsPooler.InsertManualItem(ManualItem);
        }

        void ItemsPoolerAddLog(string message)
        {
            _uiDispatcher.Invoke(new Action<string>(DoLog), message);
        }

        private void DoLog(string obj)
        {
            Logs = Logs += string.Format("{0}{1}", Environment.NewLine, obj);
        }

        void ItemsPoolerRemoveItem(StrategyAdapter obj)
        {
            _uiDispatcher.Invoke(new Action<StrategyAdapter>(RemoveItem), obj);
        }

        private void RemoveItem(StrategyAdapter obj)
        {
            RealTimeDataSourceStrategies.Remove(obj);
        }

        private void ExecuteClearData(object obj)
        {
            if (!isPooling)
            {
                itemsPooler.Clear();
                var count = RealTimeDataSourceStrategies.Count;
                for (int i = count - 1; i > -1; i--)
                {
                    RealTimeDataSourceStrategies.RemoveAt(i);
                }
                Logs = string.Empty;
            }

           
           
        }

        void ItemsPoolerAddItem(StrategyAdapter obj)
        {

            _uiDispatcher.Invoke(new Action<StrategyAdapter>(AddItem), obj);
        }

        void AddItem(StrategyAdapter strategyAdapter)
        {
            RealTimeDataSourceStrategies.Add(strategyAdapter);
        }

        public RelayCommand StartPooling
        {
            get { return startPooling; }
        }

        public RelayCommand StopPooling
        {
            get { return _stopPooling; }
        }

        public RelayCommand ClearData
        {
            get { return _clearData; }
        }

        public RelayCommand InsertItem
        {
            get { return _insertItem; }
        }

        private void ExecuteStopPooling(object obj)
        {
            itemsPooler.StopPooling();
            isPooling = false;
        }

        private void ExecuteStartPooling(object obj)
        {
            isPooling = true;
            itemsPooler.StartPooling();
        }

        // 2 types of collections to demonstrate that static filter works on both types of collections
        // However, the first one won't notify smart search when receiving or removing items
        /// <summary>
        ///   Gets or sets DataSourceStrategies.
        /// </summary>
        public List<StrategyAdapter> DataSourceStrategies { get; set; }

        /// <summary>
        ///   Gets or sets DataSourceMarkets.
        /// </summary>
        public ObservableCollection<MarketAdapter> DataSourceMarkets { get; set; }


        /// <summary>
        ///   Gets or sets DataSourceString.
        /// </summary>
        public IEnumerable<string> DataSourceString { get; set; }

        /// <summary>
        ///   Gets or sets DataSourceInt.
        /// </summary>
        public IEnumerable<int> DataSourceInt { get; set; }

        public ObservableCollection<StrategyAdapter> RealTimeDataSourceStrategies { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }


        private string _logs;
        public string Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Logs"));
            }
        }

        private StrategyAdapter _manualItem;
        private RelayCommand _insertItem;

        public StrategyAdapter ManualItem
        {
            get
            {
                return _manualItem ?? (_manualItem = new StrategyAdapter());
            }
            set { _manualItem = value; InvokePropertyChanged(new PropertyChangedEventArgs("ManualItem")); }
        }

        public List<string> LstMarkets
        {
            get { return itemsPooler.LstMarkets; }
        }

        public List<string> ProductsReferential
        {
            get { return itemsPooler.ProductsReferential; }
        }

        public List<StrategyStatus> StrategyStatusReferential
        {
            get { return itemsPooler.StrategyStatusReferential; }
        }

        public List<StrategyType> StrategyTypesReferential
        {
            get { return itemsPooler.StrategyTypesReferential; }
        }

        public List<Direction> DirectionReferential
        {
            get { return itemsPooler.DirectionReferential; }
        }
    }
}