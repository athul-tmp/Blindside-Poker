using Blindside.Engine.Settings;

namespace Blindside.Tests.Engine;

public class GameSettingsTests
{
    [Fact]
    public void Defaults_MatchHostSettingsSpec()
    {
        var settings = new GameSettings();

        Assert.Equal(6, settings.MaxPlayers);
        Assert.Equal(1000, settings.StartingChips);
        Assert.Equal(25, settings.StartingSmallBlind);
        Assert.Equal(50, settings.StartingBigBlind);
        Assert.Equal(2, settings.HandsPerLevel);
        Assert.Equal(30, settings.BlindIncreasePercent);
        Assert.Equal(20, settings.TurnTimerSeconds);
    }

    [Fact]
    public void BigBlind_IsAlwaysTwiceSmallBlind()
    {
        var settings = new GameSettings { StartingSmallBlind = 40 };

        Assert.Equal(80, settings.StartingBigBlind);
    }
}
