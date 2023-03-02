namespace OWP.Infosite.Models;

[Serializable]
public class MatchRelationEvent
{
    public MatchRelationEvent(MatchRelation matchRelation)
    {
        InfositeMatchId = matchRelation.match_id;
        ParentMatchId = matchRelation.parent_match_id;
    }

    public int InfositeMatchId { get; set; }

    public int ParentMatchId { get; set; }
}