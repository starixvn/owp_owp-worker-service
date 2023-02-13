namespace OWP.SumReport.OvertimeReport.Constants;

using System.Collections.Immutable;

public static class BetType
{
    public static readonly ImmutableList<int> MinorMatchBetTypes =
        ImmutableList.Create(
            BookingMinorBettype.Total,
            BookingMinorBettype.Next,
            BookingMinorBettype.Last,
            CornerMinorBettype.NumberOf,
            CornerMinorBettype.Next,
            CornerMinorBettype.Last,
            CornerMinorBettype.HomeSingleTeamOu,
            CornerMinorBettype.AwaySingleTeamOu
        );

    public static readonly ImmutableList<int> FullTimeBetTypes =
        ImmutableList.Create(
            FullTimeBetType.Handicap,
            FullTimeBetType.OverUnder,
            FullTimeBetType.Ft1X2
        );

    public static readonly ImmutableList<int> FullTimeMinorBetTypes =
        ImmutableList.Create(
            FullTimeBetType.Fm,
            FullTimeBetType.Aio
        );

    public static readonly ImmutableList<int> FirstHalfBetTypes =
        ImmutableList.Create(
            FirstHalfBettype.FirstHalfHandicap,
            FirstHalfBettype.FirstHalfOverUnder,
            FirstHalfBettype.FirstHalf1X2
        );

    public struct FullTimeBetType : IEquatable<FullTimeBetType>
    {
        public const int Handicap = 1;

        public const int OverUnder = 3;

        public const int Ft1X2 = 5;

        public const int Fm = -88;

        public const int Aio = -89;

        public bool Equals(FullTimeBetType other)
        {
            throw new NotImplementedException();
        }
    }

    public struct FirstHalfBettype : IEquatable<FirstHalfBettype>
    {
        public const int FirstHalfHandicap = 7;

        public const int FirstHalfOverUnder = 8;

        public const int FirstHalf1X2 = 15;

        public bool Equals(FirstHalfBettype other)
        {
            throw new NotImplementedException();
        }
    }

    public struct BookingMinorBettype : IEquatable<BookingMinorBettype>
    {
        public const int Total = -6;

        public const int Next = -7;

        public const int Last = -91;

        public bool Equals(BookingMinorBettype other)
        {
            throw new NotImplementedException();
        }
    }

    public struct CornerMinorBettype : IEquatable<CornerMinorBettype>
    {
        public const int NumberOf = -3;

        public const int Next = -4;

        public const int Last = -90;

        public const int HomeSingleTeamOu = -94;

        public const int AwaySingleTeamOu = -95;

        public bool Equals(CornerMinorBettype other)
        {
            throw new NotImplementedException();
        }
    }
}