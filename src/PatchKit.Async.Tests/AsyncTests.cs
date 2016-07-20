using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using PatchKit;

namespace PatchKitTests
{
    [TestFixture]
    public class AsyncTests
    {
        private static T DoWork<T>(long durationMiliseconds, AsyncCancellationToken cancellationToken, Func<T> result)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (watch.ElapsedMilliseconds < durationMiliseconds)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Thread.Sleep(1);
            }

            return result();
        }

        private static void DoWork(long durationMiliseconds, AsyncCancellationToken cancellationToken)
        {
            DoWork<object>(durationMiliseconds, cancellationToken, () => null);
        }

        [Test]
        public void WaitHandleTest()
        {
            var asyncResult = new AsyncResult(() => DoWork(500, AsyncCancellationToken.None));

            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.IsTrue(asyncResult.IsCompleted);
            Assert.IsFalse(asyncResult.IsCancelled);
            Assert.IsNull(asyncResult.Exception);
        }

        [Test]
        public void ThrowExceptionTest()
        {
            var exception = new Exception();

            var asyncResult = new AsyncResult(() =>
            {
                throw exception;
            });

            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.IsTrue(asyncResult.IsCompleted);
            Assert.IsFalse(asyncResult.IsCancelled);

            Assert.IsNotNull(asyncResult.Exception);

            Assert.AreSame(asyncResult.Exception, exception);
        }

        [Test]
        public void ResultTest()
        {
            var result = new object();

            var asyncResult = new AsyncResult<object>(() => DoWork(500, AsyncCancellationToken.None, () => result));

            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.IsTrue(asyncResult.IsCompleted);
            Assert.IsFalse(asyncResult.IsCancelled);
            Assert.IsNull(asyncResult.Exception);

            Assert.AreSame(asyncResult.Result, result);
        }

        [Test]
        public void StateTest()
        {
            var state = new object();

            var asyncResult = new AsyncResult(() => DoWork(500, AsyncCancellationToken.None), null, state);

            Assert.AreSame(asyncResult.AsyncState, state);

            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.IsTrue(asyncResult.IsCompleted);
            Assert.IsFalse(asyncResult.IsCancelled);
            Assert.IsNull(asyncResult.Exception);
        }

        [Test, MaxTime(1000)]
        public void CallbackTest()
        {
            AsyncResult[] asyncResult = { null };

            var waitHandle = new ManualResetEvent(false);

            AsyncCallback callback = ar =>
            {
                waitHandle.Set();
                Assert.AreSame(ar, asyncResult[0]);
            };

            asyncResult[0] = new AsyncResult(() => DoWork(500, AsyncCancellationToken.None), callback);

            asyncResult[0].AsyncWaitHandle.WaitOne();

            Assert.IsTrue(asyncResult[0].IsCompleted);
            Assert.IsFalse(asyncResult[0].IsCancelled);
            Assert.IsNull(asyncResult[0].Exception);

            waitHandle.WaitOne();
        }

        [Test, MaxTime(1000)]
        public void CancelTest1()
        {
            var asyncResult = new AsyncResult(cancellationToken => DoWork(500, cancellationToken));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (watch.ElapsedMilliseconds < 100)
            {
                Thread.Sleep(1);
            }

            Assume.That(!asyncResult.IsCompleted, "Operation had completed before it has been canceled. Increase operation time.");

            Assert.IsTrue(asyncResult.CanBeCancelled);

            Assert.IsTrue(asyncResult.Cancel());

            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.IsTrue(asyncResult.IsCompleted);
            Assert.IsTrue(asyncResult.IsCancelled);
            Assert.IsNull(asyncResult.Exception);

            Assert.IsFalse(asyncResult.Cancel());
            Assert.IsFalse(asyncResult.CanBeCancelled);
        }

        [Test]
        public void CancelTest2()
        {
            var asyncResult = new AsyncResult(() => DoWork(500, AsyncCancellationToken.None));

            Assert.IsFalse(asyncResult.CanBeCancelled);

            Assert.IsFalse(asyncResult.Cancel());

            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.IsTrue(asyncResult.IsCompleted);
            Assert.IsFalse(asyncResult.IsCancelled);
            Assert.IsNull(asyncResult.Exception);

            Assert.IsFalse(asyncResult.Cancel());
            Assert.IsFalse(asyncResult.CanBeCancelled);
        }

        [Test, MaxTime(1000)]
        public void CallbackAfterCancelTest()
        {
            AsyncResult[] asyncResult = { null };

            var waitHandle = new ManualResetEvent(false);

            CancellableAsyncCallback callback = ar =>
            {
                waitHandle.Set();
                Assert.AreSame(ar, asyncResult[0]);
            };

            asyncResult[0] = new AsyncResult(cancellationToken => DoWork(500, cancellationToken), callback);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (watch.ElapsedMilliseconds < 100)
            {
                Thread.Sleep(1);
            }

            Assume.That(!asyncResult[0].IsCompleted, "Operation had completed before it has been canceled. Increase operation time.");

            Assert.IsTrue(asyncResult[0].CanBeCancelled);

            Assert.IsTrue(asyncResult[0].Cancel());

            asyncResult[0].AsyncWaitHandle.WaitOne();

            Assert.IsTrue(asyncResult[0].IsCompleted);
            Assert.IsTrue(asyncResult[0].IsCancelled);
            Assert.IsNull(asyncResult[0].Exception);

            Assert.IsFalse(asyncResult[0].Cancel());
            Assert.IsFalse(asyncResult[0].CanBeCancelled);
        }
    }
}
