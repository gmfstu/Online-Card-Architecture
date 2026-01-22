namespace GoldenTests
{
    /// <summary>
    /// Represents the result of a single test
    /// </summary>
    public class TestResult
    {
        public bool Passed { get; }
        public string Message { get; }

        public TestResult(bool passed, string message)
        {
            Passed = passed;
            Message = message;
        }
    }
}
