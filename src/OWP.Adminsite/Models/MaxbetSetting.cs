namespace OWP.Adminsite.Models;

public class MaxbetSetting
{
    public decimal FT_HDP { get; set; }
    public decimal FT_OU { get; set; }
    public decimal FirstHalf_HDP { get; set; }
    public decimal FirstHalf_OU { get; set; }
    public decimal NoOfCorner_FT_HDP { get; set; }
    public decimal NoOfCorner_FT_OU { get; set; }
    public decimal NoOfCorner_1H_HDP { get; set; }
    public decimal NoOfCorner_1H_OU { get; set; }
    public decimal NextCorner_FT_HDP { get; set; }
    public decimal TotalBooking_FT_HDP { get; set; }
    public decimal TotalBooking_FT_OU { get; set; }
    public decimal TotalBooking_1H_HDP { get; set; }
    public decimal TotalBooking_1H_OU { get; set; }
    public decimal NextBooking_FT_HDP { get; set; }
    public string SecondAccept_Main { get; set; }
    public string SecondAccept_Corner { get; set; }
    public string SecondAccept_TotalBooking { get; set; }
    public string SecondAccept_Fantasy { get; set; }
}