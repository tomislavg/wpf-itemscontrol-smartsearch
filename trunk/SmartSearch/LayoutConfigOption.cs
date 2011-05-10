// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    /// <summary>
    /// The layout config option.
    /// </summary>
    public enum LayoutConfigOption
    {
        /// <summary>
        ///   Only filter input is visible
        /// </summary>
        Minimal,

        /// <summary>
        ///   Minimal plus "Filter" and number of items labels are visibles
        /// </summary>
        Basic,

        /// <summary>
        ///   Basic plus AND / OR switch is visible
        /// </summary>
        Intemediate,

        /// <summary>
        ///   Intermediate plus toggle visibility buton is visible
        /// </summary>
        Full,
    }
}
