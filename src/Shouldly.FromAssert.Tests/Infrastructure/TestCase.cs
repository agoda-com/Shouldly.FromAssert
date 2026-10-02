namespace Shouldly.FromAssert.Tests.Infrastructure;

public class TestCase
{
    public string NUnitAssertion { get; set; }
    public string ShouldlyAssertion { get; set; }
    public int Line { get; set; }
    public int StartColumn { get; set; }
    public int EndColumn { get; set; }
    public string SetupCode { get; set; }
}
