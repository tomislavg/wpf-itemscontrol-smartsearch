// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    /// <summary>
    /// Derived class used to be able to manage filter application when a collection item property changed
    ///   whithout having to do a refresh
    /// </summary>
    internal sealed class SmartSearchScopeObservableCollection : ObservableCollection<object>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "SmartSearchScopeObservableCollection" /> class.
        /// </summary>
        public SmartSearchScopeObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartSearchScopeObservableCollection"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public SmartSearchScopeObservableCollection(IEnumerable<object> source)
            : base(source)
        {
        }

        /// <summary>
        /// Custom Raise collection changed
        /// </summary>
        /// <param name="e">
        /// The notification action
        /// </param>
        public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }
    }
}