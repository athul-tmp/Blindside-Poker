using Blindside.Engine.Settings;

namespace Blindside.Engine.Blinds;

// Works out the blinds for any hand. Each level raises the small blind by the
// host's percentage, rounded to the nearest 5 and always by at least 5, until the
// big blind covers every chip at the table (everyone is forced all-in), then holds.
// Expects settings that have passed GameSettingsValidator.
public class BlindSchedule
{
    private readonly GameSettings _settings;

    public BlindSchedule(GameSettings settings)
    {
        _settings = settings;
    }

    // Hand numbers start at 1.
    public BlindLevel ForHand(int handNumber)
    {
        if (handNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(handNumber), "Hand numbers start at 1.");
        }

        var levelNumber = (handNumber - 1) / _settings.HandsPerLevel + 1;
        var allChips = _settings.MaxPlayers * _settings.StartingChips;

        var smallBlind = _settings.StartingSmallBlind;
        for (var level = 2; level <= levelNumber; level++)
        {
            // Everyone is already forced all-in, so growing further changes nothing
            if (smallBlind * 2 >= allChips) break;

            smallBlind = NextSmallBlind(smallBlind);
        }

        return new BlindLevel(levelNumber, smallBlind);
    }

    private int NextSmallBlind(int smallBlind)
    {
        var step = GameSettingsValidator.BlindStep;

        // decimal keeps 7.5 exact; AwayFromZero rounds halves up (7.5 → 10)
        var exactIncrease = smallBlind * _settings.BlindIncreasePercent / 100m;
        var roundedIncrease = (int)Math.Round(exactIncrease / step, MidpointRounding.AwayFromZero) * step;

        return smallBlind + Math.Max(roundedIncrease, step);
    }
}
