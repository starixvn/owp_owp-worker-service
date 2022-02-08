namespace OWP.Infosite.Models;

public class SoccerEventSchedule : soccer_event_schedule
{
    public string home_remark { get; set; }

    public string away_remark { get; set; }
}

public class soccer_event_schedule
{
    public int parent_create_match_id { get; set; }

    public long id { get; set; }

    public long match_id { get; set; }

    public DateTime? game_date { get; set; }

    public string agent { get; set; }

    public bool? is_neutral { get; set; }

    public bool? is_team_reverse { get; set; }

    public bool? rd_checked { get; set; }

    public bool? lst_checked { get; set; }

    public bool? conf_checked { get; set; }

    public bool? early_checked { get; set; }

    public bool? live_checked { get; set; }

    public string code { get; set; }

    public string op { get; set; }

    public string wb { get; set; }

    public string round { get; set; }

    public string lt_value { get; set; }

    public string et_pk { get; set; }

    public string venue { get; set; }

    public string local_tv { get; set; }

    public string tv_live { get; set; }

    public string website1 { get; set; }

    public string website2 { get; set; }

    public string website3 { get; set; }

    public string website4 { get; set; }

    public string provider { get; set; }

    public string provider_use { get; set; }

    public DateTime? create_time { get; set; }

    public int? create_by { get; set; }

    public DateTime? update_time { get; set; }

    public int? update_by { get; set; }

    public string remark { get; set; }

    public string remark2 { get; set; }

    public string round_total { get; set; }

    public string create_match_code { get; set; }

    public string create_option { get; set; }

    public int create_time_diff { get; set; }

    public int? create_display_mode { get; set; }

    public int create_creator_id { get; set; }

    public int create_match_id { get; set; }

    public DateTime? create_export_time { get; set; }

    public DateTime? create_error_time { get; set; }

    public int create_match_id_hedge { get; set; }

    public string create_export_user { get; set; }

    public int? create_event_status { get; set; }

    public int? create_streaming_src { get; set; }

    public string create_streaming_id { get; set; }

    public bool? create_in_credit { get; set; }

    public bool? create_is_live { get; set; }

    public string ip_tv { get; set; }

    public int? create_streaming2_src { get; set; }

    public string create_streaming2_id { get; set; }

    public int sport_id { get; set; }
}