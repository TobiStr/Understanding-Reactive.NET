using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Understanding.Reactive.NET;

[TestFixture]
[Category("Unit")]
internal class HotObservables
{
    [Test]
    public async Task WrongApproach()
    {
        var source = Observable
            .Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1));

        var observable = source.Replay();

        var observer1 = observable.Subscribe(Print);
        var observer2 = observable.Subscribe(Print);

        await Task.Delay(2000);
    }

    [Test]
    public async Task CorrectApproach()
    {
        var source = Observable
            .Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1))
            .Replay(3);

        source.Connect();

        var observer1 = source.Subscribe(Print);
        var observer2 = source.Subscribe(Print);

        await Task.Delay(2000);
    }

    private void Print<T>(T param)
    {
        Debug.WriteLine(param!.ToString());
    }
}