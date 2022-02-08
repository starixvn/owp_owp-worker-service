using OWP.InfositeIdSync.Constants;

namespace OWP.InfositeIdSync.Mics;

public class RepresentativeSubTypeDetector
{
    public RepresentativeSubTypeDetector NextType { get; set; }

    public virtual int GetRepresentativeSubtype(int subType)
        => NextType?.GetRepresentativeSubtype(subType) ?? subType;
}

public class Specific10MinsDetector : RepresentativeSubTypeDetector
{
    public override int GetRepresentativeSubtype(int subType)
        => (subType >= SubType.Specific10MinsFromSubType && subType <= SubType.Specific10MinsToSubType)
        ? SubType.Specific10MinsFromSubType
        : base.GetRepresentativeSubtype(subType);
}

public class Specific15MinsDetector : RepresentativeSubTypeDetector
{
    public override int GetRepresentativeSubtype(int subType) => subType switch
    {
        >= SubType.Specific15MinsFromSubType and <= SubType.Specific15MinsToSubType => SubType.Specific15MinsFromSubType,
        >= SubType.Specific15MinsCornersFromSubType and <= SubType.Specific15MinsCornersToSubType => SubType.Specific15MinsCornersFromSubType,
        >= SubType.Specific15MinsTotalBookingFromSubType and <= SubType.Specific15MinsTotalBookingToSubType => SubType.Specific15MinsTotalBookingFromSubType,
        _ => base.GetRepresentativeSubtype(subType)
    };
}

public class NextTypesDetector : RepresentativeSubTypeDetector
{
    public override int GetRepresentativeSubtype(int subType) => subType switch
    {
        >= SubType.NextBookingFromSubType and <= SubType.NextBookingToSubType => SubType.NextBookingRepresentativeSubType,
        >= SubType.NextCornerFromSubType and <= SubType.NextCornerToSubType => SubType.NextCornerRepresentativeSubType,
        >= SubType.NextGoalFromSubType and <= SubType.NextGoalToSubType => SubType.NextGoalToSubType,
        _ => base.GetRepresentativeSubtype(subType)
    };
}

public class SingleTeamPenaltyShootOut : RepresentativeSubTypeDetector
{
    public override int GetRepresentativeSubtype(int subType) => subType switch
    {
        >= SubType.HomeSingleTeamPenaltyShootOutFromSubType and <= SubType.HomeSingleTeamPenaltyShootOutToSubType => SubType.HomeSingleTeamPenaltyShootOutRepresentativeSubType,
        >= SubType.AwaySingleTeamPenaltyShootOutFromSubType and <= SubType.AwaySingleTeamPenaltyShootOutToSubType => SubType.AwaySingleTeamPenaltyShootOutRepresentativeSubType,
        _ => base.GetRepresentativeSubtype(subType)
    };
}