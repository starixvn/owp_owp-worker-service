using OWP.Infosite.Helper;
using OWP.Infosite.Models;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.Infosite.UnitTests.Helper;

public class InfoSiteSyncHelperUnitTests
{
    [Fact]
    public void ShouldReturnBuildActionIdForUpdateCase()
    {
        var result = InfoSiteSyncHelper.BuildActionId("LeagueName", "Home New", "Home Old");

        DeepAssert.Equal(result,3);
    }

    [Fact]
    public void ShouldReturnBuildActionIdForCancelCase()
    {
        var result = InfoSiteSyncHelper.BuildActionId("Provider", "", "K+");

        DeepAssert.Equal(result, 10);
    }
}   

