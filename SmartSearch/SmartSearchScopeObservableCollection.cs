#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

#endregion

namespace WPFCommons.SmartSearch
{
    /// <summary>
    ///   Derived class used to be able to manage filter application when a collection item property changed
    ///   whithout having to do a refresh
    /// </summary>
    internal sealed class SmartSearchScopeObservableCollection : ObservableCollection<object>
    {
        public SmartSearchScopeObservableCollection()
        {
        }

        public SmartSearchScopeObservableCollection(IEnumerable<object> source)
            : base(source)
        {
        }

        public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }
    }
}