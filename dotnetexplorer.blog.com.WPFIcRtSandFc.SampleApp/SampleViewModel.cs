using System.Collections.ObjectModel;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
   
    class SampleViewModel
    {
        ItemsPooler itemsPooler;

        public SampleViewModel()
        {
            itemsPooler = new ItemsPooler();
            DataSourceStrategies = itemsPooler.GetStrategiesSourceScopeOne();
            DataSourceMarkets = itemsPooler.GetMarketsSourceScopeOne();
        }


        public ObservableCollection<StrategyAdapter> DataSourceStrategies { get; set; }
        public ObservableCollection<MarketAdapter> DataSourceMarkets { get; set; }

    }
}
