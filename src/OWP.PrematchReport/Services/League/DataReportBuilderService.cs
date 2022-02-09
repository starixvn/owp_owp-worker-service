using OWP.PrematchReport.Models;
using System.Collections.Generic;
using System.Linq;

namespace OWP.PrematchReport.Services.League
{
    public interface DataReportBuilderService
    {
        IList<DataReport> Build(IList<Models.League> leagues, int bettype);
    }

    public class DataReportBuilderServiceImp : DataReportBuilderService
    {
        public IList<DataReport> Build(IList<Models.League> leagues, int bettype)
        {
            var dataReport = new List<DataReport>();
            var group = leagues
                .GroupBy(league => new { league.UserId, league.GroupId, league.EventDate })
                .ToDictionary(x => x.Key, x => x.Count());
            foreach (var key in group.Keys)
            {
                var amount = group[key];
                dataReport.Add(new DataReport
                {
                    UserId = key.UserId,
                    EventDate = key.EventDate,
                    GroupId = key.GroupId,
                    Amount = amount,
                    BetType = bettype
                });
            }

            return dataReport;
        }
    }
}