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

namespace BCharppe.WPFSmartSearch.SmartSearch
{

    #region Threading tests

    //internal class ActionWrapper
    //{
    //    public SmartSearchScope Scope { get; set; }
    //    public Action Task { get; set; }
    //}

    //internal static class WorkersManager
    //{
    //    private static readonly Queue<ActionWrapper> TaskQueue = new Queue<ActionWrapper>();
    //    private static readonly object QueueSync = new object();
    //    private static bool keeRunning = true;
    //    static private readonly Thread T1;
    //    static private readonly Thread T2;
    //    static private readonly Thread T3;
    //    static private readonly Thread T4;


    //    static WorkersManager()
    //    {
    //        T1 = new Thread(Work1);
    //        T2 = new Thread(Work1);
    //        T3 = new Thread(Work1);
    //        T4 = new Thread(Work1);
    //        T1.Start();
    //        Debug.WriteLine("T1 STARTED !");
    //    }

    //    private static void ManageThreads()
    //    {
    //        if (TaskQueue.Count > 2 && TaskQueue.Count <= 4)
    //        {
    //            if (T2.ThreadState == ThreadState.Unstarted)
    //            { T2.Start(); Debug.WriteLine("T2 STARTED !"); }

    //        }
    //        if (TaskQueue.Count > 4 && TaskQueue.Count <= 6)
    //        {
    //            if (T2.ThreadState == ThreadState.Unstarted)
    //            { T2.Start(); Debug.WriteLine("T2 STARTED !"); }
    //            if (T3.ThreadState == ThreadState.Unstarted)
    //            { T3.Start(); Debug.WriteLine("T3 STARTED !"); }

    //        }
    //        if (TaskQueue.Count > 6)
    //        {
    //            if (T2.ThreadState == ThreadState.Unstarted)
    //            { T2.Start(); Debug.WriteLine("T2 STARTED !"); }
    //            if (T3.ThreadState == ThreadState.Unstarted)
    //            { T3.Start(); Debug.WriteLine("T3 STARTED !"); }
    //            if (T4.ThreadState == ThreadState.Unstarted)
    //            { T4.Start(); Debug.WriteLine("T4 STARTED !"); }

    //        }


    //    }

    //    private static void Work1()
    //    {
    //        while (keeRunning)
    //        {
    //            ActionWrapper item = null;
    //            lock (QueueSync)
    //            {
    //                ManageThreads();
    //                if (TaskQueue.Count > 0)
    //                {
    //                    item = TaskQueue.Dequeue();
    //                }
    //            }
    //            if (item != null) item.Task.BeginInvoke(FilterCall, item.Scope);
    //        }
    //    }


    //    public static void Enqueue(ActionWrapper a)
    //    {
    //        lock (QueueSync)
    //        {
    //            TaskQueue.Enqueue(a);
    //        }
    //    }

    //    private static void FilterCall(IAsyncResult ar)
    //    {
    //        //var ssc = (SmartSearchScope)ar.AsyncState;
    //        //if (ssc != null) ssc.NotifyResults();
    //    }
    //} 

    #endregion

    /// <summary>
    /// This class repersents a search scope defined by a ICollectionView and a FilterColumns property
    /// The filter execution is delegated to this class by the main smart search control which unique purpose is now to
    /// get the user input.
    /// </summary>
    public class SmartSearchScope : ItemsControl
    {
        /// <summary>
        /// DataControl Dependency property
        /// </summary>
        public static readonly DependencyProperty DataControlProperty =
            DependencyProperty.Register("DataControl", typeof (ItemsControl), typeof (SmartSearchScope),
                                        new UIPropertyMetadata(null, OnDataControlChanged));

        /// <summary>
        /// Underlying type Dependency property
        /// </summary>
        public static readonly DependencyProperty UnderlyingTypeProperty =
            DependencyProperty.Register("UnderlyingType", typeof (Type), typeof (SmartSearchScope),
                                        new UIPropertyMetadata(null));

        /// <summary>
        /// Collection property value getters
        /// </summary>
        private readonly List<PropertyFilterValueGetter> ValueGetters = new List<PropertyFilterValueGetter>();

        private readonly Dictionary<object, string> cache = new Dictionary<object, string>();


        /// <summary>
        /// Synchronization object
        /// </summary>
        private readonly object sync = new object();

        public int Res;

        private ICollectionView cvs;


        /// <summary>
        /// Indicate whether or not ther is any property filter that has been set to be monitored for property changes
        /// If not, the component will not subscribe to items property change event handler
        /// </summary>
        private bool hasAnyPropertyChangesToMonitor;

        /// <summary>
        /// Flag indicating if current smart search scope has been initialized
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Smart search scope name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// Real original source, only used to get a copy on orignal collection during component initialization
        /// </summary>
        private INotifyCollectionChanged originalSource;

        private StringBuilder sb;

