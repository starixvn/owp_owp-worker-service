using OWP.Infosite.Builder;
using Xunit;

namespace OWP.Infosite.UnitTests.Builder;

public class WatchBetBuilderUnitTests
{
    [Fact]
    public void Build_WBhasValue_ReturnWB()
    {
        var actual = WatchBetBuilder.Build(new Models.SoccerEventSchedule { wb = "WB" });

        Assert.Equal("WB", actual);
    }

    [Fact]
    public void Build_streamSrc1hasValue_ReturnWB()
    {
        var actual = WatchBetBuilder.Build(new Models.SoccerEventSchedule
        {
            create_streaming_src = 1,
            create_streaming_id = "stream1"
        });

        Assert.Equal("1-stream1", actual);
    }

    [Fact]
    public void Build_streamSrc2hasValue_ReturnWB()
    {
        var actual = WatchBetBuilder.Build(new Models.SoccerEventSchedule
        {
            create_streaming2_src = 1,
            create_streaming2_id = "stream2"
        });

        Assert.Equal("1-stream2", actual);
    }
}