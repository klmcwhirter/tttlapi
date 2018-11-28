using Newtonsoft.Json;

namespace tttlapi.Services
{
    /// <summary>
    /// Generic JSON deserializing transformer
    /// </summary>
    /// <typeparam name="T">output type</typeparam>
    public class JsonFromStringTransformer<T> : ITransformer<string,T>
    {
        /// <summary>
        /// Transform input from JSON string to type T
        /// </summary>
        /// <param name="jsonString">JSON string as input</param>
        /// <returns>T</returns>
        public T Transform(string jsonString)
        {
            var rc = JsonConvert.DeserializeObject<T>(jsonString);
            return rc;
        }
    }
}