        /// <summary>
        /// Internal field storing input search terms
        /// </summary>
        private List<string> searchTerms = new List<string>();

        /// <summary>
        /// Property descriptor watching source changes
        /// </summary>
        private DependencyPropertyDescriptor sourcePropertyDescriptor;

        /// <summary>
        /// Copy of the collection which will hold filtered items
        /// </summary>
        private SmartSearchScopeObservableCollection substituteSource;

        /// <summary>
        /// Gets or sets the datacontrol on which the smart search applies
        /// </summary>
        public ItemsControl DataControl
        {
            get { return (ItemsControl) GetValue(DataControlProperty); }
            set { SetValue(DataControlProperty, value); }
        }


        /// <summary>
        /// Type of the collection objects
        /// </summary>
        public Type UnderlyingType
        {
            get { return (Type) GetValue(UnderlyingTypeProperty); }
            set { SetValue(UnderlyingTypeProperty, value); }
        }

        /// <summary>
        /// Get the numbers of results for this filter scope
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
        /// Method called when FilterColumns property changes
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised when a a property filter refers to a non existing candidate object type property</exception>
        public void SetFilterColumns()
        {
            List<PropertyFilter> propertyFilters = Items.Cast<PropertyFilter>().ToList();
            foreach (PropertyFilter pf in propertyFilters)
            {
                //Get the PropertyInfo
                PropertyInfo pi = UnderlyingType.GetProperty(pf.FieldName);
                //this info is mandatory
                if (pi == null)
                {
                    throw new InvalidOperationException(string.Format("Cant't fin the property {0} for type {1}",
                                                                      pf.FieldName, UnderlyingType.Name));
                }
                //If pi is ok, build the value getter and add it to the list
                var pfvg = new PropertyFilterValueGetter(pf, UnderlyingType);
                ValueGetters.Add(pfvg);
            }
            hasAnyPropertyChangesToMonitor = propertyFilters.Any(p => p.MonitorPropertyChanged);
        }


        /// <summary>
        /// Callback executed when DataControl property value changed
        /// </summary>
        /// <param name="d">Dependency object on which the data control has been set (here a SmartSearchScope object)</param>
        /// <param name="e">Argument containing the new binding value</param>
        private static void OnDataControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sscc = d as SmartSearchScope;
            if (sscc != null)
            {
                sscc.Subscribe();
            }
        }

