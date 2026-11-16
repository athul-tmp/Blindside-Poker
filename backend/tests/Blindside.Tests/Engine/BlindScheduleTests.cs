using Blindside.Engine.Blinds;
using Blindside.Engine.Settings;

namespace Blindside.Tests.Engine;

public class BlindScheduleTests
{
    [Fact]
    public void FirstHand_UsesStartingBlinds()
    {
        var level = new BlindSchedule(new GameSettings()).ForHand(1);

        Assert.Equal(1, level.Number);
        Assert.Equal(25, level.SmallBlind);
        Assert.Equal(50, level.BigBlind);
    }

    [Theory]
    [InlineData(2, 1, 1)]
    [InlineData(2, 2, 1)]
    [InlineData(2, 3, 2)]
    [InlineData(2, 7, 4)]
    [InlineData(5, 5, 1)]
    [InlineData(5, 6, 2)]
    [InlineData(1, 3, 3)]
    public void HandNumber_MapsToLevel(int handsPerLevel, int handNumber, int expectedLevel)
    {
        var schedule = new BlindSchedule(new GameSettings { HandsPerLevel = handsPerLevel });

        Assert.Equal(expectedLevel, schedule.ForHand(handNumber).Number);
    }

    [Theory]
    [InlineData(1, 25)]
    [InlineData(2, 35)]  // 25 + 7.5, rounded up to 10
    [InlineData(3, 45)]  // 35 + 10.5, rounded to 10
    [InlineData(4, 60)]  // 45 + 13.5, rounded to 15
    [InlineData(5, 80)]
    [InlineData(6, 105)]
    [InlineData(7, 135)]
    public void DefaultSettings_SmallBlindPerLevel(int levelNumber, int expectedSmallBlind)
    {
        var settings = new GameSettings();

        var level = LevelAt(settings, levelNumber);

        Assert.Equal(expectedSmallBlind, level.SmallBlind);
        Assert.Equal(expectedSmallBlind * 2, level.BigBlind);
    }

    [Fact]
    public void TinyIncrease_StillGrowsByAtLeastOneStep()
    {
        // 10% of 5 is 0.5, which rounds to 0, so the minimum step of 5 applies.
        var settings = new GameSettings { StartingSmallBlind = 5, BlindIncreasePercent = 10 };

        Assert.Equal(5, LevelAt(settings, 1).SmallBlind);
        Assert.Equal(10, LevelAt(settings, 2).SmallBlind);
        Assert.Equal(15, LevelAt(settings, 3).SmallBlind);
        Assert.Equal(20, LevelAt(settings, 4).SmallBlind);
    }

    [Theory]
    [InlineData(6, 1000, 25, 2, 30)]       // defaults
    [InlineData(10, 100_000, 5, 20, 10)]   // slowest legal settings
    [InlineData(2, 100, 10, 1, 100)]       // fastest legal settings
    [InlineData(6, 5000, 50, 3, 25)]
    public void BigBlind_GrowsEveryLevel_UntilEveryoneIsAllIn(
        int maxPlayers, int startingChips, int smallBlind, int handsPerLevel, int increasePercent)
    {
        var settings = new GameSettings
        {
            MaxPlayers = maxPlayers,
            StartingChips = startingChips,
            StartingSmallBlind = smallBlind,
            HandsPerLevel = handsPerLevel,
            BlindIncreasePercent = increasePercent,
        };
        var allChips = maxPlayers * startingChips;

        var previous = LevelAt(settings, 1);
        var levelNumber = 1;
        while (previous.BigBlind < allChips)
        {
            levelNumber++;
            Assert.True(levelNumber < 1000, "blinds never reached the all-in point");

            var next = LevelAt(settings, levelNumber);
            Assert.True(next.BigBlind > previous.BigBlind,
                $"level {levelNumber} big blind {next.BigBlind} did not grow from {previous.BigBlind}");
            previous = next;
        }
    }

    [Fact]
    public void OnceEveryoneIsAllIn_BlindsStopGrowing()
    {
        // Defaults: 6 players x 1000 chips. The big blind first reaches 6000+ at level 19.
        var settings = new GameSettings();

        var atAllIn = LevelAt(settings, 19);
        var after = LevelAt(settings, 20);

        Assert.True(LevelAt(settings, 18).BigBlind < 6000);
        Assert.True(atAllIn.BigBlind >= 6000);
        Assert.Equal(atAllIn.BigBlind, after.BigBlind);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void HandNumberBelowOne_Throws(int handNumber)
    {
        var schedule = new BlindSchedule(new GameSettings());

        Assert.Throws<ArgumentOutOfRangeException>(() => schedule.ForHand(handNumber));
    }

    // The first hand of the given level.
    private static BlindLevel LevelAt(GameSettings settings, int levelNumber)
    {
        var firstHand = (levelNumber - 1) * settings.HandsPerLevel + 1;
        return new BlindSchedule(settings).ForHand(firstHand);
    }
}
