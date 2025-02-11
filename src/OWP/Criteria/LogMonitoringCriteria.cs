using Fanex.Data.Repository;

namespace OWP.Criteria
{
    public class LogMonitoringCriteria: NonQueryCommand
    {
        public DateTime CreatedDate { get; set; }
        public string Action { get; set; }
        public string Payload { get; set; }
        public override string GetSettingKey() => "OWP_LogMonitoring_Insert";

        public override bool IsValid() => true;
    }
}
