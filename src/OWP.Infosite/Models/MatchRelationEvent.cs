namespace OWP.Infosite.Models;

[Serializable]
public class MatchRelationEvent(MatchRelation matchRelation)
{
    public int InfositeMatchId { get; set; } = matchRelation.match_id;

    public int ParentMatchId { get; set; } = matchRelation.parent_match_id;
}