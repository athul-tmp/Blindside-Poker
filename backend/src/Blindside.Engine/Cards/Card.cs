namespace Blindside.Engine.Cards;

public readonly record struct Card(Rank Rank, Suit Suit)
{
    private char SuitSymbol => Suit switch
    {
        Suit.Clubs => '♣',
        Suit.Diamonds => '♦',
        Suit.Hearts => '♥',
        Suit.Spades => '♠',
        _ => throw new ArgumentOutOfRangeException()
    };

    private char RankChar => Rank switch
    {
        >= Rank.Two and <= Rank.Nine => (char)('0' + (int)Rank),
        Rank.Ten => 'T',
        Rank.Jack => 'J',
        Rank.Queen => 'Q',
        Rank.King => 'K',
        Rank.Ace => 'A',
        _ => throw new ArgumentOutOfRangeException()
    };

    public override string ToString() => $"{RankChar}{SuitSymbol}";
}
