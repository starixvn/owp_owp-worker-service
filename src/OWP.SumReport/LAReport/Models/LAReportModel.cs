namespace OWP.SumReport.LAReport.Models;

using Olympus.Xml;

[XmlName("LAReportModel")]
public class LaReportModel
{
    public int UserId { get; set; }

    public string Username { get; set; }

    public double Link2 { get; set; }
           
    public double Link3 { get; set; }
                                     
    public double Link4 { get; set; }
                                     
    public double Link5 { get; set; }
                                     
    public double Link6 { get; set; }
                                     
    public double Link7 { get; set; }

    public double Link8Plus { get; set; }

    public double TotalMatchInGroupMultiple { get; set; }


    public double OnlyProvider { get; set; }

    [XmlName("OnlyTV")]
    public double OnlyTv { get; set; }
           
    public double OnlyReporter { get; set; }

    [XmlName("MixedProviderTVReporter")]
    public double MixedProviderTvReporter { get; set; }

    public double CornerPlus { get; set; }

    [XmlName("TBookingPlus")]
    public double BookingPlus { get; set; }

    public double Fantasy { get; set; }

    public double GoalKick { get; set; }

    public double OtherProducts { get; set; }

    [XmlName("FTCorner")]
    public double FtCorner { get; set; }

    public double ESoccer5Mins { get; set; }

    public double ESoccer6Mins { get; set; }
           
    public double ESoccer15Mins { get; set; }
           
    public double ESoccer16Mins { get; set; }
           
    public double ESoccer20Mins { get; set; }
           
    public double Others { get; set; }
           
    public double FiveMajorLeague { get; set; }

    [XmlName("FTCorner5ML")]
    public double FtCorner5Ml { get; set; }

    public string OtherProductMatchIds { get; set; } = string.Empty;

    public double TotalSingleMatches => OnlyProvider + OnlyTv + OnlyReporter + MixedProviderTvReporter;

    public double TotalSpecialProducts => CornerPlus + BookingPlus + Fantasy + OtherProducts + GoalKick;
           
    public double TotalMixedProduct => FtCorner + FtCorner5Ml;
           
    public double TotalESoccer => ESoccer5Mins + ESoccer6Mins + ESoccer15Mins + ESoccer16Mins + ESoccer20Mins;
           
    public double TotalMatches =>
        TotalMatchInGroupMultiple +
        TotalSingleMatches +
        TotalSpecialProducts +
        TotalMixedProduct +
        TotalESoccer +
        FiveMajorLeague+
        Others;
}