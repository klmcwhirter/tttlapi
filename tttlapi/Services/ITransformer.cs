namespace tttlapi.Services
{
    /// <summary>
    /// Contract for a generic transformer service
    /// </summary>
    /// <typeparam name="TIn">Input Type</typeparam>
    /// <typeparam name="TOut">Output Type</typeparam>
    public interface ITransformer<in TIn, out TOut>
    {
        /// <summary>
        /// Perform transformation on TIn to produce TOut instance
        /// </summary>
        /// <param name="input">TIn</param>
        /// <returns>TOut</returns>
        TOut Transform(TIn input);
    }
}
