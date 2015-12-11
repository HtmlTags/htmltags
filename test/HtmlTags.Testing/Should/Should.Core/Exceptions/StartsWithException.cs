namespace Should.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a collection unexpectedly does not contain the expected value.
    /// </summary>
    public class StartsWithException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ContainsException"></see> class.
        /// </summary>
        /// <param name="expectedStartString">The expected object value</param>
        /// <param name="actual">The actual object value</param>
        public StartsWithException(object expectedStartString, object actual)
            : base($"Assert.StartsWith() failure: '{expectedStartString}' not found at the beginning of '{actual}'") { }
    }
    /// <summary>
    /// Exception thrown when a collection unexpectedly does not contain the expected value.
    /// </summary>
    public class EndsWithException : AssertException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ContainsException"></see> class.
        /// </summary>
        /// <param name="expectedEndString">The expected object value</param>
        /// <param name="actual">The actual object value</param>
        public EndsWithException(object expectedEndString, object actual)
            : base($"Assert.EndsWith() failure: '{expectedEndString}' not found at the end of '{actual}'") { }
    }
}
