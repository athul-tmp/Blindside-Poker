using Blindside.Engine.Settings;

namespace Blindside.Tests.Engine;

public class GameSettingsValidatorTests
{
    private static readonly GameSettings Defaults = new();

    [Fact]
    public void Defaults_AreValid()
    {
        AssertValid(Defaults);
    }

    // ---- Max players: 2 to 10 ----

    [Theory]
    [InlineData(2)]
    [InlineData(10)]
    public void MaxPlayers_AtBounds_IsValid(int value)
    {
        AssertValid(Defaults with { MaxPlayers = value });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(11)]
    public void MaxPlayers_OutOfRange_IsInvalid(int value)
    {
        AssertOnlyError(Defaults with { MaxPlayers = value }, nameof(GameSettings.MaxPlayers));
    }

    // ---- Starting chips: 100 to 100,000 ----
    // The small blind is lowered at 100 chips so the big blind limit (1/5 of chips) still passes.

    [Theory]
    [InlineData(100, 10)]
    [InlineData(100_000, 25)]
    public void StartingChips_AtBounds_IsValid(int chips, int smallBlind)
    {
        AssertValid(Defaults with { StartingChips = chips, StartingSmallBlind = smallBlind });
    }

    [Theory]
    [InlineData(99, 5)]
    [InlineData(100_001, 25)]
    public void StartingChips_OutOfRange_IsInvalid(int chips, int smallBlind)
    {
        AssertOnlyError(Defaults with { StartingChips = chips, StartingSmallBlind = smallBlind }, nameof(GameSettings.StartingChips));
    }

    // ---- Hands per level: 1 to 20 ----

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    public void HandsPerLevel_AtBounds_IsValid(int value)
    {
        AssertValid(Defaults with { HandsPerLevel = value });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void HandsPerLevel_OutOfRange_IsInvalid(int value)
    {
        AssertOnlyError(Defaults with { HandsPerLevel = value }, nameof(GameSettings.HandsPerLevel));
    }

    // ---- Blind increase: 10% to 100% ----

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void BlindIncreasePercent_AtBounds_IsValid(int value)
    {
        AssertValid(Defaults with { BlindIncreasePercent = value });
    }

    [Theory]
    [InlineData(9)]
    [InlineData(101)]
    public void BlindIncreasePercent_OutOfRange_IsInvalid(int value)
    {
        AssertOnlyError(Defaults with { BlindIncreasePercent = value }, nameof(GameSettings.BlindIncreasePercent));
    }

    // ---- Turn timer: 10 to 60 seconds ----

    [Theory]
    [InlineData(10)]
    [InlineData(60)]
    public void TurnTimerSeconds_AtBounds_IsValid(int value)
    {
        AssertValid(Defaults with { TurnTimerSeconds = value });
    }

    [Theory]
    [InlineData(9)]
    [InlineData(61)]
    public void TurnTimerSeconds_OutOfRange_IsInvalid(int value)
    {
        AssertOnlyError(Defaults with { TurnTimerSeconds = value }, nameof(GameSettings.TurnTimerSeconds));
    }

    // ---- Starting small blind: at least 5, a multiple of 5, big blind at most 1/5 of chips ----

    [Theory]
    [InlineData(5)]
    [InlineData(100)] // big blind 200 = exactly 1/5 of 1000 chips
    public void StartingSmallBlind_Allowed_IsValid(int value)
    {
        AssertValid(Defaults with { StartingSmallBlind = value });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]   // below 5
    [InlineData(12)]  // not a multiple of 5
    [InlineData(105)] // big blind 210 is more than 1/5 of 1000 chips
    public void StartingSmallBlind_NotAllowed_IsInvalid(int value)
    {
        AssertOnlyError(Defaults with { StartingSmallBlind = value }, nameof(GameSettings.StartingSmallBlind));
    }

    // ---- Several problems ----

    [Fact]
    public void SeveralBadFields_AreAllReported()
    {
        var settings = Defaults with { MaxPlayers = 1, HandsPerLevel = 0, TurnTimerSeconds = 5 };

        var errors = GameSettingsValidator.Validate(settings);

        Assert.Equal(3, errors.Count);
        Assert.Contains(errors, e => e.Field == nameof(GameSettings.MaxPlayers));
        Assert.Contains(errors, e => e.Field == nameof(GameSettings.HandsPerLevel));
        Assert.Contains(errors, e => e.Field == nameof(GameSettings.TurnTimerSeconds));
        Assert.All(errors, e => Assert.False(string.IsNullOrWhiteSpace(e.Message)));
    }

    private static void AssertValid(GameSettings settings)
    {
        Assert.Empty(GameSettingsValidator.Validate(settings));
    }

    private static void AssertOnlyError(GameSettings settings, string field)
    {
        var error = Assert.Single(GameSettingsValidator.Validate(settings));
        Assert.Equal(field, error.Field);
    }
}
