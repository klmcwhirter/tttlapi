using Newtonsoft.Json;

namespace tttlapi.Services
{
    /// <summary>
    /// Generic JSON serializing transformer
    /// </summary>
    /// <typeparam name="T">input type</typeparam>
    public class JsonToStringTransformer<T> : ITransformer<T,string>
    {
        /// <summary>
        /// Transform input to JSON string
        /// </summary>
        /// <param name="input">T</param>
        /// <returns>JSON string</returns>
        public string Transform(T input)
        {
            var rc = JsonConvert.SerializeObject(input);
            return rc;
        }
    }
}
