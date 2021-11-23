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
            .Range(0,4)
            .Select(i => {
                if (i == 2)
                    throw new Exception();
                return i;
            });
    }

    [Test]
    public void CatchTest()
    {
        //This seems to be the only possibility to use an error handler
        //inside Catch. Lambda Expressions seem not to work.
        Func<Exception, IObservable<int>> errorHandler = ex => {
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
    public async Task RetryWhenTest()
    {
        var signal = Observable
            .Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1));

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
        //  0
        //  1
        //  Exception: Exception of type 'System.Exception' was thrown.
    }


    private void Print<T>(T param)
    {
        Debug.WriteLine(param!.ToString());
    }
}