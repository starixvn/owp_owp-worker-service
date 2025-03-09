using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria
{
    public class GetEventChangeKoCriteria: ICriteria
    {
        public DateTime EventDate { get; set; }

        public string EventInfoJson { get; set; }

        public string GetSettingKey() => "OWP_InfoSite_DetectEventChangeKo";

        public bool IsValid() => true;
        
    }
}
