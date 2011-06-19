// --------------------------------------------------------------------------------------------------------------------
// http://dotnetexplorer.blog.com
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Sample : Window
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "Sample" /> class.
        /// </summary>
        public Sample()
        {
            InitializeComponent();
            DataContext = new SampleViewModel(Dispatcher);
        }
    }
}