using Blindside.Engine.Cards;
using Blindside.Engine.Evaluation;
using static Blindside.Tests.Helpers.TestCards;

namespace Blindside.Tests.Engine;

public class HandEvaluatorTests
{
    // ---- Each category, with exact tie-breakers ----

    [Fact]
    public void HighCard()
    {
        AssertHand("Ah Jd 8c 6s 3h 2d 4c", HandCategory.HighCard, [Rank.Ace, Rank.Jack, Rank.Eight, Rank.Six, Rank.Four]);
    }

    [Fact]
    public void Pair()
    {
        AssertHand("Kh Kd Ac 9s 4h 3d 2c", HandCategory.Pair, [Rank.King, Rank.Ace, Rank.Nine, Rank.Four]);
    }

    [Fact]
    public void TwoPair()
    {
        AssertHand("Jh Jd 5c 5s Ah 3d 2c", HandCategory.TwoPair, [Rank.Jack, Rank.Five, Rank.Ace]);
    }

    [Fact]
    public void ThreeOfAKind()
    {
        AssertHand("7h 7d 7c As Th 3d 2c", HandCategory.ThreeOfAKind, [Rank.Seven, Rank.Ace, Rank.Ten]);
    }

    [Fact]
    public void Straight()
    {
        AssertHand("Qh Jd Tc 9s 8h 3d 2c", HandCategory.Straight, [Rank.Queen]);
    }

    [Fact]
    public void Flush()
    {
        AssertHand("Ah Jh 8h 6h 3h Kd 2c", HandCategory.Flush, [Rank.Ace, Rank.Jack, Rank.Eight, Rank.Six, Rank.Three]);
    }

    [Fact]
    public void FullHouse()
    {
        AssertHand("Qh Qd Qc 4s 4h 9d 2c", HandCategory.FullHouse, [Rank.Queen, Rank.Four]);
    }

    [Fact]
    public void FourOfAKind()
    {
        AssertHand("9h 9d 9c 9s Kh 3d 2c", HandCategory.FourOfAKind, [Rank.Nine, Rank.King]);
    }

    [Fact]
    public void StraightFlush()
    {
        AssertHand("9h 8h 7h 6h 5h Kd 2c", HandCategory.StraightFlush, [Rank.Nine]);
    }

    [Fact]
    public void RoyalFlush_IsAceHighStraightFlush()
    {
        AssertHand("Ah Kh Qh Jh Th 3d 2c", HandCategory.StraightFlush, [Rank.Ace]);
    }

    // ---- Straight edge cases ----

    [Fact]
    public void Wheel_IsFiveHighStraight()
    {
        AssertHand("Ah 2d 3c 4s 5h Kd 9c", HandCategory.Straight, [Rank.Five]);
    }

    [Fact]
    public void AceHighStraight()
    {
        AssertHand("Ah Kd Qc Js Th 3d 2c", HandCategory.Straight, [Rank.Ace]);
    }

    [Fact]
    public void Straight_DoesNotWrapAround()
    {
        // Q-K-A-2-3 is not a straight.
        AssertHand("Qh Kd Ac 2s 3h 8d 7c", HandCategory.HighCard, [Rank.Ace, Rank.King, Rank.Queen, Rank.Eight, Rank.Seven]);
    }

    [Fact]
    public void SixCardsInARow_UsesHigherStraight()
    {
        AssertHand("4h 5d 6c 7s 8h 9d Kc", HandCategory.Straight, [Rank.Nine]);
    }

    [Fact]
    public void Straight_WithAPairedCard_IsStillAStraight()
    {
        AssertHand("5h 6d 7c 7s 8h 9d 2c", HandCategory.Straight, [Rank.Nine]);
    }

    // ---- Picking the best 5 of 7 ----

    [Fact]
    public void FlushAndStraight_FlushWins()
    {
        // Straight 5-9 uses 7c and 8d; the five hearts make a flush.
        AssertHand("9h 6h 5h 2h Kh 7c 8d", HandCategory.Flush, [Rank.King, Rank.Nine, Rank.Six, Rank.Five, Rank.Two]);
    }

    [Fact]
    public void SixCardsOfOneSuit_FlushUsesTopFive()
    {
        AssertHand("Ah Jh 9h 7h 4h 2h Kd", HandCategory.Flush, [Rank.Ace, Rank.Jack, Rank.Nine, Rank.Seven, Rank.Four]);
    }

    [Fact]
    public void TwoThreeOfAKinds_MakeFullHouseWithHigherTrips()
    {
        AssertHand("Kh Kd Kc 7s 7h 7d 2c", HandCategory.FullHouse, [Rank.King, Rank.Seven]);
    }

    [Fact]
    public void ThreePairs_UseTopTwo_KickerCanComeFromThirdPair()
    {
        AssertHand("Qh Qd 8c 8s 5h 5d 2c", HandCategory.TwoPair, [Rank.Queen, Rank.Eight, Rank.Five]);
    }

    [Fact]
    public void FourOfAKind_KickerIsHighestRemainingCard()
    {
        // The kicker is the Ace, not the pair of Kings.
        AssertHand("9h 9d 9c 9s Kh Kd Ac", HandCategory.FourOfAKind, [Rank.Nine, Rank.Ace]);
    }

    [Fact]
    public void StraightFlush_BeatsHigherPlainStraight()
    {
        // 6-T is a higher straight, but 5-9 of hearts is a straight flush.
        AssertHand("5h 6h 7h 8h 9h Td 2c", HandCategory.StraightFlush, [Rank.Nine]);
    }

    [Fact]
    public void AllHearts_WheelStraightFlushBeatsAceHighFlush()
    {
        AssertHand("Ah Kh 8h 4h 2h 3h 5h", HandCategory.StraightFlush, [Rank.Five]);
    }

    // ---- Only the best five cards count ----

    [Fact]
    public void SamePair_DifferentSixthAndSeventhCards_Tie()
    {
        var board = "Kh Kd Ac 9s 7h";
        var first = HandEvaluator.Evaluate(Cards($"{board} 5d 2c"));
        var second = HandEvaluator.Evaluate(Cards($"{board} 4d 3c"));

        Assert.Equal(0, first.CompareTo(second));
    }

    [Fact]
    public void BoardPlays_BothPlayersTie()
    {
        var board = "Th Jd Qc Ks Ah";
        var first = HandEvaluator.Evaluate(Cards($"{board} 2c 3d"));
        var second = HandEvaluator.Evaluate(Cards($"{board} 4c 5d"));

        Assert.Equal(HandCategory.Straight, first.Category);
        Assert.Equal(0, first.CompareTo(second));
    }

    // ---- Input checks ----

    [Fact]
    public void FiveCards_AreAccepted()
    {
        AssertHand("Ah Kh Qh Jh Th", HandCategory.StraightFlush, [Rank.Ace]);
    }

    [Fact]
    public void FourCards_Throws()
    {
        Assert.Throws<ArgumentException>(() => HandEvaluator.Evaluate(Cards("Ah Kh Qh Jh")));
    }

    [Fact]
    public void EightCards_Throws()
    {
        Assert.Throws<ArgumentException>(() => HandEvaluator.Evaluate(Cards("Ah Kh Qh Jh Th 9h 8h 7h")));
    }

    private static void AssertHand(string cards, HandCategory category, Rank[] tieBreakers)
    {
        var value = HandEvaluator.Evaluate(Cards(cards));

        Assert.Equal(category, value.Category);
        Assert.Equal(tieBreakers, value.TieBreakers);
    }
}
