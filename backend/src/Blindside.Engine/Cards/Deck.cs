using Blindside.Engine.Randomness;

namespace Blindside.Engine.Cards;

public class Deck
{
    private readonly List<Card> _cards = [];
    private int _next = 0;

    public Deck(IRandom random)
    {
        foreach (var suit in Enum.GetValues<Suit>())
        {
            foreach (var rank in Enum.GetValues<Rank>())
            {
                _cards.Add(new Card(rank, suit));
            }
        }

        Shuffle(random);
    }

    public int Remaining => _cards.Count - _next;

    public Card Deal()
    {
        if (Remaining == 0)
        {
            throw new InvalidOperationException("The deck is empty.");
        }

        var card = _cards[_next];
        _next++;
        return card;
    }

    // Fisher-Yates: walk from the end, swapping each card with a random card
    // at or before it. Next(i + 1) includes i itself, which keeps it fair.
    private void Shuffle(IRandom random)
    {
        for (var i = _cards.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }
}
