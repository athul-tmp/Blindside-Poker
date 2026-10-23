using Blindside.Engine.Randomness;

namespace Blindside.Tests.Engine;

public class SystemRandomTests
{
    [Fact]
    public void SameSeed_GivesSameSequence()
    {
        var first = new SystemRandom(seed: 42);
        var second = new SystemRandom(seed: 42);

        for (var i = 0; i < 100; i++)
        {
            Assert.Equal(first.Next(52), second.Next(52));
        }
    }

    [Fact]
    public void Next_StaysWithinRange()
    {
        var random = new SystemRandom(seed: 7);

        for (var i = 0; i < 1000; i++)
        {
            var value = random.Next(10);
            Assert.InRange(value, 0, 9);
        }
    }
}
