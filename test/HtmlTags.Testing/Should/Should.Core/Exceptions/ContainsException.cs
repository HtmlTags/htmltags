namespace Should.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a collection unexpectedly does not contain the expected value.
    /// </summary>
    public class ContainsException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ContainsException"/> class.
        /// </summary>
        /// <param name="expected">The expected object value</param>
        /// <param name="actual">The actual object value</param>
        public ContainsException(object expected, object actual)
            : base($"Assert.Contains() failure: Not found: {expected} in {actual}") { }
    }
}