        /// <summary>
        /// Subscribe to Datacontrol ItemsSource changes
        /// </summary>
        private void Subscribe()
        {
            sourcePropertyDescriptor = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty,
                                                                                 typeof (ItemsControl));
            sourcePropertyDescriptor.AddValueChanged(DataControl, SourceChanged);
        }

        /// <summary>
        /// When detecting a source change, initialize the cache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceChanged(object sender, EventArgs e)
        {
            if (!isInitialized)
                InitializeCandidatesCache();
        }

        /// <summary>
        /// Event raised when results are updated
        /// </summary>
        public event EventHandler IncreaseResultsEvent;

        public void InvokeIncreaseResultsEvent(EventArgs e)
        {
            EventHandler handler = IncreaseResultsEvent;
            if (handler != null) handler(this, e);
        }

        public void NotifyResults()
        {
            InvokeIncreaseResultsEvent(EventArgs.Empty);
        }


        /// <summary>
        /// Method called from the SmartSearch component
        /// </summary>
        /// <param name="terms">List of search terms passed to search scope</param>
        public void ApplySearchCriteria(List<string> terms)
        {
            searchTerms = terms;
            ApplySearchCriteria();
        }

        /// <summary>
        /// Main method for applying filter
        /// </summary>
        private void ApplySearchCriteria()
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException(
                    "Can't apply smart search filter filter :The current search scope has not been initialized");
            }
            Res = 0;
            if (cvs.Filter == null)
                cvs.Filter = FilterSearch;
            else
                cvs.Refresh();

            NotifyResults(); //Notify results
        }

        /// <summary>
        /// Main method which will evaluate if an object si a valid candidate to be included in the view
        /// depending on the user filter input
        /// </summary>
        /// <param name="candidate">Candiate object</param>
        /// <returns>Object is/is not candidate for the inclusion in the view</returns>
        private bool FilterSearch(object candidate)
        {
            var allSearchFound = new bool[searchTerms.Count];

            //Iterate throught search terms
            for (int i = 0; i < searchTerms.Count; i++)
            {
                //Debug.WriteLine("   " +searchTerms[i]);
                bool propValueContainsSearchTerm =
                    cache[candidate].Contains(searchTerms[i]);
                if (propValueContainsSearchTerm)
                {
                    allSearchFound[i] = true;
                }
            }

            if (allSearchFound.All(b => b))
            {
                Res++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initialize the candidate cache
        /// </summary>
        public void InitializeCandidatesCache()
        {
            //Subscribe to real original source (the one from the VM or whatever) collection changes
            originalSource =
                ((ICollectionView) DataControl.ItemsSource).SourceCollection as INotifyCollectionChanged;
            if (originalSource != null) originalSource.CollectionChanged += OriginalSourceCollectionChanged;

            name = DataControl.Name;

            //Get undlerlying collection type
            UnderlyingType = UnderlyingType;

            //Build the property value getters colelction
            SetFilterColumns();

            //Build the working original source copy from real original source
            List<object> tmp = DataControl.ItemsSource.Cast<object>().ToList();
            substituteSource = new SmartSearchScopeObservableCollection(tmp);
            //Se the scope has initialized
            isInitialized = true;

            SetDataControlsource();

            CacheItems();

            NotifyResults(); //Notify results
        }

        /// <summary>
        /// Cache all items from the datacontrol souce collection
        /// </summary>
        private void CacheItems()
        {
            foreach (object item in DataControl.Items)
            {
                sb = new StringBuilder();
                foreach (PropertyFilterValueGetter pfvg in ValueGetters)
                {
                    sb.Append(pfvg.GetValue(item));
                }
                cache.Add(item, sb.ToString().ToLowerInvariant());
            }
        }

        /// <summary>
        /// Add a given item to cache
        /// </summary>
        /// <param name="item">Item to cache</param>
        private void CacheItem(object item)
        {
            sb = new StringBuilder();
            foreach (PropertyFilterValueGetter pfvg in ValueGetters)
            {
                sb.Append(pfvg.GetValue(item));
            }
            cache.Add(item, sb.ToString().ToLowerInvariant());
        }

        /// <summary>
        /// Update a cached item
        /// </summary>
        /// <param name="item"></param>
        private void UpdateCacheItem(object item)
        {
            sb = new StringBuilder();
            foreach (PropertyFilterValueGetter pfvg in ValueGetters)
            {
                sb.Append(pfvg.GetValue(item));
            }
            cache[item] = sb.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Set custom datasource to the datacontrol
        /// </summary>
        private void SetDataControlsource()
        {
            //substitute Original source with filtered source in the items control like Indiana Jones in "The Adventurers of The Lost Arch"
            DataControl.ItemsSource =
                new CollectionView(substituteSource);
            //Get the collection view source on which the filter will be done
            cvs = CollectionViewSource.GetDefaultView(DataControl.ItemsSource);
        }

        /// <summary>
        /// Callback executed when the original source notify add/remove of items
        /// Used to synchronize original source with copy of the original source
        /// </summary>
        /// <param name="sender">Original source collection</param>
        /// <param name="e">Added/removed items</param>
        private void OriginalSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CacheItem(e.NewItems[0]); //Add item to cache
                    substituteSource.Add(e.NewItems[0]);
                        //Add item to the collection, which will have as effect for the item to be evaluated by the filter

                    if (hasAnyPropertyChangesToMonitor) //subscribe only if any property monitor has been set
                    {
                        var iPropchangedSub = e.NewItems[0] as INotifyPropertyChanged;
                        if (iPropchangedSub != null)
                        {
                            //subscribe to the new item property changed event
                            iPropchangedSub.PropertyChanged += ItemPropertyChanged;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    substituteSource.Remove(e.OldItems[0]);
                    cache.Remove(e.OldItems[0]);
                    if (hasAnyPropertyChangesToMonitor) //unsubscribe only if any property monitor has been set
                    {
                        var iPropchangedUnSub = e.OldItems[0] as INotifyPropertyChanged;
                        if (iPropchangedUnSub != null)
                        {
                            //subscribe to the new item property changed event
                            iPropchangedUnSub.PropertyChanged -= ItemPropertyChanged;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            NotifyResults(); //Notify results
        }

        /// <summary>
        /// Callback executed when an item's property gets updated
        /// </summary>
        /// <param name="sender">Items</param>
        /// <param name="e">Argument containing the name of the property that raised the changes</param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //lots of properties gets updated, we watch only those which belongs to visible fieldnames
            //and those who were registered for propertychanges monitoring
            //and of course if the item is still on the list
            if (ValueGetters.Any(vg => vg.PropertyFilerDescriptor.FieldName == e.PropertyName))
            {
                UpdateCacheItem(sender);
                PropertyFilterValueGetter pf =
                    ValueGetters.FirstOrDefault(vg => vg.PropertyFilerDescriptor.FieldName == e.PropertyName);
                if (pf.PropertyFilerDescriptor.MonitorPropertyChanged && substituteSource.Contains(sender))
                {
                    //To avoid doing a refresh on a property change which would end in a very hawful user experience
                    //we simulate a replace to the collection because the filter is automatically applied in this case

                    int index = substituteSource.IndexOf(sender);

                    var argsReplace = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                                                           new List<object> {sender},
                                                                           new List<object> {sender}, index);
                    substituteSource.RaiseCollectionChanged(argsReplace);
                }

                NotifyResults(); //Notify results
            }
        }
    }
}