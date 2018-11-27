using tttlapi.Models;
using Xunit;

namespace tttlapi.Strategies
{
    public class HumanPlayerStrategyTests
    {
        [Fact]
        public void CanConstruct()
        {
            new HumanPlayerStrategy();
        }

        [Fact]
        public void CanAutomateTurnReturnsFalse()
        {
            var strategy = new HumanPlayerStrategy();

            var rc = strategy.CanAutomateTurn();

            Assert.False(rc);
        }
        [Fact]
        public void AutomateTurnReturnsNull()
        {
            var strategy = new HumanPlayerStrategy();

            var rc = strategy.AutomateTurn(PlayerIndex.X, null);

            Assert.Null(rc);
        }
    }
}