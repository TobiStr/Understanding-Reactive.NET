using NUnit.Framework;

namespace Understanding.Reactive.NET;

[TestFixture]
[Category("Unit")]
internal class ErrorHandlingTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}