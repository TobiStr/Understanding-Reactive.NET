using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Understanding.Reactive.NET
{

    internal class PackageReferences
    {
        //These interfaces are in the standard Package ("System" namespace), which comes with Microsoft.NET.Sdk
        public IObservable<long> observable;
        public IObserver<long> observer;

        [SetUp]
        public void Setup()
        {
            //The Observable class is in the Nuget-Package "System.Reactive" under the "System.Reactive.Linq" namespace.
            //It features many static creation methods
            this.observable = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1));

            // Static Observer Creation and Extension Methods are under "System.Reactive".
            this.observer = Observer.Create<long>(
                next => DoNothing(),
                error => DoNothing(),
                () => DoNothing()
            );

            this.observable.Subscribe(observer);

            // The short creation of an Observer inside the Subscribe function is under "System".
            this.observable.Subscribe(
                next => DoNothing(),
                error => DoNothing(),
                () => DoNothing()
            );
        }

        [Test]
        public async Task ReactiveLinqTest()
        {
            // Under "System.Reactive.Linq" you can find all extensions for Observables,
            // including transformations to awaitable tasks.
            var first = await this.observable
                .Select(i => i * 2)
                .FirstAsync();

            Assert.That(first == 0);
        }

        [Test]
        public async Task EnumerableExtensionsTest()
        {
            var n = 0;

            // Useful Enumerable Extensions are in the Nuget-Package "System.Interactive" under the "System.Linq" namespace.
            Enumerable.Range(0, 10).Do(x => n += x).ForEach(_ => { });
            Assert.Equals(45, n);
        }

        [Test]
        public async Task AsyncEnumerablesTest()
        {
            // Async Enumerable Extensions are in the Nuget-Package "System.Interactive.Async" under the "System.Linq" namespace.
            var res = new[] { 1, 2, 3, 4 }.ToAsyncEnumerable().AnyAsync(x => x % 2 == 0);
            Assert.True(await res);
        }

        private void DoNothing()
        {

        }

    }
}
