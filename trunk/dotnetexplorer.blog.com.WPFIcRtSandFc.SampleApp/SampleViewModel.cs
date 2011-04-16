// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    /// <summary>
    /// The sample view model.
    /// </summary>
    internal class SampleViewModel
    {
        /// <summary>
        ///   The items pooler.
        /// </summary>
        private readonly ItemsPooler itemsPooler;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SampleViewModel" /> class.
        /// </summary>
        public SampleViewModel()
        {
            itemsPooler = new ItemsPooler();
            DataSourceStrategies = new List<StrategyAdapter>(itemsPooler.GetStrategiesSourceScopeOne());
            DataSourceMarkets = itemsPooler.GetMarketsSourceScopeOne();
            DataSourceString = itemsPooler.GetStringSourceScopeOne();
            DataSourceInt = itemsPooler.GetIntSourceScopeOne();
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
    }
}