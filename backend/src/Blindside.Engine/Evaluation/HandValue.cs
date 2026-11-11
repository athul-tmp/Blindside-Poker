using Blindside.Engine.Cards;

namespace Blindside.Engine.Evaluation;

public class HandValue : IComparable<HandValue>
{
    public HandCategory Category { get; }
    public IReadOnlyList<Rank> TieBreakers { get; }

    public HandValue(HandCategory category, IReadOnlyList<Rank> tieBreakers)
    {
        Category = category;
        TieBreakers = tieBreakers;
    }

    public int CompareTo(HandValue? other)
    {
        if (other is null) return 1;

        var categoryComparison = Category.CompareTo(other.Category);
        if (categoryComparison != 0) return categoryComparison;

        for (var i = 0; i < TieBreakers.Count; i++)
        {
            var rankComparison = TieBreakers[i].CompareTo(other.TieBreakers[i]);
            if (rankComparison != 0) return rankComparison;
        }
        return 0;
    }

    public override string ToString() => $"{Category} {string.Join(", ", TieBreakers)}";
}
