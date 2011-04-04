using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BCharppe.WPFSmartSearch.SmartSearch
{
    /// <summary>
    /// Smart Search custom control
    /// </summary>
    [TemplatePart(Name = "PART_TxtInputSs")]
    [TemplatePart(Name = "PART_ToggleCpntVisibilityBtn")]
    public class SmartSearchCc : ItemsControl
    {
        /// <summary>
        /// Internal field storing the delay to apply to filter
        /// </summary>
        private const int Delay = 250;

        /// <summary>
        /// Internal field storing the search terms separator
        /// </summary>
        private const char Separator = ' ';

        /// <summary>
        /// Define toggle button margins when smart search is not visible and toggle button configured to bottom
        /// </summary>
        private readonly Thickness initialBottomMargin = new Thickness(0, 0, 0, -18);

        /// <summary>
        /// Define toggle button margins when smart search is not visible and toggle button configured to top
        /// </summary>
        private readonly Thickness initialTopMargin = new Thickness(0, -18, 0, 0);

        /// <summary>
        /// Define toggle button margins when smart search is visible
        /// </summary>
        private readonly Thickness runtimeComponentVisibleMargin = new Thickness(0, 0, 0, 0);

        private ToggleButton PART_ToggleCpntVisibilityBtn;
        private TextBox PART_TxtInputs;
        private DelayedAction deferredAction;
        private string previousInput = string.Empty;
        private string searchInput = string.Empty;

        /// <summary>
        /// Internal field storing input search terms
        /// </summary>
        private List<string> searchTerms;

        /// <summary>
        /// Private static contructor needed when building custom WPF control
        /// </summary>
        static SmartSearchCc()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (SmartSearchCc),
                                                     new FrameworkPropertyMetadata(typeof (SmartSearchCc)));
        }

        /// <summary>
        /// Event notifying that filter is finished
        /// </summary>
        public event EventHandler NotifyFilter;

        /// <summary>
        /// NotifyFilter event safe invoker
        /// </summary>
        /// <param name="e">Event arguments</param>
        public void InvokeNotifyFilter(EventArgs e)
        {
            EventHandler handler = NotifyFilter;
            if (handler != null) handler(this, e);
        }


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (PART_TxtInputs != null)
            {
                PART_TxtInputs.TextChanged -= TxtInputSsTextChanged;
            }
            if (PART_ToggleCpntVisibilityBtn != null)
            {
                PART_ToggleCpntVisibilityBtn.Click -= PartToggleCpntVisibilityBtnChecked;
            }
            base.OnApplyTemplate();

            PART_TxtInputs = GetTemplateChild("PART_TxtInputSs") as TextBox;
            PART_ToggleCpntVisibilityBtn = GetTemplateChild("PART_ToggleCpntVisibilityBtn") as ToggleButton;

            if (PART_TxtInputs != null)
            {
                PART_TxtInputs.TextChanged += TxtInputSsTextChanged;
            }
            if (PART_ToggleCpntVisibilityBtn != null)
            {
                PART_ToggleCpntVisibilityBtn.Click += PartToggleCpntVisibilityBtnChecked;
            }

            //Subscribe to each scope results notifications
            foreach (object item in Items)
            {
                var ssc = (SmartSearchScope) item;

                ssc.IncreaseResultsEvent += SscIncreaseResultsEvent;
            }

            //Initialize toggle button margins
            ManageToggleButtonMargins();
        }

        /// <summary>
        /// Callback executed when component's visibility button is clicked
        /// </summary>
        /// <param name="sender">Toggle button</param>
        /// <param name="e">Event args</param>
        private void PartToggleCpntVisibilityBtnChecked(object sender, RoutedEventArgs e)
        {
            var tgb = sender as ToggleButton;
            if (tgb != null)
            {
                //if checked, set runtime margins
                if (tgb.IsChecked.HasValue && tgb.IsChecked.Value)
                {
                    tgb.Margin = runtimeComponentVisibleMargin;
                }
                    //if not
                else
                {
                    //set initialization margins
                    ManageToggleButtonMargins();
                }
            }
        }

        /// <summary>
        /// Manage component's visibility button margins allocation when component is not visible
        /// </summary>
        private void ManageToggleButtonMargins()
        {
            if (PART_ToggleCpntVisibilityBtn != null)
            {
                if (PinLocation.ToLower() == "Bottom".ToLower())
                {
                    PART_ToggleCpntVisibilityBtn.Margin = initialBottomMargin;
                }
                else if (PinLocation.ToLower() == "Top".ToLower())
                {
                    PART_ToggleCpntVisibilityBtn.Margin = initialTopMargin;
                }
                else
                {
                    PART_ToggleCpntVisibilityBtn.Margin = runtimeComponentVisibleMargin;
                }
            }
        }


        /// <summary>
        /// Callback executed when a scope notifies results update
        /// </summary>
        /// <param name="sender">SmarSearchScope object</param>
        /// <param name="eventArgs">N/A</param>
        private void SscIncreaseResultsEvent(object sender, EventArgs eventArgs)
        {
            if (Items.Cast<SmartSearchScope>().Any(s => s.DataControl == null))
                throw new ArgumentException(
                    "One of the search scopes does not bind to a Datagrid or the binded datagrid is not correct");
            int resTemp = Items.Cast<SmartSearchScope>().Sum(sss => sss.Results);

            Results = string.Format("{0} items", resTemp.ToString());
        }


        /// <summary>
        /// Event raised when input text is modified
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TxtInputSsTextChanged(object sender, RoutedEventArgs e)
        {
            var control = e.OriginalSource as TextBox;
            if (control != null)
            {
                FilterTextChanged(control.Text);
            }
        }


        /// <summary>
        /// Method handling the unsubscription to filtersearch in the case where input is empty after filter has been executed
        /// </summary>
        /// <param name="textInput">Input text</param>
        public void FilterTextChanged(string textInput)
        {
            searchInput = textInput;
            //ExecuteFilter(); //When text search is set in the textbox, execute the filter action
            ApplySearchCriteria();
        }

        /// <summary>
        /// Method handling the delay filter
        /// </summary>
        private void ExecuteFilter()
        {
            if (deferredAction == null)
            {
                deferredAction = DelayedAction.Create(ApplySearchCriteria);
            }

            // Defer applying search criteria until time has elapsed.
            deferredAction.Defer(new TimeSpan(0, 0, 0, 0, Delay));
        }

        /// <summary>
        /// Prepare and Apply the filter to the collection view reference
        /// </summary>
        private void ApplySearchCriteria()
        {
            searchTerms = searchInput.Split(new[] {Separator}).ToList();

            if (searchTerms[searchTerms.Count - 1] != string.Empty || searchInput.Length == 0)
            {
                searchTerms.RemoveAll(term => term == string.Empty); //If last split element is string.empty, remove it

                //Trim end and start spaces
                for (int index = 0; index < searchTerms.Count; index++)
                {
                    searchTerms[index] = searchTerms[index].TrimEnd().TrimStart().ToLowerInvariant();
                }
                foreach (SmartSearchScope sss in Items)
                {
                    sss.ApplySearchCriteria(searchTerms);
                }

                InvokeNotifyFilter(EventArgs.Empty);
            }
            previousInput = searchInput;
        }

        #region DependencyProperties

        // Using a DependencyProperty as the backing store for Results.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultsProperty =
            DependencyProperty.Register("Results", typeof (string), typeof (SmartSearchCc),
                                        new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty PinLocationProperty =
            DependencyProperty.Register("PinLocation", typeof (string), typeof (SmartSearchCc),
                                        new UIPropertyMetadata("None"));


        /// <summary>
        /// Expose the filter results number
        /// </summary>
        public string Results
        {
            get { return (string) GetValue(ResultsProperty); }
            private set { SetValue(ResultsProperty, value); }
        }


        public string PinLocation
        {
            get { return (string) GetValue(PinLocationProperty); }
            set { SetValue(PinLocationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PinLocation.  This enables animation, styling, binding, etc...

        #endregion
    }
}