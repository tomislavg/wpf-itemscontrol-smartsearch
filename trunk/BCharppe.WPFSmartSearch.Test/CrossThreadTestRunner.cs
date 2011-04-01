using System;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace XHedge.Client.Tests
{
    /// <summary>
    /// Enable to instantiate UI controls for unit tests
    /// Taken from http://www.hedgate.net/articles/2007/01/08/instantiating-a-wpf-control-from-an-nunit-test/
    /// based on http://www.peterprovost.org/blog/post/NUnit-and-Multithreaded-Tests-CrossThreadTestRunner.aspx
    /// </summary>
    public class CrossThreadTestRunner
    {
        private Exception lastException;

        public void RunInMta(ThreadStart userDelegate)
        {
            Run(userDelegate, ApartmentState.MTA);
        }

        public void RunInSta(ThreadStart userDelegate)
        {
            Run(userDelegate, ApartmentState.STA);
        }

        private void Run(ThreadStart userDelegate, ApartmentState apartmentState)
        {
            lastException = null;

            Thread thread = new Thread(
              delegate()
              {
                  try
                  {
                      userDelegate.Invoke();
                  }
                  catch (Exception e)
                  {
                      lastException = e;
                  }
              });
            thread.SetApartmentState(apartmentState);

            thread.Start();
            thread.Join();

            if (ExceptionWasThrown())
                ThrowExceptionPreservingStack(lastException);
        }

        private bool ExceptionWasThrown()
        {
            return lastException != null;
        }

        [ReflectionPermission(SecurityAction.Demand)]
        private static void ThrowExceptionPreservingStack(Exception exception)
        {
            FieldInfo remoteStackTraceString = typeof(Exception).GetField(
              "_remoteStackTraceString",
              BindingFlags.Instance | BindingFlags.NonPublic);
            remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);
            throw exception;
        }
    }
}