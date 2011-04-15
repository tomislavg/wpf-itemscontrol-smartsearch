// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartSearchScope.cs" company="dotnetexplorer.blog.com">
//   2011
// </copyright>
// <summary>
//   This class repersents a search scope defined by a ICollectionView and a FilterColumns property
//   The filter execution is delegated to this class by the main smart search control which unique purpose is now to
//   get the user input.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    ///   This class repersents a search scope defined by a ICollectionView and a FilterColumns property
    ///   The filter execution is delegated to this class by the main smart search control which unique purpose is now to
    ///   get the user input.
    /// </summary>
    public sealed class SmartSearchScope : ItemsControl
    {
        /// <summary>
        ///   The property value mask cache.
        /// </summary>
        private const string PropertyValueMaskCache = "{0} ";

        public static readonly DependencyProperty DefaultMonitorPropertyChangesProperty =
            DependencyProperty.Register("DefaultMonitorPropertyChanges", typeof(bool), typeof(SmartSearchScope),
                                        new UIPropertyMetadata(false));

        /// <summary>
        ///   The empty property value mask cache.
        /// </summary>
        private static readonly string EmptyPropertyValueMaskCache = PropertyValueMaskCache.Replace("{0}", string.Empty);

        /// <summary>
        ///   DataControl Dependency property
        /// </summary>
        public static readonly DependencyProperty DataControlProperty =
            DependencyProperty.Register("DataControl", typeof(ItemsControl), typeof(SmartSearchScope),
                                        new UIPropertyMetadata(null, OnDataControlChanged));

        /// <summary>
        ///   Underlying type Dependency property
        /// </summary>
        public static readonly DependencyProperty UnderlyingTypeProperty =
            DependencyProperty.Register("UnderlyingType", typeof(Type), typeof(SmartSearchScope),
                                        new UIPropertyMetadata(null));

        /// <summary>
        ///   Collection property value getters
        /// </summary>
        private readonly List<PropertyFilterValueGetter> _valueGetters = new List<PropertyFilterValueGetter>();

        /// <summary>
        ///   Delegate for the value getter fonction
        /// </summary>
        private readonly Func<object, string> concatTestValue;

        /// <summary>
        ///   Delegate for the scope initialization function
        /// </summary>
        private readonly Action initialize;

        /// <summary>
        ///   Synchronization object
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        ///   Scope results
        /// </summary>
        public int Res;

        /// <summary>
        ///   Core item of smartsearch scope : this is here that the filter will take place
        /// </summary>
        private ICollectionView _cvs;

        /// <summary>
        ///   Filter mode (AND ||OR)
        /// </summary>
        private FilterMode _filterMode;

        /// <summary>
        ///   Indicate whether or not ther is any property filter that has been set to be monitored for property changes
        ///   If not, the component will not subscribe to items property change event handler
        /// </summary>
        private bool _hasAnyPropertyChangesToMonitor;

        /// <summary>
        ///   Flag indicating if current smart search scope has been initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        ///   Real original source, only used to get a copy on orignal collection during component initialization
        /// </summary>
        private INotifyCollectionChanged _originalSource;

        /// <summary>
        ///   The sb.
        /// </summary>
        private StringBuilder _sb;

        /// <summary>
        ///   Internal field storing input search terms with bool value indicating if terms is to be included (true) or excluded (false)
        /// </summary>
        private Dictionary<string, bool> _searchTerms = new Dictionary<string, bool>();

        /// <summary>
        ///   Property descriptor watching source changes
        /// </summary>
        private DependencyPropertyDescriptor _sourcePropertyDescriptor;

        /// <summary>
        ///   Copy of the collection which will hold filtered items
        /// </summary>
        private SmartSearchScopeObservableCollection _substituteSource;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SmartSearchScope" /> class. 
        ///   Constructor
        /// </summary>
        public SmartSearchScope()
        {
            concatTestValue =
                (Func<object, string>)
                Delegate.CreateDelegate(typeof(Func<object, string>), this, "GetConcatenatedCandidateValue", true);

            initialize = (Action)Delegate.CreateDelegate(typeof(Action), this, "InitializeFilterScope", true);
        }

        /// <summary>
        ///   Gets or Sets if the property changed is monitored by default.
        ///   This property is not taken into account when explicitly defining PropertyFilters
        /// </summary>
        public bool DefaultMonitorPropertyChanges
        {
            get { return (bool)GetValue(DefaultMonitorPropertyChangesProperty); }
            set { SetValue(DefaultMonitorPropertyChangesProperty, value); }
        }

        /// <summary>
        ///   Gets or sets the datacontrol on which the smart search applies
        /// </summary>
        public ItemsControl DataControl
        {
            get { return (ItemsControl)GetValue(DataControlProperty); }
            set { SetValue(DataControlProperty, value); }
        }


        /// <summary>
        ///   Type of the collection objects
        /// </summary>
        public Type UnderlyingType
        {
            get { return (Type)GetValue(UnderlyingTypeProperty); }
            set { SetValue(UnderlyingTypeProperty, value); }
        }

        /// <summary>
        ///   Get the numbers of results for this filter scope
        /// </summary>
        public int Results
        {
            get
            {
                lock (sync)
                {
                    return DataControl.Items.Count;
                }
            }
        }

        /// <summary>
        ///   Method called when FilterColumns property changes
        /// </summary>
        /// <exception cref = "InvalidOperationException">
        ///   Raised when a a property filter refers to a non existing candidate object type property
        /// </exception>
        private void SetFilterColumns()
        {
            var propertyFilters = Items.Cast<ValueFilter>().ToList();

            if (UnderlyingType.Equals(typeof(string)) || UnderlyingType.IsValueType)
            {
                //for native types, only one ValueFilter accepted to apply string format or IValueConverter 
                if (propertyFilters.Count > 1)
                {
                    throw new InvalidOperationException(
                        "Only one property filter is authorized when underlying type is String or a value type.");
                }
                //Then if there is no property filter at all, there is no conversion needed and we build a defautl property getter
                //and there is one, we build a property getter accordingly
                _valueGetters.Add(propertyFilters.Count == 0
                                      ? new PropertyFilterValueGetter(new ValueFilter())
                                      : new PropertyFilterValueGetter(propertyFilters[0]));
            }
            //if the underlying type if neither a string nor value type, then it is a reference type
            else
            {
                IEnumerable<PropertyFilter> apfs;
                //if there is no PropertyFilter defined, we build it by default based on every instance public properties of the underlying type
                if (propertyFilters.Count == 0)
                {
                    apfs = new List<PropertyFilter>();
                    // Get the type Properties
                    var pis = UnderlyingType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    //iterate
                    foreach (var propertyInfo in pis)
                    {
                        var pf = new PropertyFilter(propertyInfo.Name, DefaultMonitorPropertyChanges);
                        var pfvg = new PropertyFilterValueGetter(pf, UnderlyingType);//Generate...
                        _valueGetters.Add(pfvg);//...and add value getter for each properties
                    }
                }
                else
                {
                    //and thus every ValueFilter must be PropertyFilter
                    if (propertyFilters.Any(p => !(p is PropertyFilter)))
                    {
                        throw new InvalidOperationException("ValueFilter can't be used with reference type.");
                    }
                    apfs = propertyFilters.Cast<PropertyFilter>().ToList();
                    foreach (PropertyFilter pf in apfs)
                    {
                        // Get the PropertyInfo
                        PropertyInfo pi = UnderlyingType.GetProperty(pf.FieldName);

                        // this info is mandatory
                        if (pi == null)
                        {
                            throw new InvalidOperationException(
                                string.Format("Cant't find the property {0} for type {1}",
                                              pf.FieldName, UnderlyingType.Name));
                        }

                        // If pi is ok, build the value getter and add it to the list
                        var pfvg = new PropertyFilterValueGetter(pf, UnderlyingType);
                        _valueGetters.Add(pfvg);
                    }
                }

                // If there is no ValueFilter which is set to be monitored for property changes, we keep track of it to avoid to subscribe to propertychanged event later on
                _hasAnyPropertyChangesToMonitor = apfs.Any(p => p.MonitorPropertyChanged);
            }
        }


        /// <summary>
        ///   Callback executed when DataControl property value changed
        /// </summary>
        /// <param name = "d">
        ///   Dependency object on which the data control has been set (here a SmartSearchScope object)
        /// </param>
        /// <param name = "e">
        ///   Argument containing the new binding value
        /// </param>
        private static void OnDataControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sscc = d as SmartSearchScope;
            if (sscc != null)
            {
                sscc.Subscribe();
            }
        }

        /// <summary>
        ///   Subscribe to Datacontrol ItemsSource changes
        /// </summary>
        private void Subscribe()
        {
            _sourcePropertyDescriptor = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty,
                                                                                  typeof(ItemsControl));
            _sourcePropertyDescriptor.AddValueChanged(DataControl, SourceChanged);
        }

        /// <summary>
        ///   When detecting a source change, initialize the cache
        /// </summary>
        /// <param name = "sender">
        /// </param>
        /// <param name = "e">
        /// </param>
        private void SourceChanged(object sender, EventArgs e)
        {
            if (!_isInitialized)
                initialize();
        }

        /// <summary>
        ///   Event raised when results are updated
        /// </summary>
        public event EventHandler IncreaseResultsEvent;

        /// <summary>
        ///   The invoke increase results event.
        /// </summary>
        /// <param name = "e">
        ///   The e.
        /// </param>
        private void InvokeIncreaseResultsEvent(EventArgs e)
        {
            EventHandler handler = IncreaseResultsEvent;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        ///   Call the notify results event invoker
        /// </summary>
        private void NotifyResults()
        {
            InvokeIncreaseResultsEvent(EventArgs.Empty);
        }

        /// <summary>
        ///   Method called from the SmartSearch component
        /// </summary>
        /// <param name = "terms">
        ///   List of search terms passed to search scope
        /// </param>
        /// <param name = "filtertypeMode">
        /// </param>
        public void ApplySearchCriteria(IEnumerable<string> terms, FilterMode filtertypeMode)
        {
            _filterMode = filtertypeMode;
            _searchTerms = new Dictionary<string, bool>();
            foreach (string t in terms)
            {
                var exclude = t.Length > 1 ? t.Substring(0, 1) : string.Empty;
                if (exclude.Equals("!"))
                {
                    _searchTerms.Add(t.Remove(0, 1), false);
                }
                else
                {
                    _searchTerms.Add(t, true);
                }
            }

            ApplySearchCriteria();
        }

        /// <summary>
        ///   Main method for applying filter
        /// </summary>
        private void ApplySearchCriteria()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException(
                    "Can't apply smart search filter filter :The current search scope has not been initialized");
            }

            Res = 0;
            if (_cvs.Filter == null) // if the current filter delegate is empty, set the filter method
                _cvs.Filter = FilterSearch;
            else // otherwise
                _cvs.Refresh(); // just refresh collection view source

            NotifyResults(); // Notify results
        }

        /// <summary>
        ///   Main method which will evaluate if an object si a valid candidate to be included in the view
        ///   depending on the user filter input
        /// </summary>
        /// <param name = "candidate">
        ///   Candiate object
        /// </param>
        /// <returns>
        ///   Object is/is not candidate for the inclusion in the view
        /// </returns>
        private bool FilterSearch(object candidate)
        {
            if (_searchTerms.Count == 0) return true;
            var allSearchFound = new bool[_searchTerms.Count];
            string value = concatTestValue(candidate);


            // Iterate throught search terms
            int icount = 0;
            foreach (var st in _searchTerms)
            {
                if (!st.Value && !value.Contains(st.Key))
                {
                    allSearchFound[icount] = true;
                }
                else if (!st.Value && value.Contains(st.Key))
                {
                    allSearchFound[icount] = false;
                }
                else if (st.Value && !value.Contains(st.Key))
                {
                    allSearchFound[icount] = false;
                }
                else if (st.Value && value.Contains(st.Key))
                {
                    allSearchFound[icount] = true;
                }

                icount++;
            }

            if (_filterMode == FilterMode.AND)
            {
                // if AND mode, all search terms must be found in object properties
                if (allSearchFound.All(b => b))
                {
                    Res++;
                    return true;
                }
            }
            else
            {
                // if OR mode, any one of the search terms can be found in object properties
                if (allSearchFound.Any(b => b))
                {
                    Res++;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///   Get string concatenated formated value for a candidate
        /// </summary>
        /// <param name = "candidate">
        ///   Candidate for which to retreive values
        /// </param>
        /// <returns>
        ///   Formatted values (serparated given the mask previously declared)
        /// </returns>
        private string GetConcatenatedCandidateValue(object candidate)
        {
            _sb = new StringBuilder();
            foreach (PropertyFilterValueGetter pfvg in _valueGetters)
            {
                _sb.AppendFormat(PropertyValueMaskCache, pfvg.GetValue(candidate));
            }

            return _sb.ToString().ToLowerInvariant() /*.Replace(EmptyPropertyValueMaskCache, string.Empty)*/;
        }

        /// <summary>
        ///   Initialize the candidate cache
        /// </summary>
        private void InitializeFilterScope()
        {
            // Subscribe to real original source (the one from the VM or whatever) collection changes
            _originalSource =
                DataControl.ItemsSource as INotifyCollectionChanged;
            if (_originalSource != null) _originalSource.CollectionChanged += OriginalSourceCollectionChanged;

            // Build the property value getters colelction
            SetFilterColumns();

            // Build the working original source copy from real original source
            var tmp = DataControl.ItemsSource.Cast<object>().ToList();
            _substituteSource = new SmartSearchScopeObservableCollection(tmp);


            // Se the scope has initialized
            _isInitialized = true;

            SetDataControlsource();

            NotifyResults(); // Notify results
        }


        /// <summary>
        ///   Set custom datasource to the datacontrol
        /// </summary>
        private void SetDataControlsource()
        {
            // substitute Original source with filtered source in the items control like Indiana Jones in "The Adventurers of The Lost Arch"
            DataControl.ItemsSource = _substituteSource;


            // With xceed datagrids we need to use DataGridCollectionView to avoid conflicts when using DeferRefresh
            // Get the collection view source on which the filter will be done
            _cvs = CollectionViewSource.GetDefaultView(DataControl.ItemsSource);
        }

        /// <summary>
        ///   Callback executed when the original source notify add/remove of items
        ///   Used to synchronize original source with copy of the original source
        /// </summary>
        /// <param name = "sender">
        ///   Original source collection
        /// </param>
        /// <param name = "e">
        ///   Added/removed items
        /// </param>
        private void OriginalSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ManageItemAdd(e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ManageItemRemove(e.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            NotifyResults(); // Notify results
        }

        /// <summary>
        ///   Manage actions when an item is removed from the original collection
        /// </summary>
        /// <param name = "o">
        ///   Removed item
        /// </param>
        private void ManageItemRemove(object o)
        {
            _substituteSource.Remove(o);
            if (_hasAnyPropertyChangesToMonitor)
            {
                // unsubscribe only if any property monitor has been set
                var iPropchangedUnSub = o as INotifyPropertyChanged;
                if (iPropchangedUnSub != null)
                {
                    // subscribe to the new item property changed event
                    iPropchangedUnSub.PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        /// <summary>
        ///   Manage actions when an item is added to the original collection
        /// </summary>
        /// <param name = "o">
        ///   Added item
        /// </param>
        private void ManageItemAdd(object o)
        {
            _substituteSource.Add(o);


            // Add item to the collection, which will have as effect for the item to be evaluated by the filter
            if (_hasAnyPropertyChangesToMonitor)
            {
                // subscribe only if any property monitor has been set
                var iPropchangedSub = o as INotifyPropertyChanged;
                if (iPropchangedSub != null)
                {
                    // subscribe to the new item property changed event
                    iPropchangedSub.PropertyChanged += ItemPropertyChanged;
                }
            }
        }

        /// <summary>
        ///   Callback executed when an item's property gets updated
        /// </summary>
        /// <param name = "sender">
        ///   Items
        /// </param>
        /// <param name = "e">
        ///   Argument containing the name of the property that raised the changes
        /// </param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // lots of properties gets updated, we watch only those which belongs to visible fieldnames
            // and those who were registered for propertychanges monitoring
            // and of course if the item is still on the list
            if (_valueGetters.Any(vg => vg.FieldName == e.PropertyName))
            {
                PropertyFilterValueGetter pf =
                    _valueGetters.FirstOrDefault(vg => vg.FieldName == e.PropertyName);
                if (pf.MonitorPropertyChanged && _substituteSource.Contains(sender))
                {
                    // To avoid doing a refresh on a property change which would end in a very hawful user experience
                    // we simulate a replace to the collection because the filter is automatically applied in this case
                    int index = _substituteSource.IndexOf(sender);

                    var argsReplace = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                                                           new List<object> { sender },
                                                                           new List<object> { sender }, index);
                    _substituteSource.RaiseCollectionChanged(argsReplace);
                }

                NotifyResults(); // Notify results
            }
        }
    }
}
