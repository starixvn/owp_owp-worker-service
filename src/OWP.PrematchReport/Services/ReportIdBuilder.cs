using OWP.PrematchReport.Criteria;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWP.PrematchReport.Models;
using System;

namespace OWP.PrematchReport.Services
{
    public interface ReportIdBuilder
    {
        Task BuildReportId(IList<DataReport> dataReports);
    }

    public class ReportIdBuilderImp : ReportIdBuilder
    {
        private readonly Repository repository;

        public ReportIdBuilderImp(Repository repository)
        {
            this.repository = repository;
        }

        public async Task BuildReportId(IList<DataReport> dataReports)
        {
            foreach (var dataReport in dataReports)
            {
                var criteria = new BuildReportIdCriteria
                {
                    UserId = dataReport.UserId,
                    EventDate = dataReport.EventDate
                };
                var reportId = await repository.BuildReportId(criteria);
                dataReport.ReportId = reportId;
            }
        }
    }
}