using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
   
    class SampleViewModel
    {
        ItemsPooler itemsPooler;

        public SampleViewModel()
        {
            itemsPooler = new ItemsPooler();
            DataSourceStrategies = new List<StrategyAdapter>(itemsPooler.GetStrategiesSourceScopeOne());
            DataSourceMarkets = itemsPooler.GetMarketsSourceScopeOne();
        }

        //2 types of collections to demonstrate that static filter works on both types of collections
        //However, the first one won't notify smart search when receiving or removing items
        public List<StrategyAdapter> DataSourceStrategies { get; set; }
        public ObservableCollection<MarketAdapter> DataSourceMarkets { get; set; }

    }
}
