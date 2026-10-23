using Blindside.Engine.Cards;

namespace Blindside.Tests.Engine;

public class CardTests
{
    [Theory]
    [InlineData(Rank.Ace, Suit.Spades, "A♠")]
    [InlineData(Rank.Ten, Suit.Hearts, "T♥")]
    [InlineData(Rank.Two, Suit.Diamonds, "2♦")]
    [InlineData(Rank.King, Suit.Clubs, "K♣")]
    public void ToString_ShowsRankAndSuitSymbol(Rank rank, Suit suit, string expected)
    {
        var card = new Card(rank, suit);

        Assert.Equal(expected, card.ToString());
    }

    [Fact]
    public void Cards_WithSameRankAndSuit_AreEqual()
    {
        Assert.Equal(new Card(Rank.Queen, Suit.Hearts), new Card(Rank.Queen, Suit.Hearts));
    }

    [Fact]
    public void Ranks_AreOrderedTwoLowAceHigh()
    {
        Assert.True(Rank.Ace > Rank.King);
        Assert.True(Rank.Three > Rank.Two);
        Assert.Equal(14, (int)Rank.Ace);
        Assert.Equal(2, (int)Rank.Two);
    }
}
