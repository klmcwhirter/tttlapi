#pragma warning disable CS1572, CS1573, CS1591
using System;

namespace tttlapi
{
    public static class IntExtentions
    {
        static Random RandomInstance { get; set; } = new Random((int)DateTime.UtcNow.Ticks);

        public static int Random(this int i)
        {
            var rc = RandomInstance.Next(i);
            return rc;
        }
    }
}
