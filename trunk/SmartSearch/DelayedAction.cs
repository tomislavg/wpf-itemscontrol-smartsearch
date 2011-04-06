#region

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

#endregion

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    /// <summary>
    ///   This class holds the logic that handle the execution of a delayed/differed action
    /// </summary>
    internal sealed class DelayedAction
    {
        private readonly Dispatcher dispatcher;

        private readonly Timer timer;

        private DelayedAction()
        {
            dispatcher = Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;
        }

        private DelayedAction(Action action)
            : this()
        {
            timer = new Timer(delegate { dispatcher.Invoke(action); });
        }

        /// <summary>
        ///   Creates a new DeferredAction.
        /// </summary>
        /// <param name = "action">
        ///   The action that will be deferred. It is not performed until 
        ///   after <see cref = "Defer" /> is called.
        /// </param>
        public static DelayedAction Create(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return new DelayedAction(action);
        }

        /// <summary>
        ///   Defers performing the action until after time elapses. 
        ///   Repeated calls will reschedule the action
        ///   if it has not already been performed.
        /// </summary>
        /// <param name = "delay">
        ///   The amount of time to wait before performing the action.
        /// </param>
        public void Defer(TimeSpan delay)
        {
            // Fire action when time elapses (with no subsequent calls).
            timer.Change(delay, TimeSpan.FromMilliseconds(-1));
        }
    }
}