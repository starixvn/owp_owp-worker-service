using OWP.Core.Attributes;

namespace OWP.Infosite.Models;

[Serializable]
[XmlName("MatchRelationEvent")]
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