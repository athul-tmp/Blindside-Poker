using Blindside.Engine.Cards;

namespace Blindside.Engine.Evaluation;

public static class HandEvaluator
{
    public static HandValue Evaluate(IReadOnlyList<Card> cards)
    {
        // Showdown is always 2 hole cards + 5 board cards
        if (cards.Count != 7) throw new ArgumentException("A showdown hand needs exactly 7 cards.");

        // Try all 21 five-card hands (leave out 2 of the 7) and keep the best
        HandValue? best = null;

        for (var skip1 = 0; skip1 < cards.Count; skip1++)
        {
            for (var skip2 = skip1 + 1; skip2 < cards.Count; skip2++)
            {
                // Every card except the two left out
                var five = new List<Card>();
                for (var i = 0; i < cards.Count; i++)
                {
                    if (i != skip1 && i != skip2)
                    {
                        five.Add(cards[i]);
                    }
                }

                var value = EvaluateFive(five);
                if (best is null || value.CompareTo(best) > 0) best = value;
            }
        }

        return best!;
    }

    private static HandValue EvaluateFive(List<Card> five)
    {
        // Ranks from high to low
        var ranks = new List<Rank>();
        foreach (var card in five)
        {
            ranks.Add(card.Rank);
        }
        ranks.Sort();
        ranks.Reverse();

        var isFlush = true;
        foreach (var card in five)
        {
            if (card.Suit != five[0].Suit) isFlush = false;
        }

        // Group by rank, biggest group first, then highest rank first.
        // e.g. Q Q Q 4 4 → counts [3, 2], groupRanks [Queen, Four]
        var groups = five.GroupBy(c => c.Rank)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key)
            .ToList();
        var counts = groups.Select(g => g.Count()).ToList();
        var groupRanks = groups.Select(g => g.Key).ToList();

        var straightTop = StraightTop(ranks, counts);

        // Check categories from strongest to weakest; the first match wins
        if (straightTop is not null && isFlush) return new HandValue(HandCategory.StraightFlush, [straightTop.Value]);
        if (counts is [4, 1]) return new HandValue(HandCategory.FourOfAKind, groupRanks);
        if (counts is [3, 2]) return new HandValue(HandCategory.FullHouse, groupRanks);
        if (isFlush) return new HandValue(HandCategory.Flush, ranks);
        if (straightTop is not null) return new HandValue(HandCategory.Straight, [straightTop.Value]);
        if (counts is [3, 1, 1]) return new HandValue(HandCategory.ThreeOfAKind, groupRanks);
        if (counts is [2, 2, 1]) return new HandValue(HandCategory.TwoPair, groupRanks);
        if (counts is [2, 1, 1, 1]) return new HandValue(HandCategory.Pair, groupRanks);
        return new HandValue(HandCategory.HighCard, ranks);
    }

    // Returns the top card of a straight, or null if it isn't one.
    // ranks must be sorted high to low.
    private static Rank? StraightTop(List<Rank> ranks, List<int> counts)
    {
        // A straight needs five different ranks
        if (counts is not [1, 1, 1, 1, 1]) return null;

        // Five different ranks spanning exactly 4 are consecutive
        if ((int)ranks[0] - (int)ranks[4] == 4) return ranks[0];

        // The wheel (A-2-3-4-5): the Ace plays low, so Five is the top card
        if (ranks is [Rank.Ace, Rank.Five, Rank.Four, Rank.Three, Rank.Two]) return Rank.Five;

        return null;
    }
}
