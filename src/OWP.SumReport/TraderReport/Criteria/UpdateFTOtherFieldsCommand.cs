using Fanex.Data.Repository;

namespace OWP.SumReport.TraderReport.Criteria;

public class UpdateFTOtherFieldsCommand : INonQueryCommand
{
    public DateTime EventDate { get; set; }

    public string GetSettingKey() => "OWP_TraderReport_UpdateFT_OtherFields";

    public bool IsValid() => true;
}