namespace Blindside.Engine.Settings;

// The one place that decides which settings are allowed. The server enforces it;
// the client form only mirrors these rules. Reports at most one error per field.
public static class GameSettingsValidator
{
    public const int MinPlayers = 2;
    public const int MaxPlayers = 10;
    public const int MinStartingChips = 100;
    public const int MaxStartingChips = 100_000;
    public const int MinHandsPerLevel = 1;
    public const int MaxHandsPerLevel = 20;
    public const int MinBlindIncreasePercent = 10;
    public const int MaxBlindIncreasePercent = 100;
    public const int MinTurnTimerSeconds = 10;
    public const int MaxTurnTimerSeconds = 60;
    public const int BlindStep = 5;

    public static IReadOnlyList<ValidationError> Validate(GameSettings settings)
    {
        var errors = new List<ValidationError>();

        CheckRange(errors, nameof(GameSettings.MaxPlayers), "Max players",
            settings.MaxPlayers, MinPlayers, MaxPlayers);
        CheckRange(errors, nameof(GameSettings.StartingChips), "Starting chips",
            settings.StartingChips, MinStartingChips, MaxStartingChips);
        CheckRange(errors, nameof(GameSettings.HandsPerLevel), "Hands per level",
            settings.HandsPerLevel, MinHandsPerLevel, MaxHandsPerLevel);
        CheckRange(errors, nameof(GameSettings.BlindIncreasePercent), "Blind increase percent",
            settings.BlindIncreasePercent, MinBlindIncreasePercent, MaxBlindIncreasePercent);
        CheckRange(errors, nameof(GameSettings.TurnTimerSeconds), "Turn timer seconds",
            settings.TurnTimerSeconds, MinTurnTimerSeconds, MaxTurnTimerSeconds);

        // else-if so the small blind gets at most one message
        var smallBlindField = nameof(GameSettings.StartingSmallBlind);
        if (settings.StartingSmallBlind < BlindStep)
        {
            errors.Add(new ValidationError(smallBlindField, $"Starting small blind must be at least {BlindStep}."));
        }
        else if (settings.StartingSmallBlind % BlindStep != 0)
        {
            errors.Add(new ValidationError(smallBlindField, $"Starting small blind must be a multiple of {BlindStep}."));
        }
        else if (settings.StartingBigBlind * 5 > settings.StartingChips)
        {
            // BB * 5 > chips means the same as BB > chips / 5, without having to think about integer division
            errors.Add(new ValidationError(smallBlindField, "Starting big blind can be at most 1/5 of the starting chips."));
        }

        return errors;
    }

    private static void CheckRange(List<ValidationError> errors, string field, string label, int value, int min, int max)
    {
        if (value < min || value > max)
        {
            errors.Add(new ValidationError(field, $"{label} must be between {min} and {max}."));
        }
    }
}
