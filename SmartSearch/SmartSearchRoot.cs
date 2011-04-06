// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartSearchRoot.cs" company="dotnetexplorer.blog.com">
//   2011
// </copyright>
// <summary>
//   Smart Search custom control
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Smart Search custom control
    /// </summary>
    [TemplatePart(Name = "PART_TxtInputSs")]
    [TemplatePart(Name = "PART_ToggleCpntVisibilityBtn")]
    [TemplatePart(Name = "PART_BtnSwitchAndOr")]
    public sealed class SmartSearchRoot : ItemsControl
    {
        /// <summary>
        ///   Internal field storing the delay to apply to filter
        /// </summary>
        private const int Delay = 250;

        /// <summary>
        ///   Internal field storing the search terms separator
        /// </summary>
        private const char Separator = ' ';

        /// <summary>
        ///   Define toggle button margins when smart search is not visible and toggle button configured to bottom
        /// </summary>
        private readonly Thickness initialBottomMargin = new Thickness(0, 0, 0, -18);

        /// <summary>
        ///   Define toggle button margins when smart search is not visible and toggle button configured to top
        /// </summary>
        private readonly Thickness initialTopMargin = new Thickness(0, -18, 0, 0);

        /// <summary>
        ///   Define toggle button margins when smart search is visible
        /// </summary>
        private readonly Thickness runtimeComponentVisibleMargin = new Thickness(0, 0, 0, 0);

        /// <summary>
        ///   OR/AN switch button control
        /// </summary>
        private Button PART_BtnSwitchAndOr;

        /// <summary>
        ///   Show/Hide button control
        /// </summary>
        private ToggleButton PART_ToggleCpntVisibilityBtn;

        /// <summary>
        ///   User input textbox control
        /// </summary>
        private TextBox PART_TxtInputs;

        /// <summary>
        ///   Delayed filter manager
        /// </summary>
        private DelayedAction deferredAction;

        /// <summary>
        ///   Component Filter mode
        /// </summary>
        private FilterMode filterMode = FilterMode.AND;

        /// <summary>
        ///   Indicate wether or not the filter mode (OR/AND) has just changed
        /// </summary>
        private bool filterModeHasChanged;

        /// <summary>
        ///   Show/hide button location
        /// </summary>
        private ShowHideButtonLocationValue internalShowHideButtonLocation = ShowHideButtonLocationValue.None;

        /// <summary>
        ///   Store for old filter values
        /// </summary>
        private List<string> previous = new List<string>();

        /// <summary>
        ///   Store for current input value
        /// </summary>
        private string searchInput = string.Empty;

        /// <summary>
        ///   Internal field storing input search terms
        /// </summary>
        private List<string> searchTerms;

        /// <summary>
        ///   Initializes static members of the <see cref = "SmartSearchRoot" /> class. 
        ///   Private static contructor needed when building custom WPF control
        /// </summary>
        static SmartSearchRoot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (SmartSearchRoot), 
                                                     new FrameworkPropertyMetadata(typeof (SmartSearchRoot)));
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (PART_BtnSwitchAndOr != null)
            {
                PART_BtnSwitchAndOr.Click -= PartBtnSwitchAndOrSwitch;
            }

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
            PART_BtnSwitchAndOr = GetTemplateChild("PART_BtnSwitchAndOr") as Button;
            if (PART_BtnSwitchAndOr != null)
            {
                PART_BtnSwitchAndOr.Click += PartBtnSwitchAndOrSwitch;
                var tb = new TextBlock {FontSize = 8, Text = filterMode.ToString()};
                PART_BtnSwitchAndOr.Content = tb;
            }

            if (PART_TxtInputs != null)
            {
                PART_TxtInputs.TextChanged += TxtInputSsTextChanged;
            }

            if (PART_ToggleCpntVisibilityBtn != null)
            {
                PART_ToggleCpntVisibilityBtn.Click += PartToggleCpntVisibilityBtnChecked;
            }

            // Subscribe to each scope results notifications
            foreach (object item in Items)
            {
                var ssc = (SmartSearchScope) item;

                ssc.IncreaseResultsEvent += SscIncreaseResultsEvent;
            }

            // Initialize toggle button margins
            ManageToggleButtonMargins();
        }

        /// <summary>
        /// The part btn switch and or switch.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PartBtnSwitchAndOrSwitch(object sender, RoutedEventArgs e)
        {
            filterMode = filterMode == FilterMode.AND ? FilterMode.OR : FilterMode.AND;
            var tb = new TextBlock {FontSize = 8, Text = filterMode.ToString()};
            PART_BtnSwitchAndOr.Content = tb;
            filterModeHasChanged = true;
            ApplySearchCriteria();
        }


        /// <summary>
        /// Callback executed when component's visibility button is clicked
        /// </summary>
        /// <param name="sender">
        /// Toggle button
        /// </param>
        /// <param name="e">
        /// Event args
        /// </param>
        private void PartToggleCpntVisibilityBtnChecked(object sender, RoutedEventArgs e)
        {
            var tgb = sender as ToggleButton;
            if (tgb != null)
            {
                // if checked, set runtime margins
                if (tgb.IsChecked.HasValue && tgb.IsChecked.Value)
                {
                    tgb.Margin = runtimeComponentVisibleMargin;
                }
                    

// if not
                else
                {
                    // set initialization margins
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
                switch (internalShowHideButtonLocation)
                {
                    case ShowHideButtonLocationValue.None:
                        PART_ToggleCpntVisibilityBtn.Margin = runtimeComponentVisibleMargin;
                        break;
                    case ShowHideButtonLocationValue.Top:
                        PART_ToggleCpntVisibilityBtn.Margin = initialTopMargin;
                        break;
                    case ShowHideButtonLocationValue.Bottom:
                        PART_ToggleCpntVisibilityBtn.Margin = initialBottomMargin;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        /// <summary>
        /// Callback executed when a scope notifies results update
        /// </summary>
        /// <param name="sender">
        /// SmarSearchScope object
        /// </param>
        /// <param name="eventArgs">
        /// N/A
        /// </param>
        private void SscIncreaseResultsEvent(object sender, EventArgs eventArgs)
        {
            if (Items.Cast<SmartSearchScope>().Any(s => s.DataControl == null))
                throw new ArgumentException(
                    "One of the search scopes does not bind to a Datagrid or the binded datagrid is not correct");
            int resTemp = Items.Cast<SmartSearchScope>().Sum(sss => sss.Results);

            Results = string.Format("{0} items", resTemp);
        }


        /// <summary>
        /// Event raised when input text is modified
        /// </summary>
        /// <param name="sender">
        /// Event sender
        /// </param>
        /// <param name="e">
        /// Event arguments
        /// </param>
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
        /// <param name="textInput">
        /// Input text
        /// </param>
        public void FilterTextChanged(string textInput)
        {
            searchInput = textInput;
            ExecuteFilter(); // When text search is set in the textbox, execute the filter action


// ApplySearchCriteria();
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

            // Manage if there is really a need to update filter (this excludes empty search string for example)
            bool filter = false;

            searchTerms.RemoveAll(term => term == string.Empty); // If last split element is string.empty, remove it
            previous.RemoveAll(term => term == string.Empty);
            if (filterModeHasChanged && searchTerms.Count > 0)
            {
                filter = true;
                filterModeHasChanged = false;
            }
            else if (searchTerms.Count != previous.Count)
            {
                filter = true;
            }
            else
            {
                for (int i = 0; i < searchTerms.Count; i++)
                {
                    if (searchTerms[i].Length != previous[i].Length)
                    {
                        filter = true;
                    }
                }
            }


// If really need to updaet filter
            if (filter)
            {
                // Trim end and start spaces
                for (int index = 0; index < searchTerms.Count; index++)
                {
                    searchTerms[index] = searchTerms[index].TrimEnd().TrimStart().ToLowerInvariant();
                }

                foreach (SmartSearchScope sss in Items)
                {
                    sss.ApplySearchCriteria(searchTerms, filterMode);
                }
            }

            previous = searchTerms;
        }

        #region DependencyProperties

        // Using a DependencyProperty as the backing store for Results.  This enables animation, styling, binding, etc...

        /// <summary>
        ///   The results property.
        /// </summary>
        public static readonly DependencyProperty ResultsProperty =
            DependencyProperty.Register("Results", typeof (string), typeof (SmartSearchRoot), 
                                        new UIPropertyMetadata(string.Empty));

        /// <summary>
        ///   The show hide button location property.
        /// </summary>
        public static readonly DependencyProperty ShowHideButtonLocationProperty =
            DependencyProperty.Register("ShowHideButtonLocation", typeof (object), typeof (SmartSearchRoot), 
                                        new UIPropertyMetadata("None", PinLocationChanged));


        /// <summary>
        ///   Expose the filter results number
        /// </summary>
        public string Results
        {
            get { return (string) GetValue(ResultsProperty); }
            private set { SetValue(ResultsProperty, value); }
        }

        /// <summary>
        ///   Gets or set the show/hide button location
        /// </summary>
        public object ShowHideButtonLocation
        {
            get { return GetValue(ShowHideButtonLocationProperty); }
            set { SetValue(ShowHideButtonLocationProperty, value); }
        }

        /// <summary>
        /// Callback called when show/hide button location property has changed
        /// </summary>
        /// <param name="d">
        /// Smart search control
        /// </param>
        /// <param name="e">
        /// Event argument
        /// </param>
        private static void PinLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ssc = d as SmartSearchRoot;
            if (ssc != null)
            {
                if (e.NewValue != null)
                {
                    var testenum =
                        (ShowHideButtonLocationValue)
                        Enum.Parse(typeof (ShowHideButtonLocationValue), e.NewValue.ToString());

                    ssc.SetPinLocation(testenum);
                }
            }
        }

        /// <summary>
        /// Set the show/hide location button
        /// </summary>
        /// <param name="testenum">
        /// Location
        /// </param>
        private void SetPinLocation(ShowHideButtonLocationValue testenum)
        {
            internalShowHideButtonLocation = testenum;
        }

        #endregion
    }
}