namespace OWP.Infosite.Models;

    [Serializable]
    public record TeamRoundInfo
    {
        public int InfoSiteMatchId{get;init;}

        public string TeamRound{get;init;}
    }

    public record TeamRoundInfoDb : TeamRoundInfo
    {
        public Boolean IsCheck { get; set; }
    }