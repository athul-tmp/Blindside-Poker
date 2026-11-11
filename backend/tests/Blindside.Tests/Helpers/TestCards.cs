using Blindside.Engine.Cards;

namespace Blindside.Tests.Helpers;

// Lets tests write hands as text, e.g. Cards("As Kd Th 2c").
// Each card is a rank (2-9, T, J, Q, K, A) followed by a suit (s, h, d, c).
public static class TestCards
{
    public static List<Card> Cards(string text)
    {
        var cards = new List<Card>();
        foreach (var token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            cards.Add(ParseCard(token));
        }
        return cards;
    }

    private static Card ParseCard(string token)
    {
        if (token.Length != 2)
        {
            throw new ArgumentException($"Card must be 2 characters: '{token}'");
        }

        return new Card(ParseRank(token[0]), ParseSuit(token[1]));
    }

    private static Rank ParseRank(char c) => c switch
    {
        >= '2' and <= '9' => (Rank)(c - '0'),
        'T' => Rank.Ten,
        'J' => Rank.Jack,
        'Q' => Rank.Queen,
        'K' => Rank.King,
        'A' => Rank.Ace,
        _ => throw new ArgumentException($"Unknown rank: '{c}'")
    };

    private static Suit ParseSuit(char c) => c switch
    {
        's' => Suit.Spades,
        'h' => Suit.Hearts,
        'd' => Suit.Diamonds,
        'c' => Suit.Clubs,
        _ => throw new ArgumentException($"Unknown suit: '{c}'")
    };
}
