using OWP.Infosite.Builder;
using OWP.Infosite.Models;
using Xunit;

namespace OWP.Infosite.UnitTests.Builder;

public class CoverageBuilderUnitTests
{
    private readonly CoverageBuilder coverageBuilder;

    public CoverageBuilderUnitTests()
    {
        coverageBuilder = new CoverageBuilder();
    }

    [Fact]
    public void BuildCoverage_ReturnCoverage()
    {
        var actual = coverageBuilder.BuildCoverage(1, 2);

        Assert.Equal("TV", actual);
    }

    [Fact]
    public void BuildCoverageMergeMatch_ReturnCoverage()
    {
        var actual = coverageBuilder.BuildCoverageMergeMatch(new CommonMatchModel
        {
            ls_merge_match = new List<CommonSubMatch>
            {
                new() {website_id = 17, coverage_id = 1},
                new() {website_id = 2, coverage_id = 2}
            }
        });
        var expected = new List<string> { "RB2TV" };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void BuildCoverageMainMatch_ReturnCoverage()
    {
        var actual = coverageBuilder.BuildCoverageMainMatch(new CommonMatchModel
        {
            website_id = 17,
            ls_merge_match = new List<CommonSubMatch>
            {
                new() {website_id = 17, coverage_id = 2},
                new() {website_id = 20, coverage_id = 2}
            }
        });
        var expected = new List<string> { "" };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CoverageBuild_ReturnCoverage()
    {
        var actual = coverageBuilder.CoverageBuild(new CommonMatchModel
        {
            website_id = 17,
            ls_merge_match = new List<CommonSubMatch>
            {
                new() {website_id = 17, coverage_id = 1},
                new() {website_id = 20, coverage_id = 2}
            }
        });
        var expected = new List<string> { "" };

        Assert.Equal("RB2TV", actual);
    }
}