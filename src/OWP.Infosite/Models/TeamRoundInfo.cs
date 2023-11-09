namespace OWP.Infosite.Models
{
    [Serializable]
    public record TeamRoundInfo
    {
        public int InfoSiteMatchId{get;init;}

        public string TeamRound{get;init;}
    }
}