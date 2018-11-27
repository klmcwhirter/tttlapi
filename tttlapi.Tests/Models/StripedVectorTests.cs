using System.Linq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using tttlapi.Models;

namespace tttlapi.Tests
{
    public partial class StripedVectorTests
    {
        protected string[] Vector { get; set; }

        [Fact]
        public void CanConstruct()
        {
            new StripedVector<string>(new[] { new[] { 0 } }, new[] { "" });
        }

        [Fact]
        public void CanReturnNoStripes()
        {
            var stripeDef = new[] { new int[0] };
            var vector = Enumerable.Range(0, 9).Select(i => i.ToString());

            var sv = new StripedVector<string>(stripeDef, vector);

            Assert.Single(sv);
            Assert.Collection(sv, s => Assert.Empty(s));
        }

        [Fact]
        public void CanReturnSingleStripe()
        {
            var stripeDef = new[] { new[] { 0, 1, 2 } };
            var vector = Enumerable.Range(0, 9).Select(i => i.ToString());

            var sv = new StripedVector<string>(stripeDef, vector);

            Assert.Single(sv);

            var ct = 0;
            sv.ForEach(s =>
            {
                Assert.All(s, e => Assert.Equal(e, stripeDef[ct][int.Parse(e)].ToString()));
            });
        }

        [Fact]
        public void ReturnsAllStripes()
        {
            var stripeDef = new[]
            {
                // rows
                new[] { 0, 1, 2 },
                new[] { 3, 4, 5 },
                new[] { 6, 7, 8 },
                
                // cols
                new[] { 0, 3, 6 },
                new[] { 1, 4, 7 },
                new[] { 2, 5, 8 },
                
                // diagonals
                new[] { 0, 4, 8 },
                new[] { 2, 4, 6 }
            };
            var vector = Enumerable.Range(0, 9).Select(i => i.ToString());

            var sv = new StripedVector<string>(stripeDef, vector);

            Assert.Equal(stripeDef.Length, sv.Count);

            var ct = 0;
            Assert.All(sv,
                items => Assert.Collection(stripeDef[ct++],
                                            e => Assert.Equal(e.ToString(), items[0]),
                                            e => Assert.Equal(e.ToString(), items[1]),
                                            e => Assert.Equal(e.ToString(), items[2])
                                    )
            );
        }
    }
}
