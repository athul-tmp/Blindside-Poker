namespace Blindside.Engine.Randomness;

public class SystemRandom : IRandom
{
    // a private readonly field holding a System.Random
    private readonly Random _random;

    // constructor 1: no seed - truly random
    public SystemRandom()
    {
        _random = new Random();
    }

    // constructor 2: takes int seed - same sequence every time
    public SystemRandom(int seed)
    {
        _random = new Random(seed);
    }

    // Next(int maxExclusive): pass through to the wrapped Random
    public int Next(int maxExclusive) => _random.Next(maxExclusive);
}
