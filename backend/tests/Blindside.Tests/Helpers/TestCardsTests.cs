using Blindside.Engine.Cards;
using static Blindside.Tests.Helpers.TestCards;

namespace Blindside.Tests.Helpers;

public class TestCardsTests
{
    [Theory]
    [InlineData("As", Rank.Ace, Suit.Spades)]
    [InlineData("Td", Rank.Ten, Suit.Diamonds)]
    [InlineData("2c", Rank.Two, Suit.Clubs)]
    [InlineData("Kh", Rank.King, Suit.Hearts)]
    public void Cards_ParsesSingleCard(string text, Rank rank, Suit suit)
    {
        Assert.Equal([new Card(rank, suit)], Cards(text));
    }

    [Fact]
    public void Cards_ParsesSeveralCardsInOrder()
    {
        var expected = new List<Card>
        {
            new(Rank.Ace, Suit.Spades),
            new(Rank.King, Suit.Diamonds),
            new(Rank.Nine, Suit.Hearts),
        };

        Assert.Equal(expected, Cards("As Kd 9h"));
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Ax")]
    [InlineData("1s")]
    [InlineData("10s")]
    [InlineData("as")]
    public void Cards_InvalidCard_Throws(string text)
    {
        Assert.Throws<ArgumentException>(() => Cards(text));
    }
}
