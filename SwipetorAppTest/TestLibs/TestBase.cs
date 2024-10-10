using Xunit.Abstractions;

namespace SwipetorAppTest.TestLibs;

public class TestBase
{
    protected readonly ITestOutputHelper Output;

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
        TestGlobals.Initialize(Output);
    }
}