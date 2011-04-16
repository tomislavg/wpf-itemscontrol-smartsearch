// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    /// <summary>
    /// The property filter.
    /// </summary>
    public class PropertyFilter : ValueFilter
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "PropertyFilter" /> class.
        /// </summary>
        public PropertyFilter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFilter"/> class. 
        ///   Initializes a new instance of the <see cref="ValueFilter"/> class. 
        ///   Constructor with monitor parameter
        /// </summary>
        /// <param name="fieldName">
        /// </param>
        /// <param name="monitor">
        /// </param>
        public PropertyFilter(string fieldName, bool monitor)
        {
            FieldName = fieldName;
            MonitorPropertyChanged = monitor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFilter"/> class. 
        ///   Initializes a new instance of the <see cref="ValueFilter"/> class. 
        ///   Constructor with property name only
        /// </summary>
        /// <param name="fieldName">
        /// </param>
        public PropertyFilter(string fieldName)
        {
            FieldName = fieldName;
        }

        /// <summary>
        ///   Property name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        ///   Does the smart search component has to monitor property changes on that property
        /// </summary>
        public bool MonitorPropertyChanged { get; set; }
    }
}