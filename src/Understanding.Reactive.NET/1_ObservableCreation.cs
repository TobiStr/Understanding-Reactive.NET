using NUnit.Framework;
using System.Reactive.Linq;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reactive.Disposables;

namespace Understanding.Reactive.NET;

[TestFixture]
[Category("Unit")]
internal class ObservableCreationTests
{
    [Test]
    public async Task EmptyObservableTest()
    {
        var o = Observable.Empty<int>();

        o.Subscribe(
            next => Print(next),
            error => Print(error),
            () => Print("Finished")
        );

        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.That(true);

        //Output:
        // Finished
    }

    [Test]
    public async Task CreateObservableTest()
    {

        // Create requires to return an IDisposable at the end of the Sequence
        var o = Observable.Create<int>(o =>
        {
            o.OnNext(1);
            o.OnCompleted();
            return Disposable.Create(() => Print("Disposed"));
        });

        o.Subscribe(
            next => Print(next),
            error => Print(error),
            () => Print("Finished")
        );

        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.That(true);

        //Output:
        // 1
        // Finished
        // Disposed
    }

    private void Print<T>(T param)
    {
        Debug.WriteLine(param!.ToString());
    }
}