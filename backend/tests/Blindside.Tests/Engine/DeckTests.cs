using Blindside.Engine.Cards;
using Blindside.Engine.Randomness;

namespace Blindside.Tests.Engine;

public class DeckTests
{
    [Fact]
    public void NewDeck_Has52Cards()
    {
        var deck = new Deck(new SystemRandom(seed: 1));

        Assert.Equal(52, deck.Remaining);
    }

    [Fact]
    public void DealingAll52_GivesEveryCardExactlyOnce()
    {
        var deck = new Deck(new SystemRandom(seed: 1));

        var dealt = DealAll(deck);

        Assert.Equal(52, dealt.Distinct().Count());
        Assert.Equal(AllCardsInOrder().ToHashSet(), dealt.ToHashSet());
    }

    [Fact]
    public void Deal_ReducesRemaining()
    {
        var deck = new Deck(new SystemRandom(seed: 1));

        deck.Deal();
        deck.Deal();

        Assert.Equal(50, deck.Remaining);
    }

    [Fact]
    public void Deal_OnEmptyDeck_Throws()
    {
        var deck = new Deck(new SystemRandom(seed: 1));
        DealAll(deck);

        Assert.Throws<InvalidOperationException>(() => deck.Deal());
    }

    [Fact]
    public void SameSeed_GivesSameOrder()
    {
        var first = DealAll(new Deck(new SystemRandom(seed: 42)));
        var second = DealAll(new Deck(new SystemRandom(seed: 42)));

        Assert.Equal(first, second);
    }

    [Fact]
    public void DifferentSeeds_GiveDifferentOrders()
    {
        var first = DealAll(new Deck(new SystemRandom(seed: 1)));
        var second = DealAll(new Deck(new SystemRandom(seed: 2)));

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void NewDeck_IsShuffled()
    {
        var dealt = DealAll(new Deck(new SystemRandom(seed: 1)));

        Assert.NotEqual(AllCardsInOrder(), dealt);
    }

    private static List<Card> DealAll(Deck deck)
    {
        var cards = new List<Card>();
        while (deck.Remaining > 0)
        {
            cards.Add(deck.Deal());
        }
        return cards;
    }

    private static List<Card> AllCardsInOrder()
    {
        var cards = new List<Card>();
        foreach (var suit in Enum.GetValues<Suit>())
        {
            foreach (var rank in Enum.GetValues<Rank>())
            {
                cards.Add(new Card(rank, suit));
            }
        }
        return cards;
    }
}
