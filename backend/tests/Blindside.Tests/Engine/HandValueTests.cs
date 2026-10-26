using Blindside.Engine.Cards;
using Blindside.Engine.Evaluation;

namespace Blindside.Tests.Engine;

public class HandValueTests
{
    [Fact]
    public void Categories_AreOrderedWeakestToStrongest()
    {
        HandCategory[] expected =
        [
            HandCategory.HighCard,
            HandCategory.Pair,
            HandCategory.TwoPair,
            HandCategory.ThreeOfAKind,
            HandCategory.Straight,
            HandCategory.Flush,
            HandCategory.FullHouse,
            HandCategory.FourOfAKind,
            HandCategory.StraightFlush,
        ];

        Assert.Equal(expected, Enum.GetValues<HandCategory>());
    }

    [Fact]
    public void HigherCategory_Wins_EvenWithLowerRanks()
    {
        var pairOfTwos = new HandValue(HandCategory.Pair, [Rank.Two, Rank.Five, Rank.Four, Rank.Three]);
        var aceHigh = new HandValue(HandCategory.HighCard, [Rank.Ace, Rank.King, Rank.Queen, Rank.Jack, Rank.Nine]);

        AssertBeats(pairOfTwos, aceHigh);
    }

    [Fact]
    public void SameCategory_FirstTieBreaker_Decides()
    {
        var pairOfKings = new HandValue(HandCategory.Pair, [Rank.King, Rank.Five, Rank.Four, Rank.Three]);
        var pairOfQueens = new HandValue(HandCategory.Pair, [Rank.Queen, Rank.Ace, Rank.Jack, Rank.Ten]);

        AssertBeats(pairOfKings, pairOfQueens);
    }

    [Fact]
    public void SamePair_Kicker_Decides()
    {
        var kingsAceKicker = new HandValue(HandCategory.Pair, [Rank.King, Rank.Ace, Rank.Four, Rank.Three]);
        var kingsQueenKicker = new HandValue(HandCategory.Pair, [Rank.King, Rank.Queen, Rank.Jack, Rank.Ten]);

        AssertBeats(kingsAceKicker, kingsQueenKicker);
    }

    [Fact]
    public void HighCard_LastKicker_Decides()
    {
        var withFour = new HandValue(HandCategory.HighCard, [Rank.Ace, Rank.King, Rank.Nine, Rank.Seven, Rank.Four]);
        var withThree = new HandValue(HandCategory.HighCard, [Rank.Ace, Rank.King, Rank.Nine, Rank.Seven, Rank.Three]);

        AssertBeats(withFour, withThree);
    }

    [Fact]
    public void Wheel_LosesToSixHighStraight()
    {
        var wheel = new HandValue(HandCategory.Straight, [Rank.Five]);
        var sixHigh = new HandValue(HandCategory.Straight, [Rank.Six]);

        AssertBeats(sixHigh, wheel);
    }

    [Fact]
    public void IdenticalValues_Tie()
    {
        var first = new HandValue(HandCategory.TwoPair, [Rank.Jack, Rank.Five, Rank.Ace]);
        var second = new HandValue(HandCategory.TwoPair, [Rank.Jack, Rank.Five, Rank.Ace]);

        Assert.Equal(0, first.CompareTo(second));
        Assert.Equal(0, second.CompareTo(first));
    }

    // Checks both directions, so a CompareTo that always returns 1 can't pass.
    private static void AssertBeats(HandValue winner, HandValue loser)
    {
        Assert.True(winner.CompareTo(loser) > 0, $"expected {winner} to beat {loser}");
        Assert.True(loser.CompareTo(winner) < 0, $"expected {loser} to lose to {winner}");
    }
}
