// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayedAction.cs" company="dotnetexplorer.blog.com">
//   2011
// </copyright>
// <summary>
//   This class holds the logic that handle the execution of a delayed/differed action
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// This class holds the logic that handle the execution of a delayed/differed action
    /// </summary>
    internal sealed class DelayedAction
    {
        /// <summary>
        ///   UI Dispatcher
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        ///   Delay timer
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        ///   Prevents a default instance of the <see cref = "DelayedAction" /> class from being created.
        /// </summary>
        private DelayedAction()
        {
            _dispatcher = Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedAction"/> class.
        /// </summary>
        /// <param name="action">
        /// The action to execute
        /// </param>
        private DelayedAction(Action action)
            : this()
        {
            _timer = new Timer(delegate { _dispatcher.Invoke(action); });
        }

        /// <summary>
        /// Creates a new DeferredAction.
        /// </summary>
        /// <param name="action">
        /// The action that will be deferred. It is not performed until 
        ///   <see cref="Defer"/> is called.
        /// </param>
        /// <returns>
        /// The delayed
        /// </returns>
        public static DelayedAction Create(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return new DelayedAction(action);
        }

        /// <summary>
        /// Defers performing the action until after time elapses. 
        ///   Repeated calls will reschedule the action
        ///   if it has not already been performed.
        /// </summary>
        /// <param name="delay">
        /// The amount of time to wait before performing the action.
        /// </param>
        public void Defer(TimeSpan delay)
        {
            // Fire action when time elapses (with no subsequent calls).
            _timer.Change(delay, TimeSpan.FromMilliseconds(-1));
        }
    }
}