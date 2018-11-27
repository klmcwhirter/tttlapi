using System.Collections.Generic;
using System.Linq;

namespace tttlapi.Models
{
    /// <summary>
    /// Indexes IEnumerable&lt;T&gt; using stripes
    /// </summary>
    /// <typeparam name="T">The type present in the IEnumerable</typeparam>
    public class StripedVector<T> : List<T[]>
    {
        /// <summary>
        /// Contructs the StripedVector
        /// </summary>
        /// <param name="stripeDefs">definitions of the stripes as arrays of indices</param>
        /// <param name="collection">vector to stripe</param>
        public StripedVector(IEnumerable<int[]> stripeDefs, IEnumerable<T> collection)
        {
            var list = collection.ToList();

            foreach (var stripeDef in stripeDefs)
            {
                var item = new List<T>(stripeDef.Length);
                foreach (var idx in stripeDef)
                {
                    item.Add(list[idx]);
                }
                Add(item.ToArray());
            }
        }
    }
}