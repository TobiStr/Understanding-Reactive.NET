using NUnit.Framework;
using System.Reactive.Linq;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

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
        var o = Observable.Create();

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

    private void Print<T>(T param)
    {
        Debug.WriteLine(param!.ToString());
    }
}