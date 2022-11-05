using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Understanding.Reactive.NET;

[TestFixture]
[Category("Unit")]
internal class ErrorHandlingTests
{
    private IObservable<int> throwsAtTwo;

    [SetUp]
    public void Setup()
    {
        throwsAtTwo = Observable
            .Range(0, 4)
            .Select(i =>
            {
                if (i == 2)
                    throw new NullReferenceException("i is 2");
                return i;
            });
    }

    [Test]
    public void CatchTest()
    {
        //This seems to be the only possibility to use an error handler
        //inside Catch. Lambda Expressions seem not to work.
        Func<Exception, IObservable<int>> errorHandler = ex =>
        {
            Print("Exception: " + ex.Message);
            return Observable.Range(9, 2);
        };

        //Remember to put Catch near the error-throwing part of your pipeline
        throwsAtTwo
            .Catch(errorHandler)
            .Do(Print)
            .Subscribe();

        Assert.That(true);

        //Output:
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
        //  9
        //  10
    }

    [Test]
    public void DoThrowCatchTest()
    {
        Func<Exception, IObservable<int>> errorHandler = ex =>
        {
            Print("Exception: " + ex.Message);
            return Observable.Range(9, 2);
        };

        Observable
            .Range(0, 4)
            .Do(i =>
            {
                if (i == 2)
                    throw new Exception("i is 2");
            })
            .Catch(errorHandler)
            .Do(Print)
            .Subscribe();

        Assert.That(true);

        //Output:
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
        //  9
        //  10
    }

    [Test]
    public void RetryTest()
    {
        //Will finish with an Exception
        Assert.Throws(
            typeof(Exception),
            () => throwsAtTwo
                .Retry(2)
                .Subscribe(Print)
        );

        //Output:
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
    }

    [Test]
    public void SwitchTest()
    {
        Func<Exception, IObservable<int>> errorHandler = ex =>
        {
            Print("Exception: " + ex.Message);
            return Observable.Range(9, 2);
        };

        //Will finish with an Exception
        Assert.Throws(
            typeof(Exception),
            () => Observable.Return(1)
                .Select(_ => throwsAtTwo.Catch(errorHandler))
                .Switch()
                .Subscribe(Print)
        );

        //Output:
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
    }

    [Test]
    public Task RetryTestWithCatch()
    {
        var throwsAtTwo = Observable
           .Range(0, 4)
           .Select(i =>
           {
               if (i == 2)
                   throw new NullReferenceException("i is 2");
               return i;
           });

        Func<Exception, IObservable<int>> errorHandler = ex =>
        {
            Print("!Exception: " + ex.Message);

            if (ex is NullReferenceException)
                return Observable.Range(9, 2);
            else
                throw ex;
        };

        throwsAtTwo
           .Catch(errorHandler)
           .Retry(2)
           .Subscribe(Print);

        return Task.Delay(1000);

        //Output:
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
    }

    [Test]
    public async Task RetryWhenTest()
    {
        var signal = Observable
            .Timer(DateTimeOffset.Now.AddSeconds(1));

        throwsAtTwo
             .RetryWhen(_ => signal)
             .Subscribe(Print);

        await Task.Delay(1000);

        //Output:
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
    }

    [Test]
    public async Task TimerRetryTest()
    {
        Func<Exception, IObservable<string>> errorHandler = ex =>
        {
            Print("Error: " + ex.Message);
            return Observable.Throw<string>(ex);
        };

        var ticker = Observable
               .Timer(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(100));

        ticker
            .Do(i =>
            {
                if (i == 3)
                    throw new Exception("i is 3");
            })
            .Select(x => $"{x} Seconds elapsed")
            .Do(Print)
            .Catch(errorHandler)
            .Retry(5)
            .Subscribe();

        await Task.Delay(10000);
    }

    private void Print<T>(T param)
    {
        Debug.WriteLine(param!.ToString());
    }
}