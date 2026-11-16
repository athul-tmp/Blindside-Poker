namespace Blindside.Engine.Blinds;

public record BlindLevel(int Number, int SmallBlind)
{
    public int BigBlind => SmallBlind * 2;
}
