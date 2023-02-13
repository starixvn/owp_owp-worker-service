namespace OWP.SumReport.LAReport.Criteria;

using Fanex.Data.Repository;

public class SumLAReportCommand : INonQueryCommand
{
    public DateTime EventDate { get; set; }

    public string Xml { get; set; }

    public string GetSettingKey() => "OWP_LAReportUpdate";

    public bool IsValid() => true;
}