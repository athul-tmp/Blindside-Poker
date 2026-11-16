namespace Blindside.Engine.Settings;

// The host's table settings. Values can only be set when the object is created,
// so a started game's copy can't change. GameSettingsValidator checks the ranges.
public record GameSettings
{
    public int MaxPlayers { get; init; } = 6;
    public int StartingChips { get; init; } = 1000;
    public int StartingSmallBlind { get; init; } = 25;
    public int HandsPerLevel { get; init; } = 2;
    public int BlindIncreasePercent { get; init; } = 30;
    public int TurnTimerSeconds { get; init; } = 20;

    public int StartingBigBlind => StartingSmallBlind * 2;
}
